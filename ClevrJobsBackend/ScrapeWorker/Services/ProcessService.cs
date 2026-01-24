using Data.Enums;
using Data.Models;
using Data.Repositories;
using Google.GenAI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Workers.Models.Dto;
using Workers.Models.Dto.Responses;

namespace Workers.Services
{
    public class ProcessService : IProcessService
    {
        private readonly ILogger<ProcessService> _logger;
        private readonly IConfiguration _configuration;

        public ProcessService(ILogger<ProcessService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ServiceResponse> ProcessRawJobs(ICollection<RawJob> rawJobs, IProcessRepository processRepository, IJobRepository jobRepository)
        {
            var prompt = await processRepository.GetLatestActivePromptAsync();
            if (prompt is null)
            {
                _logger.LogError("Error getting prompt");
                return new ServiceResponse
                {
                    Success = false,
                    MinorError = false
                };
            }

            var processRun = await CreateProcessRunAsync(processRepository, "gemini-3-flash-preview", prompt);
            if (processRun is null)
            {
                _logger.LogError("Error creating ProcessRun");
                return new ServiceResponse
                {
                    Success = false,
                    MinorError = false
                };
            }

            List<ProcessedJob> processedJobs = new();
            List<FailedProcess> failedProcesses = new();

            foreach (var job in rawJobs)
            {
                try
                {
                    var result = await ProcessSingleJob(job, processRun, prompt);
                    if (result is null)
                    {
                        throw new Exception("ProcessedJob is null");
                    }

                    await jobRepository.MarkRawJobAsProcessed(job);

                    processedJobs.Add(result);

                    // To avoid rate limiting
                    await Task.Delay(TimeSpan.FromSeconds(13));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error processing job {jobId}. Continuing with next rawjob", job.Id);

                    var failed = new FailedProcess
                    {
                        RawJob = job,
                        ProcessRun = processRun,
                        ErrorMessage = e.Message
                    };
                    failedProcesses.Add(failed);

                    continue;
                }
            }

            bool errorOccured = false;

            if (processedJobs.Any())
            {
                _logger.LogInformation($"Trying to save {processedJobs.Count} processedjob(s) to database");
                var addResult = await processRepository.AddMultipleProcessedJobs(processedJobs);
                if (!addResult)
                {
                    _logger.LogError("Failed to save processedjob(s) to database. Process id: {processRunId}", processRun.Id);
                    errorOccured = true;
                }
            }

            if (failedProcesses.Any())
            {
                _logger.LogInformation($"Trying to save {failedProcesses.Count} failed processedjob(s) to database");
                var addFailedResult = await processRepository.AddMultipleFailedProcesses(failedProcesses);
                if (!addFailedResult)
                {
                    _logger.LogError("Failed to save failed processedjob(s) to database. Process id: {processRunId}", processRun.Id);
                    errorOccured = true;
                }
            }

            var endProcessRunResult = await EndProcessRunAsync(processRepository, processRun);
            if (!endProcessRunResult)
            {
                _logger.LogError("Error ending ProcessRun");
                return new ServiceResponse
                {
                    Success = true,
                    MinorError = true,
                    ErrorMessage = "Error ending ProcessRun"
                };
            }

            return new ServiceResponse
            {
                Success = true,
                MinorError = false
            };
        }

        private async Task<ProcessedJob?> ProcessSingleJob(RawJob rawJob, ProcessRun processRun, Prompt prompt)
        {
            try
            {
                var message = $"{prompt.Content}\n{rawJob.Description}";
                var client = new Client(false, _configuration["ApiKeys:Gemini"]);
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-3-flash-preview",
                    contents: message
                );
                //Console.WriteLine(response.Candidates[0].Content.Parts[0].Text);

                var data = JsonSerializer.Deserialize<AiResponse>(response.Candidates[0].Content.Parts[0].Text);
                if (data is null)
                {
                    throw new JsonException("Deserialized AI response is null");
                }

                var processedJob = new ProcessedJob
                {
                    Description = data.Description,
                    CompetenceRank = CompetenceRankType.Unknown,
                    Rating = 0.0f,
                    Keywords = data.Keywords,
                    RawJob = rawJob,
                    ProcessRun = processRun
                };

                return processedJob;
            }
            catch (JsonException e)
            {
                _logger.LogError(e, "Failed to deserialize AI response for job {rawJobId}", rawJob.Id);
                throw;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "API call failed for job {rawJobId}", rawJob.Id);
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong processing rawjob {rawJobId}", rawJob.Id);
                throw;
            }
        }

        private async Task<ProcessRun?> CreateProcessRunAsync(IProcessRepository processRepository, string model, Prompt prompt)
        {
            var processRun = new ProcessRun
            {
                StartedAt = DateTime.Now,
                Model = model,
                Prompt = prompt
            };

            var success = await processRepository.AddProcessRun(processRun);
            if (!success)
            {
                return null;
            }

            return processRun;
        }

        private async Task<bool> EndProcessRunAsync(IProcessRepository processRepository, ProcessRun processRun)
        {
            processRun.FinishedAt = DateTime.Now;

            var success = await processRepository.UpdateProcessRun(processRun);
            if (!success)
            {
                return false;
            }

            return true;
        }
    }
}

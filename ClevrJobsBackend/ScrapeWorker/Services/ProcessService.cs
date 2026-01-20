using Data.Enums;
using Data.Models;
using Data.Repositories;
using Google.GenAI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Workers.Models.Dto;

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

        public async Task<bool> ProcessRawJobs(ICollection<RawJob> rawJobs, IProcessRepository processRepository)
        {
            var prompt = await processRepository.GetLatestActivePromptAsync();
            if (prompt is null)
            {
                _logger.LogError("Error getting prompt");
                return false;
            }

            var processRun = await CreateProcessRunAsync(processRepository, "gemini-3-flash-preview", prompt);
            if (processRun is null)
            {
                _logger.LogError("Error creating ProcessRun");
                return false;
            }

            List<ProcessedJob> processedJobs = new();

            foreach (var job in rawJobs)
            {
                try
                {
                    var result = await ProcessSingleJob(job, processRun, prompt);
                    if (result is null)
                    {
                        // Processing failed, mark job as failed and continue
                        continue;
                    }

                    processedJobs.Add(result);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error processing job {job.Id}");
                    continue;
                }
            }

            if (!processedJobs.Any())
            {
                return true;
            }

            var addResult = await processRepository.AddMultipleProcessedJobs(processedJobs);
            if (!addResult)
            {
                _logger.LogError("Error saving processed jobs");
                return false;
            }

            var endProcessRunResult = await EndProcessRunAsync(processRepository, processRun);
            if (processRun is null)
            {
                _logger.LogError("Error ending ProcessRun");
                return false;
            }

            return true;
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
                    return null;
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
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong processing rawjob {rawJob.Id}");
                return null;
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

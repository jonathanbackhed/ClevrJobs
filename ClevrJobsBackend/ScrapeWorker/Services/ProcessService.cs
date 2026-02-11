using Data.Enums;
using Data.Models;
using Data.Repositories;
using Google.GenAI;
using System.Text.Json;
using Workers.DTOs;
using Workers.DTOs.Responses;

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

        public async Task ProcessRawJob(RawJob rawJob, IProcessRepository processRepository, IJobRepository jobRepository)
        {
            var prompt = await processRepository.GetLatestActivePromptAsync();
            if (prompt is null)
            {
                _logger.LogError("Error getting prompt");
                return;
            }

            var processRun = await CreateProcessRunAsync(processRepository, "gemini-3-flash-preview", prompt);
            if (processRun is null)
            {
                _logger.LogError("Error creating ProcessRun");
                return;
            }

            try
            {
                var result = await ProcessSingleJob(rawJob, processRun, prompt);
                if (result.ProcessedJob != null)
                {
                    await processRepository.AddProcessedJob(result.ProcessedJob);
                    await jobRepository.MarkRawJobAsProcessed(rawJob);

                    _logger.LogInformation("Successfully processed {Id}", rawJob.Id);
                }
                else
                {
                    var failed = new FailedProcess
                    {
                        RawJob = rawJob,
                        ProcessRun = processRun,
                        ErrorMessage = result.ErrorMessage ?? "Unknown error",
                        FailedAt = DateTime.UtcNow,
                        ErrorType = result.ErrorType
                    };
                    await processRepository.AddFailedProcess(failed);

                    _logger.LogWarning("Failed to process {Id}.", rawJob.Id);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error during processing for {Id}", rawJob.Id);
            }
            finally
            {
                // To avoid rate limiting
                await Task.Delay(TimeSpan.FromSeconds(13));
            }

            var endProcessRunResult = await EndProcessRunAsync(processRepository, processRun);
            if (!endProcessRunResult)
            {
                _logger.LogError("Error ending ProcessRun");
            }
        }

        private async Task<ProcessResultResponse> ProcessSingleJob(RawJob rawJob, ProcessRun processRun, Prompt prompt)
        {
            string? aiRes = string.Empty;
            try
            {
                var message = $"{prompt.Content}\n{rawJob.Description}";
                var client = new Client(false, _configuration["ApiKeys:Gemini"]);
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-3-flash-preview",
                    contents: message
                );

                var aiResponse = response.Candidates[0].Content.Parts[0].Text;
                aiRes = aiResponse;

                var data = JsonSerializer.Deserialize<AiResponse>(aiResponse);
                if (data is null)
                {
                    throw new JsonException("Deserialized AI response is null");
                }

                if (!Enum.TryParse<CompetenceRank>(data.CompetenceRank, ignoreCase: true, out var compRank))
                {
                    compRank = CompetenceRank.Unknown;
                }

                var processedJob = new ProcessedJob
                {
                    Description = data.Description,
                    RequiredTechnologies = data.RequiredTechnologies,
                    NiceTohaveTechnologies = data.NiceTohaveTechnologies,
                    CompetenceRank = compRank,
                    KeywordsCV = data.KeywordsCV,
                    KeywordsCL = data.KeywordsCL,
                    CustomCoverLetterFocus = data.CustomCoverLetterFocus,
                    Motivation = data.Motivation,
                    RawJob = rawJob,
                    ProcessRun = processRun
                };

                return ProcessResultResponse.Success(processedJob);
            }
            catch (JsonException e)
            {
                _logger.LogError(e, "Failed to deserialize AI response for {rawJobId}\nAiResponse: {aiResponse}", rawJob.Id, aiRes);
                return ProcessResultResponse.Failure(e, isRetryable: false);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "API call failed for {rawJobId}", rawJob.Id);
                return ProcessResultResponse.Failure(e, isRetryable: false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong processing rawjob {rawJobId}", rawJob.Id);
                return ProcessResultResponse.Failure(e, isRetryable: false);
            }
        }

        private async Task<ProcessRun?> CreateProcessRunAsync(IProcessRepository processRepository, string model, Prompt prompt)
        {
            var processRun = new ProcessRun
            {
                StartedAt = DateTime.UtcNow,
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
            processRun.FinishedAt = DateTime.UtcNow;

            var success = await processRepository.UpdateProcessRun(processRun);
            if (!success)
            {
                return false;
            }

            return true;
        }
    }
}

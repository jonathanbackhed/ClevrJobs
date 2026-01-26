using Data.Repositories;
using Queue.Messages;
using Queue.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Workers.Services;

namespace Workers
{
    public class ProcessingWorker : BackgroundService
    {
        private readonly ILogger<ProcessingWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IProcessService _processService;
        private readonly IMessageQueue _messageQueue;

        public ProcessingWorker(ILogger<ProcessingWorker> logger, IServiceScopeFactory scopeFactory, IProcessService processService, IMessageQueue messageQueue)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _processService = processService;
            _messageQueue = messageQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting for ScrapeCompletedEvent event...");
                await _messageQueue.SubscribeAsync<ScrapeCompletedEvent>(async evt =>
                {
                    _logger.LogInformation("ScrapeCompletedEvent recieved");
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();

                        var jobs = await jobRepository.GetUnprocessedRawJobsfromScrapeRunId(evt.ScrapeRunId);
                        if (jobs is null)
                        {
                            _logger.LogInformation("No unprocessed jobs found for ScrapeRunId: {scrapeRunId}", evt.ScrapeRunId);
                            return;
                        }

                        var processRepository = scope.ServiceProvider.GetRequiredService<IProcessRepository>();

                        var result = await _processService.ProcessRawJobs(jobs, processRepository, jobRepository);

                        _logger.LogInformation("Processing finished with success status: {success} and minor error status: {minorError}", result.Success, result.MinorError);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error occurred during processing");
                        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    }
                },
                stoppingToken);
            }
        }
    }
}

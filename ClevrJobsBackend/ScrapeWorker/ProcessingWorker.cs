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
                await _messageQueue.SubscribeAsync<JobScrapedEvent>(async evt =>
                {
                    _logger.LogInformation("ScrapeCompletedEvent recieved");
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();

                        var job = await jobRepository.GetRawJobById(evt.RawJobId);
                        if (job is null)
                        {
                            _logger.LogInformation("RawJob not found {Id}", evt.RawJobId);
                            return;
                        }

                        var processRepository = scope.ServiceProvider.GetRequiredService<IProcessRepository>();

                        await _processService.ProcessRawJob(job, processRepository, jobRepository);

                        _logger.LogInformation("Processing finished for {Id}", evt.RawJobId);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error occurred during processing");
                    }
                },
                stoppingToken);
            }
        }
    }
}

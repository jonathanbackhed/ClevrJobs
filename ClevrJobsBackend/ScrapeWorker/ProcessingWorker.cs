using Data.Repositories;
using Queue.Messages;
using Queue.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Workers
{
    public class ProcessingWorker : BackgroundService
    {
        private readonly ILogger<ProcessingWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMessageQueue _messageQueue;

        public ProcessingWorker(ILogger<ProcessingWorker> logger, IServiceScopeFactory scopeFactory, IMessageQueue messageQueue)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _messageQueue = messageQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting for ScrapeCompletedEvent event...");
                await _messageQueue.SubscribeAsync<ScrapeCompletedEvent>(async evt =>
                {
                    _logger.LogInformation("ScrapeCompletedEvent event recieved");
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();

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

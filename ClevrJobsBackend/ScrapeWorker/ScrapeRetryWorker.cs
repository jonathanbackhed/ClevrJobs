using Data.Repositories;
using Queue.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Workers.Services;

namespace Workers
{
    public class ScrapeRetryWorker : BackgroundService
    {
        private readonly ILogger<ScrapeRetryWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IScraperService _scraperService;
        private readonly IMessageQueue _messageQueue;

        public ScrapeRetryWorker(ILogger<ScrapeRetryWorker> logger, IServiceScopeFactory scopeFactory, IScraperService scraperService, IMessageQueue messageQueue)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _scraperService = scraperService;
            _messageQueue = messageQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();

                    _logger.LogInformation($"Scrape started at {DateTime.UtcNow}");

                    var result = await _scraperService.RetryFailedScrapesPlatsbankenAsync(jobRepository, _messageQueue, stoppingToken);

                    _logger.LogInformation($"Scrape ended at {DateTime.UtcNow}");

                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occurred for RetryWorker");
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromHours(1));
                }
            }
        }
    }
}

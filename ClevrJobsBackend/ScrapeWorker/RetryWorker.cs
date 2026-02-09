using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Workers.Services;

namespace Workers
{
    public class RetryWorker : BackgroundService
    {
        private readonly ILogger<RetryWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IScraperService _scraperService;

        public RetryWorker(ILogger<RetryWorker> logger, IServiceScopeFactory scopeFactory, IScraperService scraperService)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _scraperService = scraperService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
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

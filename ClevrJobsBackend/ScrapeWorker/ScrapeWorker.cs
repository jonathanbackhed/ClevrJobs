using Data.Repositories;
using Microsoft.Playwright;
using Queue.Messages;
using Queue.Services;
using Workers.Services;

namespace Workers
{
    public class ScrapeWorker : BackgroundService
    {
        private readonly ILogger<ScrapeWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IScraperService _scraperService;
        private readonly IMessageQueue _messageQueue;

        public ScrapeWorker(ILogger<ScrapeWorker> logger, IServiceScopeFactory scopeFactory, IScraperService scraperService, IMessageQueue messageQueue)
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

                    var lastRun = await jobRepository.GetLastScrapeRun();

                    if (lastRun != null && lastRun.StartedAt.Date == DateTime.UtcNow.Date)
                    {
                        var now = DateTime.UtcNow;
                        var targetTime = now.Date.AddDays(1).AddHours(2);
                        var delayUntilTarget = targetTime - now;

                        _logger.LogInformation($"Scrape already performed today. Sleeping for {delayUntilTarget.Hours} hour(s) and {delayUntilTarget.Minutes} minute(s).");
                        await Task.Delay(delayUntilTarget, stoppingToken);
                    }

                    _logger.LogInformation($"Scrape started at {DateTime.UtcNow}");

                    await _scraperService.ScrapePlatsbankenAsync(jobRepository, _messageQueue, stoppingToken);

                    _logger.LogInformation($"Scrape ended at {DateTime.UtcNow}");

                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occurred for ScrapeWorker");
                }
            }
        }
    }
}

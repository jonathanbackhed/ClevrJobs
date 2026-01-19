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
        private readonly IMessageQueue _messageService;

        public ScrapeWorker(ILogger<ScrapeWorker> logger, IServiceScopeFactory scopeFactory, IScraperService scraperService, IMessageQueue messageService)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _scraperService = scraperService;
            _messageService = messageService;
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

                    if (lastRun != null && lastRun.StartedAt.Date == DateTime.Now.Date)
                    {
                        var now = DateTime.Now;
                        var targetTime = now.Date.AddDays(1).AddHours(2);
                        var delayUntilTarget = targetTime - now;

                        _logger.LogInformation($"Scrape already performed today. Sleeping for {delayUntilTarget.Hours} hours and {delayUntilTarget.Minutes} minutes.");
                        await Task.Delay(delayUntilTarget, stoppingToken);
                        continue;
                    }

                    var lastJob = await jobRepository.GetLastPublishedRawJob();

                    var tryParseListingId = int.TryParse(lastJob?.ListingId, out var lastJobListingId);
                    if (!tryParseListingId)
                        lastJobListingId = -1;

                    _logger.LogInformation($"Scrape started at {DateTime.Now}");

                    var (success, scrapeRunId) = await _scraperService.ScrapePlatsbankenAsync(jobRepository, lastJobListingId, stoppingToken);

                    if (success)
                    {
                        await _messageService.PublishAsync(new ScrapeCompletedEvent
                        {
                            ScrapeRunId = scrapeRunId,
                            TimeStamp = DateTime.Now
                        });
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occurred during scraping");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                
            }
        }
    }
}

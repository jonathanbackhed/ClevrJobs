using Data.Repositories;
using Microsoft.Playwright;
using ScrapeWorker.Services;

namespace ScrapeWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IScraperService _scraperService;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IScraperService scraperService)
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

                    var lastRun = await jobRepository.GetLastScrapeRun();

                    if (lastRun != null && lastRun.StartedAt.Date == DateTime.Now.Date)
                    {
                        _logger.LogInformation("Scrape already performed today. Waiting until tomorrow.");

                        var now = DateTime.Now;
                        var targetTime = now.Date.AddDays(1).AddHours(2);
                        var delayUntilTarget = targetTime - now;

                        await Task.Delay(delayUntilTarget, stoppingToken);
                        continue;
                    }

                    _logger.LogInformation($"Scrape started at {DateTime.Now}");

                    await _scraperService.ScrapePlatsbankenAsync(jobRepository, stoppingToken);

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

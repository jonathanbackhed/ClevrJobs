using Data.Repositories;

namespace ScrapeWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
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
            }
        }
    }
}

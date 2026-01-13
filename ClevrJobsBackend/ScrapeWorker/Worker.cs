using Data.Repositories;
using Microsoft.Playwright;

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

                _logger.LogInformation($"Scrape started at {DateTime.Now}");

                var baseUrl = "https://arbetsformedlingen.se";
                var requestUrl = "https://arbetsformedlingen.se/platsbanken/annonser?q=c%23";

                using var playwright = await Playwright.CreateAsync();

                await using var browser = await playwright.Chromium.LaunchAsync(
                    new BrowserTypeLaunchOptions
                    {
                        Headless = false,
                        SlowMo = 200
                    });

                var page = await browser.NewPageAsync();

                await page.GotoAsync(requestUrl,
                    new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.NetworkIdle
                    });

                await page.Locator("#button-basic").ClickAsync();

                await page.WaitForSelectorAsync("ul li button", new() { State = WaitForSelectorState.Visible });

                await page.Locator("ul li > button span")
                          .Filter(new() { HasText = "publiceringsdatum" })
                          .ClickAsync();

                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                await page.WaitForSelectorAsync("div.header-container h3 a");

                var links = await page.Locator("div.header-container h3 a").AllAsync();

                foreach (var item in links)
                {
                    var href = await item.GetAttributeAsync("href");
                    var context = await browser.NewContextAsync();
                    var newPage = await context.NewPageAsync();
                    await newPage.GotoAsync(baseUrl + href);

                    await newPage.WaitForSelectorAsync("div.job-description", new() { State = WaitForSelectorState.Visible });
                    var title = await newPage.Locator("h1.break-title").InnerTextAsync();
                    var company = await newPage.Locator("h2#pb-company-name").InnerTextAsync();
                    var role = await newPage.Locator("h3#pb-job-role").InnerTextAsync();
                    var location = await newPage.Locator("h3#pb-job-location").InnerTextAsync();

                    var extent = await newPage
                        .Locator("div.ng-star-inserted:has(h3[translate='section-jobb-main-content.extent']) > span")
                        .InnerTextAsync() ?? "N/A";

                    var duration = await newPage
                        .Locator("div.ng-star-inserted:has(h3[translate='section-jobb-main-content.duration']) > span")
                        .InnerTextAsync() ?? "N/A";

                    var dateLocator = await newPage.Locator("div.last-date > strong").AllInnerTextsAsync();
                    var applicationDeadline = dateLocator.FirstOrDefault() ?? "N/A";

                    var applicationUrl = await newPage.Locator("a.apply-by-link-external").First.GetAttributeAsync("href") ?? "N/A";

                    var desc = await newPage.Locator("div.job-description").AllInnerTextsAsync();

                    var salaryType = await newPage.Locator("div.salary-type").InnerTextAsync();

                    var published = await newPage
                        .Locator("div h2 > span[translate='section-jobb-about.published']")
                        .InnerTextAsync();

                    var listingId = await newPage
                        .Locator("div h2 > span[translate='section-jobb-about.annons-id']")
                        .InnerTextAsync();

                    await newPage.CloseAsync();
                    await Task.Delay(3000);
                }

                await browser.CloseAsync();
            }
        }
    }
}

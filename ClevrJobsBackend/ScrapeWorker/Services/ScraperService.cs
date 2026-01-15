using Data;
using Data.Enums;
using Data.Models;
using Data.Repositories;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapeWorker.Services
{
    public class ScraperService : IScraperService
    {
        private readonly ILogger<ScraperService> _logger;

        public ScraperService(ILogger<ScraperService> logger)
        {
            _logger = logger;
        }

        public async Task<(bool, int)> ScrapePlatsbankenAsync(IJobRepository jobRepository, CancellationToken cancellationToken)
        {
            var baseUrl = "https://arbetsformedlingen.se";
            var requestUrl = "https://arbetsformedlingen.se/platsbanken/annonser?q=c%23";

            var scrapeRun = await CreateScrapeRunAsync(jobRepository);

            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true,
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
                try
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

                    var tryApplicationUrl = newPage.Locator("a.apply-by-link-external");
                    var tryApplicationEmail = newPage.Locator("a.apply-by-email");
                    var applicationUrl = await tryApplicationUrl.CountAsync() > 0 ?
                        await tryApplicationUrl.First.GetAttributeAsync("href") :
                        await tryApplicationEmail.First.InnerTextAsync();

                    var desc = await newPage.Locator("div.job-description").InnerTextAsync();

                    var salaryType = await newPage.Locator("div.salary-type").InnerTextAsync();

                    var published = await newPage
                        .Locator("div h2 > span[translate='section-jobb-about.published']")
                        .InnerTextAsync();

                    var listingId = await newPage
                        .Locator("div h2 > span[translate='section-jobb-about.annons-id']")
                        .InnerTextAsync();

                    await newPage.CloseAsync();

                    var rawJob = new RawJob
                    {
                        Title = title,
                        CompanyName = company,
                        RoleName = role,
                        Location = location.Split(":").ElementAtOrDefault(1)?.Trim() ?? location,
                        Extent = extent,
                        Duration = duration,
                        ApplicationDeadline = applicationDeadline,
                        ApplicationUrl = applicationUrl,
                        Description = desc,
                        SalaryType = salaryType.Split(":").ElementAtOrDefault(1)?.Trim() ?? salaryType,
                        Published = published.Split(":").ElementAtOrDefault(1)?.Trim() ?? published,
                        ListingId = listingId.Split(":").ElementAtOrDefault(1)?.Trim() ?? listingId,
                        ListingUrl = baseUrl + href,
                        Source = SourceType.Platsbanken,
                        ProcessedStatus = StatusType.New,
                        ScrapeRun = scrapeRun
                    };

                    var result = await jobRepository.AddRawJob(rawJob);
                    if (!result)
                    {
                        _logger.LogError($"Failed to save RawJob to database. Listing id: {listingId}");
                    }

                    await Task.Delay(1000);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error scraping job listing: {e.Message}\n{e.StackTrace}");
                }
            }

            await browser.CloseAsync();

            scrapeRun.Status = StatusType.Completed;
            scrapeRun.FinishedAt = DateTime.Now;
            await jobRepository.UpdateScrapeRun(scrapeRun);

            return (true, scrapeRun.Id);
        }

        private async Task<ScrapeRun> CreateScrapeRunAsync(IJobRepository jobRepository)
        {
            var scrapeRun = new ScrapeRun
            {
                StartedAt = DateTime.Now,
                Status = StatusType.InProgress
            };

            await jobRepository.AddScrapeRun(scrapeRun);

            return scrapeRun;
        }
    }
}

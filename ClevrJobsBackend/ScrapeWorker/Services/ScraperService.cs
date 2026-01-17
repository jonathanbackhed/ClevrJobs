using Data;
using Data.Enums;
using Data.Models;
using Data.Repositories;
using Microsoft.Playwright;
using ScrapeWorker.Extensions;
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
                var href = await item.GetAttributeAsync("href");
                var context = await browser.NewContextAsync();
                var newPage = await context.NewPageAsync();
                await newPage.GotoAsync(baseUrl + href);

                try
                {
                    await newPage.WaitForSelectorAsync("div.job-description", new() { State = WaitForSelectorState.Visible });

                    var title = await newPage.GetRequiredTextAsync("h1.break-title");
                    var company = await newPage.GetRequiredTextAsync("h2#pb-company-name");
                    var role = await newPage.GetRequiredTextAsync("h3#pb-job-role");
                    var location = await newPage.GetRequiredTextAsync("h3#pb-job-location");

                    var extent = await newPage
                        .GetOptionalTextAsync("div.ng-star-inserted:has(h3[translate='section-jobb-main-content.extent']) > span", "");

                    var duration = await newPage
                        .GetOptionalTextAsync("div.ng-star-inserted:has(h3[translate='section-jobb-main-content.duration']) > span", "");

                    if (string.IsNullOrEmpty(extent) && string.IsNullOrEmpty(duration))
                    {
                        extent = await newPage
                            .GetOptionalTextAsync("div.ng-star-inserted:has(h3[translate='section-jobb-main-content.employment-type']) > span");
                    }

                    var applicationDeadline = await newPage.GetRequiredTextAsync("div.last-date > strong");

                    var applicationUrlLocator = newPage.Locator("a.apply-by-link-external");
                    var applicationEmailLocator = newPage.Locator("a.apply-by-email");
                    var applicationUrl = await applicationUrlLocator.CountAsync() > 0 
                        ? await applicationUrlLocator.First.GetAttributeAsync("href") 
                        : await applicationEmailLocator.First.InnerTextAsync();

                    var desc = await newPage.GetRequiredTextAsync("div.job-description");

                    var salaryType = await newPage.GetOptionalTextAsync("div.salary-type");

                    var published = await newPage
                        .GetOptionalTextAsync("div h2 > span[translate='section-jobb-about.published']");

                    var listingId = await newPage
                        .GetRequiredTextAsync("div h2 > span[translate='section-jobb-about.annons-id']");

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
                        throw new InvalidOperationException("Error saving to database");
                    }

                    await Task.Delay(500);
                }
                catch (InvalidOperationException e)
                {
                    await newPage.CloseAsync();
                    _logger.LogError(e, $"Missing required field for listing: {href}");
                }
                catch (Exception e)
                {
                    await newPage.CloseAsync();
                    _logger.LogError(e, $"Unexpected error scraping listing: {href}");
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

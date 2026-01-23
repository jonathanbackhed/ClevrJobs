using Data.Enums;
using Data.Models;
using Data.Repositories;
using Microsoft.Playwright;
using Workers.Extensions;
using Workers.Models.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Workers.Services
{
    public class ScraperService : IScraperService
    {
        private readonly ILogger<ScraperService> _logger;

        private string _platsbankenBaseUrl = "https://arbetsformedlingen.se";
        private string _platsbankenRequestUrl = "https://arbetsformedlingen.se/platsbanken/annonser?q=c%23";

        public ScraperService(ILogger<ScraperService> logger)
        {
            _logger = logger;
        }

        public async Task<(bool, int)> ScrapePlatsbankenAsync(IJobRepository jobRepository, CancellationToken cancellationToken)
        {
            var scrapeRun = await CreateScrapeRunAsync(jobRepository);
            if (scrapeRun is null)
            {
                _logger.LogError("Error creating ScrapeRun, exiting");
                return (false, -1);
            }

            var lastJob = await jobRepository.GetLastPublishedRawJob();
            var tryParseListingId = int.TryParse(lastJob?.ListingId, out var lastJobListingId);
            if (!tryParseListingId)
                lastJobListingId = -1;

            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    SlowMo = 1500
                });

            _logger.LogInformation("Opening browser and navigating to Platsbanken");

            var page = await browser.NewPageAsync();

            await page.GotoAsync(_platsbankenRequestUrl,
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

            List<RawJob> allJobs = new();
            List<FailedScrape> allFailedJobs = new();

            while (true)
            {
                var links = await page.Locator("div.header-container h3 a").AllAsync();

                var scrapeResult = await ScrapeListingsAsync(links, browser, scrapeRun, lastJobListingId);

                scrapeRun.ScrapedJobs += scrapeResult.Jobs.Count;
                scrapeRun.FailedJobs += scrapeResult.FailedJobs.Count;

                if (scrapeResult.Jobs.Any())
                {
                    _logger.LogInformation($"Scraped {scrapeResult.Jobs.Count} rawjob(s).");
                    allJobs.AddRange(scrapeResult.Jobs);
                }

                if (scrapeResult.FailedJobs.Any())
                {
                    _logger.LogInformation($"Scraped {scrapeResult.FailedJobs.Count} failed rawjob(s).");
                    allFailedJobs.AddRange(scrapeResult.FailedJobs);
                }

                if (!scrapeResult.ShouldContinue)
                {
                    break;
                }

                var nextPageStatus = await GoToNextpageAsync(page);
                if (!nextPageStatus)
                {
                    break;
                }
            }

            await browser.CloseAsync();

            bool errorOccured = false;

            if (allJobs.Any())
            {
                _logger.LogInformation($"Trying to save {allJobs.Count} rawjob(s) to database");
                var addResult = await jobRepository.AddMultipleRawJobs(allJobs);
                if (!addResult)
                {
                    _logger.LogError("Failed to save rawjob(s) to database. Scrape id: {scrapeRunId}", scrapeRun.Id);
                    errorOccured = true;
                }
            }

            if (allFailedJobs.Any())
            {
                _logger.LogInformation($"Trying to save {allFailedJobs.Count} failed rawjob(s) to database");
                var addFailedResult = await jobRepository.AddMultipleFailedScrapes(allFailedJobs);
                if (!addFailedResult)
                {
                    _logger.LogError("Failed to save failed rawjob(s) to database. Scrape id: {scrapeRunId}", scrapeRun.Id);
                    errorOccured = true;
                }
            }

            var endScrapeResult = await EndScrapeRunAsync(jobRepository, scrapeRun, errorOccured);
            if (!endScrapeResult)
            {
                _logger.LogError("Failed to end ScrapeRun. Scrape id: {scrapeRunId}", scrapeRun.Id);
                return (false, -1);
            }

            return (true, scrapeRun.Id);
        }

        private async Task<ScrapeResultDto> ScrapeListingsAsync(IReadOnlyList<ILocator> links, IBrowser browser, ScrapeRun scrapeRun, int lastJobListingId)
        {
            bool shouldContinue = true;
            List<RawJob> jobList = new();
            List<FailedScrape> failedJobList = new();

            var random = new Random();

            foreach (var item in links)
            {
                IPage? newPage = null;
                int listId = 0;
                string listUrl = "";
                try
                {
                    var href = await item.GetAttributeAsync("href");

                    string currentListingIdStr = href?.Split("/").Last() ?? "-1";
                    var parseSuccess = int.TryParse(currentListingIdStr, out var currentListingId);
                    if (parseSuccess && currentListingId == lastJobListingId)
                    {
                        _logger.LogInformation($"Job already scraped, exiting run.");
                        shouldContinue = false;
                        break;
                    }

                    listUrl = _platsbankenBaseUrl + href;
                    listId = currentListingId;

                    var context = await browser.NewContextAsync();
                    newPage = await context.NewPageAsync();
                    await newPage.GotoAsync(_platsbankenBaseUrl + href);

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
                        ListingUrl = _platsbankenBaseUrl + href,
                        Source = SourceType.Platsbanken,
                        ProcessedStatus = StatusType.New,
                        ScrapeRun = scrapeRun
                    };

                    jobList.Add(rawJob);
                }
                catch (TimeoutException e)
                {
                    _logger.LogError(e, "Timeout happened for listing id: {listId}", listId);

                    var failedJob = new FailedScrape
                    {
                        ListingUrl = listUrl,
                        ListingId = listId.ToString(),
                        ScrapeRun = scrapeRun,
                        ErrorMessage = e.Message,
                        ErrorType = "TimeoutException",
                        Status = FailedStatus.ReadyForRetry
                    };
                    failedJobList.Add(failedJob);
                }
                catch (InvalidOperationException e)
                {
                    _logger.LogError(e, "Missing required field for listing id: {listId}", listId);

                    var failedJob = new FailedScrape
                    {
                        ListingUrl = listUrl,
                        ListingId = listId.ToString(),
                        ScrapeRun = scrapeRun,
                        ErrorMessage = e.Message,
                        ErrorType = "InvalidOperationException",
                        Status = FailedStatus.Failed
                    };
                    failedJobList.Add(failedJob);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected error scraping listing id: {listId}", listId);

                    var failedJob = new FailedScrape
                    {
                        ListingUrl = listUrl,
                        ListingId = listId.ToString(),
                        ScrapeRun = scrapeRun,
                        ErrorMessage = e.Message,
                        ErrorType = "Exception",
                        Status = FailedStatus.Failed
                    };
                    failedJobList.Add(failedJob);
                }
                finally
                {
                    if (newPage != null)
                    {
                        await newPage.CloseAsync();
                        int delay = random.Next(4, 16);
                        await Task.Delay(TimeSpan.FromSeconds(delay));
                    }
                }
            }

            return new ScrapeResultDto
            {
                Jobs = jobList,
                FailedJobs = failedJobList,
                ShouldContinue = shouldContinue
            };
        }

        private async Task<bool> GoToNextpageAsync(IPage page)
        {
            var nextBtn = page.Locator("button.digi-button.digi-button--variation-secondary:has-text('Nästa')");

            var count = await nextBtn.CountAsync();
            if (count == 0)
            {
                _logger.LogInformation("Next button not found");
                return false;
            }

            var isDisabled = await nextBtn.IsDisabledAsync();
            if (isDisabled)
            {
                _logger.LogInformation("Next button is disabled");
                return false;
            }

            await nextBtn.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            _logger.LogInformation("Successfully navigated to next page");
            return true;
        }

        private async Task<ScrapeRun?> CreateScrapeRunAsync(IJobRepository jobRepository)
        {
            var scrapeRun = new ScrapeRun
            {
                StartedAt = DateTime.Now,
                Status = StatusType.InProgress
            };

            var success = await jobRepository.AddScrapeRun(scrapeRun);
            if (!success)
            {
                return null;
            }

            return scrapeRun;
        }

        private async Task<bool> EndScrapeRunAsync(IJobRepository jobRepository, ScrapeRun scrapeRun, bool errorOccured)
        {
            scrapeRun.Status = errorOccured ? StatusType.Failed : StatusType.Completed;
            scrapeRun.FinishedAt = DateTime.Now;
            scrapeRun.ScrapedJobs = scrapeRun.ScrapedJobs;
            scrapeRun.FailedJobs = scrapeRun.FailedJobs;

            var success = await jobRepository.UpdateScrapeRun(scrapeRun);
            if (!success)
            {
                return false;
            }

            return true;
        }
    }
}

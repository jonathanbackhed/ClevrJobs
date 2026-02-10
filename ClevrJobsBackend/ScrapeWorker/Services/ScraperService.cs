using Data.Enums;
using Data.Models;
using Data.Repositories;
using Microsoft.Playwright;
using Queue.Messages;
using Queue.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Workers.DTOs.Responses;
using Workers.Extensions;

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

        public async Task<(bool success, int scrapeRunId)> ScrapePlatsbankenAsync(IJobRepository jobRepository, CancellationToken cancellationToken)
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

        public async Task RetryFailedScrapesPlatsbankenAsync(IJobRepository jobRepository, IMessageQueue messageQueue, CancellationToken cancellationToken)
        {
            var failedScrapes = await jobRepository.GetFailedScrapesForRetryAsync();
            if (!failedScrapes.Any())
            {
                _logger.LogInformation("No failed scrapes to retry.");
                return;
            }

            _logger.LogInformation("Retrying {Count} failed scrapes.", failedScrapes.Count);

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });

            foreach (var item in failedScrapes)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Cancellation requested, stopping retry process.");
                    break;
                }

                IPage? page = null;

                try
                {
                    page = await browser.NewPageAsync();

                    _logger.LogInformation("Retrying scrape for {Url}.", item.ListingUrl);
                    var result = await ScrapePlatsbankenListingAsync(page, item.ListingUrl, item.ScrapeRun);

                    if (result.RawJob != null)
                    {
                        await jobRepository.AddRawJob(result.RawJob);

                        item.Status = FailedStatus.Resolved;
                        item.RetriedAt = DateTime.UtcNow;
                        await jobRepository.UpdateFailedScrape(item);
                        await messageQueue.PublishAsync(new JobScrapedEvent
                        {
                            RawJobId = result.RawJob.Id
                        });

                        _logger.LogInformation("Successfully scraped and resolved {Id}", item.ListingUrl);
                    }
                    else
                    {
                        item.ErrorMessage = result.ErrorMessage ?? "Unknown error";
                        item.ErrorType = result.ErrorType ?? "Unknown";
                        item.RetriedAt = DateTime.UtcNow;
                        item.Status = result.IsRetryable ? FailedStatus.ReadyForRetry : FailedStatus.Failed;

                        await jobRepository.UpdateFailedScrape(item);

                        _logger.LogWarning("Failed to scrape {Url}.", item.ListingUrl);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected error during retry for {Url}", item.ListingUrl);
                }
                finally
                {
                    if (page != null)
                    {
                        await page.CloseAsync();
                    }

                    await Task.Delay(Random.Shared.Next(4000, 16000));
                }
            }
        }

        private async Task<ScrapeResultResponse> ScrapePlatsbankenListingAsync(IPage page, string listingUrl, ScrapeRun scrapeRun)
        {
            try
            {
                await page.GotoAsync(listingUrl, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle,
                    Timeout = 30000
                });

                await page.WaitForSelectorAsync("div.job-description", new()
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 10000
                });

                var title = await page.GetRequiredTextAsync("h1.break-title");
                var company = await page.GetRequiredTextAsync("h2#pb-company-name");
                var role = await page.GetRequiredTextAsync("h3#pb-job-role");
                var location = await page.GetRequiredTextAsync("h3#pb-job-location");
                var applicationDeadline = await page.GetRequiredTextAsync("div.last-date > strong");
                var desc = await page.GetRequiredTextAsync("div.job-description");
                var listingId = await page.GetRequiredTextAsync("div h2 > span[translate='section-jobb-about.annons-id']");

                var extent = await page
                    .GetOptionalTextAsync("div.ng-star-inserted:has(h3[translate='section-jobb-main-content.extent']) > span", "");

                var duration = await page
                    .GetOptionalTextAsync("div.ng-star-inserted:has(h3[translate='section-jobb-main-content.duration']) > span", "");

                if (string.IsNullOrWhiteSpace(extent) && string.IsNullOrWhiteSpace(duration))
                {
                    extent = await page
                        .GetOptionalTextAsync("div.ng-star-inserted:has(h3[translate='section-jobb-main-content.employment-type']) > span");
                }

                var salaryType = await page.GetOptionalTextAsync("div.salary-type");
                var published = await page.GetOptionalTextAsync("div h2 > span[translate='section-jobb-about.published']");

                var applicationUrlLocator = page.Locator("a.apply-by-link-external");
                var applicationEmailLocator = page.Locator("a.apply-by-email");

                string? applicationUrl = null;
                if (await applicationUrlLocator.CountAsync() > 0)
                {
                    applicationUrl = await applicationUrlLocator.First.GetAttributeAsync("href");
                }
                else if (await applicationEmailLocator.CountAsync() > 0)
                {
                    applicationUrl = await applicationEmailLocator.First.InnerTextAsync();
                }

                if (string.IsNullOrWhiteSpace(applicationUrl))
                {
                    throw new InvalidOperationException("Missing application URL or email");
                }

                var rawJob = new RawJob
                {
                    Title = title,
                    CompanyName = company,
                    RoleName = role,
                    Location = ExtractAfterColon(location),
                    Extent = extent,
                    Duration = duration,
                    ApplicationDeadline = applicationDeadline,
                    ApplicationUrl = applicationUrl,
                    Description = desc,
                    SalaryType = ExtractAfterColon(salaryType),
                    Published = ExtractAfterColon(published),
                    ListingId = ExtractAfterColon(listingId),
                    ListingUrl = listingUrl,
                    Source = Source.Platsbanken,
                    ProcessedStatus = false,
                    ScrapeRun = scrapeRun
                };

                return ScrapeResultResponse.Success(rawJob);
            }
            catch (TimeoutException e)
            {
                _logger.LogError(e, "Timeout for {Url}", listingUrl);
                return ScrapeResultResponse.Failure(e, isRetryable: true);
            }
            catch (PlaywrightException e)
            {
                _logger.LogError(e, "Playwright error for {Url}", listingUrl);
                return ScrapeResultResponse.Failure(e, isRetryable: false);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Missing required field for {Url}", listingUrl);
                return ScrapeResultResponse.Failure(e, isRetryable: false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error for {Url}", listingUrl);
                return ScrapeResultResponse.Failure(e, isRetryable: false);
            }
        }

        private async Task<ScrapeResultResponse> ScrapeListingsAsync(IReadOnlyList<ILocator> links, IBrowser browser, ScrapeRun scrapeRun, int lastJobListingId)
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
                    if (!parseSuccess)
                    {
                        _logger.LogInformation("Job listing id parse failed for {jobListingID}", currentListingIdStr);
                    }
                    else if (parseSuccess && currentListingId == lastJobListingId)
                    {
                        _logger.LogInformation("Job {jobId} already scraped, exiting run.", currentListingId);
                        shouldContinue = false;
                        break;
                    }

                    listUrl = _platsbankenBaseUrl + href;
                    listId = currentListingId;

                    _logger.LogInformation("Scraping job {jobId}", currentListingId);

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
                        Source = Source.Platsbanken,
                        ProcessedStatus = false,
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

            return new ScrapeResultResponse
            {
                Jobs = jobList,
                FailedJobs = failedJobList,
                ShouldContinue = shouldContinue
            };
        }

        private static string ExtractAfterColon(string input)
        {
            var parts = input.Split(":", 2);
            return parts.Length > 1 ? parts[1].Trim() : input;
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
                StartedAt = DateTime.UtcNow,
                Status = Status.InProgress
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
            scrapeRun.Status = errorOccured ? Status.Failed : Status.Completed;
            scrapeRun.FinishedAt = DateTime.UtcNow;
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

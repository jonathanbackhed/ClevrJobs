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

        public async Task ScrapePlatsbankenAsync(IJobRepository jobRepository, IMessageQueue messageQueue, CancellationToken cancellationToken)
        {
            var scrapeRun = await CreateScrapeRunAsync(jobRepository);
            if (scrapeRun is null)
            {
                _logger.LogError("Error creating ScrapeRun, exiting.");
                return;
            }

            var lastJob = await jobRepository.GetLastPublishedRawJob();
            var tryParseListingId = int.TryParse(lastJob?.ListingId, out var lastJobListingId);
            if (!tryParseListingId)
            {
                lastJobListingId = -1;
            }

            _logger.LogInformation("Beginning new scrape.");

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });

            IPage? page = null;
            IPage? subPage = null;

            try
            {
                page = await browser.NewPageAsync();

                await page.GotoAsync(_platsbankenRequestUrl, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle,
                    Timeout = 30000
                });

                await page.Locator("#button-basic").ClickAsync();

                await page.WaitForSelectorAsync("ul li button", new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

                await page.Locator("ul li > button span")
                          .Filter(new() { HasText = "publiceringsdatum" })
                          .ClickAsync();

                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                await page.WaitForSelectorAsync("div.header-container h3 a");

                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("Cancellation requested, stopping scraping process.");
                        break;
                    }

                    bool shouldContinue = true;

                    var links = await page.Locator("div.header-container h3 a").AllAsync();

                    foreach (var item in links)
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

                        string listingUrl = _platsbankenBaseUrl + href;

                        try
                        {
                            var context = await browser.NewContextAsync();
                            subPage = await context.NewPageAsync();

                            _logger.LogInformation("Scraping job {jobId}", currentListingId);
                            var result = await ScrapePlatsbankenListingAsync(subPage, listingUrl, scrapeRun);

                            if (result.RawJob != null)
                            {
                                await jobRepository.AddRawJob(result.RawJob);
                                await messageQueue.PublishAsync(new JobScrapedEvent()
                                {
                                    RawJobId = result.RawJob.Id
                                });

                                scrapeRun.ScrapedJobs++;

                                _logger.LogInformation("Successfully scraped listing {Id}", currentListingId);
                            }
                            else
                            {
                                var failedScrape = new FailedScrape
                                {
                                    ListingUrl = listingUrl,
                                    ListingId = currentListingId.ToString(),
                                    ScrapeRun = scrapeRun,
                                    ErrorMessage = result.ErrorMessage ?? "Unknown error",
                                    ErrorType = result.ErrorType ?? "Unknown",
                                    Status = result.IsRetryable ? FailedStatus.ReadyForRetry : FailedStatus.Failed,
                                };
                                await jobRepository.AddFailedScrape(failedScrape);

                                scrapeRun.FailedJobs++;

                                _logger.LogWarning("Failed to scrape listing {Id}.", currentListingId);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Unexpected error during scraping for {Id}", currentListingId);
                        }
                        finally
                        {
                            if (subPage != null)
                            {
                                await subPage.CloseAsync();
                            }

                            await Task.Delay(Random.Shared.Next(4000, 16000));
                        }
                    }

                    if (!shouldContinue)
                    {
                        break;
                    }

                    var nextPageStatus = await GoToNextpageAsync(page);
                    if (!nextPageStatus)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error during scraping for scrapeRun {ScrapeId}", scrapeRun.Id);
            }

            var endScrapeResult = await EndScrapeRunAsync(jobRepository, scrapeRun);
            if (!endScrapeResult)
            {
                _logger.LogError("Failed to end ScrapeRun. Scrape id: {scrapeRunId}", scrapeRun.Id);
            }
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

        private async Task<bool> EndScrapeRunAsync(IJobRepository jobRepository, ScrapeRun scrapeRun)
        {
            scrapeRun.Status = Status.Completed;
            scrapeRun.FinishedAt = DateTime.UtcNow;

            var success = await jobRepository.UpdateScrapeRun(scrapeRun);
            if (!success)
            {
                return false;
            }

            return true;
        }
    }
}

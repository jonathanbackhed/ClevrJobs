using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories
{
    public interface IJobRepository
    {
        Task<ScrapeRun?> GetLastScrapeRun();
        Task<RawJob?> GetLastAddedRawJobWithQuery(string query);
        Task<bool> AddRawJob(RawJob rawJob);
        Task AddMultipleRawJobs(IEnumerable<RawJob> rawJobs);
        Task AddMultipleFailedScrapes(IEnumerable<FailedScrape> failedScrapes);
        Task AddFailedScrape(FailedScrape failedScrape);
        Task AddScrapeRun(ScrapeRun scrapeRun);
        Task UpdateScrapeRun(ScrapeRun scrapeRun);
        Task<ICollection<RawJob>> GetUnprocessedRawJobsfromScrapeRunId(int id);
        Task<RawJob?> GetRawJobById(int id);
        Task UpdateRawJobs(ICollection<RawJob> rawJobs);
        Task MarkRawJobAsProcessed(RawJob rawJob);
        Task<List<FailedScrape>> GetFailedScrapesForRetryAsync();
        Task UpdateFailedScrape(FailedScrape failedScrape);
    }
}

using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories
{
    public interface IJobRepository
    {
        Task<ScrapeRun?> GetLastScrapeRun();
        Task<RawJob?> GetLastPublishedRawJob();
        Task<bool> AddRawJob(RawJob rawJob);
        Task<bool> AddMultipleRawJobs(IEnumerable<RawJob> rawJobs);
        Task<bool> AddMultipleFailedScrapes(IEnumerable<FailedScrape> failedScrapes);
        Task<bool> AddScrapeRun(ScrapeRun scrapeRun);
        Task<bool> UpdateScrapeRun(ScrapeRun scrapeRun);
        Task<ICollection<RawJob>> GetUnprocessedRawJobsfromScrapeRunId(int id);
        Task<bool> UpdateRawJobs(ICollection<RawJob> rawJobs);
        Task<bool> MarkRawJobAsProcessed(RawJob rawJob);
    }
}

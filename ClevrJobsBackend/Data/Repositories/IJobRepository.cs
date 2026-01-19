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
        Task<bool> AddScrapeRun(ScrapeRun scrapeRun);
        Task<bool> UpdateScrapeRun(ScrapeRun scrapeRun);
    }
}

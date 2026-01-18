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
        Task AddScrapeRun(ScrapeRun scrapeRun);
        Task UpdateScrapeRun(ScrapeRun scrapeRun);
    }
}

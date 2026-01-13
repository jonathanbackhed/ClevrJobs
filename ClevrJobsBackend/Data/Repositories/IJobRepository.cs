using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories
{
    public interface IJobRepository
    {
        Task<ScrapeRun?> GetLastScrapeRun();
        Task<bool> AddRawJob(RawJob rawJob);
    }
}

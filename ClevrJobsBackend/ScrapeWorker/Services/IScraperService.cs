using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapeWorker.Services
{
    public interface IScraperService
    {
        // bool = success, int = scraperunid
        Task<(bool, int)> ScrapePlatsbankenAsync(IJobRepository jobRepository, CancellationToken cancellationToken);
    }
}

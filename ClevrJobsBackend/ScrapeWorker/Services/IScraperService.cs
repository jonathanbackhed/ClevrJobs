using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapeWorker.Services
{
    public interface IScraperService
    {
        Task ScrapePlatsbankenAsync(IJobRepository jobRepository, CancellationToken cancellationToken);
    }
}

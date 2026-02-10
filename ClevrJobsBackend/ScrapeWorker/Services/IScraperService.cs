using Data.Repositories;
using Queue.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Workers.Services
{
    public interface IScraperService
    {
        Task<(bool success, int scrapeRunId)> ScrapePlatsbankenAsync(IJobRepository jobRepository, CancellationToken cancellationToken);
        Task RetryFailedScrapesPlatsbankenAsync(IJobRepository jobRepository, IMessageQueue messageQueue, CancellationToken cancellationToken);
    }
}

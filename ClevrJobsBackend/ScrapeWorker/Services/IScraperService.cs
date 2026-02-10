using Data.Repositories;
using Queue.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Workers.Services
{
    public interface IScraperService
    {
        Task ScrapePlatsbankenAsync(IJobRepository jobRepository, IMessageQueue messageQueue, CancellationToken cancellationToken);
        Task RetryFailedScrapesPlatsbankenAsync(IJobRepository jobRepository, IMessageQueue messageQueue, CancellationToken cancellationToken);
    }
}

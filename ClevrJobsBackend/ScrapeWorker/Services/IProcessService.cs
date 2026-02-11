using Data.Models;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Workers.DTOs.Responses;

namespace Workers.Services
{
    public interface IProcessService
    {
        Task ProcessRawJob(RawJob rawJobs, IProcessRepository processRepository, IJobRepository jobRepository);
        Task RetryFailedProcesses(IProcessRepository processRepository, IJobRepository jobRepository, CancellationToken cancellationToken);
    }
}

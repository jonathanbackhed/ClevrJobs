using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories
{
    public interface IProcessRepository
    {
        Task<Prompt?> GetLatestActivePromptAsync();
        Task<Prompt?> GetPromptByIdAsync(int id);
        Task<bool> AddProcessRun(ProcessRun processRun);
        Task<bool> UpdateProcessRun(ProcessRun processRun);
        Task<bool> AddMultipleProcessedJobs(IEnumerable<ProcessedJob> processedJobs);
        Task AddProcessedJob(ProcessedJob processedJobs);
        Task<bool> AddMultipleFailedProcesses(IEnumerable<FailedProcess> failedProcesses);
        Task AddFailedProcess(FailedProcess failedProcess);
        Task<(List<ProcessedJob> items, int totalCount)> GetFullProcessedJobsByLatestAsync(int page, int pageSize);
        Task<ProcessedJob?> GetFullProcessedJobByIdAsync(int id);
        Task<bool> JobExists(int id);
        Task AddJobReport(JobReport jobReport);
    }
}

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
        Task AddProcessRun(ProcessRun processRun);
        Task UpdateProcessRun(ProcessRun processRun);
        Task AddMultipleProcessedJobs(IEnumerable<ProcessedJob> processedJobs);
        Task AddProcessedJob(ProcessedJob processedJobs);
        Task AddMultipleFailedProcesses(IEnumerable<FailedProcess> failedProcesses);
        Task AddFailedProcess(FailedProcess failedProcess);
        Task<(List<ProcessedJob> items, int totalCount)> GetFullProcessedJobsByLatestAsync(int page, int pageSize);
        Task<ProcessedJob?> GetFullProcessedJobByIdAsync(int id);
        Task<bool> JobExists(int id);
        Task AddJobReport(JobReport jobReport);
        Task<JobReport?> GetJobReportByJobAndIpOrUserId(int jobId, string? identifier);
        Task<List<FailedProcess>> GetFailedProcessesForRetryAsync();
        Task UpdateFailedProcess(FailedProcess failedProcess);
    }
}

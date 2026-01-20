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
    }
}

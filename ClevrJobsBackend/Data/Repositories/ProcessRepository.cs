using Data.Enums;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories
{
    public class ProcessRepository : IProcessRepository
    {
        private readonly AppDbContext _dbc;

        public ProcessRepository(AppDbContext dbc)
        {
            _dbc = dbc;
        }

        public async Task<bool> AddMultipleFailedProcesses(IEnumerable<FailedProcess> failedProcesses)
        {
            await _dbc.FailedProcesses.AddRangeAsync(failedProcesses);

            return await _dbc.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddMultipleProcessedJobs(IEnumerable<ProcessedJob> processedJobs)
        {
            await _dbc.ProcessedJobs.AddRangeAsync(processedJobs);

            return await _dbc.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddProcessRun(ProcessRun processRun)
        {
            await _dbc.ProcessRuns.AddAsync(processRun);

            return await _dbc.SaveChangesAsync() > 0;
        }

        public async Task<ProcessedJob?> GetFullProcessedJobByIdAsync(int id)
        {
            var job = await _dbc.ProcessedJobs
                .AsNoTracking()
                .Include(i => i.RawJob)
                .Include(i => i.ProcessRun)
                .FirstOrDefaultAsync(p => p.Id == id);

            return job;
        }

        public async Task<Prompt?> GetLatestActivePromptAsync()
        {
            var prompt = await _dbc.Prompts
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            return prompt;
        }

        public async Task<(List<ProcessedJob> items, int totalCount)> GetFullProcessedJobsByLatestAsync(int page, int pageSize)
        {
            var query = _dbc.ProcessedJobs
                .AsNoTracking()
                .Include(i => i.RawJob)
                .Include(i => i.ProcessRun)
                .OrderByDescending(o => o.RawJob.ScrapeRunId)
                .ThenByDescending(o => o.RawJob.ListingId);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Prompt?> GetPromptByIdAsync(int id)
        {
            var prompt = await _dbc.Prompts.Where(p => p.Id == id).FirstOrDefaultAsync();

            return prompt;
        }

        public async Task<bool> UpdateProcessRun(ProcessRun processRun)
        {
            _dbc.ProcessRuns.Update(processRun);

            return await _dbc.SaveChangesAsync() > 0;
        }

        public async Task AddJobReport(JobReport jobReport)
        {
            await _dbc.JobReports.AddAsync(jobReport);
            await _dbc.SaveChangesAsync();
        }

        public async Task<bool> JobExists(int id)
        {
            var exists = await _dbc.ProcessedJobs.AnyAsync(j => j.Id == id);
            return exists;
        }

        public async Task AddFailedProcess(FailedProcess failedProcess)
        {
            await _dbc.FailedProcesses.AddAsync(failedProcess);
            await _dbc.SaveChangesAsync();
        }

        public async Task AddProcessedJob(ProcessedJob processedJob)
        {
            await _dbc.ProcessedJobs.AddAsync(processedJob);
            await _dbc.SaveChangesAsync();
        }

        public async Task<List<FailedProcess>> GetFailedProcessesForRetryAsync()
        {
            var failed = await _dbc.FailedProcesses
                .Include(i => i.RawJob)
                .Include(i => i.ProcessRun)
                .Where(f => f.Status == FailedStatus.ReadyForRetry)
                .ToListAsync();

            return failed;
        }

        public async Task<bool> UpdateFailedProcess(FailedProcess failedProcess)
        {
            var rowsAffected = await _dbc.FailedProcesses
                .Where(x => x.Id == failedProcess.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.RetryCount, p => p.RetryCount + 1)
                    .SetProperty(p => p.RetriedAt, failedProcess.RetriedAt)
                    .SetProperty(p => p.Status, failedProcess.Status)
                    .SetProperty(p => p.ErrorType, failedProcess.ErrorType)
                    .SetProperty(p => p.ErrorMessage, failedProcess.ErrorMessage)
                );

            return rowsAffected > 0;
        }
    }
}

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

        public async Task<Prompt?> GetLatestActivePromptAsync()
        {
            var prompt = await _dbc.Prompts.Where(p => p.IsActive)
                                     .OrderByDescending(p => p.CreatedAt)
                                     .FirstOrDefaultAsync();

            return prompt;
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
    }
}

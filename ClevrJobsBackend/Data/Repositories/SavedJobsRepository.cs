using Data.Models;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories
{
    public class SavedJobsRepository : ISavedJobsRepository
    {
        private readonly AppDbContext _dbc;

        public SavedJobsRepository(AppDbContext dbc)
        {
            _dbc = dbc;
        }

        public async Task<SavedJob?> SaveJobAsync(SavedJob savedJob, string userId)
        {
            savedJob.UserId = userId;
            await _dbc.SavedJobs.AddAsync(savedJob);
            await _dbc.SaveChangesAsync();
            return savedJob;
        }

        public async Task<bool> DeleteSavedJobAsync(Guid id, string userId)
        {
            var entity = await GetSavedJobByIdAsync(id, userId);
            if (entity is null) return false;

            _dbc.SavedJobs.Remove(entity);
            await _dbc.SaveChangesAsync();
            return true;
        }

        public async Task<(List<SavedJob>, int)> GetAllSavedJobsAsync(int page, int pageSize, string userId)
        {
            var query = _dbc.SavedJobs
                .AsNoTracking()
                .Include(i => i.ProcessedJob)
                    .ThenInclude(i => i!.RawJob)
                .Include(i => i.ProcessedJob)
                    .ThenInclude(i => i!.ProcessRun)
                .Where(w => w.UserId == userId)
                .OrderByDescending(o => o.SavedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<(int processedJobId, Guid savedJobId)>> GetAllSavedIdsAsync(string userId)
        {
            var result = await _dbc.SavedJobs
                .Where(w => w.UserId == userId)
                .Select(s => new { s.ProcessedJobId, s.Id })
                .ToListAsync();

            return result.Select(s => (s.ProcessedJobId, s.Id)).ToList();
        }

        public async Task<SavedJob?> GetSavedJobByIdAsync(Guid id, string userId)
        {
            return await _dbc.SavedJobs.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        }
    }
}

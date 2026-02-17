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

        public async Task<SavedJob> CreateForCurrentUserAsync(SavedJob entity, string userId)
        {
            entity.UserId = userId;
            await _dbc.SavedJobs.AddAsync(entity);
            await _dbc.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteForCurrentUserAsync(Guid id, string userId)
        {
            var entity = await GetByIdForCurrentUserAsync(id, userId);
            if (entity is null) return false;

            _dbc.SavedJobs.Remove(entity);
            await _dbc.SaveChangesAsync();
            return true;
        }

        public async Task<(List<SavedJob>, int)> GetAllForCurrentUserAsync(int page, int pageSize, string userId)
        {
            var query = _dbc.SavedJobs
                .AsNoTracking()
                .Include(i => i.ProcessedJob)
                .ThenInclude(i => i.RawJob)
                .Where(w => w.UserId == userId)
                .OrderByDescending(o => o.SavedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<(int processedJobId, Guid savedJobId)>> GetAllSavedProcessedJobIdsForUserAsync(string userId)
        {
            var result = await _dbc.SavedJobs
                .Where(w => w.UserId == userId && w.ProcessedJobId != null)
                .Select(s => new { s.ProcessedJobId, s.Id })
                .ToListAsync();

            return result.Select(s => ((int)s.ProcessedJobId!, s.Id)).ToList();
        }

        public async Task<SavedJob?> GetByIdForCurrentUserAsync(Guid id, string userId)
        {
            return await _dbc.SavedJobs.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        }

        public async Task<SavedJob?> UpdateForCurrentUserAsync(SavedJob entity, string userId)
        {
            var rowsAffected = await _dbc.SavedJobs
                .Where(w => w.Id == entity.Id && w.UserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.HaveApplied, entity.HaveApplied)
                    .SetProperty(p => p.ApplicationStatus, entity.ApplicationStatus)
                    .SetProperty(p => p.RejectReason, entity.RejectReason)
                    .SetProperty(p => p.Notes, entity.Notes)
                    .SetProperty(p => p.Title, entity.Title)
                    .SetProperty(p => p.Description, entity.Description)
                    .SetProperty(p => p.CompanyName, entity.CompanyName)
                    .SetProperty(p => p.Location, entity.Location)
                    .SetProperty(p => p.ApplicationDeadline, entity.ApplicationDeadline)
                    .SetProperty(p => p.ListingUrl, entity.ListingUrl)
                );

            return rowsAffected > 0 ? entity : null;
        }
    }
}

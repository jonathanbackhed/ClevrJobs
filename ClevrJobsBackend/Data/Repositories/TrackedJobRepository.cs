using Data.Models;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories
{
    public class TrackedJobRepository : ITrackedJobRepository
    {
        private readonly AppDbContext _dbc;

        public TrackedJobRepository(AppDbContext dbc)
        {
            _dbc = dbc;
        }

        public async Task<TrackedJob> CreateAsync(TrackedJob trackedJob, string userId)
        {
            trackedJob.UserId = userId;
            await _dbc.TrackedJobs.AddAsync(trackedJob);
            await _dbc.SaveChangesAsync();
            return trackedJob;
        }

        public async Task<bool> DeleteAsync(Guid id, string userId)
        {
            var entity = await GetByIdAsync(id, userId);
            if (entity is null) return false;

            _dbc.TrackedJobs.Remove(entity);
            await _dbc.SaveChangesAsync();
            return true;
        }

        public async Task<(List<TrackedJob> items, int totalCount)> GetAllAsync(int page, int pageSize, string userId)
        {
            var query = _dbc.TrackedJobs
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .OrderByDescending(o => o.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<TrackedJob?> GetByIdAsync(Guid id, string userId)
        {
            return await _dbc.TrackedJobs.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        }

        public async Task UpdateAsync() => await _dbc.SaveChangesAsync();
    }
}

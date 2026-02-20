using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories.Interfaces
{
    public interface ITrackedJobRepository
    {
        Task<(List<TrackedJob> items, int totalCount)> GetAllAsync(int page, int pageSize, string userId);
        Task<TrackedJob?> GetByIdAsync(Guid id, string userId);
        Task<TrackedJob> CreateAsync(TrackedJob trackedJob, string userId);
        Task UpdateAsync();
        Task<bool> DeleteAsync(Guid id, string userId);
    }
}

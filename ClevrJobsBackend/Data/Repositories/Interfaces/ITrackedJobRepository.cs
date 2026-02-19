using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories.Interfaces
{
    public interface ITrackedJobRepository
    {
        Task<(List<TrackedJob> items, int totalCount)> GetAllTrackedJobsAsync(int page, int pageSize, string userId);
        Task<TrackedJob?> GetTrackedJobByIdAsync(Guid id, string userId);
        Task<TrackedJob?> CreateNewTrackedJobAsync(TrackedJob trackedJob, string userId);
        Task<TrackedJob?> UpdateTrackedJobAsync(TrackedJob trackedJob, string userId);
        Task<bool> DeleteTrackedJob(Guid id, string userId);
    }
}

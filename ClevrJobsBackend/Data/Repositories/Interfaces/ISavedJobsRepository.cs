using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories.Interfaces
{
    public interface ISavedJobsRepository
    {
        Task<(List<SavedJob> items, int totalCount)> GetAllSavedJobsAsync(int page, int pageSize, string userId);
        Task<List<(int processedJobId, Guid savedJobId)>> GetAllSavedIdsAsync(string userId);
        Task<SavedJob?> GetSavedJobByIdAsync(Guid id, string userId);
        Task<SavedJob?> SaveJobAsync(SavedJob savedJob, string userId);
        Task<bool> DeleteSavedJobAsync(Guid id, string userId);
    }
}

using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories.Interfaces
{
    public interface ISavedJobsRepository : IUserContextRepository<SavedJob>
    {
        Task<List<(int processedJobId, Guid savedJobId)>> GetAllSavedProcessedJobIdsForUserAsync(string userId);
    }
}

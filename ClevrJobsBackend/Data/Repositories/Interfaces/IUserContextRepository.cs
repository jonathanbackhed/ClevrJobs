using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories.Interfaces
{
    public interface IUserContextRepository<T> where T : class
    {
        Task<List<T>> GetAllForCurrentUserAsync(string userId);
        Task<T?> GetByIdForCurrentUserAsync(Guid id, string userId);
        Task<T> CreateForCurrentUserAsync(T entity, string userId);
        Task<T?> UpdateForCurrentUserAsync(T entity, string userId);
        Task<bool> DeleteForCurrentUserAsync(Guid id, string userId);
    }
}

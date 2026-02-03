using Api.DTOs.Responses;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Data
{
    public class JobCache : IJobCache
    {
        private readonly IMemoryCache _cache;

        public JobCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void AddJob(JobListingResponse job)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                Size = 1,
                SlidingExpiration = TimeSpan.FromMinutes(15),
                Priority = CacheItemPriority.High
            };
            _cache.Set(job.Id, job, cacheOptions);
        }

        public void AddJobs(PagedResult<JobListingMiniResponse> jobs)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                Size = jobs.Items.Count,
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                Priority = CacheItemPriority.Low
            };
            _cache.Set(jobs.Page, jobs, cacheOptions);
        }

        public JobListingResponse? GetJob(int id)
        {
            _cache.TryGetValue(id, out JobListingResponse? job);
            return job;
        }

        public PagedResult<JobListingMiniResponse>? GetJobs(int page)
        {
            _cache.TryGetValue(page, out PagedResult<JobListingMiniResponse>? jobs);
            return jobs;
        }
    }
}

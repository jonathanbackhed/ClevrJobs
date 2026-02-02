using Api.Models;
using Api.Models.Dto;
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

        public void AddJob(JobListingDto job)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                Size = 1,
                SlidingExpiration = TimeSpan.FromMinutes(15),
                Priority = CacheItemPriority.High
            };
            _cache.Set(job.Id, job, cacheOptions);
        }

        public void AddJobs(PagedResult<JobListingMiniDto> jobs)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                Size = jobs.Items.Count,
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                Priority = CacheItemPriority.Low
            };
            _cache.Set(jobs.Page, jobs, cacheOptions);
        }

        public JobListingDto? GetJob(int id)
        {
            _cache.TryGetValue(id, out JobListingDto? job);
            return job;
        }

        public PagedResult<JobListingMiniDto>? GetJobs(int page)
        {
            _cache.TryGetValue(page, out PagedResult<JobListingMiniDto>? jobs);
            return jobs;
        }
    }
}

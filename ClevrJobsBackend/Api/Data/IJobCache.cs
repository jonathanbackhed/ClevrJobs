using Api.Models;
using Api.Models.Dto;

namespace Api.Data
{
    public interface IJobCache
    {
        void AddJobs(PagedResult<JobListingMiniDto> jobs);
        void AddJob(JobListingDto job);
        PagedResult<JobListingMiniDto>? GetJobs(int page);
        JobListingDto? GetJob(int id);
    }
}

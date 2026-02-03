using Api.DTOs.Responses;

namespace Api.Data
{
    public interface IJobCache
    {
        void AddJobs(PagedResult<JobListingMiniResponse> jobs);
        void AddJob(JobListingResponse job);
        PagedResult<JobListingMiniResponse>? GetJobs(int page);
        JobListingResponse? GetJob(int id);
    }
}

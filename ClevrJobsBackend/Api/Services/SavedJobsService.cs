using Api.DTOs.Requests;
using Api.DTOs.Responses;
using Data.Enums;
using Data.Models;
using Data.Repositories;
using Data.Repositories.Interfaces;

namespace Api.Services
{
    public class SavedJobsService
    {
        private readonly ISavedJobsRepository _savedJobsRepository;
        private readonly IProcessRepository _processRepository;

        public SavedJobsService(ISavedJobsRepository savedJobsRepository, IProcessRepository processRepository)
        {
            _savedJobsRepository = savedJobsRepository;
            _processRepository = processRepository;
        }

        public async Task<PagedResult<SavedJobResponse>> GetAllSavedJobs(int page, int pageSize, string userId)
        {
            var (items, totalCount) = await _savedJobsRepository.GetAllSavedJobsAsync(page, pageSize, userId);

            var savedJobs = items.Select(i => new SavedJobResponse
            {
                Id = i.Id,
                JobListingMini = new JobListingMiniResponse
                {
                    Title = i.ProcessedJob!.RawJob.Title,
                    CompanyName = i.ProcessedJob.RawJob.CompanyName,
                    Location = i.ProcessedJob.RawJob.Location,
                    Extent = i.ProcessedJob.RawJob.Extent,
                    Duration = i.ProcessedJob.RawJob.Duration,
                    ApplicationDeadline = i.ProcessedJob.RawJob.ApplicationDeadline,
                    Source = i.ProcessedJob.RawJob.Source,
                    ProcessedAt = (DateTime)i.ProcessedJob.ProcessRun.FinishedAt!,

                    Id = i.ProcessedJobId,
                    Description = i.ProcessedJob.Description,
                    RequiredTechnologies = i.ProcessedJob.RequiredTechnologies,
                    CompetenceRank = i.ProcessedJob.CompetenceRank
                },
                SavedAt = i.SavedAt,
            }).ToList();

            return new PagedResult<SavedJobResponse>
            {
                Items = savedJobs,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<SavedJob?> SaveJob(int jobId, string userId)
        {
            var exists = await _processRepository.JobExists(jobId);
            if (!exists) return null;

            var newSavedJob = new SavedJob
            {
                UserId = userId,
                ProcessedJobId = jobId,
                SavedAt = DateTime.UtcNow
            };

            var result = await _savedJobsRepository.SaveJobAsync(newSavedJob, userId);

            return result;
        }
    }
}

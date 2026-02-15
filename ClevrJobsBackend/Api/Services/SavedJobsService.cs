using Api.DTOs.Requests;
using Data.Enums;
using Data.Models;
using Data.Repositories;
using Data.Repositories.Interfaces;

namespace Api.Services
{
    public class SavedJobsService
    {
        private readonly IUserContextRepository<SavedJob> _savedJobsRepository;
        private readonly IProcessRepository _processRepository;

        public SavedJobsService(IUserContextRepository<SavedJob> savedJobsRepository, IProcessRepository processRepository)
        {
            _savedJobsRepository = savedJobsRepository;
            _processRepository = processRepository;
        }

        public async Task<SavedJob> CreateCustomJob(SavedJobRequest savedJob, string userId)
        {
            var newSavedJob = new SavedJob
            {
                UserId = userId,
                SaveType = SaveType.ManuallyAdded,
                ProcessedJobId = null,
                ProcessedJob = null,

                HaveApplied = savedJob.HaveApplied,
                ApplicationStatus = savedJob.ApplicationStatus,
                RejectReason = savedJob.RejectReason,
                Notes = savedJob.Notes,

                Title = savedJob.Title,
                Description = savedJob.Description,
                CompanyName = savedJob.CompanyName,
                Location = savedJob.Location,
                ApplicationDeadline = savedJob.ApplicationDeadline,
                ListingUrl = savedJob.ListingUrl
            };

            var result = await _savedJobsRepository.CreateForCurrentUserAsync(newSavedJob, userId);

            return result;
        }

        public async Task<SavedJob?> SaveExistingJob(int jobId, string userId)
        {
            var exists = await _processRepository.JobExists(jobId);
            if (!exists) return null;

            var newSavedJob = new SavedJob
            {
                UserId = userId,
                SaveType = SaveType.SavedFromListing,
                ProcessedJobId = jobId,

                HaveApplied = false,
                ApplicationStatus = ApplicationStatus.NotApplied
            };

            var result = await _savedJobsRepository.CreateForCurrentUserAsync(newSavedJob, userId);

            return result;
        }

        public async Task<SavedJob?> UpdateSavedJob(SavedJobRequest savedJob, string userId)
        {
            var existing = await _savedJobsRepository.GetByIdForCurrentUserAsync(savedJob.Id, userId);
            if (existing is null) return null;

            existing.HaveApplied = savedJob.HaveApplied;
            existing.ApplicationStatus = savedJob.ApplicationStatus;
            existing.RejectReason = savedJob.RejectReason;
            existing.Notes = savedJob.Notes;

            if (existing.SaveType == SaveType.ManuallyAdded)
            {
                existing.Title = savedJob.Title;
                existing.Description = savedJob.Description;
                existing.CompanyName = savedJob.CompanyName;
                existing.Location = savedJob.Location;
                existing.ApplicationDeadline = savedJob.ApplicationDeadline;
                existing.ListingUrl = savedJob.ListingUrl;
            }

            var result = await _savedJobsRepository.UpdateForCurrentUserAsync(existing, userId);

            return result;
        }
    }
}

using Api.DTOs.Requests;
using Api.DTOs.Responses;
using Data.Enums;
using Data.Models;
using Data.Repositories;
using Data.Repositories.Interfaces;

namespace Api.Services
{
    public class TrackedJobService
    {
        private readonly ILogger<TrackedJobService> _logger;
        private readonly ITrackedJobRepository _trackedJobRepository;
        private readonly IProcessRepository _processRepository;

        public TrackedJobService(ILogger<TrackedJobService> logger, ITrackedJobRepository trackedJobRepository, IProcessRepository processRepository)
        {
            _logger = logger;
            _trackedJobRepository = trackedJobRepository;
            _processRepository = processRepository;
        }

        public async Task<PagedResult<TrackedJobResponse>> GetAllTrackedJobsAsync(int page, int pageSize, string userId)
        {
            var (items, totalCount) = await _trackedJobRepository.GetAllAsync(page, pageSize, userId);

            return new PagedResult<TrackedJobResponse>
            {
                Items = [.. items.Select(MapToResponse)],
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<TrackedJobResponse> CreateTrackedJobAsync(TrackedJobRequest trackedJobReq, string userId)
        {
            var trackedJob = new TrackedJob
            {
                UserId = userId,
                SaveType = SaveType.ManuallyAdded,
                ApplicationStatus = trackedJobReq.ApplicationStatus,
                RejectReason = trackedJobReq.RejectReason,
                Notes = trackedJobReq.Notes,
                CreatedAt = DateTime.UtcNow,
                ApplyDate =  trackedJobReq.ApplyDate,
                HaveCalled = trackedJobReq.HaveCalled,
                SpontaneousApplication = trackedJobReq.SpontaneousApplication,

                Title = trackedJobReq.Title,
                CompanyName = trackedJobReq.CompanyName,
                Location = trackedJobReq.Location,
                ApplicationDeadline = trackedJobReq.ApplicationDeadline,
                ListingUrl = trackedJobReq.ListingUrl
            };

            var created = await _trackedJobRepository.CreateAsync(trackedJob, userId);

            return MapToResponse(created);
        }

        public async Task<TrackedJobResponse?> CreateTrackedJobFromExistingAsync(int id, string userId)
        {
            var existing = await _processRepository.GetFullProcessedJobByIdAsync(id);
            if (existing is null) return null;

            var trackedJob = new TrackedJob
            {
                UserId = userId,
                SaveType = SaveType.SavedFromListing,
                ProcessedJobId = id,
                ApplicationStatus = ApplicationStatus.NotApplied,
                CreatedAt = DateTime.UtcNow,
                HaveCalled = false,
                SpontaneousApplication = false,

                Title = existing.RawJob.Title,
                CompanyName = existing.RawJob.CompanyName,
                Location = existing.RawJob.Location,
                ApplicationDeadline = existing.RawJob.ApplicationDeadline,
                ListingUrl = existing.RawJob.ListingUrl
            };

            var created = await _trackedJobRepository.CreateAsync(trackedJob, userId);

            return MapToResponse(created);
        }

        public async Task<TrackedJobResponse?> UpdateTrackedJobAsync(Guid id, TrackedJobRequest trackedJobReq, string userId)
        {
            var existing = await _trackedJobRepository.GetByIdAsync(id, userId);
            if (existing is null) return null;

            existing.ApplicationStatus = trackedJobReq.ApplicationStatus;
            existing.RejectReason = trackedJobReq.RejectReason;
            existing.Notes = trackedJobReq.Notes;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.ApplyDate = trackedJobReq.ApplyDate;
            existing.HaveCalled = trackedJobReq.HaveCalled;
            existing.SpontaneousApplication = trackedJobReq.SpontaneousApplication;

            existing.Title = trackedJobReq.Title;
            existing.CompanyName = trackedJobReq.CompanyName;
            existing.Location = trackedJobReq.Location;
            existing.ApplicationDeadline = trackedJobReq.ApplicationDeadline;
            existing.ListingUrl = trackedJobReq.ListingUrl;

            await _trackedJobRepository.UpdateAsync();
            return MapToResponse(existing);
        }

        private static TrackedJobResponse MapToResponse(TrackedJob trackedJob)
        {
            var response = new TrackedJobResponse
            {
                Id = trackedJob.Id,
                SaveType = trackedJob.SaveType,
                ProcessedJobId = trackedJob.ProcessedJobId,
                ApplicationStatus = trackedJob.ApplicationStatus,
                RejectReason = trackedJob.RejectReason,
                Notes = trackedJob.Notes,
                CreatedAt = trackedJob.CreatedAt,
                UpdatedAt =  trackedJob.UpdatedAt,
                ApplyDate = trackedJob.ApplyDate,
                HaveCalled = trackedJob.HaveCalled,
                SpontaneousApplication = trackedJob.SpontaneousApplication,
                
                Title = trackedJob.Title,
                CompanyName = trackedJob.CompanyName,
                Location = trackedJob.Location,
                ApplicationDeadline = trackedJob.ApplicationDeadline,
                ListingUrl = trackedJob.ListingUrl
            };

            return response;
        }
    }
}

using Api.DTOs.Requests;
using Api.Services;
using Asp.Versioning;
using Data.Models;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ISavedJobsRepository _savedJobsRepository;
        private readonly SavedJobsService _savedJobsService;

        public UserController(ILogger<UserController> logger, ISavedJobsRepository savedJobsRepository, SavedJobsService savedJobsService)
        {
            _logger = logger;
            _savedJobsRepository = savedJobsRepository;
            _savedJobsService = savedJobsService;
        }

        [HttpGet]
        [Route("saved")]
        public async Task<IActionResult> GetAllSavedJobs([FromQuery] int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var pageSize = 30;

            var savedJobs = await _savedJobsService.GetSavedJobs(page, pageSize, userId);

            return Ok(savedJobs);
        }

        [HttpGet]
        [Route("saved/{savedId}")]
        public async Task<IActionResult> GetSavedJob([FromRoute] Guid savedId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var savedJob = await _savedJobsRepository.GetByIdForCurrentUserAsync(savedId, userId);
            if (savedJob is null) return NotFound(new { error = $"Saved job with id {savedId} not found." });

            return Ok(savedJob);
        }

        [HttpGet]
        [Route("saved/ids")]
        public async Task<IActionResult> GetSavedProcessedJobIds()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var ids = await _savedJobsRepository.GetAllSavedProcessedJobIdsForUserAsync(userId);

            return Ok(ids.Select(s => new {s.processedJobId, s.savedJobId}));
        }

        [HttpPost]
        [Route("saved")]
        public async Task<IActionResult> SaveCustomJob([FromBody] SavedJobRequest savedJob)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var saved = await _savedJobsService.CreateCustomJob(savedJob, userId);

            return Ok(saved);
        }

        [HttpPost]
        [Route("saved/{jobId}")]
        public async Task<IActionResult> SaveExistingJob([FromRoute] int jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var saved = await _savedJobsService.SaveExistingJob(jobId, userId);
            if (saved is null) return NotFound(new { error = $"Job with id {jobId} not found." });

            return Ok(saved);
        }

        [HttpPut]
        [Route("saved")]
        public async Task<IActionResult> UpdateSavedJob([FromBody] SavedJobRequest saveJob)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var updated = await _savedJobsService.UpdateSavedJob(saveJob, userId);
            if (updated is null) return Ok(new { });

            return Ok(updated);
        }

        [HttpDelete]
        [Route("saved/{savedJobId}")]
        public async Task<IActionResult> DeleteSavedJob([FromRoute] Guid savedJobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _savedJobsRepository.DeleteForCurrentUserAsync(savedJobId, userId);
            if (!result) return NotFound(new { error = $"Saved job with id {savedJobId} not found." });

            return Ok(new {});
        }
    }
}

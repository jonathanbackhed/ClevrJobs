using Api.DTOs.Requests;
using Api.Services;
using Asp.Versioning;
using Data.Models;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserContextRepository<SavedJob> _savedJobsRepository;
        private readonly SavedJobsService _savedJobsService;

        public UserController(ILogger<UserController> logger, IUserContextRepository<SavedJob> savedJobsRepository, SavedJobsService savedJobsService)
        {
            _logger = logger;
            _savedJobsRepository = savedJobsRepository;
            _savedJobsService = savedJobsService;
        }

        [HttpGet]
        [Route("saved")]
        public async Task<IActionResult> GetAllSavedJobs()
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var savedJobs = await _savedJobsRepository.GetAllForCurrentUserAsync(userId);

            return Ok(savedJobs);
        }

        [HttpGet]
        [Route("saved/{savedId}")]
        public async Task<IActionResult> GetSavedJob([FromRoute] Guid savedId)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var savedJob = await _savedJobsRepository.GetByIdForCurrentUserAsync(savedId, userId);
            if (savedJob is null) return NotFound(new { error = $"Saved job with id {savedId} not found." });

            return Ok(savedJob);
        }

        [HttpPost]
        [Route("saved")]
        public async Task<IActionResult> AddCustomJob([FromBody] SavedJobRequest savedJob)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var saved = await _savedJobsService.CreateCustomJob(savedJob, userId);

            return Ok(saved);
        }

        [HttpPost]
        [Route("saved/{jobId}")]
        public async Task<IActionResult> AddCustomJob([FromRoute] int jobId)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var saved = await _savedJobsService.SaveExistingJob(jobId, userId);
            if (saved is null) return NotFound(new { error = $"Job with id {jobId} not found." });

            return Ok(saved);
        }

        [HttpPut]
        [Route("saved")]
        public async Task<IActionResult> UpdateJob([FromBody] SavedJobRequest saveJob)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var updated = await _savedJobsService.UpdateSavedJob(saveJob, userId);
            if (updated is null) return Ok(new { });

            return Ok(updated);
        }

        [HttpDelete]
        [Route("saved")]
        public async Task<IActionResult> DeleteJob([FromRoute] Guid jobId)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var result = await _savedJobsRepository.DeleteForCurrentUserAsync(jobId, userId);
            if (!result) return NotFound(new { error = $"Saved job with id {jobId} not found." });

            return Ok(new {});
        }
    }
}

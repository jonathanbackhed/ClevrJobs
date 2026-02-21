using Api.DTOs.Requests;
using Api.Services;
using Asp.Versioning;
using Data.Models;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class SavedController : ControllerBase
    {
        private readonly ILogger<SavedController> _logger;
        private readonly ISavedJobsRepository _savedJobsRepository;
        private readonly SavedJobsService _savedJobsService;

        public SavedController(ILogger<SavedController> logger, ISavedJobsRepository savedJobsRepository, SavedJobsService savedJobsService)
        {
            _logger = logger;
            _savedJobsRepository = savedJobsRepository;
            _savedJobsService = savedJobsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSavedJobs([FromQuery] int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var pageSize = 30;

            var savedJobs = await _savedJobsService.GetAllSavedJobs(page, pageSize, userId);

            return Ok(savedJobs);
        }

        [HttpGet]
        [Route("ids")]
        public async Task<IActionResult> GetSavedIds()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var ids = await _savedJobsRepository.GetAllSavedIdsAsync(userId);

            return Ok(ids.Select(s => new { s.processedJobId, s.savedJobId }));
        }

        [HttpPost]
        [Route("{jobId}")]
        [EnableRateLimiting("saveJobByUser")]
        public async Task<IActionResult> SaveJob([FromRoute] int jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var saved = await _savedJobsService.SaveJob(jobId, userId);
            if (saved is null) return NotFound(new { error = $"Job with id {jobId} not found." });

            return Ok(new { });
        }

        [HttpDelete]
        [Route("{savedJobId}")]
        [EnableRateLimiting("saveJobByUser")]
        public async Task<IActionResult> DeleteSavedJob([FromRoute] Guid savedJobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var result = await _savedJobsRepository.DeleteSavedJobAsync(savedJobId, userId);
            if (!result) return NotFound(new { error = $"Saved job with id {savedJobId} not found." });

            return Ok(new { });
        }
    }
}

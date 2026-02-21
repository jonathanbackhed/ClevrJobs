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
    public class TrackedController : ControllerBase
    {
        private readonly ILogger<TrackedController> _logger;
        private readonly ITrackedJobRepository _trackedJobRepository;
        private readonly TrackedJobService _trackedJobService;

        public TrackedController(ILogger<TrackedController> logger, ITrackedJobRepository trackedJobRepository, TrackedJobService trackedJobService)
        {
            _logger = logger;
            _trackedJobRepository = trackedJobRepository;
            _trackedJobService = trackedJobService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTrackedJobs([FromQuery] int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var pageSize = 30;

            var savedJobs = await _trackedJobService.GetAllTrackedJobsAsync(page, pageSize, userId);

            return Ok(savedJobs);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrackedJob([FromBody] TrackedJobRequest trackedJobReq)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var response = await _trackedJobService.CreateTrackedJobAsync(trackedJobReq, userId);

            return Ok(response);
        }

        [HttpPost]
        [Route("{processedJobId}")]
        public async Task<IActionResult> CreateTrackedJobFromExisting([FromRoute] int processedJobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var result = await _trackedJobService.CreateTrackedJobFromExistingAsync(processedJobId, userId);
            if (result is null) return NotFound(new { error = $"Joblisting with id {processedJobId} not found." });

            return Ok(result);

        }

        [HttpPut]
        [Route("{trackedJobId}")]
        public async Task<IActionResult> UpdateTrackedJob([FromRoute] Guid trackedJobId, [FromBody] TrackedJobRequest trackedJobReq)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var result = await _trackedJobService.UpdateTrackedJobAsync(trackedJobId, trackedJobReq, userId);
            if (result is null) return NotFound(new { error = $"Tracked job with id {trackedJobId} not found." });

            return Ok(result);
        }

        [HttpDelete]
        [Route("{trackedJobId}")]
        public async Task<IActionResult> DeleteTrackedJob([FromRoute] Guid trackedJobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var result = await _trackedJobRepository.DeleteAsync(trackedJobId, userId);
            if (!result) return NotFound(new { error = $"Tracked job with id {trackedJobId} not found." });

            return Ok(new { });
        }
    }
}

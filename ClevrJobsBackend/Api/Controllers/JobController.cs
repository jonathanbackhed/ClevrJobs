using Api.Data;
using Api.DTOs.Requests;
using Api.DTOs.Responses;
using Api.Extensions;
using Asp.Versioning;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/jobs")]
    [ApiVersion("1.0")]
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;
        private readonly IProcessRepository _processRepository;
        private readonly IJobCache _cache;
        private readonly IConfiguration _configuration;

        public JobController(ILogger<JobController> logger, IProcessRepository processRepository, IJobCache cache, IConfiguration configuration)
        {
            _logger = logger;
            _processRepository = processRepository;
            _cache = cache;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<PagedResult<JobListingMiniResponse>>> GetJobsByLatest([FromQuery] int page = 1)
        {
            var pageSize = 15;

            var result = _cache.GetJobs(page);
            if (result is null)
            {
                _logger.LogInformation("Cache miss for jobs page {page}", page);

                var (items, totalCount) = await _processRepository.GetFullProcessedJobsByLatestAsync(page, pageSize);

                var dtos = items.Select(i => new JobListingMiniResponse
                {
                    Title = i.RawJob.Title,
                    CompanyName = i.RawJob.CompanyName,
                    Location = i.RawJob.Location,
                    Extent = i.RawJob.Extent,
                    Duration = i.RawJob.Duration,
                    ApplicationDeadline = i.RawJob.ApplicationDeadline,
                    Source = i.RawJob.Source,
                    ProcessedAt = (DateTime)i.ProcessRun.FinishedAt!,
                    Id = i.Id,
                    Description = i.Description,
                    RequiredTechnologies = i.RequiredTechnologies,
                    CompetenceRank = i.CompetenceRank
                }).ToList();

                result = new PagedResult<JobListingMiniResponse>
                {
                    Items = dtos,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };

                _cache.AddJobs(result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<JobListingResponse>> GetJobById([FromRoute] int id)
        {
            var result = _cache.GetJob(id);
            if (result is null)
            {
                _logger.LogInformation("Cache miss for job id {id}", id);

                var job = await _processRepository.GetFullProcessedJobByIdAsync(id);
                if (job is null)
                {
                    _logger.LogInformation("Job with id {id} not found.", id);
                    return NotFound($"Job with id {id} not found.");
                }

                result = new JobListingResponse
                {
                    Title = job.RawJob.Title,
                    CompanyName = job.RawJob.CompanyName,
                    RoleName = job.RawJob.RoleName,
                    Location = job.RawJob.Location,
                    Extent = job.RawJob.Extent,
                    Duration = job.RawJob.Duration,
                    ApplicationDeadline = job.RawJob.ApplicationDeadline,
                    Published = job.RawJob.Published,
                    ListingId = job.RawJob.ListingId,
                    ListingUrl = job.RawJob.ListingUrl,
                    Source = job.RawJob.Source,
                    ProcessedAt = (DateTime)job.ProcessRun.FinishedAt!,
                    Id = job.Id,
                    Description = job.Description,
                    RequiredTechnologies = job.RequiredTechnologies,
                    NiceTohaveTechnologies = job.NiceTohaveTechnologies,
                    CompetenceRank = job.CompetenceRank,
                    KeywordsCV = job.KeywordsCV,
                    KeywordsCL = job.KeywordsCL,
                    CustomCoverLetterFocus = job.CustomCoverLetterFocus,
                    Motivation = job.Motivation
                };

                _cache.AddJob(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("{jobId}/report")]
        [EnableRateLimiting("reportByIp")]
        public async Task<IActionResult> ReportJob([FromRoute] int jobId, [FromBody] ReportJobRequest request)
        {
            _logger.LogInformation("Received report for job id {jobId} with reason {reason}", jobId, request.Reason);

            var jobExists = await _processRepository.JobExists(jobId);
            if (!jobExists)
            {
                _logger.LogWarning("Attempted to report non-existent job with id {jobId}", jobId);
                return NotFound(new { error = "Job not found" });
            }

            var ipAddress = HttpContext.GetHashedIp(_configuration["Security:IpHashSalt"]);

            var existingReport = await _processRepository.GetJobReportByJobAndIpOrUserId(jobId, ipAddress);
            if (existingReport != null)
            {
                _logger.LogWarning("Duplicate report attempt for job {jobId} from IP {ip}", jobId, ipAddress);
                return Conflict(new { error = "You have already reported this job" });
            }

            var report = new JobReport
            {
                ProcessedJobId = jobId,
                Reason = request.Reason,
                Description = request.Description,
                IpAddress = ipAddress,
                UserIdentifier = null
            };

            try
            {
                await _processRepository.AddJobReport(report);
                return Ok(new { });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving report for job {jobId}", jobId);
                return StatusCode(500, new { error = "An error occurred while saving the report" });
            }
        }
    }
}

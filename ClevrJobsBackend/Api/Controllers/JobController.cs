using Api.Data;
using Api.Models;
using Api.Models.Dto;
using Api.Models.Dto.Requests;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IProcessRepository _processRepository;
        private readonly IJobCache _cache;

        public JobController(IProcessRepository processRepository, IJobCache cache)
        {
            _processRepository = processRepository;
            _cache = cache;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<PagedResult<JobListingMiniDto>>> GetJobsByLatest([FromQuery] int page = 1)
        {
            var pageSize = 15;

            var result = _cache.GetJobs(page);
            if (result is null)
            {
                var (items, totalCount) = await _processRepository.GetFullProcessedJobsByLatestAsync(page, pageSize);

                var dtos = items.Select(i => new JobListingMiniDto
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

                result = new PagedResult<JobListingMiniDto>
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
        public async Task<ActionResult<JobListingDto>> GetJobById([FromRoute] int id)
        {
            var result = _cache.GetJob(id);
            if (result is null)
            {
                var job = await _processRepository.GetFullProcessedJobByIdAsync(id);
                if (job is null)
                {
                    return BadRequest($"Job with id {id} not found.");
                }

                result = new JobListingDto
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
        public async Task<IActionResult> ReportJob([FromRoute]int jobId, [FromBody] ReportJobRequest request)
        {
            try
            {
                var jobExists = await _processRepository.JobExists(jobId);
                if (!jobExists)
                {
                    return NotFound($"Job with id {jobId} not found.");
                }

                var report = new JobReport
                {
                    ProcessedJobId = jobId,
                    Reason = request.Reason,
                    Description = request.Description,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserIdentifier = null
                };

                await _processRepository.AddJobReport(report);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(500, "An error occurred while saving the report");
            }

            return Ok();
        }
    }
}

using Data.Enums;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _dbc;

        public JobRepository(AppDbContext dbc)
        {
            _dbc = dbc;
        }

        public async Task<bool> AddMultipleFailedScrapes(IEnumerable<FailedScrape> failedScrapes)
        {
            await _dbc.FailedScrapes.AddRangeAsync(failedScrapes);

            return await _dbc.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddMultipleRawJobs(IEnumerable<RawJob> rawJobs)
        {
            await _dbc.RawJobs.AddRangeAsync(rawJobs);

            return await _dbc.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddRawJob(RawJob rawJob)
        {
            await _dbc.RawJobs.AddAsync(rawJob);

            return await _dbc.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddScrapeRun(ScrapeRun scrapeRun)
        {
            await _dbc.ScrapeRuns.AddAsync(scrapeRun);

            return await _dbc.SaveChangesAsync() > 0;
        }

        public async Task<List<FailedScrape>> GetFailedScrapesForRetryAsync()
        {
            var failed = await _dbc.FailedScrapes
                .Include(i => i.ScrapeRun)
                .Where(f => f.Status == FailedStatus.ReadyForRetry)
                .ToListAsync();

            return failed;
        }

        public async Task<RawJob?> GetLastPublishedRawJob()
        {
            return await _dbc.RawJobs.OrderByDescending(o => o.ListingId).FirstOrDefaultAsync();
        }

        public async Task<ScrapeRun?> GetLastScrapeRun()
        {
            var lastRun = await _dbc.ScrapeRuns.OrderBy(i => i.StartedAt).LastOrDefaultAsync();

            return lastRun;
        }

        public async Task<ICollection<RawJob>> GetUnprocessedRawJobsfromScrapeRunId(int id)
        {
            var jobs = await _dbc.RawJobs
                .Where(j => j.ScrapeRunId == id && j.ProcessedStatus == false)
                .ToListAsync();

            return jobs;
        }

        public async Task<bool> MarkRawJobAsProcessed(RawJob rawJob)
        {
            var result = await _dbc.RawJobs
                .Where(j => j.Id == rawJob.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ProcessedStatus, true));

            return result > 0;
        }

        public async Task<bool> UpdateFailedScrape(FailedScrape failedScrape)
        {
            var rowsAffected = await _dbc.FailedScrapes
                .Where(x => x.Id == failedScrape.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.RetryCount, p => p.RetryCount + 1)
                    .SetProperty(p => p.RetriedAt, failedScrape.RetriedAt)
                    .SetProperty(p => p.Status, failedScrape.Status)
                    .SetProperty(p => p.ErrorType, failedScrape.ErrorType)
                    .SetProperty(p => p.ErrorMessage, failedScrape.ErrorMessage)
                );

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateRawJobs(ICollection<RawJob> rawJobs)
        {
            _dbc.RawJobs.UpdateRange(rawJobs);

            return await _dbc.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateScrapeRun(ScrapeRun scrapeRun)
        {
            _dbc.ScrapeRuns.Update(scrapeRun);

            return await _dbc.SaveChangesAsync() > 0;
        }
    }
}

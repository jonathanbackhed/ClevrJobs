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

        public async Task<bool> AddRawJob(RawJob rawJob)
        {
            await _dbc.RawJobs.AddAsync(rawJob);

            return await _dbc.SaveChangesAsync() > 0;
        }

        public async Task AddScrapeRun(ScrapeRun scrapeRun)
        {
            await _dbc.ScrapeRuns.AddAsync(scrapeRun);
            await _dbc.SaveChangesAsync();
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

        public async Task UpdateScrapeRun(ScrapeRun scrapeRun)
        {
            _dbc.ScrapeRuns.Update(scrapeRun);
            await _dbc.SaveChangesAsync();
        }
    }
}

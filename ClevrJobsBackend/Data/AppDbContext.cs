using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ScrapeRun> ScrapeRuns { get; set; }
        public DbSet<RawJob> RawJobs { get; set; }
        public DbSet<ProcessRun> ProcessRuns { get; set; }
        public DbSet<ProcessedJob> ProcessedJobs { get; set; }
        public DbSet<Prompt> Prompts { get; set; }
        public DbSet<FailedScrape> FailedScrapes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
        }
    }
}

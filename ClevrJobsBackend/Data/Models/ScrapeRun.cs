using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class ScrapeRun
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.Now;

        public DateTime? FinishedAt { get; set; }

        [Required]
        public required Status Status { get; set; } = Status.InProgress;

        public int ScrapedJobs { get; set; } = 0;

        public int FailedJobs { get; set; } = 0;

        public ICollection<RawJob>? RawJobs { get; set; }

        public ICollection<FailedScrape>? FailedScrapes { get; set; }
    }
}

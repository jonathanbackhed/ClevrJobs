using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class FailedScrape
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string ListingUrl { get; set; }

        [Required]
        [MaxLength(100)]
        public required string ListingId { get; set; }

        public int ScrapeRunId { get; set; }
        public required ScrapeRun ScrapeRun { get; set; }

        [Required]
        public required string ErrorMessage { get; set; }

        public DateTime FailedAt { get; set; } = DateTime.Now;

        public string? ErrorType { get; set; }

        public FailedScrapeStatusType Status { get; set; } = FailedScrapeStatusType.Failed;

        public DateTime? RetriedAt { get; set; }

        public int RetryCount { get; set; } = 0;
    }
}

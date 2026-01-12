using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class RawJob
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public required string Title { get; set; }

        [Required]
        [MaxLength(100)]
        public required string CompanyName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string RoleName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Location { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Extent { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Duration { get; set; }

        [Required]
        [MaxLength(20)]
        public required string ApplicationDeadline { get; set; }

        [Required]
        public required string ApplicationUrl { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public required string SalaryType { get; set; }

        [Required]
        public required string Published { get; set; }

        [Required]
        [MaxLength(100)]
        public required string ListingId { get; set; }

        [Required]
        public required SourceType Source { get; set; }

        [Required]
        public required StatusType ProcessedStatus { get; set; } = StatusType.New;

        public int ScrapeRunId { get; set; }
        public required ScrapeRun ScrapeRun { get; set; }

        public ProcessedJob? ProcessedJob { get; set; }
    }
}

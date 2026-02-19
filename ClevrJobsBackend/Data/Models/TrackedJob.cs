using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Models
{
    public class TrackedJob
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public required string UserId { get; set; }
        [Required]
        public required SaveType SaveType { get; set; }
        public int? ProcessedJobId { get; set; }
        public ProcessedJob? ProcessedJob { get; set; }
        [Required]
        public required ApplicationStatus ApplicationStatus { get; set; }
        [MaxLength(500)]
        public string? RejectReason { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(150)]
        public required string Title { get; set; }
        [Required]
        [MaxLength(100)]
        public required string CompanyName { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Location { get; set; }
        [MaxLength(20)]
        public string? ApplicationDeadline { get; set; }
        [MaxLength(100)]
        public string? ListingUrl { get; set; }
    }
}

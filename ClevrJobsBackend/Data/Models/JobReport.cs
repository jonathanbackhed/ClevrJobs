using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class JobReport
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public required int ProcessedJobId { get; set; }
        public ProcessedJob? ProcessedJob { get; set; }
        [Required]
        public required ReportReason Reason { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public string? UserIdentifier { get; set; }
    }
}

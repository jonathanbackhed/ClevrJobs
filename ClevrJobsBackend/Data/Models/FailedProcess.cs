using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class FailedProcess
    {
        [Key]
        public int Id { get; set; }

        public int RawJobId { get; set; }
        public required RawJob RawJob { get; set; }

        public int ProcessRunId { get; set; }
        public required ProcessRun ProcessRun { get; set; }

        [Required]
        public required string ErrorMessage { get; set; }

        public DateTime FailedAt { get; set; } = DateTime.Now;

        public string? ErrorType { get; set; }

        public FailedStatus Status { get; set; } = FailedStatus.Failed;

        public DateTime? RetriedAt { get; set; }

        public int RetryCount { get; set; } = 0;
    }
}

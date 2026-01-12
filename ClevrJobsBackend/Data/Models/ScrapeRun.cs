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

        public DateTime FinishedAt { get; set; }

        [Required]
        public required StatusType Status { get; set; } = StatusType.InProgress;

        public ICollection<RawJob>? RawJobs { get; set; }
    }
}

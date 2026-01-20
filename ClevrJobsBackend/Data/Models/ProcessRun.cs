using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class ProcessRun
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.Now;

        public DateTime? FinishedAt { get; set; }

        [Required]
        public required string Model { get; set; }

        public int PromptId { get; set; }
        public required Prompt Prompt { get; set; }

        public ICollection<ProcessedJob>? ProcessedJobs { get; set; }
    }
}

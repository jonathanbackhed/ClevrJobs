using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class Prompt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(10)]
        public required string Version { get; set; }

        [Required]
        public required string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Language Language { get; set; }

        public required bool IsActive { get; set; }

        public int? ParentPromptId { get; set; }
        public Prompt? ParentPrompt { get; set; }

        public ICollection<ProcessRun>? ProcessRuns { get; set; }
    }
}

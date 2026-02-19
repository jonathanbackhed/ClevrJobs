using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Models
{
    public class SavedJob
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public required string UserId { get; set; }
        [Required]
        public required int ProcessedJobId { get; set; }
        public ProcessedJob? ProcessedJob { get; set; }
        public required DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}

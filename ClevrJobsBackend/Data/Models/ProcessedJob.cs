using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class ProcessedJob
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public required string RequiredTechnologies { get; set; }

        [Required]
        public required string NiceTohaveTechnologies { get; set; }

        [Required]
        public required CompetenceRank CompetenceRank { get; set; }

        [Required]
        public required string KeywordsCV { get; set; }

        [Required]
        public required string KeywordsCL { get; set; }

        [Required]
        public required string CustomCoverLetterFocus { get; set; }

        [Required]
        public required string Motivation { get; set; }

        public int RawJobId { get; set; }
        public required RawJob RawJob { get; set; }

        public int ProcessRunId { get; set; }
        public required ProcessRun ProcessRun { get; set; }
    }
}

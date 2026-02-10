using System;
using System.Collections.Generic;
using System.Text;

namespace Workers.DTOs
{
    public class AiResponse
    {
        public required string Description { get; set; }

        public required string RequiredTechnologies { get; set; }

        public required string NiceTohaveTechnologies { get; set; }

        public required string CompetenceRank { get; set; }

        public required string KeywordsCV { get; set; }

        public required string KeywordsCL { get; set; }

        public required string CustomCoverLetterFocus { get; set; }

        public required string Motivation { get; set; }
    }
}

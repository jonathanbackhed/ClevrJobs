using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Workers.Models.Dto
{
    public record ScrapeResultDto
    {
        public required List<RawJob> Jobs { get; init; }

        public required List<FailedScrape> FailedJobs { get; init; }

        public required bool ShouldContinue { get; init; } 
    }
}

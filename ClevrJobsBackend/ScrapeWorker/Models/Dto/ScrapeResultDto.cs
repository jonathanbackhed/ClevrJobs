using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapeWorker.Models.Dto
{
    public record ScrapeResultDto
    {
        public required List<RawJob> Jobs { get; init; }

        public required int ScrapedJobs { get; init; }

        public required int FailedJobs { get; init; }

        public required bool ShouldContinue { get; init; } 
    }
}

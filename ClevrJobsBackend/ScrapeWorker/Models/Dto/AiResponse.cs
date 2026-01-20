using System;
using System.Collections.Generic;
using System.Text;

namespace Workers.Models.Dto
{
    public class AiResponse
    {
        public required string Description { get; set; }

        public required string CompetenceRank { get; set; }

        public required string Keywords { get; set; }
    }
}

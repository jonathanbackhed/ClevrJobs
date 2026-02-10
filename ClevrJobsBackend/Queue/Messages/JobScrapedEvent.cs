using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.Messages
{
    public record JobScrapedEvent
    {
        public int RawJobId { get; init; }
    }
}

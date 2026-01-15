using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.Messages
{
    public class ScrapeCompletedEvent
    {
        public int ScrapeRunId { get; init; }
        public DateTime TimeStamp { get; init; }
    }
}

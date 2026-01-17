using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Enums
{
    public enum FailedScrapeStatusType
    {
        Failed,
        UnderInvestigation,
        ReadyForRetry,
        Resolved,
        Ignored
    }
}

using Data.Models;

namespace Workers.DTOs.Responses
{
    public record ScrapeResultResponse
    {
        public RawJob? RawJob { get; init; }
        public string? ErrorMessage { get; init; }
        public string? ErrorType { get; init; }
        public bool IsRetryable { get; init; }

        public static ScrapeResultResponse Success(RawJob rawJob) => new() { RawJob = rawJob };

        public static ScrapeResultResponse Failure(Exception e, bool isRetryable) => new()
        {
            ErrorMessage = e.Message,
            ErrorType = e.GetType().Name,
            IsRetryable = isRetryable
        };
    }
}

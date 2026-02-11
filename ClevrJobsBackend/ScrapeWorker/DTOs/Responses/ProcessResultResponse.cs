using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Workers.DTOs.Responses
{
    public record ProcessResultResponse
    {
        public ProcessedJob? ProcessedJob { get; init; }
        public string? ErrorMessage { get; init; }
        public string? ErrorType { get; init; }
        public bool IsRetryable { get; init; }

        public static ProcessResultResponse Success(ProcessedJob processedJob) => new() { ProcessedJob = processedJob };

        public static ProcessResultResponse Failure(Exception e, bool isRetryable) => new()
        {
            ErrorMessage = e.Message,
            ErrorType = e.GetType().Name,
            IsRetryable = isRetryable
        };
    }
}

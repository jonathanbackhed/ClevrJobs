using System;
using System.Collections.Generic;
using System.Text;

namespace Workers.DTOs.Responses
{
    public record ServiceResponse
    {
        public required bool Success { get; init; }
        public required bool MinorError { get; init; } = false;
        public string? ErrorMessage { get; init; }
    }
}

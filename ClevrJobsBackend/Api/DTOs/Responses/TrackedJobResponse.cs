using Data.Enums;

namespace Api.DTOs.Responses
{
    public record TrackedJobResponse
    {
        public required Guid Id { get; init; }
        public required SaveType SaveType { get; init; }
        public int? ProcessedJobId { get; set; }
        public required ApplicationStatus ApplicationStatus { get; init; }
        public string? RejectReason { get; init; }
        public string? Notes { get; init; }
        public required DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public DateOnly? ApplyDate { get; init; }
        public required bool HaveCalled { get; init; }
        public required bool SpontaneousApplication { get; init; }

        public required string Title { get; init; }
        public required string CompanyName { get; init; }
        public required string Location { get; init; }
        public string? ApplicationDeadline { get; init; }
        public string? ListingUrl { get; init; }
    }
}

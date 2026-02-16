using Data.Enums;
using Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.DTOs.Responses
{
    public record SavedJobResponse
    {
        public required Guid Id { get; init; }
        public required SaveType SaveType { get; init; }
        public int? ProcessedJobId { get; init; }

        public required bool HaveApplied { get; init; }
        public required ApplicationStatus ApplicationStatus { get; init; }
        public string? RejectReason { get; init; }
        public string? Notes { get; init; }
        public required DateTime SavedAt { get; set; }

        public required string Title { get; init; }
        public required string Description { get; init; }
        public required string CompanyName { get; init; }
        public required string Location { get; init; }
        public required string ApplicationDeadline { get; init; }
        public required string ListingUrl { get; init; }
    }
}

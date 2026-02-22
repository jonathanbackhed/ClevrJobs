using Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Requests
{
    public record TrackedJobRequest
    {
        [Required]
        public required ApplicationStatus ApplicationStatus { get; init; }
        [MaxLength(500)]
        public string? RejectReason { get; init; }
        [MaxLength(1000)]
        public string? Notes { get; init; }
        public DateTime? ApplyDate { get; init; }
        [Required]
        public bool HaveCalled { get; init; }
        [Required]
        public bool SpontaneousApplication { get; init; }

        [Required]
        [MaxLength(150)]
        public required string Title { get; init; }
        [Required]
        [MaxLength(100)]
        public required string CompanyName { get; init; }
        [Required]
        [MaxLength(100)]
        public required string Location { get; init; }
        [MaxLength(20)]
        public string? ApplicationDeadline { get; init; }
        [MaxLength(100)]
        public string? ListingUrl { get; init; }
    }
}

using Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Requests
{
    public record SavedJobRequest(
        [Required] Guid Id,
        [Required] bool HaveApplied,
        [Required] ApplicationStatus ApplicationStatus,
        [MaxLength(500)] string? RejectReason,
        [MaxLength(1000)] string? Notes,

        [MaxLength(150)] string? Title,
        string? Description,
        [MaxLength(100)] string? CompanyName,
        [MaxLength(100)] string? Location,
        [MaxLength(20)] string? ApplicationDeadline,
        [MaxLength(100)] string? ListingUrl
    );
}

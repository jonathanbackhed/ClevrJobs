using Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Requests
{
    public record ReportJobRequest(
        [Required] ReportReason Reason,
        [MaxLength(300)] string? Description
    );
}

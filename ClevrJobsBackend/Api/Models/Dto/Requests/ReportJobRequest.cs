using Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Dto.Requests
{
    public record ReportJobRequest(
        [Required] ReportReason Reason,
        [MaxLength(500)] string? Description
    );
}

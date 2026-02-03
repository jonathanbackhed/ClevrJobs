using Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Dto.Requests
{
    public record ReportJobRequest(
        [Required] ReportReason Reason,
        [MaxLength(300)] string? Description
    );
}

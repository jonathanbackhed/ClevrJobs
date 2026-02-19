using Data.Enums;
using Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.DTOs.Responses
{
    public record SavedJobResponse
    {
        public required Guid Id { get; init; }
        public required JobListingMiniResponse JobListingMini { get; init; }
        public required DateTime SavedAt { get; set; }
    }
}

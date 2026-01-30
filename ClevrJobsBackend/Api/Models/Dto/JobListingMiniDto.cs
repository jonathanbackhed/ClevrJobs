using Data.Enums;

namespace Api.Models.Dto
{
    public class JobListingMiniDto
    {
        public required string Title { get; init; }
        public required string CompanyName { get; init; }
        public required string Location { get; init; }
        public string? Extent { get; init; }
        public string? Duration { get; init; }
        public required string ApplicationDeadline { get; init; }
        public required Source Source { get; init; }
        public required DateTime ProcessedAt { get; set; }

        // Processed fields
        public required int Id { get; init; }
        public required string Description { get; init; }
        public required string RequiredTechnologies { get; init; }
        public required CompetenceRank CompetenceRank { get; init; }
    }
}

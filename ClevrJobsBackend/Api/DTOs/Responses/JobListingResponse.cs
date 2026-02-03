using Data.Enums;

namespace Api.DTOs.Responses
{
    public record JobListingResponse
    {
        public required string Title { get; init; }
        public required string CompanyName { get; init; }
        public required string RoleName { get; init; }
        public required string Location { get; init; }
        public string? Extent { get; init; }
        public string? Duration { get; init; }
        public required string ApplicationDeadline { get; init; }
        public required string Published { get; init; }
        public required string ListingId { get; init; }
        public required string ListingUrl { get; init; }
        public required Source Source { get; init; }
        public required DateTime ProcessedAt { get; init; }

        // Processed fields
        public required int Id { get; init; }
        public required string Description { get; init; }
        public required string RequiredTechnologies { get; init; }
        public required string NiceTohaveTechnologies { get; init; }
        public required CompetenceRank CompetenceRank { get; init; }
        public required string KeywordsCV { get; init; }
        public required string KeywordsCL { get; init; }
        public required string CustomCoverLetterFocus { get; init; }
        public required string Motivation { get; init; }
    }
}

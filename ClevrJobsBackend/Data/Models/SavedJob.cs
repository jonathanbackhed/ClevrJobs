using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Models
{
    public class SavedJob
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public required string UserId { get; set; }
        [Required]
        public required SaveType SaveType { get; set; }
        public int? ProcessedJobId { get; set; }
        public ProcessedJob? ProcessedJob { get; set; }

        // Used for both SaveTypes
        [Required]
        public required bool HaveApplied { get; set; }
        [Required]
        public required ApplicationStatus ApplicationStatus { get; set; }
        [MaxLength(500)]
        public string? RejectReason { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }

        // For manually added jobs
        [MaxLength(150)]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [MaxLength(100)]
        public string? CompanyName { get; set; }
        [MaxLength(100)]
        public string? Location { get; set; }
        [MaxLength(20)]
        public string? ApplicationDeadline { get; set; }
        [MaxLength(100)]
        public string? ListingUrl { get; set; }

        // Computed properties
        [NotMapped]
        public string DisplayTitle => this.SaveType == SaveType.SavedFromListing 
            ? ProcessedJob?.RawJob.Title ?? "Unknown" 
            : this.Title ?? "Unknown";
        [NotMapped]
        public string DisplayCompany => this.SaveType == SaveType.SavedFromListing
            ? ProcessedJob?.RawJob.CompanyName ?? "Unknown"
            : this.CompanyName ?? "Unknown";
        [NotMapped]
        public string DisplayLocation => this.SaveType == SaveType.SavedFromListing
            ? ProcessedJob?.RawJob.Location?? "Unknown"
            : this.Location ?? "Unknown";
        [NotMapped]
        public string DisplayDescription => this.SaveType == SaveType.SavedFromListing
            ? ProcessedJob?.Description ?? ""
            : this.Description ?? "";
        [NotMapped]
        public string DisplayDeadline => this.SaveType == SaveType.SavedFromListing
            ? ProcessedJob?.RawJob.ApplicationDeadline ?? "Unknown"
            : this.ApplicationDeadline ?? "Unknown";
        [NotMapped]
        public string DisplayUrl => this.SaveType == SaveType.SavedFromListing
            ? ProcessedJob?.RawJob.ListingUrl ?? "Unknown"
            : this.ListingUrl ?? "Unknown";
    }
}

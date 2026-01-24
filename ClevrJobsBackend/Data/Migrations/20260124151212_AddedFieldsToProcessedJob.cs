using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedFieldsToProcessedJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "ProcessedJobs");

            migrationBuilder.RenameColumn(
                name: "Keywords",
                table: "ProcessedJobs",
                newName: "RequiredTechnologies");

            migrationBuilder.AddColumn<string>(
                name: "CustomCoverLetterFocus",
                table: "ProcessedJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KeywordsCL",
                table: "ProcessedJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KeywordsCV",
                table: "ProcessedJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Motivation",
                table: "ProcessedJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NiceTohaveTechnologies",
                table: "ProcessedJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomCoverLetterFocus",
                table: "ProcessedJobs");

            migrationBuilder.DropColumn(
                name: "KeywordsCL",
                table: "ProcessedJobs");

            migrationBuilder.DropColumn(
                name: "KeywordsCV",
                table: "ProcessedJobs");

            migrationBuilder.DropColumn(
                name: "Motivation",
                table: "ProcessedJobs");

            migrationBuilder.DropColumn(
                name: "NiceTohaveTechnologies",
                table: "ProcessedJobs");

            migrationBuilder.RenameColumn(
                name: "RequiredTechnologies",
                table: "ProcessedJobs",
                newName: "Keywords");

            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "ProcessedJobs",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}

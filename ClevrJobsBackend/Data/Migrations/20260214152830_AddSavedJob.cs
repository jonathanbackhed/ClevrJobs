using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SaveType = table.Column<int>(type: "int", nullable: false),
                    ProcessedJobId = table.Column<int>(type: "int", nullable: true),
                    HaveApplied = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationStatus = table.Column<int>(type: "int", nullable: false),
                    RejectReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApplicationDeadline = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ListingUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedJobs_ProcessedJobs_ProcessedJobId",
                        column: x => x.ProcessedJobId,
                        principalTable: "ProcessedJobs",
                        principalColumn: "Id");

                    // Added manually
                    table.CheckConstraint(
                        "CK_SavedJob_ValidSavedJobType",
                        "([SaveType] = 0 AND [ProcessedJobId] IS NOT NULL AND [Title] IS NULL) OR ([SaveType] = 1 AND [ProcessedJobId] IS NULL AND [Title] IS NOT NULL)"
                    );
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobs_ProcessedJobId",
                table: "SavedJobs",
                column: "ProcessedJobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedJobs");
        }
    }
}

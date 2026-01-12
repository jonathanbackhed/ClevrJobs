using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScrapeRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapeRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PromptId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessRuns_Prompts_PromptId",
                        column: x => x.PromptId,
                        principalTable: "Prompts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RawJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Extent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApplicationDeadline = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApplicationUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalaryType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Published = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListingId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    ProcessedStatus = table.Column<int>(type: "int", nullable: false),
                    ScrapeRunId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RawJobs_ScrapeRuns_ScrapeRunId",
                        column: x => x.ScrapeRunId,
                        principalTable: "ScrapeRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessedJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompetenceRank = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<float>(type: "real", nullable: false),
                    Keywords = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RawJobId = table.Column<int>(type: "int", nullable: false),
                    ProcessRunId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessedJobs_ProcessRuns_ProcessRunId",
                        column: x => x.ProcessRunId,
                        principalTable: "ProcessRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessedJobs_RawJobs_RawJobId",
                        column: x => x.RawJobId,
                        principalTable: "RawJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedJobs_ProcessRunId",
                table: "ProcessedJobs",
                column: "ProcessRunId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedJobs_RawJobId",
                table: "ProcessedJobs",
                column: "RawJobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessRuns_PromptId",
                table: "ProcessRuns",
                column: "PromptId");

            migrationBuilder.CreateIndex(
                name: "IX_RawJobs_ScrapeRunId",
                table: "RawJobs",
                column: "ScrapeRunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedJobs");

            migrationBuilder.DropTable(
                name: "ProcessRuns");

            migrationBuilder.DropTable(
                name: "RawJobs");

            migrationBuilder.DropTable(
                name: "Prompts");

            migrationBuilder.DropTable(
                name: "ScrapeRuns");
        }
    }
}

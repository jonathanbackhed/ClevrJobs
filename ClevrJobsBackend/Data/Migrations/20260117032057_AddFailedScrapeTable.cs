using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFailedScrapeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FailedScrapes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListingUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ListingId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScrapeRunId = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FailedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RetriedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailedScrapes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FailedScrapes_ScrapeRuns_ScrapeRunId",
                        column: x => x.ScrapeRunId,
                        principalTable: "ScrapeRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FailedScrapes_ScrapeRunId",
                table: "FailedScrapes",
                column: "ScrapeRunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FailedScrapes");
        }
    }
}

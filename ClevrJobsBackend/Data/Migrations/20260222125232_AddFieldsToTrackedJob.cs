using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToTrackedJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApplyDate",
                table: "TrackedJobs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HaveCalled",
                table: "TrackedJobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SpontaneousApplication",
                table: "TrackedJobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TrackedJobs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyDate",
                table: "TrackedJobs");

            migrationBuilder.DropColumn(
                name: "HaveCalled",
                table: "TrackedJobs");

            migrationBuilder.DropColumn(
                name: "SpontaneousApplication",
                table: "TrackedJobs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TrackedJobs");
        }
    }
}

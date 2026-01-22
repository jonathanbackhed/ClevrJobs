using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessRunFieldToFailedProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProcessRunId",
                table: "FailedProcesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FailedProcesses_ProcessRunId",
                table: "FailedProcesses",
                column: "ProcessRunId");

            migrationBuilder.AddForeignKey(
                name: "FK_FailedProcesses_ProcessRuns_ProcessRunId",
                table: "FailedProcesses",
                column: "ProcessRunId",
                principalTable: "ProcessRuns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FailedProcesses_ProcessRuns_ProcessRunId",
                table: "FailedProcesses");

            migrationBuilder.DropIndex(
                name: "IX_FailedProcesses_ProcessRunId",
                table: "FailedProcesses");

            migrationBuilder.DropColumn(
                name: "ProcessRunId",
                table: "FailedProcesses");
        }
    }
}

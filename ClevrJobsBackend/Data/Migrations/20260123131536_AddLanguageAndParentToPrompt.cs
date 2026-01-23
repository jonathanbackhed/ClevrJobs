using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageAndParentToPrompt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Prompts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentPromptId",
                table: "Prompts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_ParentPromptId",
                table: "Prompts",
                column: "ParentPromptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_Prompts_ParentPromptId",
                table: "Prompts",
                column: "ParentPromptId",
                principalTable: "Prompts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prompts_Prompts_ParentPromptId",
                table: "Prompts");

            migrationBuilder.DropIndex(
                name: "IX_Prompts_ParentPromptId",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "ParentPromptId",
                table: "Prompts");
        }
    }
}

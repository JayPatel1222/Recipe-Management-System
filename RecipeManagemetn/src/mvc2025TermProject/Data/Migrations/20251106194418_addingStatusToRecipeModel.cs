using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc2025TermProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class addingStatusToRecipeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Recipes",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "draft");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Recipes");
        }
    }
}

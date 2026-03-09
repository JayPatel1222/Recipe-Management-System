using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc2025TermProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class modifiedRecipeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CookTime",
                table: "Recipes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrepTime",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CookTime",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "PrepTime",
                table: "Recipes");
        }
    }
}

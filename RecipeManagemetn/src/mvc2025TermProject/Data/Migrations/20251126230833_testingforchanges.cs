using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc2025TermProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class testingforchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Recipes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "RecipeNutritions",
                columns: table => new
                {
                    RecipeNutritionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeID = table.Column<int>(type: "int", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: true),
                    Protein = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Carbs = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Fat = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeNutritions", x => x.RecipeNutritionID);
                    table.ForeignKey(
                        name: "FK_RecipeNutritions_Recipes_RecipeID",
                        column: x => x.RecipeID,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeNutritions_RecipeID",
                table: "RecipeNutritions",
                column: "RecipeID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeNutritions");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Recipes");
        }
    }
}

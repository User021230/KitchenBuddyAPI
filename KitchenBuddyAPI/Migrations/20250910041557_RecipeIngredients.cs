using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KitchenBuddyAPI.Migrations
{
    /// <inheritdoc />
    public partial class RecipeIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ingridients",
                table: "Recipes",
                newName: "Ingredients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ingredients",
                table: "Recipes",
                newName: "Ingridients");
        }
    }
}

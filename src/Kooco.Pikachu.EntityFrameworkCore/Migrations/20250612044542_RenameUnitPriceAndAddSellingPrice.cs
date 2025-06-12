using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class RenameUnitPriceAndAddSellingPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "AppCartItems",
                newName: "GroupBuyPrice");

            migrationBuilder.AddColumn<int>(
                name: "SellingPrice",
                table: "AppCartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellingPrice",
                table: "AppCartItems");

            migrationBuilder.RenameColumn(
                name: "GroupBuyPrice",
                table: "AppCartItems",
                newName: "UnitPrice");
        }
    }
}

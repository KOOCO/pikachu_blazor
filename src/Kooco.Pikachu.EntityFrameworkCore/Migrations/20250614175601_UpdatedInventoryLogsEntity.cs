using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedInventoryLogsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StockType",
                table: "AppInventoryLogs",
                newName: "StockOnHand");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "AppInventoryLogs",
                newName: "SaleableQuantity");

            migrationBuilder.AddColumn<int>(
                name: "PreOrderQuantity",
                table: "AppInventoryLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleablePreOrderQuantity",
                table: "AppInventoryLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreOrderQuantity",
                table: "AppInventoryLogs");

            migrationBuilder.DropColumn(
                name: "SaleablePreOrderQuantity",
                table: "AppInventoryLogs");

            migrationBuilder.RenameColumn(
                name: "StockOnHand",
                table: "AppInventoryLogs",
                newName: "StockType");

            migrationBuilder.RenameColumn(
                name: "SaleableQuantity",
                table: "AppInventoryLogs",
                newName: "Amount");
        }
    }
}

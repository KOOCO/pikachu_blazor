using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class Add_DeliveryCostForNormal_DeliveryCostForFreeze_DeliveryCostForFrozen_DeliveryCost_AppOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryCost",
                table: "AppOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryCostForFreeze",
                table: "AppOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryCostForFrozen",
                table: "AppOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryCostForNormal",
                table: "AppOrders",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryCost",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryCostForFreeze",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryCostForFrozen",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryCostForNormal",
                table: "AppOrders");
        }
    }
}

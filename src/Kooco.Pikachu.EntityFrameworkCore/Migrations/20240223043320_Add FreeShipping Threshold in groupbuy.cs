using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddFreeShippingThresholdingroupbuy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreeShippingThreshold",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.AddColumn<decimal>(
                name: "FreeShippingThreshold",
                table: "AppGroupBuys",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreeShippingThreshold",
                table: "AppGroupBuys");

            migrationBuilder.AddColumn<int>(
                name: "FreeShippingThreshold",
                table: "AppLogisticsProviderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

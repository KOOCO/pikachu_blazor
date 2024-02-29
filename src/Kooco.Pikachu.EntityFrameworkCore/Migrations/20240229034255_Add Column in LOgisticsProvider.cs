using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddColumninLOgisticsProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OuterIslandFreight",
                table: "AppLogisticsProviderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Payment",
                table: "AppLogisticsProviderSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "AppLogisticsProviderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "AppLogisticsProviderSettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterIslandFreight",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "Payment",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "AppLogisticsProviderSettings");
        }
    }
}

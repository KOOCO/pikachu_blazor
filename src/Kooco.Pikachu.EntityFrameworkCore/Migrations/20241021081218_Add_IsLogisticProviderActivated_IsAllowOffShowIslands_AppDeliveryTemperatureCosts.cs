using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsLogisticProviderActivated_IsAllowOffShowIslands_AppDeliveryTemperatureCosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAllowOffShoreIslands",
                table: "AppDeliveryTemperatureCosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLogisticProviderActivated",
                table: "AppDeliveryTemperatureCosts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAllowOffShoreIslands",
                table: "AppDeliveryTemperatureCosts");

            migrationBuilder.DropColumn(
                name: "IsLogisticProviderActivated",
                table: "AppDeliveryTemperatureCosts");
        }
    }
}

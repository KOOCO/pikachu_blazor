using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class Add_LogisticProvider_DeliveryMethod_AppDeliveryTemperatureCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryMethod",
                table: "AppDeliveryTemperatureCosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LogisticProvider",
                table: "AppDeliveryTemperatureCosts",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryMethod",
                table: "AppDeliveryTemperatureCosts");

            migrationBuilder.DropColumn(
                name: "LogisticProvider",
                table: "AppDeliveryTemperatureCosts");
        }
    }
}

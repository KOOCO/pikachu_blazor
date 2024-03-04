using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryTimeColumninGroupBuy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlackCatDeliveryTime",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeDeliveryDeliveryTime",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelfPickupDeliveryTime",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlackCatDeliveryTime",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "HomeDeliveryDeliveryTime",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "SelfPickupDeliveryTime",
                table: "AppGroupBuys");
        }
    }
}

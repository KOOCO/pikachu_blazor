using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddColumninOrderDeliveryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrderDeliveries_AppEnumValues_CarrierId",
                table: "AppOrderDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_AppOrderDeliveries_CarrierId",
                table: "AppOrderDeliveries");

            migrationBuilder.DropColumn(
                name: "CarrierId",
                table: "AppOrderDeliveries");

            migrationBuilder.AddColumn<string>(
                name: "AllPayLogisticsID",
                table: "AppOrderDeliveries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Editor",
                table: "AppOrderDeliveries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllPayLogisticsID",
                table: "AppOrderDeliveries");

            migrationBuilder.DropColumn(
                name: "Editor",
                table: "AppOrderDeliveries");

            migrationBuilder.AddColumn<int>(
                name: "CarrierId",
                table: "AppOrderDeliveries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderDeliveries_CarrierId",
                table: "AppOrderDeliveries",
                column: "CarrierId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrderDeliveries_AppEnumValues_CarrierId",
                table: "AppOrderDeliveries",
                column: "CarrierId",
                principalTable: "AppEnumValues",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeDeliveryOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrderDeliveries_AppEnumValues_CarrierId",
                table: "AppOrderDeliveries");

            migrationBuilder.AlterColumn<int>(
                name: "CarrierId",
                table: "AppOrderDeliveries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "AppOrderDeliveries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderDeliveries_OrderId",
                table: "AppOrderDeliveries",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrderDeliveries_AppEnumValues_CarrierId",
                table: "AppOrderDeliveries",
                column: "CarrierId",
                principalTable: "AppEnumValues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrderDeliveries_AppOrders_OrderId",
                table: "AppOrderDeliveries",
                column: "OrderId",
                principalTable: "AppOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrderDeliveries_AppEnumValues_CarrierId",
                table: "AppOrderDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_AppOrderDeliveries_AppOrders_OrderId",
                table: "AppOrderDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_AppOrderDeliveries_OrderId",
                table: "AppOrderDeliveries");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "AppOrderDeliveries");

            migrationBuilder.AlterColumn<int>(
                name: "CarrierId",
                table: "AppOrderDeliveries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrderDeliveries_AppEnumValues_CarrierId",
                table: "AppOrderDeliveries",
                column: "CarrierId",
                principalTable: "AppEnumValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

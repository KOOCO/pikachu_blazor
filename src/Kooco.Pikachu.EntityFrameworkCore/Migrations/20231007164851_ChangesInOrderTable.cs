using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GroupBuyId",
                table: "AppOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OrderNo",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                table: "AppOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "AppOrders",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalQuantity",
                table: "AppOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_GroupBuyId",
                table: "AppOrders",
                column: "GroupBuyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrders_AppGroupBuys_GroupBuyId",
                table: "AppOrders",
                column: "GroupBuyId",
                principalTable: "AppGroupBuys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrders_AppGroupBuys_GroupBuyId",
                table: "AppOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppOrders_GroupBuyId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "GroupBuyId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "OrderNo",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "TotalQuantity",
                table: "AppOrders");
        }
    }
}

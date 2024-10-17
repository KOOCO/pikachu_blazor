using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnInOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountAmount",
                table: "AppOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountCodeId",
                table: "AppOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_DiscountCodeId",
                table: "AppOrders",
                column: "DiscountCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrders_AppDiscountCodes_DiscountCodeId",
                table: "AppOrders",
                column: "DiscountCodeId",
                principalTable: "AppDiscountCodes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrders_AppDiscountCodes_DiscountCodeId",
                table: "AppOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppOrders_DiscountCodeId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "DiscountCodeId",
                table: "AppOrders");
        }
    }
}

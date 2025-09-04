using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedOrderItemRelationToInventoryLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderItemId",
                table: "AppInventoryLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryLogs_OrderItemId",
                table: "AppInventoryLogs",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventoryLogs_AppOrderItems_OrderItemId",
                table: "AppInventoryLogs",
                column: "OrderItemId",
                principalTable: "AppOrderItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppInventoryLogs_AppOrderItems_OrderItemId",
                table: "AppInventoryLogs");

            migrationBuilder.DropIndex(
                name: "IX_AppInventoryLogs_OrderItemId",
                table: "AppInventoryLogs");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "AppInventoryLogs");
        }
    }
}

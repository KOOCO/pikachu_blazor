using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInOrdersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone2",
                table: "AppOrders",
                newName: "RecipientPhone");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "AppOrders",
                newName: "RecipientName");

            migrationBuilder.RenameColumn(
                name: "Name2",
                table: "AppOrders",
                newName: "RecipientEmail");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AppOrders",
                newName: "CustomerPhone");

            migrationBuilder.RenameColumn(
                name: "Email2",
                table: "AppOrders",
                newName: "CustomerName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "AppOrders",
                newName: "CustomerEmail");

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

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppOrders",
                type: "uniqueidentifier",
                nullable: true);

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
                name: "TenantId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "TotalQuantity",
                table: "AppOrders");

            migrationBuilder.RenameColumn(
                name: "RecipientPhone",
                table: "AppOrders",
                newName: "Phone2");

            migrationBuilder.RenameColumn(
                name: "RecipientName",
                table: "AppOrders",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "RecipientEmail",
                table: "AppOrders",
                newName: "Name2");

            migrationBuilder.RenameColumn(
                name: "CustomerPhone",
                table: "AppOrders",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "AppOrders",
                newName: "Email2");

            migrationBuilder.RenameColumn(
                name: "CustomerEmail",
                table: "AppOrders",
                newName: "Email");
        }
    }
}

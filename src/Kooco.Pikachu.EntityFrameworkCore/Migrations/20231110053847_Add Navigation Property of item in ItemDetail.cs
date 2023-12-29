using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationPropertyofiteminItemDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemId1",
                table: "AppItemDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppItemDetails_ItemId1",
                table: "AppItemDetails",
                column: "ItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppItemDetails_AppItems_ItemId1",
                table: "AppItemDetails",
                column: "ItemId1",
                principalTable: "AppItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppItemDetails_AppItems_ItemId1",
                table: "AppItemDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppItemDetails_ItemId1",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "ItemId1",
                table: "AppItemDetails");
        }
    }
}

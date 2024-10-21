using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedItemDetailIdInsteadOfSkusInCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemSkuJson",
                table: "AppCartItems");

            migrationBuilder.AddColumn<Guid>(
                name: "ItemDetailId",
                table: "AppCartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppCartItems_ItemDetailId",
                table: "AppCartItems",
                column: "ItemDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppCartItems_ItemDetailId",
                table: "AppCartItems");

            migrationBuilder.DropColumn(
                name: "ItemDetailId",
                table: "AppCartItems");

            migrationBuilder.AddColumn<string>(
                name: "ItemSkuJson",
                table: "AppCartItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

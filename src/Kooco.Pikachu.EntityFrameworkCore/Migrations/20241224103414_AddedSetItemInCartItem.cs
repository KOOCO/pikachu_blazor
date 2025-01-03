using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedSetItemInCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCartItems_AppItems_ItemId",
                table: "AppCartItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "AppCartItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemDetailId",
                table: "AppCartItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "SetItemId",
                table: "AppCartItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCartItems_SetItemId",
                table: "AppCartItems",
                column: "SetItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCartItems_AppItems_ItemId",
                table: "AppCartItems",
                column: "ItemId",
                principalTable: "AppItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCartItems_AppSetItems_SetItemId",
                table: "AppCartItems",
                column: "SetItemId",
                principalTable: "AppSetItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCartItems_AppItems_ItemId",
                table: "AppCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AppCartItems_AppSetItems_SetItemId",
                table: "AppCartItems");

            migrationBuilder.DropIndex(
                name: "IX_AppCartItems_SetItemId",
                table: "AppCartItems");

            migrationBuilder.DropColumn(
                name: "SetItemId",
                table: "AppCartItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "AppCartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemDetailId",
                table: "AppCartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppCartItems_AppItems_ItemId",
                table: "AppCartItems",
                column: "ItemId",
                principalTable: "AppItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

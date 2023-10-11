using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedSetIemAndFreebieNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppItems_ItemId",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppOrderItems_AppItems_ItemId",
                table: "AppOrderItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "AppOrderItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "FreebieId",
                table: "AppOrderItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "AppOrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SetItemId",
                table: "AppOrderItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "AppGroupBuyItemGroupDetails",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "AppGroupBuyItemGroupDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SetItemId",
                table: "AppGroupBuyItemGroupDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderItems_FreebieId",
                table: "AppOrderItems",
                column: "FreebieId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderItems_SetItemId",
                table: "AppOrderItems",
                column: "SetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuyItemGroupDetails_SetItemId",
                table: "AppGroupBuyItemGroupDetails",
                column: "SetItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppItems_ItemId",
                table: "AppGroupBuyItemGroupDetails",
                column: "ItemId",
                principalTable: "AppItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppSetItems_SetItemId",
                table: "AppGroupBuyItemGroupDetails",
                column: "SetItemId",
                principalTable: "AppSetItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrderItems_AppFreebie_FreebieId",
                table: "AppOrderItems",
                column: "FreebieId",
                principalTable: "AppFreebie",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrderItems_AppItems_ItemId",
                table: "AppOrderItems",
                column: "ItemId",
                principalTable: "AppItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrderItems_AppSetItems_SetItemId",
                table: "AppOrderItems",
                column: "SetItemId",
                principalTable: "AppSetItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppItems_ItemId",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppSetItems_SetItemId",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppOrderItems_AppFreebie_FreebieId",
                table: "AppOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AppOrderItems_AppItems_ItemId",
                table: "AppOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AppOrderItems_AppSetItems_SetItemId",
                table: "AppOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_AppOrderItems_FreebieId",
                table: "AppOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_AppOrderItems_SetItemId",
                table: "AppOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_AppGroupBuyItemGroupDetails_SetItemId",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.DropColumn(
                name: "FreebieId",
                table: "AppOrderItems");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "AppOrderItems");

            migrationBuilder.DropColumn(
                name: "SetItemId",
                table: "AppOrderItems");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.DropColumn(
                name: "SetItemId",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "AppOrderItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "AppGroupBuyItemGroupDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppItems_ItemId",
                table: "AppGroupBuyItemGroupDetails",
                column: "ItemId",
                principalTable: "AppItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrderItems_AppItems_ItemId",
                table: "AppOrderItems",
                column: "ItemId",
                principalTable: "AppItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

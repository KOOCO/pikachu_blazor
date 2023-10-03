using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class GroupBuyModulesRedo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppImages_ImageId",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppItems_ItemId",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppGroupBuyItemGroupDetails_ImageId",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AppGroupBuyItemGroups");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.DropColumn(
                name: "ItemDescription",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.AddColumn<int>(
                name: "GroupBuyModuleType",
                table: "AppGroupBuyItemGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "AppGroupBuyItemGroupDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinimumAmount",
                table: "AppFreebie",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FreebieAmount",
                table: "AppFreebie",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppItems_ItemId",
                table: "AppGroupBuyItemGroupDetails",
                column: "ItemId",
                principalTable: "AppItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppItems_ItemId",
                table: "AppGroupBuyItemGroupDetails");

            migrationBuilder.DropColumn(
                name: "GroupBuyModuleType",
                table: "AppGroupBuyItemGroups");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AppGroupBuyItemGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "AppGroupBuyItemGroupDetails",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "AppGroupBuyItemGroupDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemDescription",
                table: "AppGroupBuyItemGroupDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinimumAmount",
                table: "AppFreebie",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FreebieAmount",
                table: "AppFreebie",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuyItemGroupDetails_ImageId",
                table: "AppGroupBuyItemGroupDetails",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppImages_ImageId",
                table: "AppGroupBuyItemGroupDetails",
                column: "ImageId",
                principalTable: "AppImages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppGroupBuyItemGroupDetails_AppItems_ItemId",
                table: "AppGroupBuyItemGroupDetails",
                column: "ItemId",
                principalTable: "AppItems",
                principalColumn: "Id");
        }
    }
}

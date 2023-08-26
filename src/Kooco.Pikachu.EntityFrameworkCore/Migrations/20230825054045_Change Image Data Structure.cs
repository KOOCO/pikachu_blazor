using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class ChangeImageDataStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppItemDetails_AppItems_ItemId",
                table: "AppItemDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppItemDetails_ItemId",
                table: "AppItemDetails");

            migrationBuilder.AddColumn<int>(
                name: "SortNO",
                table: "AppImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetID",
                table: "AppImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppItemDetails_AppItems_Id",
                table: "AppItemDetails",
                column: "Id",
                principalTable: "AppItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppItemDetails_AppItems_Id",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "SortNO",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "TargetID",
                table: "AppImages");

            migrationBuilder.CreateIndex(
                name: "IX_AppItemDetails_ItemId",
                table: "AppItemDetails",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppItemDetails_AppItems_ItemId",
                table: "AppItemDetails",
                column: "ItemId",
                principalTable: "AppItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

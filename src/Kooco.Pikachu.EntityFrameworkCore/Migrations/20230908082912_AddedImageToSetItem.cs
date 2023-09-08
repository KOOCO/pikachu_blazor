using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedImageToSetItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SetItemId",
                table: "AppImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppImages_SetItemId",
                table: "AppImages",
                column: "SetItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppImages_AppSetItems_SetItemId",
                table: "AppImages",
                column: "SetItemId",
                principalTable: "AppSetItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppImages_AppSetItems_SetItemId",
                table: "AppImages");

            migrationBuilder.DropIndex(
                name: "IX_AppImages_SetItemId",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "SetItemId",
                table: "AppImages");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedForeignKeyInItemDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppItemDetails_AppItems_Id",
                table: "AppItemDetails");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppItemDetails_AppItems_ItemId",
                table: "AppItemDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppItemDetails_ItemId",
                table: "AppItemDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_AppItemDetails_AppItems_Id",
                table: "AppItemDetails",
                column: "Id",
                principalTable: "AppItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

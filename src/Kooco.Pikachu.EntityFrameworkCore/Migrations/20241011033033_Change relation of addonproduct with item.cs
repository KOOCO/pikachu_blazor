using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class Changerelationofaddonproductwithitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAddOnProducts_AppItems_ProductId",
                table: "AppAddOnProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_AppAddOnProducts_AppItemDetails_ProductId",
                table: "AppAddOnProducts",
                column: "ProductId",
                principalTable: "AppItemDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAddOnProducts_AppItemDetails_ProductId",
                table: "AppAddOnProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_AppAddOnProducts_AppItems_ProductId",
                table: "AppAddOnProducts",
                column: "ProductId",
                principalTable: "AppItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

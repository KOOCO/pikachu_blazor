using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class Add_ProductGroupModuleTitle_ProductGroupModuleImageSize_AppGroupBuyItemGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductGroupModuleImageSize",
                table: "AppGroupBuyItemGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductGroupModuleTitle",
                table: "AppGroupBuyItemGroups",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductGroupModuleImageSize",
                table: "AppGroupBuyItemGroups");

            migrationBuilder.DropColumn(
                name: "ProductGroupModuleTitle",
                table: "AppGroupBuyItemGroups");
        }
    }
}

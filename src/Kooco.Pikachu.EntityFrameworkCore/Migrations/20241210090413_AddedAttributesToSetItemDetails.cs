using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedAttributesToSetItemDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attribute1Value",
                table: "AppSetItemDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attribute2Value",
                table: "AppSetItemDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attribute3Value",
                table: "AppSetItemDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attribute1Value",
                table: "AppSetItemDetails");

            migrationBuilder.DropColumn(
                name: "Attribute2Value",
                table: "AppSetItemDetails");

            migrationBuilder.DropColumn(
                name: "Attribute3Value",
                table: "AppSetItemDetails");
        }
    }
}

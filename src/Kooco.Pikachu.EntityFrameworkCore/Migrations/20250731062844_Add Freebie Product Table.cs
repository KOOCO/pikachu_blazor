using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddFreebieProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ApplyToAllProducts",
                table: "AppFreebie",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AppFreebieProducts",
                columns: table => new
                {
                    FreebieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFreebieProducts", x => new { x.FreebieId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_AppFreebieProducts_AppFreebie_FreebieId",
                        column: x => x.FreebieId,
                        principalTable: "AppFreebie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppFreebieProducts");

            migrationBuilder.DropColumn(
                name: "ApplyToAllProducts",
                table: "AppFreebie");
        }
    }
}

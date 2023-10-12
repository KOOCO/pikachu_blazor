using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedSetItemPriceAndLimitQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LimitQuantity",
                table: "AppSetItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SetItemPrice",
                table: "AppSetItems",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimitQuantity",
                table: "AppSetItems");

            migrationBuilder.DropColumn(
                name: "SetItemPrice",
                table: "AppSetItems");
        }
    }
}

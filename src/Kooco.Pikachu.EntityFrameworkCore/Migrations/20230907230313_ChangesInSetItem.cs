using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInSetItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isFreeShipping",
                table: "AppSetItems",
                newName: "IsFreeShipping");

            migrationBuilder.AlterColumn<string>(
                name: "SetItemStatus",
                table: "AppSetItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "AppSetItemDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "AppSetItemDetails");

            migrationBuilder.RenameColumn(
                name: "IsFreeShipping",
                table: "AppSetItems",
                newName: "isFreeShipping");

            migrationBuilder.AlterColumn<string>(
                name: "SetItemStatus",
                table: "AppSetItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}

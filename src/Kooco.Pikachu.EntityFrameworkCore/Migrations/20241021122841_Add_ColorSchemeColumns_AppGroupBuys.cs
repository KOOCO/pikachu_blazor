using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class Add_ColorSchemeColumns_AppGroupBuys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlertColor",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ColorSchemeType",
                table: "AppGroupBuys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryBackgroundColor",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryColor",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertColor",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "ColorSchemeType",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "SecondaryBackgroundColor",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "SecondaryColor",
                table: "AppGroupBuys");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddSelfPickUpinfocolumninGroupbuytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelfPickupAddress",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelfPickupCity",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelfPickupZipCode",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelfPickupAddress",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "SelfPickupCity",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "SelfPickupZipCode",
                table: "AppGroupBuys");
        }
    }
}

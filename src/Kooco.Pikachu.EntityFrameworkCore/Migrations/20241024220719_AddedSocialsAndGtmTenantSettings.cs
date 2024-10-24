using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedSocialsAndGtmTenantSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "AppTenantSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GtmContainerId",
                table: "AppTenantSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GtmEnabled",
                table: "AppTenantSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "AppTenantSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Line",
                table: "AppTenantSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "AppTenantSettings");

            migrationBuilder.DropColumn(
                name: "GtmContainerId",
                table: "AppTenantSettings");

            migrationBuilder.DropColumn(
                name: "GtmEnabled",
                table: "AppTenantSettings");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "AppTenantSettings");

            migrationBuilder.DropColumn(
                name: "Line",
                table: "AppTenantSettings");
        }
    }
}

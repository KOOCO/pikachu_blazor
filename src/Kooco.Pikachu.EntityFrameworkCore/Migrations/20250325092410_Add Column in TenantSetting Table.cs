using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddColumninTenantSettingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Line",
                table: "AppTenantSettings",
                newName: "LineLink");

            migrationBuilder.RenameColumn(
                name: "Instagram",
                table: "AppTenantSettings",
                newName: "LineDisplayName");

            migrationBuilder.RenameColumn(
                name: "Facebook",
                table: "AppTenantSettings",
                newName: "InstagramLink");

            migrationBuilder.AddColumn<string>(
                name: "FacebookDisplayName",
                table: "AppTenantSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookLink",
                table: "AppTenantSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramDisplayName",
                table: "AppTenantSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacebookDisplayName",
                table: "AppTenantSettings");

            migrationBuilder.DropColumn(
                name: "FacebookLink",
                table: "AppTenantSettings");

            migrationBuilder.DropColumn(
                name: "InstagramDisplayName",
                table: "AppTenantSettings");

            migrationBuilder.RenameColumn(
                name: "LineLink",
                table: "AppTenantSettings",
                newName: "Line");

            migrationBuilder.RenameColumn(
                name: "LineDisplayName",
                table: "AppTenantSettings",
                newName: "Instagram");

            migrationBuilder.RenameColumn(
                name: "InstagramLink",
                table: "AppTenantSettings",
                newName: "Facebook");
        }
    }
}

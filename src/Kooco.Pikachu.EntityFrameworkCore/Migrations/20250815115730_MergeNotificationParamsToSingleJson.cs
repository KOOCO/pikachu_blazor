using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class MergeNotificationParamsToSingleJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageParamsJson",
                table: "AppNotifications");

            migrationBuilder.DropColumn(
                name: "TitleParamsJson",
                table: "AppNotifications");

            migrationBuilder.RenameColumn(
                name: "UrlParamsJson",
                table: "AppNotifications",
                newName: "ParametersJson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParametersJson",
                table: "AppNotifications",
                newName: "UrlParamsJson");

            migrationBuilder.AddColumn<string>(
                name: "MessageParamsJson",
                table: "AppNotifications",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleParamsJson",
                table: "AppNotifications",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }
    }
}

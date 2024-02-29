using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class ChangeinLogisticProviderSettingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogisticsType",
                table: "AppLogisticsProviderSettings",
                newName: "SenderPostalCode");

            migrationBuilder.RenameColumn(
                name: "LogisticsSubTypes",
                table: "AppLogisticsProviderSettings",
                newName: "SenderAddress");

            migrationBuilder.AddColumn<int>(
                name: "City",
                table: "AppLogisticsProviderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PlatFormId",
                table: "AppLogisticsProviderSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "PlatFormId",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.RenameColumn(
                name: "SenderPostalCode",
                table: "AppLogisticsProviderSettings",
                newName: "LogisticsType");

            migrationBuilder.RenameColumn(
                name: "SenderAddress",
                table: "AppLogisticsProviderSettings",
                newName: "LogisticsSubTypes");
        }
    }
}

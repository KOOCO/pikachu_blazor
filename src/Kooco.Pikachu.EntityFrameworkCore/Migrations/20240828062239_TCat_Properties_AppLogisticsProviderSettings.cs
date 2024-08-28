using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class TCat_Properties_AppLogisticsProviderSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "AppLogisticsProviderSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerToken",
                table: "AppLogisticsProviderSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DeclaredValue",
                table: "AppLogisticsProviderSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReverseLogisticShippingFee",
                table: "AppLogisticsProviderSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TCatPickingListForm",
                table: "AppLogisticsProviderSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TCatShippingLabelForm",
                table: "AppLogisticsProviderSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TCatShippingLabelForm711",
                table: "AppLogisticsProviderSettings",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "CustomerToken",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "DeclaredValue",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "ReverseLogisticShippingFee",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "TCatPickingListForm",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "TCatShippingLabelForm",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "TCatShippingLabelForm711",
                table: "AppLogisticsProviderSettings");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedHualienAndSurchargeInLogisticProviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HolidaySurcharge",
                table: "AppLogisticsProviderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "HolidaySurchargeEndTime",
                table: "AppLogisticsProviderSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HolidaySurchargeStartTime",
                table: "AppLogisticsProviderSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HualienAndTaitungShippingFee",
                table: "AppLogisticsProviderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HolidaySurcharge",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "HolidaySurchargeEndTime",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "HolidaySurchargeStartTime",
                table: "AppLogisticsProviderSettings");

            migrationBuilder.DropColumn(
                name: "HualienAndTaitungShippingFee",
                table: "AppLogisticsProviderSettings");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class add_DeliveryByStore_RecipientDetails_AppOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressDetailsDbsFreeze",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressDetailsDbsFrozen",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressDetailsDbsNormal",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CVSStoreOutSideFreeze",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CVSStoreOutSideFrozen",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CVSStoreOutSideNormal",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityDbsFreeze",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityDbsFrozen",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityDbsNormal",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCodeDbsFreeze",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCodeDbsFrozen",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCodeDbsNormal",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientNameDbsFreeze",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientNameDbsFrozen",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientNameDbsNormal",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhoneDbsFreeze",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhoneDbsFrozen",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhoneDbsNormal",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemarksDbsFreeze",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemarksDbsFrozen",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemarksDbsNormal",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreIdFreeze",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreIdFrozen",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreIdNormal",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressDetailsDbsFreeze",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "AddressDetailsDbsFrozen",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "AddressDetailsDbsNormal",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CVSStoreOutSideFreeze",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CVSStoreOutSideFrozen",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CVSStoreOutSideNormal",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CityDbsFreeze",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CityDbsFrozen",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CityDbsNormal",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "PostalCodeDbsFreeze",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "PostalCodeDbsFrozen",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "PostalCodeDbsNormal",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "RecipientNameDbsFreeze",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "RecipientNameDbsFrozen",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "RecipientNameDbsNormal",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "RecipientPhoneDbsFreeze",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "RecipientPhoneDbsFrozen",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "RecipientPhoneDbsNormal",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "RemarksDbsFreeze",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "RemarksDbsFrozen",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "RemarksDbsNormal",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "StoreIdFreeze",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "StoreIdFrozen",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "StoreIdNormal",
                table: "AppOrders");
        }
    }
}

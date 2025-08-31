using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedTCatCodTradeInfoRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "AppTenantPayoutRecords",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "AppEcPayCodTradeInfoRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AppTCatCodTradeInfoRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HandlingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CollectionSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryCompletionSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantTradeNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExtraShippingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExtraServiceItems = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CashCollected = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnedGoods = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SameDayDelivery = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipmentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CODAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTCatCodTradeInfoRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTCatCodTradeInfoRecords");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "AppEcPayCodTradeInfoRecords");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "AppTenantPayoutRecords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}

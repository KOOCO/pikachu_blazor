using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedEcPayCodTradeInfoRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppEcPayCodTradeInfoRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ActualWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AllPayLogisticsID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookingNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CollectionAllocateAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CollectionAllocateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CollectionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CollectionChargeFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CVSPaymentNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CVSValidationNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoodsAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GoodsName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoodsWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HandlingCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LogisticsStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogisticsType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantTradeNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderCellPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipChargeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShipmentNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TradeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckMacValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEcPayCodTradeInfoRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppEcPayCodTradeInfoRecords_AppOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "AppOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEcPayCodTradeInfoRecords_OrderId",
                table: "AppEcPayCodTradeInfoRecords",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEcPayCodTradeInfoRecords");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedBankTransferRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppManualBankTransferRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentAmount = table.Column<int>(type: "int", nullable: false),
                    ReceivedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppManualBankTransferRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppManualBankTransferRecords_AbpUsers_ConfirmById",
                        column: x => x.ConfirmById,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppManualBankTransferRecords_AppOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "AppOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppManualBankTransferRecords_ConfirmById",
                table: "AppManualBankTransferRecords",
                column: "ConfirmById");

            migrationBuilder.CreateIndex(
                name: "IX_AppManualBankTransferRecords_OrderId",
                table: "AppManualBankTransferRecords",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppManualBankTransferRecords");
        }
    }
}

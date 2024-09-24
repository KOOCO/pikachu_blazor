using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserShoppingCreditToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreditDeductionAmount",
                table: "AppOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "CreditDeductionRecordId",
                table: "AppOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefundAmount",
                table: "AppOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RefundRecordId",
                table: "AppOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_CreditDeductionRecordId",
                table: "AppOrders",
                column: "CreditDeductionRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_RefundRecordId",
                table: "AppOrders",
                column: "RefundRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrders_AppUserShoppingCredits_CreditDeductionRecordId",
                table: "AppOrders",
                column: "CreditDeductionRecordId",
                principalTable: "AppUserShoppingCredits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrders_AppUserShoppingCredits_RefundRecordId",
                table: "AppOrders",
                column: "RefundRecordId",
                principalTable: "AppUserShoppingCredits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrders_AppUserShoppingCredits_CreditDeductionRecordId",
                table: "AppOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_AppOrders_AppUserShoppingCredits_RefundRecordId",
                table: "AppOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppOrders_CreditDeductionRecordId",
                table: "AppOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppOrders_RefundRecordId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CreditDeductionAmount",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CreditDeductionRecordId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "RefundRecordId",
                table: "AppOrders");
        }
    }
}

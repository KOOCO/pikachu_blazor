using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEcPayReconciliationRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppEcPayReconciliationRecord",
                table: "AppEcPayReconciliationRecord");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppEcPayReconciliationRecord");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "AppEcPayReconciliationRecord");

            migrationBuilder.DropColumn(
                name: "RefundDate",
                table: "AppEcPayReconciliationRecord");

            migrationBuilder.RenameTable(
                name: "AppEcPayReconciliationRecord",
                newName: "AppEcPayReconciliationRecords");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "AppEcPayReconciliationRecords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppEcPayReconciliationRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OrderNo",
                table: "AppEcPayReconciliationRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TransactionHandlingFee",
                table: "AppEcPayReconciliationRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppEcPayReconciliationRecords",
                table: "AppEcPayReconciliationRecords",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppEcPayReconciliationRecords_OrderId",
                table: "AppEcPayReconciliationRecords",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEcPayReconciliationRecords_AppOrders_OrderId",
                table: "AppEcPayReconciliationRecords",
                column: "OrderId",
                principalTable: "AppOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppEcPayReconciliationRecords_AppOrders_OrderId",
                table: "AppEcPayReconciliationRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppEcPayReconciliationRecords",
                table: "AppEcPayReconciliationRecords");

            migrationBuilder.DropIndex(
                name: "IX_AppEcPayReconciliationRecords_OrderId",
                table: "AppEcPayReconciliationRecords");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppEcPayReconciliationRecords");

            migrationBuilder.DropColumn(
                name: "OrderNo",
                table: "AppEcPayReconciliationRecords");

            migrationBuilder.DropColumn(
                name: "TransactionHandlingFee",
                table: "AppEcPayReconciliationRecords");

            migrationBuilder.RenameTable(
                name: "AppEcPayReconciliationRecords",
                newName: "AppEcPayReconciliationRecord");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "AppEcPayReconciliationRecord",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppEcPayReconciliationRecord",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount",
                table: "AppEcPayReconciliationRecord",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundDate",
                table: "AppEcPayReconciliationRecord",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppEcPayReconciliationRecord",
                table: "AppEcPayReconciliationRecord",
                column: "Id");
        }
    }
}

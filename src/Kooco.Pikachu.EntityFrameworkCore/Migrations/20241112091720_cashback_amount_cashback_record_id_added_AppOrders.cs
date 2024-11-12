using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class cashback_amount_cashback_record_id_added_AppOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrders_AppUserShoppingCredits_RefundRecordId",
                table: "AppOrders");

            migrationBuilder.RenameColumn(
                name: "RefundRecordId",
                table: "AppOrders",
                newName: "cashback_record_id");

            migrationBuilder.RenameIndex(
                name: "IX_AppOrders_RefundRecordId",
                table: "AppOrders",
                newName: "IX_AppOrders_cashback_record_id");

            migrationBuilder.AddColumn<decimal>(
                name: "cashback_amount",
                table: "AppOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrders_AppUserShoppingCredits_cashback_record_id",
                table: "AppOrders",
                column: "cashback_record_id",
                principalTable: "AppUserShoppingCredits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrders_AppUserShoppingCredits_cashback_record_id",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "cashback_amount",
                table: "AppOrders");

            migrationBuilder.RenameColumn(
                name: "cashback_record_id",
                table: "AppOrders",
                newName: "RefundRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_AppOrders_cashback_record_id",
                table: "AppOrders",
                newName: "IX_AppOrders_RefundRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrders_AppUserShoppingCredits_RefundRecordId",
                table: "AppOrders",
                column: "RefundRecordId",
                principalTable: "AppUserShoppingCredits",
                principalColumn: "Id");
        }
    }
}

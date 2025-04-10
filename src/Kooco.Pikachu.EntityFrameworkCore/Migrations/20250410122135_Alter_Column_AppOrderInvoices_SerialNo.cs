using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class Alter_Column_AppOrderInvoices_SerialNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceStatus",
                table: "AppOrderInvoices");

            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                table: "AppOrderInvoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RelateNo",
                table: "AppOrderInvoices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "SerialNo",
                table: "AppOrderInvoices",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderInvoices_SerialNo_RelateNo",
                table: "AppOrderInvoices",
                columns: new[] { "SerialNo", "RelateNo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppOrderInvoices_SerialNo_RelateNo",
                table: "AppOrderInvoices");

            migrationBuilder.DropColumn(
                name: "IsVoided",
                table: "AppOrderInvoices");

            migrationBuilder.DropColumn(
                name: "RelateNo",
                table: "AppOrderInvoices");

            migrationBuilder.DropColumn(
                name: "SerialNo",
                table: "AppOrderInvoices");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceStatus",
                table: "AppOrderInvoices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

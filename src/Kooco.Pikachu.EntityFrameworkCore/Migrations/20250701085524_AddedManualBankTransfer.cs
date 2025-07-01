using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedManualBankTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InstallmentPeriodsJson",
                table: "AppPaymentGateways",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "AppPaymentGateways",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "AppPaymentGateways",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "AppPaymentGateways",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "AppPaymentGateways",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchName",
                table: "AppPaymentGateways",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumAmountLimit",
                table: "AppPaymentGateways",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinimumAmountLimit",
                table: "AppPaymentGateways",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "AppPaymentGateways");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "AppPaymentGateways");

            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "AppPaymentGateways");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "AppPaymentGateways");

            migrationBuilder.DropColumn(
                name: "BranchName",
                table: "AppPaymentGateways");

            migrationBuilder.DropColumn(
                name: "MaximumAmountLimit",
                table: "AppPaymentGateways");

            migrationBuilder.DropColumn(
                name: "MinimumAmountLimit",
                table: "AppPaymentGateways");

            migrationBuilder.AlterColumn<string>(
                name: "InstallmentPeriodsJson",
                table: "AppPaymentGateways",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}

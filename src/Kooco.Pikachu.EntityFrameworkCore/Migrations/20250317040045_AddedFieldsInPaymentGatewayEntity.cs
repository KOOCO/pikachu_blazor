using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedFieldsInPaymentGatewayEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBankTransferEnabled",
                table: "AppPaymentGateways",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreditCardEnabled",
                table: "AppPaymentGateways",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBankTransferEnabled",
                table: "AppPaymentGateways");

            migrationBuilder.DropColumn(
                name: "IsCreditCardEnabled",
                table: "AppPaymentGateways");
        }
    }
}

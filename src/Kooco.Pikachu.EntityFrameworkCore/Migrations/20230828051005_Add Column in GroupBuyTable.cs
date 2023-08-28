using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddColumninGroupBuyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerInformation",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExcludeShippingMethod",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupBuyCondition",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultPaymentGateWay",
                table: "AppGroupBuys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerInformation",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "ExcludeShippingMethod",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "GroupBuyCondition",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "IsDefaultPaymentGateWay",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "AppGroupBuys");
        }
    }
}

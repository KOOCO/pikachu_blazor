using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedCampaignUsagePolicyToCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseableWithAllAddOnProducts",
                table: "AppCampaigns");

            migrationBuilder.DropColumn(
                name: "UseableWithAllDiscounts",
                table: "AppCampaigns");

            migrationBuilder.DropColumn(
                name: "UseableWithAllShoppingCredits",
                table: "AppCampaigns");

            migrationBuilder.AddColumn<int>(
                name: "AddOnProductUsagePolicy",
                table: "AppCampaigns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiscountUsagePolicy",
                table: "AppCampaigns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShoppingCreditUsagePolicy",
                table: "AppCampaigns",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddOnProductUsagePolicy",
                table: "AppCampaigns");

            migrationBuilder.DropColumn(
                name: "DiscountUsagePolicy",
                table: "AppCampaigns");

            migrationBuilder.DropColumn(
                name: "ShoppingCreditUsagePolicy",
                table: "AppCampaigns");

            migrationBuilder.AddColumn<bool>(
                name: "UseableWithAllAddOnProducts",
                table: "AppCampaigns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseableWithAllDiscounts",
                table: "AppCampaigns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseableWithAllShoppingCredits",
                table: "AppCampaigns",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

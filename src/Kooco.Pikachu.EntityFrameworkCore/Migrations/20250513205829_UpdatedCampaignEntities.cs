using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCampaignEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicableItem",
                table: "AppCampaignShoppingCredits");

            migrationBuilder.AddColumn<bool>(
                name: "ApplicableToAddOnProducts",
                table: "AppCampaignShoppingCredits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplicableToShippingFees",
                table: "AppCampaignShoppingCredits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "CapAmount",
                table: "AppCampaignShoppingCredits",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicableToAddOnProducts",
                table: "AppCampaignShoppingCredits");

            migrationBuilder.DropColumn(
                name: "ApplicableToShippingFees",
                table: "AppCampaignShoppingCredits");

            migrationBuilder.DropColumn(
                name: "CapAmount",
                table: "AppCampaignShoppingCredits");

            migrationBuilder.AddColumn<int>(
                name: "ApplicableItem",
                table: "AppCampaignShoppingCredits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCampaignSpendCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductCondition",
                table: "AppCampaignAddOnProducts",
                newName: "SpendCondition");

            migrationBuilder.AddColumn<int>(
                name: "SpendCondition",
                table: "AppCampaignShoppingCredits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Threshold",
                table: "AppCampaignShoppingCredits",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpendCondition",
                table: "AppCampaignShoppingCredits");

            migrationBuilder.DropColumn(
                name: "Threshold",
                table: "AppCampaignShoppingCredits");

            migrationBuilder.RenameColumn(
                name: "SpendCondition",
                table: "AppCampaignAddOnProducts",
                newName: "ProductCondition");
        }
    }
}

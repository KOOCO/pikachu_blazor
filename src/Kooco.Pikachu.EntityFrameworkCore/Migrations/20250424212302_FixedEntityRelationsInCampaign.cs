using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class FixedEntityRelationsInCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppCampaignProducts_ProductId",
                table: "AppCampaignProducts");

            migrationBuilder.DropIndex(
                name: "IX_AppCampaignGroupBuys_GroupBuyId",
                table: "AppCampaignGroupBuys");

            migrationBuilder.DropIndex(
                name: "IX_AppCampaignAddOnProducts_ProductId",
                table: "AppCampaignAddOnProducts");

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignProducts_ProductId",
                table: "AppCampaignProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignGroupBuys_GroupBuyId",
                table: "AppCampaignGroupBuys",
                column: "GroupBuyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignAddOnProducts_ProductId",
                table: "AppCampaignAddOnProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppCampaignProducts_ProductId",
                table: "AppCampaignProducts");

            migrationBuilder.DropIndex(
                name: "IX_AppCampaignGroupBuys_GroupBuyId",
                table: "AppCampaignGroupBuys");

            migrationBuilder.DropIndex(
                name: "IX_AppCampaignAddOnProducts_ProductId",
                table: "AppCampaignAddOnProducts");

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignProducts_ProductId",
                table: "AppCampaignProducts",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignGroupBuys_GroupBuyId",
                table: "AppCampaignGroupBuys",
                column: "GroupBuyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignAddOnProducts_ProductId",
                table: "AppCampaignAddOnProducts",
                column: "ProductId",
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedCampaignRelationshipToEdm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AppEdms_CampaignId",
                table: "AppEdms",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEdms_AppCampaigns_CampaignId",
                table: "AppEdms",
                column: "CampaignId",
                principalTable: "AppCampaigns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppEdms_AppCampaigns_CampaignId",
                table: "AppEdms");

            migrationBuilder.DropIndex(
                name: "IX_AppEdms_CampaignId",
                table: "AppEdms");
        }
    }
}

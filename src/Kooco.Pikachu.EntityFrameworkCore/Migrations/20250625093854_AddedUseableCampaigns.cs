using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedUseableCampaigns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "AppUseableCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowedCampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PromotionModule = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUseableCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUseableCampaigns_AppCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "AppCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUseableCampaigns_CampaignId",
                table: "AppUseableCampaigns",
                column: "CampaignId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUseableCampaigns");

            migrationBuilder.DropColumn(
                name: "UseableWithAllAddOnProducts",
                table: "AppCampaigns");

            migrationBuilder.DropColumn(
                name: "UseableWithAllDiscounts",
                table: "AppCampaigns");

            migrationBuilder.DropColumn(
                name: "UseableWithAllShoppingCredits",
                table: "AppCampaigns");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedAppliedCampaignsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrders_AppCampaigns_CampaignId",
                table: "AppOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppOrders_CampaignId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                table: "AppOrders");

            migrationBuilder.CreateTable(
                name: "AppAppliedCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Module = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAppliedCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppAppliedCampaigns_AppCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "AppCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppAppliedCampaigns_AppOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "AppOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAppliedCampaigns_CampaignId",
                table: "AppAppliedCampaigns",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_AppAppliedCampaigns_OrderId",
                table: "AppAppliedCampaigns",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppAppliedCampaigns");

            migrationBuilder.AddColumn<Guid>(
                name: "CampaignId",
                table: "AppOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_CampaignId",
                table: "AppOrders",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrders_AppCampaigns_CampaignId",
                table: "AppOrders",
                column: "CampaignId",
                principalTable: "AppCampaigns",
                principalColumn: "Id");
        }
    }
}

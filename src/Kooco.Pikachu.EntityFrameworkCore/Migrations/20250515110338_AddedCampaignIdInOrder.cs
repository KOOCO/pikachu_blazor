using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedCampaignIdInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}

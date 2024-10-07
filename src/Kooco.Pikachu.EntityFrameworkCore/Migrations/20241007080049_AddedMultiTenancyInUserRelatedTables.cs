using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedMultiTenancyInUserRelatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppUserCumulativeOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppUserCumulativeFinancials",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppUserCumulativeCredits",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppShopCarts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCartItems",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppUserCumulativeOrders");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppUserCumulativeFinancials");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppUserCumulativeCredits");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppShopCarts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCartItems");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationofShoppingCreditEarnSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppShoppingCreditEarnSpecificProducts_AppShoppingCreditEarnSettings_ShoppingCreditEarnSettingId",
                table: "AppShoppingCreditEarnSpecificProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_AppShoppingCreditEarnSpecificProducts_AppShoppingCreditUsageSettings_ShoppingCreditEranSettingId",
                table: "AppShoppingCreditEarnSpecificProducts");

            migrationBuilder.DropIndex(
                name: "IX_AppShoppingCreditEarnSpecificProducts_ShoppingCreditEarnSettingId",
                table: "AppShoppingCreditEarnSpecificProducts");

            migrationBuilder.DropColumn(
                name: "ShoppingCreditEarnSettingId",
                table: "AppShoppingCreditEarnSpecificProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_AppShoppingCreditEarnSpecificProducts_AppShoppingCreditEarnSettings_ShoppingCreditEranSettingId",
                table: "AppShoppingCreditEarnSpecificProducts",
                column: "ShoppingCreditEranSettingId",
                principalTable: "AppShoppingCreditEarnSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppShoppingCreditEarnSpecificProducts_AppShoppingCreditEarnSettings_ShoppingCreditEranSettingId",
                table: "AppShoppingCreditEarnSpecificProducts");

            migrationBuilder.AddColumn<Guid>(
                name: "ShoppingCreditEarnSettingId",
                table: "AppShoppingCreditEarnSpecificProducts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppShoppingCreditEarnSpecificProducts_ShoppingCreditEarnSettingId",
                table: "AppShoppingCreditEarnSpecificProducts",
                column: "ShoppingCreditEarnSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppShoppingCreditEarnSpecificProducts_AppShoppingCreditEarnSettings_ShoppingCreditEarnSettingId",
                table: "AppShoppingCreditEarnSpecificProducts",
                column: "ShoppingCreditEarnSettingId",
                principalTable: "AppShoppingCreditEarnSettings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppShoppingCreditEarnSpecificProducts_AppShoppingCreditUsageSettings_ShoppingCreditEranSettingId",
                table: "AppShoppingCreditEarnSpecificProducts",
                column: "ShoppingCreditEranSettingId",
                principalTable: "AppShoppingCreditUsageSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

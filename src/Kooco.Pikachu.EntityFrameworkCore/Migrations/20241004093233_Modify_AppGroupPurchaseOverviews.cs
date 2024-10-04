using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class Modify_AppGroupPurchaseOverviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppGroupPurchaseOverviews_AppGroupBuys_GroupBuyId",
                table: "AppGroupPurchaseOverviews");

            migrationBuilder.DropIndex(
                name: "IX_AppGroupPurchaseOverviews_GroupBuyId",
                table: "AppGroupPurchaseOverviews");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppGroupPurchaseOverviews");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppGroupPurchaseOverviews");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppGroupPurchaseOverviews");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "AppGroupPurchaseOverviews");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppGroupPurchaseOverviews");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppGroupPurchaseOverviews");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppGroupPurchaseOverviews");

            migrationBuilder.AlterTable(
                name: "AppGroupPurchaseOverviews",
                comment: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "AppGroupPurchaseOverviews",
                oldComment: "");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppGroupPurchaseOverviews",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppGroupPurchaseOverviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppGroupPurchaseOverviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "AppGroupPurchaseOverviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppGroupPurchaseOverviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppGroupPurchaseOverviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppGroupPurchaseOverviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupPurchaseOverviews_GroupBuyId",
                table: "AppGroupPurchaseOverviews",
                column: "GroupBuyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppGroupPurchaseOverviews_AppGroupBuys_GroupBuyId",
                table: "AppGroupPurchaseOverviews",
                column: "GroupBuyId",
                principalTable: "AppGroupBuys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

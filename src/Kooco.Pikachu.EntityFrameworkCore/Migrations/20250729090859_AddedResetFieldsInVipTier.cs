using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedResetFieldsInVipTier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsResetConfigured",
                table: "AppVipTierSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsResetEnabled",
                table: "AppVipTierSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "AppVipTierSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastResetDate",
                table: "AppVipTierSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextResetDate",
                table: "AppVipTierSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResetFrequency",
                table: "AppVipTierSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "AppVipTierSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsResetConfigured",
                table: "AppVipTierSettings");

            migrationBuilder.DropColumn(
                name: "IsResetEnabled",
                table: "AppVipTierSettings");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "AppVipTierSettings");

            migrationBuilder.DropColumn(
                name: "LastResetDate",
                table: "AppVipTierSettings");

            migrationBuilder.DropColumn(
                name: "NextResetDate",
                table: "AppVipTierSettings");

            migrationBuilder.DropColumn(
                name: "ResetFrequency",
                table: "AppVipTierSettings");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "AppVipTierSettings");
        }
    }
}

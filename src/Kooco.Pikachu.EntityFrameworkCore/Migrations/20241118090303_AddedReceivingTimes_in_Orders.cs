﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedReceivingTimes_in_Orders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivingTimeFreeze",
                table: "AppOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivingTimeFrozen",
                table: "AppOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivingTimeNormal",
                table: "AppOrders",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceivingTimeFreeze",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "ReceivingTimeFrozen",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "ReceivingTimeNormal",
                table: "AppOrders");
        }
    }
}

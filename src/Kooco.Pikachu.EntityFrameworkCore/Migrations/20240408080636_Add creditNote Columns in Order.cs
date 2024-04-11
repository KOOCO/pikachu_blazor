using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddcreditNoteColumnsinOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreditNoteDate",
                table: "AppOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreditNoteUser",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VoidDate",
                table: "AppOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoidUser",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditNoteDate",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CreditNoteUser",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "VoidDate",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "VoidUser",
                table: "AppOrders");
        }
    }
}

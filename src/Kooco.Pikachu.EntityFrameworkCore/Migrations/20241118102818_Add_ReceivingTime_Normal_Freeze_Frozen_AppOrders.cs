using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class Add_ReceivingTime_Normal_Freeze_Frozen_AppOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReceivingTimeFreeze",
                table: "AppOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivingTimeFrozen",
                table: "AppOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivingTimeNormal",
                table: "AppOrders",
                type: "int",
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

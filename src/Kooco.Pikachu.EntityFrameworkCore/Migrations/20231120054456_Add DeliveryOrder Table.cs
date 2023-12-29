using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryOrderId",
                table: "AppOrderItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppOrderDeliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryMethod = table.Column<int>(type: "int", nullable: false),
                    DeliveryStatus = table.Column<int>(type: "int", nullable: false),
                    DeliveryNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarrierId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppOrderDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppOrderDeliveries_AppEnumValues_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "AppEnumValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderItems_DeliveryOrderId",
                table: "AppOrderItems",
                column: "DeliveryOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderDeliveries_CarrierId",
                table: "AppOrderDeliveries",
                column: "CarrierId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrderItems_AppOrderDeliveries_DeliveryOrderId",
                table: "AppOrderItems",
                column: "DeliveryOrderId",
                principalTable: "AppOrderDeliveries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrderItems_AppOrderDeliveries_DeliveryOrderId",
                table: "AppOrderItems");

            migrationBuilder.DropTable(
                name: "AppOrderDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_AppOrderItems_DeliveryOrderId",
                table: "AppOrderItems");

            migrationBuilder.DropColumn(
                name: "DeliveryOrderId",
                table: "AppOrderItems");
        }
    }
}

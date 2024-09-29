using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddAddonProductandAddonProductSpecificGroupBuy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppAddOnProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddOnAmount = table.Column<int>(type: "int", nullable: false),
                    AddOnLimitPerOrder = table.Column<int>(type: "int", nullable: false),
                    QuantitySetting = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "int", nullable: false),
                    DisplayOriginalPrice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddOnConditions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumSpendAmount = table.Column<int>(type: "int", nullable: false),
                    GroupbuysScope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    SellingQuantity = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAddOnProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppAddOnProducts_AppItems_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppAddOnProductSpecificGroupbuys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddOnProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupbuyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAddOnProductSpecificGroupbuys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppAddOnProductSpecificGroupbuys_AppAddOnProducts_AddOnProductId",
                        column: x => x.AddOnProductId,
                        principalTable: "AppAddOnProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppAddOnProductSpecificGroupbuys_AppGroupBuys_GroupbuyId",
                        column: x => x.GroupbuyId,
                        principalTable: "AppGroupBuys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppAddOnProducts_ProductId",
                table: "AppAddOnProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppAddOnProductSpecificGroupbuys_AddOnProductId",
                table: "AppAddOnProductSpecificGroupbuys",
                column: "AddOnProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppAddOnProductSpecificGroupbuys_GroupbuyId",
                table: "AppAddOnProductSpecificGroupbuys",
                column: "GroupbuyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppAddOnProductSpecificGroupbuys");

            migrationBuilder.DropTable(
                name: "AppAddOnProducts");
        }
    }
}

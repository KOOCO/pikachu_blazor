using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountCodeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppDiscountCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecifiedCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "int", nullable: false),
                    MaxUsePerPerson = table.Column<int>(type: "int", nullable: false),
                    GroupbuysScope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductsScope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumSpendAmount = table.Column<int>(type: "int", nullable: true),
                    ShippingDiscountScope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecificShippingMethods = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountPercentage = table.Column<int>(type: "int", nullable: true),
                    DiscountAmount = table.Column<int>(type: "int", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDiscountCodes", x => x.Id);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppDiscountCodeUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalOrders = table.Column<int>(type: "int", nullable: false),
                    TotalUsers = table.Column<int>(type: "int", nullable: false),
                    TotalDiscountAmount = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDiscountCodeUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDiscountCodeUsages_AppDiscountCodes_DiscountCodeId",
                        column: x => x.DiscountCodeId,
                        principalTable: "AppDiscountCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppDiscountSpecificGroupbuys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupbuyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDiscountSpecificGroupbuys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDiscountSpecificGroupbuys_AppDiscountCodes_DiscountCodeId",
                        column: x => x.DiscountCodeId,
                        principalTable: "AppDiscountCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppDiscountSpecificGroupbuys_AppGroupBuys_GroupbuyId",
                        column: x => x.GroupbuyId,
                        principalTable: "AppGroupBuys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppDiscountSpecificProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDiscountSpecificProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDiscountSpecificProducts_AppDiscountCodes_DiscountCodeId",
                        column: x => x.DiscountCodeId,
                        principalTable: "AppDiscountCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppDiscountSpecificProducts_AppItems_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppDiscountCodeUsages_DiscountCodeId",
                table: "AppDiscountCodeUsages",
                column: "DiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDiscountSpecificGroupbuys_DiscountCodeId",
                table: "AppDiscountSpecificGroupbuys",
                column: "DiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDiscountSpecificGroupbuys_GroupbuyId",
                table: "AppDiscountSpecificGroupbuys",
                column: "GroupbuyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDiscountSpecificProducts_DiscountCodeId",
                table: "AppDiscountSpecificProducts",
                column: "DiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDiscountSpecificProducts_ProductId",
                table: "AppDiscountSpecificProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDiscountCodeUsages");

            migrationBuilder.DropTable(
                name: "AppDiscountSpecificGroupbuys");

            migrationBuilder.DropTable(
                name: "AppDiscountSpecificProducts");

            migrationBuilder.DropTable(
                name: "AppDiscountCodes");
        }
    }
}

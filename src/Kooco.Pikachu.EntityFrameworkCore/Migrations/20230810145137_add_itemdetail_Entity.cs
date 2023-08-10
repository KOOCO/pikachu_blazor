using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class add_itemdetail_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSetItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SetItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SetItemDescriptionTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SetItemMainImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SetItemStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetItemSaleableQuantity = table.Column<int>(type: "int", nullable: true),
                    SellingPrice = table.Column<int>(type: "int", nullable: false),
                    GroupBuyPrice = table.Column<int>(type: "int", nullable: true),
                    SaleableQuantity = table.Column<int>(type: "int", nullable: true),
                    PreOrderableQuantity = table.Column<int>(type: "int", nullable: true),
                    SaleablePreOrderQuantity = table.Column<int>(type: "int", nullable: true),
                    SalesAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Returnable = table.Column<bool>(type: "bit", nullable: false),
                    LimitAvaliableTimeStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LimitAvaliableTimeEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShareProfit = table.Column<int>(type: "int", nullable: false),
                    isFreeShipping = table.Column<bool>(type: "bit", nullable: false),
                    TaxName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxPercentage = table.Column<int>(type: "int", nullable: true),
                    TaxType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_AppSetItems", x => x.Id);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppSetItemDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SetItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_AppSetItemDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSetItemDetails_AppItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "AppItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppSetItemDetails_AppSetItems_SetItemId",
                        column: x => x.SetItemId,
                        principalTable: "AppSetItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppSetItemDetails_ItemId",
                table: "AppSetItemDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSetItemDetails_SetItemId",
                table: "AppSetItemDetails",
                column: "SetItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSetItemDetails");

            migrationBuilder.DropTable(
                name: "AppSetItems");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddItemDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InventoryAccount",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "IsGroupProduct",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "OpeningStock",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "OpeningStockValue",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "ReorderLevel",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "StockOnHand",
                table: "AppItems");

            migrationBuilder.RenameColumn(
                name: "WarehouseName",
                table: "AppItems",
                newName: "PropertyName3");

            migrationBuilder.RenameColumn(
                name: "SellingPrice",
                table: "AppItems",
                newName: "ShareProfit");

            migrationBuilder.RenameColumn(
                name: "PurchaseDescription",
                table: "AppItems",
                newName: "PropertyName2");

            migrationBuilder.RenameColumn(
                name: "PurchaseAccount",
                table: "AppItems",
                newName: "PropertyName1");

            migrationBuilder.RenameColumn(
                name: "PreferredVendor",
                table: "AppItems",
                newName: "ItemMainImageURL");

            migrationBuilder.RenameColumn(
                name: "PartNumber",
                table: "AppItems",
                newName: "ItemDescriptionTitle");

            migrationBuilder.AddColumn<DateTime>(
                name: "LimitAvaliableTimeEnd",
                table: "AppItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LimitAvaliableTimeStart",
                table: "AppItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "isFreeShipping",
                table: "AppItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AppItemDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemDetailTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDetailStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDetailDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Property1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Property2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Property3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SellingPrice = table.Column<int>(type: "int", nullable: false),
                    GroupBuyPrice = table.Column<int>(type: "int", nullable: true),
                    SaleableQuantity = table.Column<int>(type: "int", nullable: true),
                    PreOrderableQuantity = table.Column<int>(type: "int", nullable: true),
                    SaleablePreOrderQuantity = table.Column<int>(type: "int", nullable: true),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: true),
                    WarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InventoryAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpeningStock = table.Column<int>(type: "int", nullable: true),
                    OpeningStockValue = table.Column<int>(type: "int", nullable: false),
                    PurchasePrice = table.Column<int>(type: "int", nullable: true),
                    PurchaseAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreferredVendor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StockOnHand = table.Column<int>(type: "int", nullable: true),
                    PartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_AppItemDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppItemDetails_AppItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "AppItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppItemDetails_ItemId",
                table: "AppItemDetails",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "LimitAvaliableTimeEnd",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "LimitAvaliableTimeStart",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "isFreeShipping",
                table: "AppItems");

            migrationBuilder.RenameColumn(
                name: "ShareProfit",
                table: "AppItems",
                newName: "SellingPrice");

            migrationBuilder.RenameColumn(
                name: "PropertyName3",
                table: "AppItems",
                newName: "WarehouseName");

            migrationBuilder.RenameColumn(
                name: "PropertyName2",
                table: "AppItems",
                newName: "PurchaseDescription");

            migrationBuilder.RenameColumn(
                name: "PropertyName1",
                table: "AppItems",
                newName: "PurchaseAccount");

            migrationBuilder.RenameColumn(
                name: "ItemMainImageURL",
                table: "AppItems",
                newName: "PreferredVendor");

            migrationBuilder.RenameColumn(
                name: "ItemDescriptionTitle",
                table: "AppItems",
                newName: "PartNumber");

            migrationBuilder.AddColumn<string>(
                name: "InventoryAccount",
                table: "AppItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroupProduct",
                table: "AppItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemType",
                table: "AppItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OpeningStock",
                table: "AppItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OpeningStockValue",
                table: "AppItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurchasePrice",
                table: "AppItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReorderLevel",
                table: "AppItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "AppItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StockOnHand",
                table: "AppItems",
                type: "int",
                nullable: true);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class ChangeItemNoToLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemNo = table.Column<long>(type: "bigint", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SellingPrice = table.Column<int>(type: "int", nullable: false),
                    SalesAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Returnable = table.Column<bool>(type: "bit", nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManufactorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageWeight = table.Column<short>(type: "smallint", nullable: false),
                    PackageLength = table.Column<short>(type: "smallint", nullable: false),
                    PackageHeight = table.Column<short>(type: "smallint", nullable: false),
                    DiemensionsUnit = table.Column<int>(type: "int", nullable: false),
                    WeightUnit = table.Column<int>(type: "int", nullable: false),
                    TaxName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxPercentage = table.Column<int>(type: "int", nullable: false),
                    TaxType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseTaxName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseTaxPercentage = table.Column<int>(type: "int", nullable: false),
                    ProductType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    ReferenceID = table.Column<int>(type: "int", nullable: false),
                    LastSyncTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UPC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EAN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchasePrice = table.Column<int>(type: "int", nullable: false),
                    PurchaseAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InventoryAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false),
                    PreferredVendor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpeningStock = table.Column<int>(type: "int", nullable: false),
                    OpeningStockValue = table.Column<int>(type: "int", nullable: false),
                    StockOnHand = table.Column<int>(type: "int", nullable: false),
                    IsComboProduct = table.Column<bool>(type: "bit", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField1Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField2Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField2Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField3Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField3Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField4Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField4Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField5Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField5Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField6Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField6Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField7Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField7Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField8Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField8Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField9Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField9Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField10Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomeField10Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_AppItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppItems_ItemNo",
                table: "AppItems",
                column: "ItemNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppItems");
        }
    }
}

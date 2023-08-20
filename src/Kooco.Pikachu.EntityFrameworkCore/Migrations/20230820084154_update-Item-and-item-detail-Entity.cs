using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class updateItemanditemdetailEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyName1",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "PropertyName2",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "PropertyName3",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "TaxType",
                table: "AppItems");

            migrationBuilder.RenameColumn(
                name: "isFreeShipping",
                table: "AppItems",
                newName: "IsFreeShipping");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "AppItems",
                newName: "UnitId");

            migrationBuilder.AlterColumn<float>(
                name: "ShareProfit",
                table: "AppItems",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "PackageWeight",
                table: "AppItems",
                type: "real",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "PackageLength",
                table: "AppItems",
                type: "real",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "PackageHeight",
                table: "AppItems",
                type: "real",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsItemAvaliable",
                table: "AppItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReturnable",
                table: "AppItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ItemTags",
                table: "AppItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShippingMethodId",
                table: "AppItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxTypeId",
                table: "AppItems",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "SellingPrice",
                table: "AppItemDetails",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "SaleableQuantity",
                table: "AppItemDetails",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "SaleablePreOrderQuantity",
                table: "AppItemDetails",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "PreOrderableQuantity",
                table: "AppItemDetails",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "GroupBuyPrice",
                table: "AppItemDetails",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "AppItemDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppItems_ShippingMethodId",
                table: "AppItems",
                column: "ShippingMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_AppItems_TaxTypeId",
                table: "AppItems",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppItems_UnitId",
                table: "AppItems",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppItems_AppEnumValues_ShippingMethodId",
                table: "AppItems",
                column: "ShippingMethodId",
                principalTable: "AppEnumValues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppItems_AppEnumValues_TaxTypeId",
                table: "AppItems",
                column: "TaxTypeId",
                principalTable: "AppEnumValues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppItems_AppEnumValues_UnitId",
                table: "AppItems",
                column: "UnitId",
                principalTable: "AppEnumValues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppItems_AppEnumValues_ShippingMethodId",
                table: "AppItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AppItems_AppEnumValues_TaxTypeId",
                table: "AppItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AppItems_AppEnumValues_UnitId",
                table: "AppItems");

            migrationBuilder.DropIndex(
                name: "IX_AppItems_ShippingMethodId",
                table: "AppItems");

            migrationBuilder.DropIndex(
                name: "IX_AppItems_TaxTypeId",
                table: "AppItems");

            migrationBuilder.DropIndex(
                name: "IX_AppItems_UnitId",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "IsItemAvaliable",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "IsReturnable",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "ItemTags",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "ShippingMethodId",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "TaxTypeId",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "AppItemDetails");

            migrationBuilder.RenameColumn(
                name: "IsFreeShipping",
                table: "AppItems",
                newName: "isFreeShipping");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "AppItems",
                newName: "Unit");

            migrationBuilder.AlterColumn<int>(
                name: "ShareProfit",
                table: "AppItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<short>(
                name: "PackageWeight",
                table: "AppItems",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "PackageLength",
                table: "AppItems",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "PackageHeight",
                table: "AppItems",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyName1",
                table: "AppItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyName2",
                table: "AppItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyName3",
                table: "AppItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxType",
                table: "AppItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SellingPrice",
                table: "AppItemDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "SaleableQuantity",
                table: "AppItemDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SaleablePreOrderQuantity",
                table: "AppItemDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PreOrderableQuantity",
                table: "AppItemDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GroupBuyPrice",
                table: "AppItemDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);
        }
    }
}

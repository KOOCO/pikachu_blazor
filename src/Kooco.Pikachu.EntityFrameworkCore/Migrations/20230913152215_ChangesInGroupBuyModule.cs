using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInGroupBuyModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "issueInvoice",
                table: "AppGroupBuys",
                newName: "IssueInvoice");

            migrationBuilder.RenameColumn(
                name: "allowShipToOuterTaiwan",
                table: "AppGroupBuys",
                newName: "AllowShipToOuterTaiwan");

            migrationBuilder.RenameColumn(
                name: "allowShipOversea",
                table: "AppGroupBuys",
                newName: "AllowShipOversea");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "AppGroupBuys",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "CustomerInformationDescription",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExchangePolicyDescription",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupBuyConditionDescription",
                table: "AppGroupBuys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppGroupBuyItemGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroupBuyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppGroupBuyItemGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppGroupBuyItemGroups_AppGroupBuys_GroupBuyId",
                        column: x => x.GroupBuyId,
                        principalTable: "AppGroupBuys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppGroupBuyItemGroupDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupBuyItemGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppGroupBuyItemGroupDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppGroupBuyItemGroupDetails_AppGroupBuyItemGroups_GroupBuyItemGroupId",
                        column: x => x.GroupBuyItemGroupId,
                        principalTable: "AppGroupBuyItemGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppGroupBuyItemGroupDetails_AppImages_ImageId",
                        column: x => x.ImageId,
                        principalTable: "AppImages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppGroupBuyItemGroupDetails_AppItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "AppItems",
                        principalColumn: "Id");
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuyItemGroupDetails_GroupBuyItemGroupId",
                table: "AppGroupBuyItemGroupDetails",
                column: "GroupBuyItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuyItemGroupDetails_ImageId",
                table: "AppGroupBuyItemGroupDetails",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuyItemGroupDetails_ItemId",
                table: "AppGroupBuyItemGroupDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuyItemGroups_GroupBuyId",
                table: "AppGroupBuyItemGroups",
                column: "GroupBuyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppGroupBuyItemGroupDetails");

            migrationBuilder.DropTable(
                name: "AppGroupBuyItemGroups");

            migrationBuilder.DropColumn(
                name: "CustomerInformationDescription",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "ExchangePolicyDescription",
                table: "AppGroupBuys");

            migrationBuilder.DropColumn(
                name: "GroupBuyConditionDescription",
                table: "AppGroupBuys");

            migrationBuilder.RenameColumn(
                name: "IssueInvoice",
                table: "AppGroupBuys",
                newName: "issueInvoice");

            migrationBuilder.RenameColumn(
                name: "AllowShipToOuterTaiwan",
                table: "AppGroupBuys",
                newName: "allowShipToOuterTaiwan");

            migrationBuilder.RenameColumn(
                name: "AllowShipOversea",
                table: "AppGroupBuys",
                newName: "allowShipOversea");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "AppGroupBuys",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class GroupBuyItemGroup : Migration
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

            migrationBuilder.CreateTable(
                name: "AppGroupBuyItemGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroupBuyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ItemDescription1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Item1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Item1Order = table.Column<int>(type: "int", nullable: true),
                    Item2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Item2Order = table.Column<int>(type: "int", nullable: true),
                    Item3Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Item3Order = table.Column<int>(type: "int", nullable: true),
                    Item4Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Item4Order = table.Column<int>(type: "int", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_AppGroupBuyItemGroups_AppItems_Item1Id",
                        column: x => x.Item1Id,
                        principalTable: "AppItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppGroupBuyItemGroups_AppItems_Item2Id",
                        column: x => x.Item2Id,
                        principalTable: "AppItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppGroupBuyItemGroups_AppItems_Item3Id",
                        column: x => x.Item3Id,
                        principalTable: "AppItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppGroupBuyItemGroups_AppItems_Item4Id",
                        column: x => x.Item4Id,
                        principalTable: "AppItems",
                        principalColumn: "Id");
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuyItemGroups_GroupBuyId",
                table: "AppGroupBuyItemGroups",
                column: "GroupBuyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuyItemGroups_Item1Id",
                table: "AppGroupBuyItemGroups",
                column: "Item1Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuyItemGroups_Item2Id",
                table: "AppGroupBuyItemGroups",
                column: "Item2Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuyItemGroups_Item3Id",
                table: "AppGroupBuyItemGroups",
                column: "Item3Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuyItemGroups_Item4Id",
                table: "AppGroupBuyItemGroups",
                column: "Item4Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppGroupBuyItemGroups");

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
        }
    }
}

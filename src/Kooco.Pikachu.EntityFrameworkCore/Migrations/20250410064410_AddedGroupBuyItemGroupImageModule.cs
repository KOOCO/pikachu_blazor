using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedGroupBuyItemGroupImageModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupBuyItemGroupImageModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupBuyItemGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupBuyItemGroupImageModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupBuyItemGroupImageModule_AppGroupBuyItemGroups_GroupBuyItemGroupId",
                        column: x => x.GroupBuyItemGroupId,
                        principalTable: "AppGroupBuyItemGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupBuyItemGroupImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupBuyItemGroupImageModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlobImageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupBuyItemGroupImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupBuyItemGroupImage_GroupBuyItemGroupImageModule_GroupBuyItemGroupImageModuleId",
                        column: x => x.GroupBuyItemGroupImageModuleId,
                        principalTable: "GroupBuyItemGroupImageModule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupBuyItemGroupImage_GroupBuyItemGroupImageModuleId",
                table: "GroupBuyItemGroupImage",
                column: "GroupBuyItemGroupImageModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupBuyItemGroupImageModule_GroupBuyItemGroupId",
                table: "GroupBuyItemGroupImageModule",
                column: "GroupBuyItemGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppGroupBuyItemsPriceses");

            migrationBuilder.DropTable(
                name: "GroupBuyItemGroupImage");

            migrationBuilder.DropTable(
                name: "GroupBuyItemGroupImageModule");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AlterGroupBuyItemGroupImageModuleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupBuyItemGroupImage_GroupBuyItemGroupImageModule_GroupBuyItemGroupImageModuleId",
                table: "GroupBuyItemGroupImage");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupBuyItemGroupImageModule_AppGroupBuyItemGroups_GroupBuyItemGroupId",
                table: "GroupBuyItemGroupImageModule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupBuyItemGroupImageModule",
                table: "GroupBuyItemGroupImageModule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupBuyItemGroupImage",
                table: "GroupBuyItemGroupImage");

            migrationBuilder.RenameTable(
                name: "GroupBuyItemGroupImageModule",
                newName: "AppGroupBuyItemGroupImageModules");

            migrationBuilder.RenameTable(
                name: "GroupBuyItemGroupImage",
                newName: "AppGroupBuyItemGroupImages");

            migrationBuilder.RenameIndex(
                name: "IX_GroupBuyItemGroupImageModule_GroupBuyItemGroupId",
                table: "AppGroupBuyItemGroupImageModules",
                newName: "IX_AppGroupBuyItemGroupImageModules_GroupBuyItemGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupBuyItemGroupImage_GroupBuyItemGroupImageModuleId",
                table: "AppGroupBuyItemGroupImages",
                newName: "IX_AppGroupBuyItemGroupImages_GroupBuyItemGroupImageModuleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppGroupBuyItemGroupImageModules",
                table: "AppGroupBuyItemGroupImageModules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppGroupBuyItemGroupImages",
                table: "AppGroupBuyItemGroupImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppGroupBuyItemGroupImageModules_AppGroupBuyItemGroups_GroupBuyItemGroupId",
                table: "AppGroupBuyItemGroupImageModules",
                column: "GroupBuyItemGroupId",
                principalTable: "AppGroupBuyItemGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppGroupBuyItemGroupImages_AppGroupBuyItemGroupImageModules_GroupBuyItemGroupImageModuleId",
                table: "AppGroupBuyItemGroupImages",
                column: "GroupBuyItemGroupImageModuleId",
                principalTable: "AppGroupBuyItemGroupImageModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppGroupBuyItemGroupImageModules_AppGroupBuyItemGroups_GroupBuyItemGroupId",
                table: "AppGroupBuyItemGroupImageModules");

            migrationBuilder.DropForeignKey(
                name: "FK_AppGroupBuyItemGroupImages_AppGroupBuyItemGroupImageModules_GroupBuyItemGroupImageModuleId",
                table: "AppGroupBuyItemGroupImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppGroupBuyItemGroupImages",
                table: "AppGroupBuyItemGroupImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppGroupBuyItemGroupImageModules",
                table: "AppGroupBuyItemGroupImageModules");

            migrationBuilder.RenameTable(
                name: "AppGroupBuyItemGroupImages",
                newName: "GroupBuyItemGroupImage");

            migrationBuilder.RenameTable(
                name: "AppGroupBuyItemGroupImageModules",
                newName: "GroupBuyItemGroupImageModule");

            migrationBuilder.RenameIndex(
                name: "IX_AppGroupBuyItemGroupImages_GroupBuyItemGroupImageModuleId",
                table: "GroupBuyItemGroupImage",
                newName: "IX_GroupBuyItemGroupImage_GroupBuyItemGroupImageModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_AppGroupBuyItemGroupImageModules_GroupBuyItemGroupId",
                table: "GroupBuyItemGroupImageModule",
                newName: "IX_GroupBuyItemGroupImageModule_GroupBuyItemGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupBuyItemGroupImage",
                table: "GroupBuyItemGroupImage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupBuyItemGroupImageModule",
                table: "GroupBuyItemGroupImageModule",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupBuyItemGroupImage_GroupBuyItemGroupImageModule_GroupBuyItemGroupImageModuleId",
                table: "GroupBuyItemGroupImage",
                column: "GroupBuyItemGroupImageModuleId",
                principalTable: "GroupBuyItemGroupImageModule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupBuyItemGroupImageModule_AppGroupBuyItemGroups_GroupBuyItemGroupId",
                table: "GroupBuyItemGroupImageModule",
                column: "GroupBuyItemGroupId",
                principalTable: "AppGroupBuyItemGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

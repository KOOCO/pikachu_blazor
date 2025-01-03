using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedWebsiteSettingsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "AppWebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "AppWebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Line",
                table: "AppWebsiteSettings");

            migrationBuilder.DropColumn(
                name: "LogoName",
                table: "AppWebsiteSettings");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "AppWebsiteSettings");

            migrationBuilder.RenameColumn(
                name: "TitleDisplayOption",
                table: "AppWebsiteSettings",
                newName: "PageType");

            migrationBuilder.RenameColumn(
                name: "StoreTitle",
                table: "AppWebsiteSettings",
                newName: "PageTitle");

            migrationBuilder.RenameColumn(
                name: "ReturnExchangePolicy",
                table: "AppWebsiteSettings",
                newName: "PageLink");

            migrationBuilder.RenameColumn(
                name: "NotificationBar",
                table: "AppWebsiteSettings",
                newName: "ExtraProperties");

            migrationBuilder.AddColumn<string>(
                name: "ArticleHtml",
                table: "AppWebsiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppWebsiteSettings",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GroupBuyModuleType",
                table: "AppWebsiteSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageDescription",
                table: "AppWebsiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductCategoryId",
                table: "AppWebsiteSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SetAsHomePage",
                table: "AppWebsiteSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TemplateType",
                table: "AppWebsiteSettings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppWebsiteSettingsInstructionModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebsiteSettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWebsiteSettingsInstructionModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppWebsiteSettingsInstructionModules_AppWebsiteSettings_WebsiteSettingsId",
                        column: x => x.WebsiteSettingsId,
                        principalTable: "AppWebsiteSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppWebsiteSettingsModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WebsiteSettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    GroupBuyModuleType = table.Column<int>(type: "int", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductGroupModuleTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductGroupModuleImageSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWebsiteSettingsModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppWebsiteSettingsModules_AppWebsiteSettings_WebsiteSettingsId",
                        column: x => x.WebsiteSettingsId,
                        principalTable: "AppWebsiteSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppWebsiteSettingsOverviewModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebsiteSettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsButtonEnable = table.Column<bool>(type: "bit", nullable: false),
                    ButtonText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWebsiteSettingsOverviewModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppWebsiteSettingsOverviewModules_AppWebsiteSettings_WebsiteSettingsId",
                        column: x => x.WebsiteSettingsId,
                        principalTable: "AppWebsiteSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppWebsiteSettingsProductRankingModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebsiteSettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleNumber = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWebsiteSettingsProductRankingModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppWebsiteSettingsProductRankingModules_AppWebsiteSettings_WebsiteSettingsId",
                        column: x => x.WebsiteSettingsId,
                        principalTable: "AppWebsiteSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppWebsiteSettingsModuleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WebsiteSettingsModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SetItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleNumber = table.Column<int>(type: "int", nullable: true),
                    WebsiteSettingsModuleId2 = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWebsiteSettingsModuleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppWebsiteSettingsModuleItems_AppItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "AppItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppWebsiteSettingsModuleItems_AppSetItems_SetItemId",
                        column: x => x.SetItemId,
                        principalTable: "AppSetItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppWebsiteSettingsModuleItems_AppWebsiteSettingsModules_WebsiteSettingsModuleId",
                        column: x => x.WebsiteSettingsModuleId,
                        principalTable: "AppWebsiteSettingsModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppWebsiteSettingsInstructionModules_WebsiteSettingsId",
                table: "AppWebsiteSettingsInstructionModules",
                column: "WebsiteSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWebsiteSettingsModuleItems_ItemId",
                table: "AppWebsiteSettingsModuleItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWebsiteSettingsModuleItems_SetItemId",
                table: "AppWebsiteSettingsModuleItems",
                column: "SetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWebsiteSettingsModuleItems_WebsiteSettingsModuleId",
                table: "AppWebsiteSettingsModuleItems",
                column: "WebsiteSettingsModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWebsiteSettingsModules_WebsiteSettingsId",
                table: "AppWebsiteSettingsModules",
                column: "WebsiteSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWebsiteSettingsOverviewModules_WebsiteSettingsId",
                table: "AppWebsiteSettingsOverviewModules",
                column: "WebsiteSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWebsiteSettingsProductRankingModules_WebsiteSettingsId",
                table: "AppWebsiteSettingsProductRankingModules",
                column: "WebsiteSettingsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppWebsiteSettingsInstructionModules");

            migrationBuilder.DropTable(
                name: "AppWebsiteSettingsModuleItems");

            migrationBuilder.DropTable(
                name: "AppWebsiteSettingsOverviewModules");

            migrationBuilder.DropTable(
                name: "AppWebsiteSettingsProductRankingModules");

            migrationBuilder.DropTable(
                name: "AppWebsiteSettingsModules");

            migrationBuilder.DropColumn(
                name: "ArticleHtml",
                table: "AppWebsiteSettings");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppWebsiteSettings");

            migrationBuilder.DropColumn(
                name: "GroupBuyModuleType",
                table: "AppWebsiteSettings");

            migrationBuilder.DropColumn(
                name: "PageDescription",
                table: "AppWebsiteSettings");

            migrationBuilder.DropColumn(
                name: "ProductCategoryId",
                table: "AppWebsiteSettings");

            migrationBuilder.DropColumn(
                name: "SetAsHomePage",
                table: "AppWebsiteSettings");

            migrationBuilder.DropColumn(
                name: "TemplateType",
                table: "AppWebsiteSettings");

            migrationBuilder.RenameColumn(
                name: "PageType",
                table: "AppWebsiteSettings",
                newName: "TitleDisplayOption");

            migrationBuilder.RenameColumn(
                name: "PageTitle",
                table: "AppWebsiteSettings",
                newName: "StoreTitle");

            migrationBuilder.RenameColumn(
                name: "PageLink",
                table: "AppWebsiteSettings",
                newName: "ReturnExchangePolicy");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                table: "AppWebsiteSettings",
                newName: "NotificationBar");

            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "AppWebsiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "AppWebsiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Line",
                table: "AppWebsiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogoName",
                table: "AppWebsiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "AppWebsiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

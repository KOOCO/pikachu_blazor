using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingCreditUsageSettingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppShoppingCreditUsageSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowUsage = table.Column<bool>(type: "bit", nullable: false),
                    DeductionMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnifiedMaxDeductiblePoints = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StagedSettings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicableItems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsableGroupbuysScope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsableProductsScope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppShoppingCreditUsageSettings", x => x.Id);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppShoppingCreditUsageSpecificGroupbuys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShoppingCreditsUsageSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupbuyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppShoppingCreditUsageSpecificGroupbuys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppShoppingCreditUsageSpecificGroupbuys_AppGroupBuys_GroupbuyId",
                        column: x => x.GroupbuyId,
                        principalTable: "AppGroupBuys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppShoppingCreditUsageSpecificGroupbuys_AppShoppingCreditUsageSettings_ShoppingCreditsUsageSettingId",
                        column: x => x.ShoppingCreditsUsageSettingId,
                        principalTable: "AppShoppingCreditUsageSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppShoppingCreditUsageSpecificProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShoppingCreditsUsageSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppShoppingCreditUsageSpecificProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppShoppingCreditUsageSpecificProducts_AppItems_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppShoppingCreditUsageSpecificProducts_AppShoppingCreditUsageSettings_ShoppingCreditsUsageSettingId",
                        column: x => x.ShoppingCreditsUsageSettingId,
                        principalTable: "AppShoppingCreditUsageSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppShoppingCreditUsageSpecificGroupbuys_GroupbuyId",
                table: "AppShoppingCreditUsageSpecificGroupbuys",
                column: "GroupbuyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppShoppingCreditUsageSpecificGroupbuys_ShoppingCreditsUsageSettingId",
                table: "AppShoppingCreditUsageSpecificGroupbuys",
                column: "ShoppingCreditsUsageSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_AppShoppingCreditUsageSpecificProducts_ProductId",
                table: "AppShoppingCreditUsageSpecificProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppShoppingCreditUsageSpecificProducts_ShoppingCreditsUsageSettingId",
                table: "AppShoppingCreditUsageSpecificProducts",
                column: "ShoppingCreditsUsageSettingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppShoppingCreditUsageSpecificGroupbuys");

            migrationBuilder.DropTable(
                name: "AppShoppingCreditUsageSpecificProducts");

            migrationBuilder.DropTable(
                name: "AppShoppingCreditUsageSettings");
        }
    }
}

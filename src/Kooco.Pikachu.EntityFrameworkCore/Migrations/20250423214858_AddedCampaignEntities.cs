using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedCampaignEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TargetAudienceJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PromotionModule = table.Column<int>(type: "int", nullable: false),
                    ApplyToAllGroupBuys = table.Column<bool>(type: "bit", nullable: false),
                    ApplyToAllProducts = table.Column<bool>(type: "bit", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
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
                    table.PrimaryKey("PK_AppCampaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppCampaignAddOnProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductAmount = table.Column<int>(type: "int", nullable: false),
                    LimitPerOrder = table.Column<int>(type: "int", nullable: false),
                    IsUnlimitedQuantity = table.Column<bool>(type: "bit", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "int", nullable: true),
                    DisplayPrice = table.Column<int>(type: "int", nullable: false),
                    ProductCondition = table.Column<int>(type: "int", nullable: false),
                    Threshold = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCampaignAddOnProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCampaignAddOnProducts_AppCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "AppCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppCampaignAddOnProducts_AppItems_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCampaignDiscounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDiscountCodeRequired = table.Column<bool>(type: "bit", nullable: false),
                    DiscountCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    AvailableQuantity = table.Column<int>(type: "int", nullable: false),
                    MaximumUsePerPerson = table.Column<int>(type: "int", nullable: false),
                    DiscountMethod = table.Column<int>(type: "int", nullable: false),
                    MinimumSpendAmount = table.Column<int>(type: "int", nullable: true),
                    ApplyToAllShippingMethods = table.Column<bool>(type: "bit", nullable: true),
                    DeliveryMethodsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscountType = table.Column<int>(type: "int", nullable: true),
                    DiscountAmount = table.Column<int>(type: "int", nullable: true),
                    DiscountPercentage = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCampaignDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCampaignDiscounts_AppCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "AppCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCampaignGroupBuys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupBuyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCampaignGroupBuys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCampaignGroupBuys_AppCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "AppCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppCampaignGroupBuys_AppGroupBuys_GroupBuyId",
                        column: x => x.GroupBuyId,
                        principalTable: "AppGroupBuys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCampaignProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCampaignProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCampaignProducts_AppCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "AppCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppCampaignProducts_AppItems_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCampaignShoppingCredits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanExpire = table.Column<bool>(type: "bit", nullable: false),
                    ValidForDays = table.Column<int>(type: "int", nullable: true),
                    CalculationMethod = table.Column<int>(type: "int", nullable: false),
                    CalculationPercentage = table.Column<double>(type: "float", nullable: true),
                    ApplicableItem = table.Column<int>(type: "int", nullable: false),
                    Budget = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCampaignShoppingCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCampaignShoppingCredits_AppCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "AppCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCampaignStageSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShoppingCreditId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Spend = table.Column<int>(type: "int", nullable: false),
                    PointsToReceive = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCampaignStageSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCampaignStageSettings_AppCampaignShoppingCredits_ShoppingCreditId",
                        column: x => x.ShoppingCreditId,
                        principalTable: "AppCampaignShoppingCredits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignAddOnProducts_CampaignId",
                table: "AppCampaignAddOnProducts",
                column: "CampaignId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignAddOnProducts_ProductId",
                table: "AppCampaignAddOnProducts",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignDiscounts_CampaignId",
                table: "AppCampaignDiscounts",
                column: "CampaignId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignGroupBuys_CampaignId",
                table: "AppCampaignGroupBuys",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignGroupBuys_GroupBuyId",
                table: "AppCampaignGroupBuys",
                column: "GroupBuyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignProducts_CampaignId",
                table: "AppCampaignProducts",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignProducts_ProductId",
                table: "AppCampaignProducts",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignShoppingCredits_CampaignId",
                table: "AppCampaignShoppingCredits",
                column: "CampaignId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCampaignStageSettings_ShoppingCreditId",
                table: "AppCampaignStageSettings",
                column: "ShoppingCreditId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCampaignAddOnProducts");

            migrationBuilder.DropTable(
                name: "AppCampaignDiscounts");

            migrationBuilder.DropTable(
                name: "AppCampaignGroupBuys");

            migrationBuilder.DropTable(
                name: "AppCampaignProducts");

            migrationBuilder.DropTable(
                name: "AppCampaignStageSettings");

            migrationBuilder.DropTable(
                name: "AppCampaignShoppingCredits");

            migrationBuilder.DropTable(
                name: "AppCampaigns");
        }
    }
}

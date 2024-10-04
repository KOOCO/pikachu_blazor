using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingCreditEarnSettingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppShoppingCreditEarnSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationBonusEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationEarnedPoints = table.Column<int>(type: "int", nullable: false),
                    RegistrationUsagePeriodType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationValidDays = table.Column<int>(type: "int", nullable: false),
                    BirthdayBonusEnabled = table.Column<bool>(type: "bit", nullable: false),
                    BirthdayEarnedPoints = table.Column<int>(type: "int", nullable: false),
                    BirthdayUsagePeriodType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthdayValidDays = table.Column<int>(type: "int", nullable: false),
                    CashbackEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CashbackUsagePeriodType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CashbackValidDays = table.Column<int>(type: "int", nullable: false),
                    CashbackCalculationMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CashbackUnifiedMaxDeductiblePoints = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashbackStagedSettings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CashbackApplicableItems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CashbackApplicableGroupbuys = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CashbackApplicableProducts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppShoppingCreditEarnSettings", x => x.Id);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppShoppingCreditEarnSpecificGroupbuys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShoppingCreditEarnSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupbuyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppShoppingCreditEarnSpecificGroupbuys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppShoppingCreditEarnSpecificGroupbuys_AppGroupBuys_GroupbuyId",
                        column: x => x.GroupbuyId,
                        principalTable: "AppGroupBuys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppShoppingCreditEarnSpecificGroupbuys_AppShoppingCreditEarnSettings_ShoppingCreditEarnSettingId",
                        column: x => x.ShoppingCreditEarnSettingId,
                        principalTable: "AppShoppingCreditEarnSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppShoppingCreditEarnSpecificProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShoppingCreditEranSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShoppingCreditEarnSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppShoppingCreditEarnSpecificProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppShoppingCreditEarnSpecificProducts_AppItems_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppShoppingCreditEarnSpecificProducts_AppShoppingCreditEarnSettings_ShoppingCreditEarnSettingId",
                        column: x => x.ShoppingCreditEarnSettingId,
                        principalTable: "AppShoppingCreditEarnSettings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppShoppingCreditEarnSpecificProducts_AppShoppingCreditUsageSettings_ShoppingCreditEranSettingId",
                        column: x => x.ShoppingCreditEranSettingId,
                        principalTable: "AppShoppingCreditUsageSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppShoppingCreditEarnSpecificGroupbuys_GroupbuyId",
                table: "AppShoppingCreditEarnSpecificGroupbuys",
                column: "GroupbuyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppShoppingCreditEarnSpecificGroupbuys_ShoppingCreditEarnSettingId",
                table: "AppShoppingCreditEarnSpecificGroupbuys",
                column: "ShoppingCreditEarnSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_AppShoppingCreditEarnSpecificProducts_ProductId",
                table: "AppShoppingCreditEarnSpecificProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppShoppingCreditEarnSpecificProducts_ShoppingCreditEarnSettingId",
                table: "AppShoppingCreditEarnSpecificProducts",
                column: "ShoppingCreditEarnSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_AppShoppingCreditEarnSpecificProducts_ShoppingCreditEranSettingId",
                table: "AppShoppingCreditEarnSpecificProducts",
                column: "ShoppingCreditEranSettingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppShoppingCreditEarnSpecificGroupbuys");

            migrationBuilder.DropTable(
                name: "AppShoppingCreditEarnSpecificProducts");

            migrationBuilder.DropTable(
                name: "AppShoppingCreditEarnSettings");
        }
    }
}

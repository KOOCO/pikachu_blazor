using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupBuysTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    isDefault = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Warehouse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppGroupBuys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroupBuyNo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupBuyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntryURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntryURL2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectLine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BannerURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FreeShipping = table.Column<bool>(type: "bit", nullable: false),
                    allowShipToOuterTaiwan = table.Column<bool>(type: "bit", nullable: false),
                    allowShipOversea = table.Column<bool>(type: "bit", nullable: false),
                    ExpectShippingDateFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectShippingDateTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoneyTransferValidDayBy = table.Column<int>(type: "int", nullable: false),
                    MoneyTransferValidDays = table.Column<int>(type: "int", nullable: true),
                    issueInvoice = table.Column<bool>(type: "bit", nullable: false),
                    AutoIssueTriplicateInvoice = table.Column<bool>(type: "bit", nullable: false),
                    InvoiceNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProtectPrivacyData = table.Column<bool>(type: "bit", nullable: false),
                    InviteCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfitShare = table.Column<int>(type: "int", nullable: false),
                    MetaPixelNo = table.Column<int>(type: "int", nullable: true),
                    FBID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IGID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LineID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GAID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GTM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarningMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderContactInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExchangePolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotifyMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_AppGroupBuys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppGroupBuys_Warehouse_DefaultWarehouseId",
                        column: x => x.DefaultWarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupBuys_DefaultWarehouseId",
                table: "AppGroupBuys",
                column: "DefaultWarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppGroupBuys");

            migrationBuilder.DropTable(
                name: "Warehouse");
        }
    }
}

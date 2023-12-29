using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedLogisticsProvidersSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppLogisticsProviderSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    StoreCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HashKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HashIV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogisticsType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogisticsSubTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FreeShippingThreshold = table.Column<int>(type: "int", nullable: false),
                    Freight = table.Column<int>(type: "int", nullable: false),
                    CustomTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainIslands = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OuterIslands = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogisticProvider = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AppLogisticsProviderSettings", x => x.Id);
                },
                comment: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLogisticsProviderSettings");
        }
    }
}

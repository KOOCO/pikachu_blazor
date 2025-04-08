using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupBuyItemsPriceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupBuyPrice",
                table: "AppSetItems");

            migrationBuilder.DropColumn(
                name: "GroupBuyPrice",
                table: "AppItemDetails");

            migrationBuilder.CreateTable(
                name: "AppGroupBuyItemsPriceses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SetItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroupBuyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroupBuyPrice = table.Column<float>(type: "real", nullable: true),
                    ItemDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_AppGroupBuyItemsPriceses", x => x.Id);
                },
                comment: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppGroupBuyItemsPriceses");

            migrationBuilder.AddColumn<int>(
                name: "GroupBuyPrice",
                table: "AppSetItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "GroupBuyPrice",
                table: "AppItemDetails",
                type: "real",
                nullable: true);
        }
    }
}

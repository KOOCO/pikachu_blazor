using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedFreebie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FreebieId",
                table: "AppImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppFreebie",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplyToAllGroupBuy = table.Column<bool>(type: "bit", nullable: false),
                    UnCondition = table.Column<bool>(type: "bit", nullable: false),
                    ActivityStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivityEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FreebieOrderReach = table.Column<int>(type: "int", nullable: true),
                    MinimumAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinimumPiece = table.Column<int>(type: "int", nullable: true),
                    FreebieAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_AppFreebie", x => x.Id);
                },
                comment: "");

            migrationBuilder.CreateTable(
                name: "AppFreebieGroupBuys",
                columns: table => new
                {
                    FreebieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupBuyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFreebieGroupBuys", x => new { x.FreebieId, x.GroupBuyId });
                    table.ForeignKey(
                        name: "FK_AppFreebieGroupBuys_AppFreebie_FreebieId",
                        column: x => x.FreebieId,
                        principalTable: "AppFreebie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppImages_FreebieId",
                table: "AppImages",
                column: "FreebieId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppImages_AppFreebie_FreebieId",
                table: "AppImages",
                column: "FreebieId",
                principalTable: "AppFreebie",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppImages_AppFreebie_FreebieId",
                table: "AppImages");

            migrationBuilder.DropTable(
                name: "AppFreebieGroupBuys");

            migrationBuilder.DropTable(
                name: "AppFreebie");

            migrationBuilder.DropIndex(
                name: "IX_AppImages_FreebieId",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "FreebieId",
                table: "AppImages");
        }
    }
}

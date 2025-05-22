using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class EdmRemovedGroupBuyMultiSelection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEdmGroupBuys");

            migrationBuilder.DropColumn(
                name: "ApplyToAllGroupBuys",
                table: "AppEdms");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupBuyId",
                table: "AppEdms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppEdms_GroupBuyId",
                table: "AppEdms",
                column: "GroupBuyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEdms_AppGroupBuys_GroupBuyId",
                table: "AppEdms",
                column: "GroupBuyId",
                principalTable: "AppGroupBuys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppEdms_AppGroupBuys_GroupBuyId",
                table: "AppEdms");

            migrationBuilder.DropIndex(
                name: "IX_AppEdms_GroupBuyId",
                table: "AppEdms");

            migrationBuilder.DropColumn(
                name: "GroupBuyId",
                table: "AppEdms");

            migrationBuilder.AddColumn<bool>(
                name: "ApplyToAllGroupBuys",
                table: "AppEdms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AppEdmGroupBuys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EdmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupBuyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEdmGroupBuys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppEdmGroupBuys_AppEdms_EdmId",
                        column: x => x.EdmId,
                        principalTable: "AppEdms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppEdmGroupBuys_AppGroupBuys_GroupBuyId",
                        column: x => x.GroupBuyId,
                        principalTable: "AppGroupBuys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEdmGroupBuys_EdmId",
                table: "AppEdmGroupBuys",
                column: "EdmId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEdmGroupBuys_GroupBuyId",
                table: "AppEdmGroupBuys",
                column: "GroupBuyId");
        }
    }
}

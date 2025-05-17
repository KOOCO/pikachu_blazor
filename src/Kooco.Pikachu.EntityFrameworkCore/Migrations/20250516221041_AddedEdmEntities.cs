using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedEdmEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppEdms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateType = table.Column<int>(type: "int", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MemberType = table.Column<int>(type: "int", nullable: false),
                    MemberTagsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplyToAllGroupBuys = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SendFrequency = table.Column<int>(type: "int", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_AppEdms", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEdmGroupBuys");

            migrationBuilder.DropTable(
                name: "AppEdms");
        }
    }
}

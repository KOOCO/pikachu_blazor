using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedMemberTagFilterEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppMemberTagFilters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    MemberTypesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberTagsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountSpent = table.Column<int>(type: "int", nullable: true),
                    OrdersCompleted = table.Column<int>(type: "int", nullable: true),
                    MinRegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxRegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMemberTagFilters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppMemberTagFilters_Tag",
                table: "AppMemberTagFilters",
                column: "Tag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppMemberTagFilters");
        }
    }
}

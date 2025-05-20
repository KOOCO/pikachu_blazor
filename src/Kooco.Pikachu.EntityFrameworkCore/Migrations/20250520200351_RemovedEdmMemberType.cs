using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class RemovedEdmMemberType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberType",
                table: "AppEdms");

            migrationBuilder.AddColumn<bool>(
                name: "ApplyToAllMembers",
                table: "AppEdms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyToAllMembers",
                table: "AppEdms");

            migrationBuilder.AddColumn<int>(
                name: "MemberType",
                table: "AppEdms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

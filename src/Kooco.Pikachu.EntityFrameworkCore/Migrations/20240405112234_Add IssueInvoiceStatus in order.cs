using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddIssueInvoiceStatusinorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IssueStatus",
                table: "AppOrders",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueStatus",
                table: "AppOrders");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddcreditNoteReasonColumnsinOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreditNoteReason",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditNoteReason",
                table: "AppOrders");
        }
    }
}

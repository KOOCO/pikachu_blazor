using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddModuleNumberinOrderInstructionandPurchaseOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModuleNumber",
                table: "AppGroupPurchaseOverviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModuleNumber",
                table: "AppGroupBuyOrderInstructions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModuleNumber",
                table: "AppGroupPurchaseOverviews");

            migrationBuilder.DropColumn(
                name: "ModuleNumber",
                table: "AppGroupBuyOrderInstructions");
        }
    }
}

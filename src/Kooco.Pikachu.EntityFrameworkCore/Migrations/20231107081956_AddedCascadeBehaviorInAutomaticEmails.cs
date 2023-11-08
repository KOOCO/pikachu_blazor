using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddedCascadeBehaviorInAutomaticEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAutomaticEmailGroupBuys_AppGroupBuys_GroupBuyId",
                table: "AppAutomaticEmailGroupBuys");

            migrationBuilder.AddForeignKey(
                name: "FK_AppAutomaticEmailGroupBuys_AppGroupBuys_GroupBuyId",
                table: "AppAutomaticEmailGroupBuys",
                column: "GroupBuyId",
                principalTable: "AppGroupBuys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAutomaticEmailGroupBuys_AppGroupBuys_GroupBuyId",
                table: "AppAutomaticEmailGroupBuys");

            migrationBuilder.AddForeignKey(
                name: "FK_AppAutomaticEmailGroupBuys_AppGroupBuys_GroupBuyId",
                table: "AppAutomaticEmailGroupBuys",
                column: "GroupBuyId",
                principalTable: "AppGroupBuys",
                principalColumn: "Id");
        }
    }
}

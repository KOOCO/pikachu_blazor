using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class ChangeItemNoToClusterAndAddUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppItems",
                table: "AppItems");

            migrationBuilder.DropIndex(
                name: "IX_AppItems_ItemNo",
                table: "AppItems");

            migrationBuilder.DropColumn(
                 name: "ItemNo",
                 table: "AppItems");

            migrationBuilder.AddColumn<long>(
                name: "ItemNo",
                table: "AppItems",
                type: "bigint",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppItems",
                table: "AppItems",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_AppItems_ItemNo",
                table: "AppItems",
                column: "ItemNo",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppItems",
                table: "AppItems");

            migrationBuilder.DropColumn(
             name: "ItemNo",
             table: "AppItems");

            migrationBuilder.AddColumn<long>(
                name: "ItemNo",
                table: "AppItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0);


            migrationBuilder.AddPrimaryKey(
                name: "PK_AppItems",
                table: "AppItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppItems_ItemNo",
                table: "AppItems",
                column: "ItemNo");
        }
    }
}

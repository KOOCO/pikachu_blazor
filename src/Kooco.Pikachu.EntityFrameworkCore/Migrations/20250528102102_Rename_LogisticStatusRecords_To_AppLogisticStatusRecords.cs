using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class Rename_LogisticStatusRecords_To_AppLogisticStatusRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LogisticStatusRecords",
                table: "LogisticStatusRecords");

            migrationBuilder.RenameTable(
                name: "LogisticStatusRecords",
                newName: "AppLogisticStatusRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppLogisticStatusRecords",
                table: "AppLogisticStatusRecords",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppLogisticStatusRecords",
                table: "AppLogisticStatusRecords");

            migrationBuilder.RenameTable(
                name: "AppLogisticStatusRecords",
                newName: "LogisticStatusRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogisticStatusRecords",
                table: "LogisticStatusRecords",
                column: "Id");
        }
    }
}

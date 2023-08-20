using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class updatecolumnname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomeField9Value",
                table: "AppItems",
                newName: "CustomField9Value");

            migrationBuilder.RenameColumn(
                name: "CustomeField9Name",
                table: "AppItems",
                newName: "CustomField9Name");

            migrationBuilder.RenameColumn(
                name: "CustomeField8Value",
                table: "AppItems",
                newName: "CustomField8Value");

            migrationBuilder.RenameColumn(
                name: "CustomeField8Name",
                table: "AppItems",
                newName: "CustomField8Name");

            migrationBuilder.RenameColumn(
                name: "CustomeField7Value",
                table: "AppItems",
                newName: "CustomField7Value");

            migrationBuilder.RenameColumn(
                name: "CustomeField7Name",
                table: "AppItems",
                newName: "CustomField7Name");

            migrationBuilder.RenameColumn(
                name: "CustomeField6Value",
                table: "AppItems",
                newName: "CustomField6Value");

            migrationBuilder.RenameColumn(
                name: "CustomeField6Name",
                table: "AppItems",
                newName: "CustomField6Name");

            migrationBuilder.RenameColumn(
                name: "CustomeField5Value",
                table: "AppItems",
                newName: "CustomField5Value");

            migrationBuilder.RenameColumn(
                name: "CustomeField5Name",
                table: "AppItems",
                newName: "CustomField5Name");

            migrationBuilder.RenameColumn(
                name: "CustomeField4Value",
                table: "AppItems",
                newName: "CustomField4Value");

            migrationBuilder.RenameColumn(
                name: "CustomeField4Name",
                table: "AppItems",
                newName: "CustomField4Name");

            migrationBuilder.RenameColumn(
                name: "CustomeField3Value",
                table: "AppItems",
                newName: "CustomField3Value");

            migrationBuilder.RenameColumn(
                name: "CustomeField3Name",
                table: "AppItems",
                newName: "CustomField3Name");

            migrationBuilder.RenameColumn(
                name: "CustomeField2Value",
                table: "AppItems",
                newName: "CustomField2Value");

            migrationBuilder.RenameColumn(
                name: "CustomeField2Name",
                table: "AppItems",
                newName: "CustomField2Name");

            migrationBuilder.RenameColumn(
                name: "CustomeField1Value",
                table: "AppItems",
                newName: "CustomField1Value");

            migrationBuilder.RenameColumn(
                name: "CustomeField1Name",
                table: "AppItems",
                newName: "CustomField1Name");

            migrationBuilder.RenameColumn(
                name: "CustomeField10Value",
                table: "AppItems",
                newName: "CustomField10Value");

            migrationBuilder.RenameColumn(
                name: "CustomeField10Name",
                table: "AppItems",
                newName: "CustomField10Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomField9Value",
                table: "AppItems",
                newName: "CustomeField9Value");

            migrationBuilder.RenameColumn(
                name: "CustomField9Name",
                table: "AppItems",
                newName: "CustomeField9Name");

            migrationBuilder.RenameColumn(
                name: "CustomField8Value",
                table: "AppItems",
                newName: "CustomeField8Value");

            migrationBuilder.RenameColumn(
                name: "CustomField8Name",
                table: "AppItems",
                newName: "CustomeField8Name");

            migrationBuilder.RenameColumn(
                name: "CustomField7Value",
                table: "AppItems",
                newName: "CustomeField7Value");

            migrationBuilder.RenameColumn(
                name: "CustomField7Name",
                table: "AppItems",
                newName: "CustomeField7Name");

            migrationBuilder.RenameColumn(
                name: "CustomField6Value",
                table: "AppItems",
                newName: "CustomeField6Value");

            migrationBuilder.RenameColumn(
                name: "CustomField6Name",
                table: "AppItems",
                newName: "CustomeField6Name");

            migrationBuilder.RenameColumn(
                name: "CustomField5Value",
                table: "AppItems",
                newName: "CustomeField5Value");

            migrationBuilder.RenameColumn(
                name: "CustomField5Name",
                table: "AppItems",
                newName: "CustomeField5Name");

            migrationBuilder.RenameColumn(
                name: "CustomField4Value",
                table: "AppItems",
                newName: "CustomeField4Value");

            migrationBuilder.RenameColumn(
                name: "CustomField4Name",
                table: "AppItems",
                newName: "CustomeField4Name");

            migrationBuilder.RenameColumn(
                name: "CustomField3Value",
                table: "AppItems",
                newName: "CustomeField3Value");

            migrationBuilder.RenameColumn(
                name: "CustomField3Name",
                table: "AppItems",
                newName: "CustomeField3Name");

            migrationBuilder.RenameColumn(
                name: "CustomField2Value",
                table: "AppItems",
                newName: "CustomeField2Value");

            migrationBuilder.RenameColumn(
                name: "CustomField2Name",
                table: "AppItems",
                newName: "CustomeField2Name");

            migrationBuilder.RenameColumn(
                name: "CustomField1Value",
                table: "AppItems",
                newName: "CustomeField1Value");

            migrationBuilder.RenameColumn(
                name: "CustomField1Name",
                table: "AppItems",
                newName: "CustomeField1Name");

            migrationBuilder.RenameColumn(
                name: "CustomField10Value",
                table: "AppItems",
                newName: "CustomeField10Value");

            migrationBuilder.RenameColumn(
                name: "CustomField10Name",
                table: "AppItems",
                newName: "CustomeField10Name");
        }
    }
}

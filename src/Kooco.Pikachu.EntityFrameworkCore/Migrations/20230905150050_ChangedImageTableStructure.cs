using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class ChangedImageTableStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetID",
                table: "AppImages",
                newName: "TargetId");

            migrationBuilder.RenameColumn(
                name: "SortNO",
                table: "AppImages",
                newName: "SortNo");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "AppImages",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "BlobImageName",
                table: "AppImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AppImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobImageName",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AppImages");

            migrationBuilder.RenameColumn(
                name: "TargetId",
                table: "AppImages",
                newName: "TargetID");

            migrationBuilder.RenameColumn(
                name: "SortNo",
                table: "AppImages",
                newName: "SortNO");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AppImages",
                newName: "ImagePath");
        }
    }
}

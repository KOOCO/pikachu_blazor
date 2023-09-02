using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInItemAndItemDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppImages");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppImages");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                table: "AppItemDetails",
                newName: "Attribute3Value");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LimitAvaliableTimeStart",
                table: "AppItems",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LimitAvaliableTimeEnd",
                table: "AppItems",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Attribute1Name",
                table: "AppItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attribute2Name",
                table: "AppItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attribute3Name",
                table: "AppItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attribute1Value",
                table: "AppItemDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attribute2Value",
                table: "AppItemDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LimitQuantity",
                table: "AppItemDetails",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attribute1Name",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "Attribute2Name",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "Attribute3Name",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "Attribute1Value",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "Attribute2Value",
                table: "AppItemDetails");

            migrationBuilder.DropColumn(
                name: "LimitQuantity",
                table: "AppItemDetails");

            migrationBuilder.RenameColumn(
                name: "Attribute3Value",
                table: "AppItemDetails",
                newName: "ExtraProperties");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LimitAvaliableTimeStart",
                table: "AppItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LimitAvaliableTimeEnd",
                table: "AppItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppItemDetails",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppItemDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppItemDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "AppItemDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "AppItemDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppItemDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppItemDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppItemDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppImages",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "AppImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "AppImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "AppImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppImages",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}

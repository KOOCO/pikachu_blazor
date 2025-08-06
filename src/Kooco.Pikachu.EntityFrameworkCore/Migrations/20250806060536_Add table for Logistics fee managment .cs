using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kooco.Pikachu.Migrations
{
    /// <inheritdoc />
    public partial class AddtableforLogisticsfeemanagment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppLogisticsFeeFileImports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BatchStatus = table.Column<int>(type: "int", nullable: false),
                    SuccessfulRecords = table.Column<int>(type: "int", nullable: false),
                    FailedRecords = table.Column<int>(type: "int", nullable: false),
                    ProcessingNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UploadedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessingStartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessingCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLogisticsFeeFileImports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppTenantLogisticsFeeFileProcessingSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FileImportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantTotalRecords = table.Column<int>(type: "int", nullable: false),
                    TenantTotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenantSuccessfulRecords = table.Column<int>(type: "int", nullable: false),
                    TenantFailedRecords = table.Column<int>(type: "int", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantBatchStatus = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTenantLogisticsFeeFileProcessingSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTenantLogisticsFeeFileProcessingSummaries_AppLogisticsFeeFileImports_FileImportId",
                        column: x => x.FileImportId,
                        principalTable: "AppLogisticsFeeFileImports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppTenantLogisticsFeeRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LogisticFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionStatus = table.Column<int>(type: "int", nullable: false),
                    DeductionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    LastRetryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileImportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantWalletTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTenantLogisticsFeeRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTenantLogisticsFeeRecords_AppLogisticsFeeFileImports_FileImportId",
                        column: x => x.FileImportId,
                        principalTable: "AppLogisticsFeeFileImports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppTenantLogisticsFeeRecords_AppTenantWalletTransactions_TenantWalletTransactionId",
                        column: x => x.TenantWalletTransactionId,
                        principalTable: "AppTenantWalletTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppLogisticsFeeFileImports_BatchStatus",
                table: "AppLogisticsFeeFileImports",
                column: "BatchStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AppLogisticsFeeFileImports_FileType",
                table: "AppLogisticsFeeFileImports",
                column: "FileType");

            migrationBuilder.CreateIndex(
                name: "IX_AppLogisticsFeeFileImports_FileType_UploadDate",
                table: "AppLogisticsFeeFileImports",
                columns: new[] { "FileType", "UploadDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AppLogisticsFeeFileImports_UploadDate",
                table: "AppLogisticsFeeFileImports",
                column: "UploadDate");

            migrationBuilder.CreateIndex(
                name: "IX_AppLogisticsFeeFileImports_UploadedByUserId",
                table: "AppLogisticsFeeFileImports",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeFileProcessingSummaries_FileImportId",
                table: "AppTenantLogisticsFeeFileProcessingSummaries",
                column: "FileImportId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeFileProcessingSummaries_TenantId",
                table: "AppTenantLogisticsFeeFileProcessingSummaries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeFileProcessingSummaries_TenantId_FileImportId",
                table: "AppTenantLogisticsFeeFileProcessingSummaries",
                columns: new[] { "TenantId", "FileImportId" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeRecords_DeductionStatus",
                table: "AppTenantLogisticsFeeRecords",
                column: "DeductionStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeRecords_FileImportId",
                table: "AppTenantLogisticsFeeRecords",
                column: "FileImportId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeRecords_FileType",
                table: "AppTenantLogisticsFeeRecords",
                column: "FileType");

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeRecords_OrderNumber",
                table: "AppTenantLogisticsFeeRecords",
                column: "OrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeRecords_TenantId",
                table: "AppTenantLogisticsFeeRecords",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeRecords_TenantId_FileImportId",
                table: "AppTenantLogisticsFeeRecords",
                columns: new[] { "TenantId", "FileImportId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeRecords_TenantId_FileType",
                table: "AppTenantLogisticsFeeRecords",
                columns: new[] { "TenantId", "FileType" });

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeRecords_TenantId_FileType_DeductionStatus",
                table: "AppTenantLogisticsFeeRecords",
                columns: new[] { "TenantId", "FileType", "DeductionStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantLogisticsFeeRecords_TenantWalletTransactionId",
                table: "AppTenantLogisticsFeeRecords",
                column: "TenantWalletTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTenantLogisticsFeeFileProcessingSummaries");

            migrationBuilder.DropTable(
                name: "AppTenantLogisticsFeeRecords");

            migrationBuilder.DropTable(
                name: "AppLogisticsFeeFileImports");
        }
    }
}

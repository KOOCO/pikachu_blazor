using Kooco.Pikachu.EnumValues;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.LogisticsFeeManagements.Services
{
    public class FileParsingService : IFileParsingService, ITransientDependency
    {
        private readonly ILogger<FileParsingService> _logger;

        public FileParsingService(ILogger<FileParsingService> logger)
        {
            _logger = logger;
        }

        public async Task<FileParsingResult> ParseFileAsync(string filePath, LogisticsFileType fileType)
        {
            try
            {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();

                return extension switch
                {
                    ".csv" => await ParseCsvFileAsync(filePath, fileType),
                    ".xlsx" => await ParseExcelFileAsync(filePath, fileType),
                    _ => new FileParsingResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Unsupported file format. Only CSV and XLSX files are supported."
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing file: {FilePath}", filePath);
                return new FileParsingResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error parsing file: {ex.Message}"
                };
            }
        }

        private async Task<FileParsingResult> ParseCsvFileAsync(string filePath, LogisticsFileType fileType)
        {
            var result = new FileParsingResult();
            var records = new List<LogisticsFeeRecord>();
            decimal totalAmount = 0;
            int rowNumber = 0;

            try
            {
                using var reader = new StreamReader(filePath);
                string line;

                // Skip header rows based on file type
                int skipRows = fileType == LogisticsFileType.TCAT ? 3 : 2;

                while ((line = await reader.ReadLineAsync()) != null && rowNumber < skipRows)
                {
                    rowNumber++;
                }

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    rowNumber++;

                    var parsedRecord = ParseCsvLine(line, fileType, rowNumber);
                    if (parsedRecord != null)
                    {
                        records.Add(parsedRecord);
                        totalAmount += parsedRecord.FeeAmount;
                    }
                }

                result.IsSuccess = true;
                result.Records = records;
                result.TotalRecords = records.Count;
                result.TotalAmount = totalAmount;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = $"Error parsing CSV: {ex.Message}";
            }

            return result;
        }

        private async Task<FileParsingResult> ParseExcelFileAsync(string filePath, LogisticsFileType fileType)
        {
            var result = new FileParsingResult();
            var records = new List<LogisticsFeeRecord>();
            decimal totalAmount = 0;

            try
            {
                ExcelPackage.License.SetNonCommercialOrganization("Pikachu");

                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(filePath);

                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync();
                using var package = new ExcelPackage(stream);
                // Access the first worksheet
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                // Start from appropriate row based on file type
                int startRow = fileType == LogisticsFileType.TCAT ? 4 : 3; // T-Cat has Title(1) + Empty(2) + Headers(3) + Data(4+)
                                                                           // ECPay has Title(1) + Headers(2) + Data(3+)

                for (int row = startRow; row <= rowCount; row++)
                {
                    var parsedRecord = ParseExcelRow(worksheet, row, fileType);
                    if (parsedRecord != null)
                    {
                        records.Add(parsedRecord);
                        totalAmount += parsedRecord.FeeAmount;
                    }
                }

                result.IsSuccess = true;
                result.Records = records;
                result.TotalRecords = records.Count;
                result.TotalAmount = totalAmount;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = $"Error parsing Excel: {ex.Message}";
            }

            return result;
        }

        private LogisticsFeeRecord ParseCsvLine(string line, LogisticsFileType fileType, int rowNumber)
        {
            try
            {
                var values = line.Split(',');

                return fileType switch
                {
                    LogisticsFileType.TCAT => ParseTCatCsvValues(values, rowNumber),
                    LogisticsFileType.ECPay => ParseECPayCsvValues(values, rowNumber),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error parsing CSV line {RowNumber}: {Error}", rowNumber, ex.Message);
                return null;
            }
        }

        private LogisticsFeeRecord ParseExcelRow(ExcelWorksheet worksheet, int row, LogisticsFileType fileType)
        {
            try
            {
                return fileType switch
                {
                    LogisticsFileType.TCAT => ParseTCatExcelRow(worksheet, row),
                    LogisticsFileType.ECPay => ParseECPayExcelRow(worksheet, row),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error parsing Excel row {RowNumber}: {Error}", row, ex.Message);
                return null;
            }
        }

        private LogisticsFeeRecord ParseTCatCsvValues(string[] values, int rowNumber)
        {
            // T-Cat CSV Structure: Skip first 3 rows (Title, Empty, Headers), then data starts
            if (values.Length < 9) return null;

            var merchantTradeNo = values[5]?.Trim().Trim('"'); // 訂單編號 (Column 5)
            var shippingFeeStr = values[7]?.Trim().Trim('"');  // 運費金額 (Column 7)
            var extraFeeStr = values[8]?.Trim().Trim('"');     // 附加服務金 (Column 8)

            // Skip footer rows (like "依客戶代號、集貨日期、託運單號排序")
            if (!string.IsNullOrEmpty(merchantTradeNo) && merchantTradeNo.Contains("排序"))
            {
                return null;
            }

            if (string.IsNullOrEmpty(merchantTradeNo))
            {
                return null;
            }

            var shippingFee = decimal.TryParse(shippingFeeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var sf) ? sf : 0;
            var extraFee = decimal.TryParse(extraFeeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var ef) ? ef : 0;
            var totalLogisticsFee = shippingFee + extraFee;

            return new LogisticsFeeRecord
            {
                MerchantTradeNo = merchantTradeNo,
                FeeAmount = totalLogisticsFee,
                RowNumber = rowNumber,
                ShippingFee = shippingFee,
                ExtraShippingFee = extraFee,
                ShippingNo = values[6]?.Trim().Trim('"'), // 託運單號 (Column 6)
                // Additional metadata
                CustomerCode = values[0]?.Trim().Trim('"'),     // 客戶代號
                CollectionDate = values[1]?.Trim().Trim('"'),   // 集貨日期
                CollectionOffice = values[2]?.Trim().Trim('"'), // 集貨所
                DeliveryDate = values[3]?.Trim().Trim('"'),     // 配完日期
                DeliveryOffice = values[4]?.Trim().Trim('"')    // 配完所
            };
        }

        private LogisticsFeeRecord ParseECPayCsvValues(string[] values, int rowNumber)
        {
            // ECPay CSV Structure: Title(0) + Headers(1) + Data(2+)
            if (values.Length < 21) return null;

            var merchantTradeNo = values[1]?.Trim().Trim('"'); // 廠商訂單編號 (Column 1)
            var shippingFeeStr = values[19]?.Trim().Trim('"'); // 實際物流運費 (Column 19)
            var extraFeeStr = values[20]?.Trim().Trim('"');    // 加價費用 (Column 20)

            if (string.IsNullOrEmpty(merchantTradeNo))
            {
                return null;
            }

            var shippingFee = decimal.TryParse(shippingFeeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var sf) ? sf : 0;
            var extraFee = decimal.TryParse(extraFeeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var ef) ? ef : 0;
            var totalLogisticsFee = shippingFee + extraFee;

            return new LogisticsFeeRecord
            {
                MerchantTradeNo = merchantTradeNo,
                FeeAmount = totalLogisticsFee,
                RowNumber = rowNumber,
                ShippingFee = shippingFee,
                ExtraShippingFee = extraFee,
                ShippingNo = values[2]?.Trim().Trim('"'), // 綠界物流訂單編號 (Column 2)
                // Additional metadata
                OrderTime = values[0]?.Trim().Trim('"'),          // 訂單時間
                LogisticsProvider = values[7]?.Trim().Trim('"'),  // 物流廠商
                LogisticsStatus = values[12]?.Trim().Trim('"'),   // 物流狀態
                RecipientName = values[5]?.Trim().Trim('"'),      // 收件人姓名
                RecipientPhone = values[6]?.Trim().Trim('"'),     // 收件人手機
                DeductionDate = values.Length > 22 ? values[22]?.Trim().Trim('"') : null // 物流費扣款日期
            };
        }

        private LogisticsFeeRecord ParseTCatExcelRow(ExcelWorksheet worksheet, int row)
        {
            // T-Cat Excel Structure: Title(0) + Empty(1) + Headers(2) + Data(3+)
            // Column indices are 1-based in EPPlus
            var merchantTradeNo = worksheet.Cells[row, 6].Text?.Trim(); // 訂單編號 (Column 6, 1-indexed)
            var shippingFeeStr = worksheet.Cells[row, 8].Text?.Trim();  // 運費金額 (Column 8)
            var extraFeeStr = worksheet.Cells[row, 9].Text?.Trim();     // 附加服務金 (Column 9)

            // Skip footer rows
            if (!string.IsNullOrEmpty(merchantTradeNo) && merchantTradeNo.Contains("排序"))
            {
                return null;
            }

            if (string.IsNullOrEmpty(merchantTradeNo))
            {
                return null;
            }

            var shippingFee = decimal.TryParse(shippingFeeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var sf) ? sf : 0;
            var extraFee = decimal.TryParse(extraFeeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var ef) ? ef : 0;
            var totalLogisticsFee = shippingFee + extraFee;

            return new LogisticsFeeRecord
            {
                MerchantTradeNo = merchantTradeNo,
                FeeAmount = totalLogisticsFee,
                RowNumber = row,
                ShippingFee = shippingFee,
                ExtraShippingFee = extraFee,
                ShippingNo = worksheet.Cells[row, 7].Text?.Trim(), // 託運單號 (Column 7)
                // Additional metadata
                CustomerCode = worksheet.Cells[row, 1].Text?.Trim(),     // 客戶代號
                CollectionDate = worksheet.Cells[row, 2].Text?.Trim(),   // 集貨日期
                CollectionOffice = worksheet.Cells[row, 3].Text?.Trim(), // 集貨所
                DeliveryDate = worksheet.Cells[row, 4].Text?.Trim(),     // 配完日期
                DeliveryOffice = worksheet.Cells[row, 5].Text?.Trim()    // 配完所
            };
        }

        private LogisticsFeeRecord ParseECPayExcelRow(ExcelWorksheet worksheet, int row)
        {
            // ECPay Excel Structure: Title(0) + Headers(1) + Data(2+)
            // Column indices are 1-based in EPPlus
            var merchantTradeNo = worksheet.Cells[row, 2].Text?.Trim(); // 廠商訂單編號 (Column 2, 1-indexed)
            var shippingFeeStr = worksheet.Cells[row, 20].Text?.Trim(); // 實際物流運費 (Column 20)
            var extraFeeStr = worksheet.Cells[row, 21].Text?.Trim();    // 加價費用 (Column 21)

            if (string.IsNullOrEmpty(merchantTradeNo))
            {
                return null;
            }

            var shippingFee = decimal.TryParse(shippingFeeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var sf) ? sf : 0;
            var extraFee = decimal.TryParse(extraFeeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var ef) ? ef : 0;
            var totalLogisticsFee = shippingFee + extraFee;

            return new LogisticsFeeRecord
            {
                MerchantTradeNo = merchantTradeNo,
                FeeAmount = totalLogisticsFee,
                RowNumber = row,
                ShippingFee = shippingFee,
                ExtraShippingFee = extraFee,
                ShippingNo = worksheet.Cells[row, 3].Text?.Trim(), // 綠界物流訂單編號 (Column 3)
                // Additional metadata
                OrderTime = worksheet.Cells[row, 1].Text?.Trim(),          // 訂單時間
                LogisticsProvider = worksheet.Cells[row, 8].Text?.Trim(),  // 物流廠商
                LogisticsStatus = worksheet.Cells[row, 13].Text?.Trim(),   // 物流狀態
                RecipientName = worksheet.Cells[row, 6].Text?.Trim(),      // 收件人姓名
                RecipientPhone = worksheet.Cells[row, 7].Text?.Trim(),     // 收件人手機
                DeductionDate = worksheet.Cells[row, 23].Text?.Trim()      // 物流費扣款日期
            };
        }
    }
}

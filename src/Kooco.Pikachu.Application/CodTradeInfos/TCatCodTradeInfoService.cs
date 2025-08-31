using CsvHelper;
using CsvHelper.Configuration;
using Kooco.Pikachu.TenantPayouts;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.CodTradeInfos;

public class TCatCodTradeInfoService : ITransientDependency
{
    private readonly TenantPayoutRecordService _tenantPayoutRecordService;
    private readonly IDataFilter<IMultiTenant> _multiTenantFilter;

    public TCatCodTradeInfoService(
        TenantPayoutRecordService tenantPayoutRecordService,
        IDataFilter<IMultiTenant> multiTenantFilter
        )
    {
        _tenantPayoutRecordService = tenantPayoutRecordService;
        _multiTenantFilter = multiTenantFilter;
    }

    public Task InsertPayoutRecordsAsync(List<TenantPayoutRecord> records)
    {
        return _tenantPayoutRecordService.InsertPayoutRecordsAsync(records);
    }

    public async Task<List<TCatCodTradeInfoRecordDto>> ProcessFile(string fileName, byte[] fileBytes)
    {
        using (_multiTenantFilter.Disable())
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            var records = extension switch
            {
                ".csv" => ReadCsvFile(fileBytes),
                ".xlsx" => ReadXlsxFile(fileBytes),
                _ => throw new ArgumentException($"Unsupported file format: {extension}")
            };

            records = [.. records.Where(r => r.CollectionDate.HasValue)];

            if (records.Count > 0)
            {
                return await _tenantPayoutRecordService.CreateTCatCodPayouts(records);
            }

            return records;
        }
    }

    private static List<TCatCodTradeInfoRecordDto> ReadCsvFile(byte[] fileBytes)
    {
        using var stream = new MemoryStream(fileBytes);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        });

        var records = new List<TCatCodTradeInfoRecordDto>();
        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            if (IsEmptyRow(csv))
                continue;

            records.Add(new TCatCodTradeInfoRecordDto
            {
                Id = Guid.NewGuid(),
                CreationTime = DateTime.Now,
                CustomerID = GetStringValue(csv, 0)!,
                CollectionDate = GetDateValue(csv, 1),
                CollectionSite = GetStringValue(csv, 2),
                DeliveryCompletionDate = GetDateValue(csv, 3),
                DeliveryCompletionSite = GetStringValue(csv, 4),
                MerchantTradeNo = GetStringValue(csv, 5)!,
                ShippingNo = GetStringValue(csv, 6),
                ShippingFee = GetDecimalValue(csv, 7),
                ExtraShippingFee = GetDecimalValue(csv, 8),
                ExtraServiceItems = GetStringValue(csv, 9),
                CashCollected = GetStringValue(csv, 10),
                ReturnedGoods = GetStringValue(csv, 11),
                SameDayDelivery = GetStringValue(csv, 12),
                ShipmentType = GetStringValue(csv, 13),
                CODAmount = GetDecimalValue(csv, 14),
                PaymentMethod = GetStringValue(csv, 15)
            });
        }

        return records;
    }

    private static List<TCatCodTradeInfoRecordDto> ReadXlsxFile(byte[] fileBytes)
    {
        using var stream = new MemoryStream(fileBytes);
        var rows = MiniExcel.Query(stream).Skip(1); // Skip header row

        var records = new List<TCatCodTradeInfoRecordDto>();

        foreach (var row in rows)
        {
            var dict = (IDictionary<string, object>)row;
            if (IsEmptyRow(dict))
                continue;

            records.Add(new TCatCodTradeInfoRecordDto
            {
                Id = Guid.NewGuid(),
                CreationTime = DateTime.Now,
                CustomerID = GetStringValue(dict, "A")!,
                CollectionDate = GetDateValue(dict, "B"),
                CollectionSite = GetStringValue(dict, "C"),
                DeliveryCompletionDate = GetDateValue(dict, "D"),
                DeliveryCompletionSite = GetStringValue(dict, "E"),
                MerchantTradeNo = GetStringValue(dict, "F")!,
                ShippingNo = GetStringValue(dict, "G"),
                ShippingFee = GetDecimalValue(dict, "H"),
                ExtraShippingFee = GetDecimalValue(dict, "I"),
                ExtraServiceItems = GetStringValue(dict, "J"),
                CashCollected = GetStringValue(dict, "K"),
                ReturnedGoods = GetStringValue(dict, "L"),
                SameDayDelivery = GetStringValue(dict, "M"),
                ShipmentType = GetStringValue(dict, "N"),
                CODAmount = GetDecimalValue(dict, "O"),
                PaymentMethod = GetStringValue(dict, "P")
            });
        }

        return records;
    }

    // Helper methods for CSV
    private static bool IsEmptyRow(IReaderRow csv)
    {
        for (int i = 0; i < 16; i++)
        {
            if (!string.IsNullOrWhiteSpace(GetStringValue(csv, i)))
                return false;
        }
        return true;
    }

    private static string? GetStringValue(IReaderRow csv, int index)
    {
        try
        {
            var value = csv.GetField(index);
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
        catch
        {
            return null;
        }
    }

    private static DateTime? GetDateValue(IReaderRow csv, int index)
    {
        var value = GetStringValue(csv, index);
        if (string.IsNullOrEmpty(value)) return null;

        return DateTime.TryParseExact(value, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date : null;
    }

    private static decimal? GetDecimalValue(IReaderRow csv, int index)
    {
        var value = GetStringValue(csv, index);
        if (string.IsNullOrEmpty(value)) return null;

        return decimal.TryParse(value, out var result) ? result : null;
    }

    // Helper methods for XLSX (MiniExcel)
    private static bool IsEmptyRow(IDictionary<string, object> dict)
    {
        var columns = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P" };
        return columns.All(col => string.IsNullOrWhiteSpace(GetStringValue(dict, col)));
    }

    private static string? GetStringValue(IDictionary<string, object> dict, string column)
    {
        try
        {
            if (!dict.TryGetValue(column, out var value) || value == null)
                return null;

            var stringValue = value.ToString();
            return string.IsNullOrWhiteSpace(stringValue) ? null : stringValue.Trim();
        }
        catch
        {
            return null;
        }
    }

    private static DateTime? GetDateValue(IDictionary<string, object> dict, string column)
    {
        try
        {
            if (!dict.TryGetValue(column, out var value) || value == null)
                return null;

            if (value is DateTime dateTime)
                return dateTime;

            var stringValue = value.ToString();
            if (string.IsNullOrEmpty(stringValue))
                return null;

            return DateTime.TryParseExact(stringValue, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                ? date : null;
        }
        catch
        {
            return null;
        }
    }

    private static decimal? GetDecimalValue(IDictionary<string, object> dict, string column)
    {
        try
        {
            if (!dict.TryGetValue(column, out var value) || value == null)
                return null;

            if (value is decimal dec)
                return dec;
            if (value is double dbl)
                return (decimal)dbl;
            if (value is int i)
                return i;

            var stringValue = value.ToString();
            if (string.IsNullOrEmpty(stringValue))
                return null;

            return decimal.TryParse(stringValue, out var result) ? result : null;
        }
        catch
        {
            return null;
        }
    }
}

using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Volo.Abp.DependencyInjection;

namespace Kooco.Reconciliations;

public class EcPayReconciliationService : IEcPayReconciliationService, ITransientDependency
{
    private readonly EcPayHttpOptions _options;
    private readonly ILogger<EcPayReconciliationService> _logger;

    public EcPayReconciliationService(
        IOptions<EcPayHttpOptions> options,
        ILogger<EcPayReconciliationService> logger
        )
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<List<EcPayReconciliationResponse>> QueryMediaFileAsync(
        CancellationToken cancellationToken = default
        )
    {
        var today = DateTime.Today;
        var beginDate = today.AddDays(-15).ToString("yyyy-MM-dd");
        var endDate = today.ToString("yyyy-MM-dd");

        _logger.LogInformation("Reconciliation Job: Querying media file from {begin} to {end}", beginDate, endDate);

        var formData = new Dictionary<string, string>
        {
            ["MerchantID"] = _options.MerchantID,
            ["DateType"] = "4",
            ["BeginDate"] = beginDate,
            ["EndDate"] = endDate,
            ["MediaFormated"] = "2",
            ["CharSet"] = "2"
        };

        formData["CheckMacValue"] = EcPayCheckMacValue.Generate(formData, _options);

        var client = new RestClient();
        var request = new RestRequest(_options.QueryMediaFileUrl, Method.Post);
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        foreach (var pair in formData)
        {
            request.AddParameter(pair.Key, pair.Value);
        }

        var response = await client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful || response.Content == null || response.Content.Contains("Parameter Error"))
        {
            _logger.LogError("Reconciliation Job: Failed to fetch EcPay media file. Response: {response.Content}", response.Content);
            return [];
        }

        string csvContent = response.Content;
        return ParseEcPayReconciliationAsync(csvContent);
    }

    private List<EcPayReconciliationResponse> ParseEcPayReconciliationAsync(string csvContent)
    {
        try
        {
            if (csvContent.Contains("請確認下載的IP是否與廠商後台設定相同") ||
                csvContent.Contains("Parameter Error"))
            {
                _logger.LogWarning("Reconciliation Job: EcPay returned error message: {csvContent}", csvContent);
                return [];
            }

            csvContent = csvContent.TrimStart('\uFEFF');
            csvContent = Regex.Replace(csvContent, @"=""([^""]*)""", "\"$1\""); // Remove ="value" wrappers
            csvContent = Regex.Replace(csvContent, @"=(\d+(?:\.\d+)?)", "$1"); // Remove = from =6 or =6.00
            csvContent = csvContent.Replace("=,", ","); // Remove = before empty values
            csvContent = csvContent.Replace("=\"", "\"").Replace("=\r\n", "\r\n"); // Clean extra quotes

            // Step 1: Skip metadata lines and find the header
            using var rawReader = new StringReader(csvContent);
            string? line;
            var cleanedLines = new List<string>();

            while ((line = rawReader.ReadLine()) != null)
            {
                if (line.StartsWith("\"訂單日期\""))
                {
                    cleanedLines.Add(line);
                    break;
                }
            }

            // Read the remaining lines, but filter out error rows
            while ((line = rawReader.ReadLine()) != null)
            {
                // Skip rows that are clearly error messages or placeholder data
                if (!string.IsNullOrWhiteSpace(line) &&
                    !line.Contains("請確認下載的IP是否與廠商後台設定相同") &&
                    !line.StartsWith("\"-\",\"-\",\"-\""))
                {
                    cleanedLines.Add(line);
                }
            }

            if (cleanedLines.Count <= 1)
            {
                _logger.LogInformation("Reconciliation Job: No transaction data found in EcPay response");
                return [];
            }

            // Step 2: Clean Excel-style ="value" wrappers
            var rawCleaned = string.Join(Environment.NewLine, cleanedLines);
            string cleanedCsv = Regex.Replace(rawCleaned, @"=""([^""]*)""", "\"$1\"");

            // Step 3: Parse with CsvHelper
            using var finalReader = new StringReader(cleanedCsv);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                Encoding = Encoding.UTF8,
                MissingFieldFound = null, // Ignore missing fields
                BadDataFound = null, // Ignore bad data
                HeaderValidated = null, // Don't validate headers
                ReadingExceptionOccurred = args =>
                {
                    _logger.LogWarning("Reconciliation Job: CSV parsing exception: {args.Exception.Message}", args.Exception.Message);
                    return false; // Skip bad records
                }
            };

            using var finalCsv = new CsvReader(finalReader, config);
            finalCsv.Context.RegisterClassMap<EcPayReconciliationMap>();

            var records = new List<EcPayReconciliationResponse>();

            while (finalCsv.Read())
            {
                try
                {
                    var record = finalCsv.GetRecord<EcPayReconciliationResponse>();
                    if (record != null)
                    {
                        records.Add(record);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Reconciliation Job: Failed to parse CSV record: {ex.Message}", ex.Message);
                }
            }

            return records;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reconciliation Job: Error parsing EcPay reconciliation CSV");
            _logger.LogException(ex);
            return [];
        }
    }
}
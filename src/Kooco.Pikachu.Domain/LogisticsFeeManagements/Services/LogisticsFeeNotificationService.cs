using Kooco.Pikachu.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.LogisticsFeeManagements.Services
{
    public class LogisticsFeeNotificationService : ILogisticsFeeNotificationService, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogisticsFeeFileImportRepository _fileImportRepository;
        private readonly ITenantLogisticsFeeFileProcessingSummaryRepository _summaryRepository;
        private readonly IRepository<Tenant, Guid> _tenantRepository;
        private readonly ILogger<LogisticsFeeNotificationService> _logger;
        private readonly IStringLocalizer<PikachuResource> _localizer;

        public LogisticsFeeNotificationService(
            IEmailSender emailSender,
            ILogisticsFeeFileImportRepository fileImportRepository,
            ITenantLogisticsFeeFileProcessingSummaryRepository summaryRepository,
            IRepository<Tenant, Guid> tenantRepository,
            ILogger<LogisticsFeeNotificationService> logger,
            IStringLocalizer<PikachuResource> localizer)
        {
            _emailSender = emailSender;
            _fileImportRepository = fileImportRepository;
            _summaryRepository = summaryRepository;
            _tenantRepository = tenantRepository;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task SendBatchProcessingNotificationAsync(Guid fileImportId)
        {
            try
            {
                var fileImport = await _fileImportRepository.GetWithDetailsAsync(fileImportId);
                if (fileImport == null)
                {
                    _logger.LogWarning("File import not found: {FileImportId}", fileImportId);
                    return;
                }

                var summaries = await _summaryRepository.GetByFileImportIdAsync(fileImportId);

                foreach (var summary in summaries)
                {
                    await SendTenantNotificationAsync(fileImport, summary);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending batch processing notifications for file: {FileImportId}", fileImportId);
            }
        }

        public async Task SendRetryNotificationAsync(Guid tenantId, BatchRetryResult result)
        {
            try
            {
                var tenant = await _tenantRepository.GetAsync(tenantId);
                if (tenant.GetProperty<string>("TenantContactEmail") == null)
                {
                    _logger.LogWarning("No contact email found for tenant: {TenantId}", tenantId);
                    return;
                }

                var subject = "Logistics Fee Retry Processing Complete";
                var body = BuildRetryNotificationBody(tenant.Name, result, result.FileName, result.FileType, DateTime.Now);
                // Convert to HTML
                var htmlBody = ConvertPlainTextToHtml(body);
                await _emailSender.SendAsync(tenant.GetProperty<string>("TenantContactEmail"), subject, htmlBody);

                _logger.LogInformation("Retry notification sent to tenant: {TenantId}", tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending retry notification to tenant: {TenantId}", tenantId);
            }
        }

        private async Task SendTenantNotificationAsync(
            LogisticsFeeFileImport fileImport,
            TenantLogisticsFeeFileProcessingSummary summary)
        {
            try
            {
                var tenant = await _tenantRepository.GetAsync(summary.TenantId.Value);
                if (tenant.GetProperty<string>("TenantContactEmail") == null)
                {
                    _logger.LogWarning("No contact email found for tenant: {TenantId}", summary.TenantId);
                    return;
                }

                var subject = "Logistics Fee Processing Complete";
                var body = BuildNotificationBody(tenant.Name, fileImport, summary);

                await _emailSender.SendAsync(tenant.GetProperty<string>("TenantContactEmail"), subject, body);

                _logger.LogInformation("Notification sent to tenant: {TenantId}", summary.TenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to tenant: {TenantId}", summary.TenantId);
            }
        }

        private string BuildNotificationBody(
            string tenantName,
            LogisticsFeeFileImport fileImport,
            TenantLogisticsFeeFileProcessingSummary summary)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Dear {tenantName},");
            sb.AppendLine();
            sb.AppendLine("Your logistics fee processing has been completed with the following results:");
            sb.AppendLine();
            sb.AppendLine($"File: {fileImport.OriginalFileName}");
            sb.AppendLine($"File Type: {fileImport.FileType}");
            sb.AppendLine($"Processing Date: {summary.ProcessedAt:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();
            sb.AppendLine("Summary:");
            sb.AppendLine($"- Total Records: {summary.TenantTotalRecords}");
            sb.AppendLine($"- Successful Deductions: {summary.TenantSuccessfulRecords}");
            sb.AppendLine($"- Failed Deductions: {summary.TenantFailedRecords}");
            sb.AppendLine($"- Total Amount Processed: ${summary.TenantTotalAmount:F2}");
            sb.AppendLine();

            if (summary.TenantFailedRecords > 0)
            {
                sb.AppendLine("Please review the failed records in your dashboard and retry if necessary.");
                sb.AppendLine();
            }

            sb.AppendLine("Thank you for using our logistics fee management system.");
            sb.AppendLine();
            sb.AppendLine("Best regards,");
            sb.AppendLine("System Administration");

            return sb.ToString();
        }

        private string BuildRetryNotificationBody(
        string tenantName,
        BatchRetryResult result,
        string fileName,
        string fileType,
        DateTime processingDate)
        {
            var sb = new StringBuilder();

            // Email greeting
            sb.AppendLine(_localizer["Email:Greeting", tenantName]);
            sb.AppendLine();

            // Email intro
            sb.AppendLine(_localizer["Email:RetryProcessingCompleted"]);
            sb.AppendLine();

            // File information
            sb.AppendLine(_localizer["Email:File", fileName]);
            sb.AppendLine(_localizer["Email:FileType", fileType]);
            sb.AppendLine(_localizer["Email:ProcessingDate", processingDate.ToString("yyyy-MM-dd HH:mm:ss")]);
            sb.AppendLine();

            // Results section
            sb.AppendLine(_localizer["Email:Results"]);
            sb.AppendLine(_localizer["Email:SuccessfulRetries", result.SuccessCount]);
            sb.AppendLine(_localizer["Email:FailedRetries", result.FailureCount]);
            sb.AppendLine(_localizer["Email:TotalAmountProcessed", result.SuccessfulAmount.ToString("F2") ?? "0.00"]);
            sb.AppendLine();

            // Failure reasons (if any)
            if (result.FailureReasons.Any())
            {
                sb.AppendLine(_localizer["Email:FailureReasons"]);
                foreach (var reason in result.FailureReasons.Distinct())
                {
                    sb.AppendLine($"- {reason}");
                }
                sb.AppendLine();
            }

            // Closing
            sb.AppendLine(_localizer["Email:ThankYou"]);
            sb.AppendLine();
            sb.AppendLine(_localizer["Email:BestRegards"]);
            sb.AppendLine(_localizer["Email:TeamSignature"]);

            return sb.ToString();
        }
        private string ConvertPlainTextToHtml(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            // HTML encode the text to prevent XSS and handle special characters
            var htmlEncoded = System.Web.HttpUtility.HtmlEncode(plainText);

            // Replace line breaks with HTML line breaks
            var htmlBody = htmlEncoded
                .Replace("\r\n", "<br>")
                .Replace("\n", "<br>")
                .Replace("\r", "<br>");

            // Wrap in HTML structure with proper styling
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            line-height: 1.6; 
            margin: 20px;
            white-space: pre-line;
        }}
    </style>
</head>
<body>
    {htmlBody}
</body>
</html>";
        }
        public string GetRetryNotificationSubject()
        {
            return _localizer["Email:Subject:LogisticsFeeRetryProcessing"];
        }


    }
}

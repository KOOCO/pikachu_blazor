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

        public LogisticsFeeNotificationService(
            IEmailSender emailSender,
            ILogisticsFeeFileImportRepository fileImportRepository,
            ITenantLogisticsFeeFileProcessingSummaryRepository summaryRepository,
            IRepository<Tenant, Guid> tenantRepository,
            ILogger<LogisticsFeeNotificationService> logger)
        {
            _emailSender = emailSender;
            _fileImportRepository = fileImportRepository;
            _summaryRepository = summaryRepository;
            _tenantRepository = tenantRepository;
            _logger = logger;
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
                var body = BuildRetryNotificationBody(tenant.Name, result);

                await _emailSender.SendAsync(tenant.GetProperty<string>("TenantContactEmail"), subject, body);

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

        private string BuildRetryNotificationBody(string tenantName, BatchRetryResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Dear {tenantName},");
            sb.AppendLine();
            sb.AppendLine("Your logistics fee retry processing has been completed:");
            sb.AppendLine();
            sb.AppendLine("Results:");
            sb.AppendLine($"- Successful Retries: {result.SuccessCount}");
            sb.AppendLine($"- Failed Retries: {result.FailureCount}");
            sb.AppendLine();

            if (result.FailureReasons.Any())
            {
                sb.AppendLine("Failure Reasons:");
                foreach (var reason in result.FailureReasons.Distinct())
                {
                    sb.AppendLine($"- {reason}");
                }
                sb.AppendLine();
            }

            sb.AppendLine("Thank you for using our logistics fee management system.");
            sb.AppendLine();
            sb.AppendLine("Best regards,");
            sb.AppendLine("System Administration");

            return sb.ToString();
        }
    }
}

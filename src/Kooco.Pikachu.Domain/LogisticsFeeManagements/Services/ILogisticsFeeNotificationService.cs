using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.LogisticsFeeManagements.Services
{
    public interface ILogisticsFeeNotificationService
    {
        Task SendBatchProcessingNotificationAsync(Guid fileImportId);
        Task SendRetryNotificationAsync(Guid tenantId, BatchRetryResult result);
    }

    public class BatchRetryResult
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> FailureReasons { get; set; } = new();
    }
}
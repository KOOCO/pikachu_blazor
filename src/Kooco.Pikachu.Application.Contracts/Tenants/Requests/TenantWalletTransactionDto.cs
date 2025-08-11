using Kooco.Pikachu.TenantManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Tenants.Requests
{
    public class TenantWalletTransactionDto
    {
        public Guid Id { get; set; } // for selection
        public DateTime Timestamp { get; set; }
        public string? TransactionNo { get; set; }
        public WalletTransactionType TransactionType { get; set; }
        public WalletDeductionStatus TransactionStatus { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string? Note { get; set; }
    }
}

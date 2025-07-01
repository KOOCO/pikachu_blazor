using Kooco.Pikachu.EnumValues;

namespace Kooco.Pikachu.PaymentGateways;

public class ManualBankTransferDto
{
    public PaymentIntegrationType PaymentIntegrationType { get; set; }
    public bool IsEnabled { get; set; }
    public string? AccountName { get; set; }
    public string? BankName { get; set; }
    public string? BranchName { get; set; }
    public string? BankCode { get; set; }
    public string? BankAccountNumber { get; set; }
    public int? MinimumAmountLimit { get; set; }
    public int? MaximumAmountLimit { get; set; }
}

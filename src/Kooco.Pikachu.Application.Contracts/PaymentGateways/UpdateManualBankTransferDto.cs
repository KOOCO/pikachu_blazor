using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.PaymentGateways;

public class UpdateManualBankTransferDto
{
    public bool IsEnabled { get; set; }

    [Required]
    public string? AccountName { get; set; }

    [Required]
    public string? BankName { get; set; }

    [Required]
    public string? BranchName { get; set; }

    [Required]
    public string? BankCode { get; set; }

    [Required]
    public string? BankAccountNumber { get; set; }

    [Required]
    public int? MinimumAmountLimit { get; set; }

    [Required]
    public int? MaximumAmountLimit { get; set; }
}

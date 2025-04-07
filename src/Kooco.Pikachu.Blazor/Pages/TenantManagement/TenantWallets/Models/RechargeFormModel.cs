using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantWallets.Models;
public class RechargeFormModel
{
    [Required(ErrorMessage = "請輸入金額")]
    [Range(typeof(decimal), "1", "9999999999999999", ErrorMessage = "金額必須大於 0")]
    public decimal Amount { get; set; }

    [StringLength(20, ErrorMessage = "備註長度不能超過 20 個字元。")]
    public string? Remark { get; set; }
    public bool EnableEmailNotifications { get; set; }
    public bool EnableTextMessageNotifications { get; set; }
    public bool EnableBackstageNotifications { get; set; }
}
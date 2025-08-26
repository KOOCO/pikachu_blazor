using System.Collections.Generic;

namespace Kooco.Pikachu.Blazor.Pages.TenantDeliveryFees;

//TODO: These classes needs to be removed when EcPay Fee Configuration is implemented properly
public class EcPayFeeConfiguration
{
    public string Title { get; set; }
    public List<EcPayFeeConfigurationItems> Items { get; set; } = [];
}
public class EcPayFeeConfigurationItems
{
    public string Title { get; set; }
    public bool IsBase { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsPercentageFee { get; set; }
    public double Amount { get; set; }
}


public class TempModels
{
    public static readonly List<EcPayFeeConfiguration> TCatConfigurations =
        [
            new(){
                Title= "TCatPaymentOptions",
                Items =
                [
                    new(){
                        Title="CashOnDelivery",
                        IsEnabled = true,
                        IsPercentageFee = true,
                        Amount = 2
                    }
                ]
            }
        ];
    public static readonly List<EcPayFeeConfiguration> Configurations =
        [
                new(){
                Title  ="CreditCardPaymentOptions",
                Items =
                [
                    new(){
                        Title="ProcessingFee",
                        IsBase = true,
                        IsEnabled = true,
                        IsPercentageFee = true,
                        Amount = 1.5
                    },
                    new(){
                        Title="OneTimePayment",
                        IsEnabled = true,
                        IsPercentageFee = true,
                        Amount = 2.5
                    },
                    new(){
                        Title="3MonthInstallment",
                        IsEnabled = true,
                        IsPercentageFee = true,
                        Amount = 3
                    },
                    new(){
                        Title="6MonthInstallment",
                        IsEnabled = false,
                        IsPercentageFee = true,
                        Amount = 3.5
                    },
                    new(){
                        Title="12MonthInstallment",
                        IsEnabled = false,
                        IsPercentageFee = true,
                        Amount = 4
                    },
                    new(){
                        Title="18MonthInstallment",
                        IsEnabled = false,
                        IsPercentageFee = true,
                        Amount = 5
                    },
                    new(){
                        Title="24MonthInstallment",
                        IsEnabled = false,
                        IsPercentageFee = true,
                        Amount = 5
                    },
                    new(){
                        Title="DreamPlan30MonthInstallment",
                        IsEnabled = false,
                        IsPercentageFee = true,
                        Amount = 6
                    },
                    new(){
                        Title="UnionPayCard",
                        IsEnabled = true,
                        IsPercentageFee = true,
                        Amount = 2.8
                    }
                ]
            },
            new() {
                Title = "OtherPaymentMethods",
                Items =
                [
                    new() {
                        Title = "VirtualAccount",
                        IsEnabled = true,
                        IsPercentageFee = true,
                        Amount = 1.5
                    },
                    new() {
                        Title = "OnlineAtm",
                        IsEnabled = true,
                        IsPercentageFee = true,
                        Amount = 1
                    },
                    new() {
                        Title = "CvsCode",
                        IsEnabled = false,
                        IsPercentageFee = false,
                        Amount = 15
                    },
                    new() {
                        Title = "CvsBarcode",
                        IsEnabled = false,
                        IsPercentageFee = false,
                        Amount = 15
                    },
                    new() {
                        Title = "CashOnDelivery",
                        IsEnabled = true,
                        IsPercentageFee = true,
                        Amount = 2
                    }
                ]
            }
        ];
}
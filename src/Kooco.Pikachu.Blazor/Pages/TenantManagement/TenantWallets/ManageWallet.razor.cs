using Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantWallets.Models;
using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.TenantManagement;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantWallets;
public partial class ManageWallet
{
    decimal balance;
    decimal rechargeAmountOfStored;
    decimal deductionAmountOfStored;
    const int Denomination = 100;

    private async Task HandleRechargeSubmit()
    {
        try
        {
            await Loading.Show();
            await TenantWalletAppService.AddRechargeTransactionAsync(Id, RechargeFormModel.Amount, new()
            {
                TenantWalletId = Id,
                DeductionStatus = WalletDeductionStatus.Completed,
                TradingMethods = WalletTradingMethods.SystemInput,
                TransactionType = WalletTransactionType.Deposit,
                TransactionAmount = RechargeFormModel.Amount,
                TransactionNotes = RechargeFormModel.Remark,
            });

            RechargeFormModel = new();
            await RefreshDataAsync();
            StateHasChanged();
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
        finally
        {
            await Loading.Hide();
        }
    }
    private async Task HandleDeductionSubmit()
    {
        try
        {
            await Loading.Show();
            var wallet = await TenantWalletRepository.GetAsync(Id);

            if (DeductionFormModel.Amount > wallet.WalletBalance)
            {
                SubmitMessage = PL["TenantWallet:LowBalanceAlert"];
                return;
            }

            await TenantWalletAppService.AddDeductionTransactionAsync(Id, DeductionFormModel.Amount, new()
            {
                TenantWalletId = Id,
                DeductionStatus = WalletDeductionStatus.Completed,
                TradingMethods = WalletTradingMethods.SystemInput,
                TransactionType = WalletTransactionType.Deduction,
                TransactionAmount = DeductionFormModel.Amount,
                TransactionNotes = DeductionFormModel.Remark,
            });

            DeductionFormModel = new();
            await RefreshDataAsync();
            StateHasChanged();
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task RefreshDataAsync()
    {
        balance = await TenantWalletRepository.GetBalanceAsync(Id);
        rechargeAmountOfStored = await TenantWalletRepository.SumDepositTransactionAmountAsync(Id);
        deductionAmountOfStored = await TenantWalletRepository.SumDeductionTransactionAmountAsync(Id);
    }
    protected override async Task OnInitializedAsync()
    {
        try
        {
            await RefreshDataAsync();
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }
    public string BalanceCurrency
    {
        get
        {
            return balance.FormatAmountWithThousandsSigns();
        }
    }
    public string RechargeAmountOfStoredCurrency
    {
        get
        {
            return rechargeAmountOfStored.FormatAmountWithThousandsSigns();
        }
    }
    public string DeductionAmountOfStoredCurrency
    {
        get
        {
            return deductionAmountOfStored.FormatAmountWithThousandsSigns();
        }
    }

    public string? SubmitMessage { get; set; }
    RechargeFormModel RechargeFormModel { get; set; } = new();
    RechargeFormModel DeductionFormModel { get; set; } = new();
}
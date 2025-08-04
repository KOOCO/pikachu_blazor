using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.Tenants.Requests;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantWallets
{
    public partial class TenantWalletTransaction
    {

        decimal balance;
        decimal rechargeAmountOfStored;
        decimal deductionAmountOfStored;

        private List<TenantWalletTransactionDto> Transactions;
        private List<TenantWalletTransactionDto> SelectedTransactions = new();
        protected override async Task OnParametersSetAsync()
        {

            Transactions = await WalletService.GetWalletTransactionsByTenantIdAsync(CurrentTenant.Id.Value);
            await RefreshDataAsync();
            StateHasChanged();
        }


        private async Task ExportSelected()
        {
            var selectedIds = SelectedTransactions.Select(x => x.Id).ToList();
            var stream = await WalletService.ExportWalletTransactionsByTenantIdAsync(CurrentTenant.Id.Value, selectedIds);
            await ExcelDownloadHelper.DownloadExcelAsync(stream);
            SelectedTransactions = [];
        }

        private async Task ExportAll()
        {
            var stream = await WalletService.ExportWalletTransactionsByTenantIdAsync(CurrentTenant.Id.Value);
            await ExcelDownloadHelper.DownloadExcelAsync(stream);
        }
        async Task RefreshDataAsync()
        {
            balance = await TenantWalletRepository.GetBalanceByTenantIdAsync(CurrentTenant.Id.Value);
            rechargeAmountOfStored = await TenantWalletRepository.SumDepositTransactionAmountByTenantIdAsync(CurrentTenant.Id.Value);
            deductionAmountOfStored = await TenantWalletRepository.SumDeductionTransactionAmountByTenantIdAsync(CurrentTenant.Id.Value);
        }

    }
}

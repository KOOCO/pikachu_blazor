using Blazorise.DataGrid;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.Tenants.Requests;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantWallets
{
    public partial class TenantWalletTransaction
    {

        decimal balance;
        decimal rechargeAmountOfStored;
        decimal deductionAmountOfStored;
        private int CurrentPage = 1;
        private int PageSize = 10;
        private int TotalCount = 0;

        private PagedResultDto<TenantWalletTransactionDto> Transactions;
        private List<TenantWalletTransactionDto> SelectedTransactions = new();
        protected override async Task OnParametersSetAsync()
        {

            await LoadData();
            await RefreshDataAsync();
            StateHasChanged();
        }

        private async Task OnReadData(DataGridReadDataEventArgs<TenantWalletTransactionDto> e)
        {
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                var skipCount = (CurrentPage - 1) * PageSize;
                Transactions = await WalletService.GetWalletTransactionsByTenantIdAsync(CurrentTenant.Id.Value, skipCount, PageSize);
                TotalCount = (int)Transactions.TotalCount;

                // Clear selection when data changes
                SelectedTransactions.Clear();

                StateHasChanged();
            }
            catch (Exception ex)
            {
                // Handle error appropriately
                Console.WriteLine($"Error loading wallet transactions: {ex.Message}");
            }
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
        private string SplitNotesWithRegex(string note)

        {
            var pattern = @"^(Logistics fee deduction for order)\s+(.+)$";
            if (string.IsNullOrEmpty(note))
                return null;

           
            var regex = new Regex(pattern);
            var match = regex.Match(note);

            if (match.Success)
            {
                return L["NoteDescription", match.Groups[2].Value];
            }

            // If it doesn't match the pattern, return the original note as description
            return note;
        }
            async Task RefreshDataAsync()
        {
            balance = await TenantWalletRepository.GetBalanceByTenantIdAsync(CurrentTenant.Id.Value);
            rechargeAmountOfStored = await TenantWalletRepository.SumDepositTransactionAmountByTenantIdAsync(CurrentTenant.Id.Value);
            deductionAmountOfStored = await TenantWalletRepository.SumDeductionTransactionAmountByTenantIdAsync(CurrentTenant.Id.Value);
        }

    }
}

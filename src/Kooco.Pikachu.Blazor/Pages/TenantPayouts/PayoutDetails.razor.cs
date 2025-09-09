using AutoMapper;
using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Members.MemberTags;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts;
using PdfSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.TenantPayouts;

public partial class PayoutDetails
{
    [Parameter] public int? Year { get; set; }
    private IReadOnlyList<PayoutDetailDto> PayoutDetailList { get; set; } = PayoutDetailData.GetSampleData();
    private List<PayoutDetailDto> SelectedPayoutDetail { get; set; } = [];
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                PayoutDetailList = PayoutDetailData.GetSampleData();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    private async Task GetMembersAsync()
    {
        try
        {
            //var result = await MemberAppService.GetListAsync(
            //    new GetMemberListDto
            //    {
            //        MaxResultCount = PageSize,
            //        SkipCount = (CurrentPage - 1) * PageSize,
            //        Sorting = CurrentSorting,
            //        Filter = Filters.Filter,
            //        MemberType = Filters.MemberType,
            //        SelectedMemberTags = Filters.SelectedMemberTags,
            //        MinCreationTime = Filters.MinCreationTime,
            //        MaxCreationTime = Filters.MaxCreationTime,
            //        MinOrderCount = Filters.MinOrderCount,
            //        MaxOrderCount = Filters.MaxOrderCount,
            //        MinSpent = Filters.MinSpent,
            //        MaxSpent = Filters.MaxSpent
            //    }
            //);

            //MembersList = result.Items;
            //TotalCount = (int)result.TotalCount;
            PayoutDetailList = PayoutDetailData.GetSampleData();
            TotalCount = PayoutDetailList.Count;
        }
        catch (Exception ex)
        {

            await HandleErrorAsync(ex);
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PayoutDetailDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetMembersAsync();

        await InvokeAsync(StateHasChanged);
    }

    private static bool RowSelectableHandler(RowSelectableEventArgs<PayoutDetailDto> rowSelectableEventArgs)
        => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick;
}

public class PayoutDetailDto
{
    public DateTime OrderCreationDate { get; set; }
    public string OrderNumber { get; set; }
    public string PaymentType { get; set; }
    public decimal OrderAmount { get; set; }
    public decimal FeeRate { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal ProcessingFee { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; }
}

// Dummy data
public static class PayoutDetailData
{
    public static List<PayoutDetailDto> GetSampleData()
    {
        return
        [
            new PayoutDetailDto
            {
                OrderCreationDate = new DateTime(2024, 8, 21),
                OrderNumber = "ORD-2024-08-001",
                PaymentType = "Credit Card",
                OrderAmount = 1250.00m,
                FeeRate = 2.9m,
                HandlingFee = 36.25m,
                ProcessingFee = 5.00m,
                NetAmount = 1208.75m,
                Status = "unpaid",
            },
            new PayoutDetailDto
            {
                OrderCreationDate = new DateTime(2024, 8, 20),
                OrderNumber = "ORD-2024-08-002",
                PaymentType = "Bank Transfer",
                OrderAmount = 890.50m,
                FeeRate = 2.9m,
                HandlingFee = 25.82m,
                ProcessingFee = 5.00m,
                NetAmount = 859.68m,
                Status = "paid",
            },
            new PayoutDetailDto
            {
                OrderCreationDate = new DateTime(2024, 8, 19),
                OrderNumber = "ORD-2024-08-003",
                PaymentType = "Digital Wallet",
                OrderAmount = 2100.00m,
                FeeRate = 2.9m,
                HandlingFee = 60.90m,
                ProcessingFee = 5.00m,
                NetAmount = 2034.10m,
                Status = "unpaid",
            },
            new PayoutDetailDto
            {
                OrderCreationDate = new DateTime(2024, 8, 18),
                OrderNumber = "ORD-2024-08-004",
                PaymentType = "Debit Card",
                OrderAmount = 567.25m,
                FeeRate = 2.9m,
                HandlingFee = 16.45m,
                ProcessingFee = 5.00m,
                NetAmount = 545.80m,
                Status = "unpaid",
            },
            new PayoutDetailDto
            {
                OrderCreationDate = new DateTime(2024, 8, 17),
                OrderNumber = "ORD-2024-08-005",
                PaymentType = "PayPal",
                OrderAmount = 3200.00m,
                FeeRate = 2.9m,
                HandlingFee = 92.80m,
                ProcessingFee = 5.00m,
                NetAmount = 3102.20m,
                Status = "paid",
            },
            new PayoutDetailDto
            {
                OrderCreationDate = new DateTime(2024, 8, 16),
                OrderNumber = "ORD-2024-08-006",
                PaymentType = "Apple Pay",
                OrderAmount = 445.75m,
                FeeRate = 2.9m,
                HandlingFee = 12.93m,
                ProcessingFee = 5.00m,
                NetAmount = 427.82m,
                Status = "unpaid",
            },
            new PayoutDetailDto
            {
                OrderCreationDate = new DateTime(2024, 8, 15),
                OrderNumber = "ORD-2024-08-007",
                PaymentType = "Google Pay",
                OrderAmount = 1875.00m,
                FeeRate = 2.9m,
                HandlingFee = 54.38m,
                ProcessingFee = 5.00m,
                NetAmount = 1815.62m,
                Status = "unpaid",
            },
            new PayoutDetailDto
            {
                OrderCreationDate = new DateTime(2024, 8, 14),
                OrderNumber = "ORD-2024-08-008",
                PaymentType = "Wire Transfer",
                OrderAmount = 750.25m,
                FeeRate = 2.9m,
                HandlingFee = 21.76m,
                ProcessingFee = 5.00m,
                NetAmount = 723.49m,
                Status = "paid",
            }
        ];
    }
}
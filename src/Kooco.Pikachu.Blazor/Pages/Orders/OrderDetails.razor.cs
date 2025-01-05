using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.StoreLogisticOrders;
using Kooco.Pikachu.TestLables;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Polly;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Http.Modeling;
using Volo.Abp.ObjectMapping;
using static Kooco.Pikachu.Permissions.PikachuPermissions;


namespace Kooco.Pikachu.Blazor.Pages.Orders;

public partial class OrderDetails
{
    #region Inject
    [Parameter]
    public string id { get; set; }
    private Guid OrderId { get; set; }
    private CreateUpdateOrderMessageDto StoreCustomerService { get; set; } = new();
    private OrderDto Order { get; set; }
    private decimal? OrderDeliveryCost { get; set; } = 0.00m;
    private CreateOrderDto UpdateOrder { get; set; } = new();
    private StoreCommentsModel StoreComments = new();
    private ModificationTrack ModificationTrack = new();
    private Shipments shipments = new();
    private RefundOrder refunds = new();
    private List<UpdateOrderItemDto> EditingItems { get; set; } = new();
    private IReadOnlyList<OrderMessageDto> CustomerServiceHistory { get; set; }=[];
    private Modal CreateShipmentModal { get; set; }
    private Modal RefundModal { get; set; }
    private LoadingIndicator loading { get; set; } = new();
    private bool IsItemsEditMode { get; set; } = false;
    private List<OrderDeliveryDto> OrderDeliveries { get; set; }
    private readonly HashSet<Guid> ExpandedRows = new();
    private OrderDeliveryDto SelectedOrder { get; set; }
    private Guid OrderDeliveryId { get; set; }
    string? CheckoutForm { get; set; } = null;
    private readonly ITestLableAppService _testLableAppService;
    private readonly IObjectMapper _ObjectMapper;

    private bool isDeliveryCostDisplayed = false;
    private bool isNormal = false;
    private bool isFreeze = false;
    private bool isFrozen = false;

    private readonly IConfiguration _Configuration;

    private readonly IPaymentGatewayAppService _PaymentGatewayAppService;
    private readonly IOrderMessageAppService _OrderMessageAppService;

    private readonly IDeliveryTemperatureCostAppService _DeliveryTemperatureCostAppService;

    private string? PaymentStatus;

    private bool IsShowConvenienceStoreDetails = false;

    private ItemStorageTemperature? SelectedTemperatureControl = ItemStorageTemperature.Normal;
    #endregion

    #region Constructor
    public OrderDetails(
        ITestLableAppService testLableAppService,
        IObjectMapper ObjectMapper,
        IConfiguration Configuration,
        IPaymentGatewayAppService PaymentGatewayAppService,
        IOrderMessageAppService orderMessageAppService,
        IDeliveryTemperatureCostAppService DeliveryTemperatureCostAppService
    )
    {
        _testLableAppService = testLableAppService;
        _ObjectMapper = ObjectMapper;
        _Configuration = Configuration;
        _PaymentGatewayAppService = PaymentGatewayAppService;
        _OrderMessageAppService = orderMessageAppService;
        _DeliveryTemperatureCostAppService = DeliveryTemperatureCostAppService;
        Order = new();
    }
    #endregion

    #region Methods
    protected async override Task OnAfterRenderAsync(bool isFirstRender)
    {
        if (isFirstRender)
        {
            try
            {
                await loading.Show();
                OrderId = Guid.Parse(id);
                await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
                await loading.Hide();
            }
            catch (Exception ex)
            {
                await loading.Hide();
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
        }
    }

    public void ChangeStore(ChangeEventArgs e)
    {
        IsShowConvenienceStoreDetails = e.Value is not null && e.Value.ToString() is "convenienceStore" ? true : false;
    }

    public void TemperatureControlChange(ChangeEventArgs e)
    {
        SelectedTemperatureControl = Enum.TryParse(e.Value.ToString(), out ItemStorageTemperature temperature) ? temperature : null;
    }

    void ToggleRow(DataGridRowMouseEventArgs<OrderDeliveryDto> e)
    {
        if (ExpandedRows.Contains(e.Item.Id))
        {
            ExpandedRows.Remove(e.Item.Id);
        }
        else
        {
            ExpandedRows.Add(e.Item.Id);
        }
    }

    private decimal GetDeliveryCost(ItemStorageTemperature item)
    {
        if (Order.DeliveryMethod is DeliveryMethod.DeliveredByStore)
        {
            switch (item)
            {
                case ItemStorageTemperature.Normal:
                    return Order.DeliveryCostForNormal ?? 0.00m;
                case ItemStorageTemperature.Freeze:
                    return Order.DeliveryCostForFreeze ?? 0.00m;
                case ItemStorageTemperature.Frozen:
                    return Order.DeliveryCostForFrozen ?? 0.00m;
            }
        }

        return 0.00m;
    }

    private void SetTotalAmount(OrderDeliveryDto entity)
    {
        if (isNormal || isFreeze || isFrozen) 
            entity.TotalAmount = (OrderDeliveryCost ?? 0.00m);

        else entity.TotalAmount = 0.00m;
    }

    async Task GetOrderDetailsAsync()
    {
        Order = await _orderAppService.GetWithDetailsAsync(OrderId) ?? new();

        if (Order.DeliveryMethod is not DeliveryMethod.SelfPickup &&
            Order.DeliveryMethod is not DeliveryMethod.DeliveredByStore)
                OrderDeliveryCost = Order.DeliveryCost;

        OrderDeliveries = await _orderDeliveryAppService.GetListByOrderAsync(OrderId);

        OrderDeliveries = [.. OrderDeliveries.Where(w => w.Items.Count > 0)];

        PaymentStatus = await GetPaymentStatus();

        var result= await _OrderMessageAppService.GetListAsync(new GetOrderMessageListDto { MaxResultCount = 1000, OrderId = Order.Id });
        
        CustomerServiceHistory = result.Items;
        
        await InvokeAsync(StateHasChanged);
    }

    async Task SubmitStoreCommentsAsync()
    {
        try
        {
            await loading.Show();
            string comment = StoreComments.Comment;
            if (comment.IsNullOrWhiteSpace())
            {
                return;
            }
            if (StoreComments.Id != null)
            {
                Guid id = StoreComments.Id.Value;
                await _orderAppService.UpdateStoreCommentAsync(OrderId, id, comment);
            }
            else
            {
                await _orderAppService.AddStoreCommentAsync(OrderId, comment);
            }

            StoreComments = new();
            await GetOrderDetailsAsync();
            await loading.Hide();
        }
        catch (BusinessException ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(L[ex.Code]);
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        catch (Exception ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
    }

    void EditStoreComment(Guid id, string comment)
    {
        StoreComments = new StoreCommentsModel
        {
            Id = id,
            Comment = comment
        };
    }
    async Task SubmitCustomerServiceReplyAsync()
    {
        try
        {
            await loading.Show();
            string comment = StoreCustomerService.Message;
            StoreCustomerService.SenderId = CurrentUser.Id;
            StoreCustomerService.OrderId = Order.Id;
            StoreCustomerService.Timestamp = DateTime.Now;
            if (comment.IsNullOrWhiteSpace())
            {
                return;
            }
            //if (StoreCustomerService.Id != null)
            //{
            //    Guid id = StoreComments.Id.Value;
            //    await _orderAppService.UpdateStoreCommentAsync(OrderId, id, comment);
            //}
            //else
            //{
                await _OrderMessageAppService.CreateAsync(StoreCustomerService);
            //}

            StoreCustomerService = new();
           var result= await _OrderMessageAppService.GetListAsync(new GetOrderMessageListDto { MaxResultCount = 1000, OrderId = Order.Id });
            CustomerServiceHistory = result.Items;

            await loading.Hide();
            StateHasChanged();
        }
        catch (BusinessException ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(L[ex.Code]);
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        catch (Exception ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
    }
    void EditCVSStoreId()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewCVSStoreId ??= Order.StoreId;
        ModificationTrack.IsCVSStoreIdInputVisible = true;
    }
    void EditCVSStoreOutside()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewCVSStoreOutside ??= Order.CVSStoreOutSide;
        ModificationTrack.IsCVSStoreOutsideInputVisible = true;
    }
    void EditRecipientName()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewName ??= Order.RecipientName;
        ModificationTrack.IsNameInputVisible = true;
    }
    void EditRecipientNameDbsNormal()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewRecipientNameDbsNormal ??= Order.RecipientNameDbsNormal;
        ModificationTrack.IsRecipientNameDbsNormalInputVisible = true;
    }
    void EditRecipientNameDbsFreeze()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewRecipientNameDbsFreeze ??= Order.RecipientNameDbsFreeze;
        ModificationTrack.IsRecipientNameDbsFreezeInputVisible = true;
    }
    void EditRecipientNameDbsFrozen()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewRecipientNameDbsFrozen ??= Order.RecipientNameDbsFrozen;
        ModificationTrack.IsRecipientNameDbsFrozenInputVisible = true;
    }
    void EditRecipientPhoneDbsNormal()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewRecipientPhoneDbsNormal ??= Order.RecipientPhoneDbsNormal;
        ModificationTrack.IsRecipientPhoneDbsNormalInputVisible = true;
    }
    void EditRecipientPhoneDbsFreeze()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewRecipientPhoneDbsFreeze ??= Order.RecipientPhoneDbsFreeze;
        ModificationTrack.IsRecipientPhoneDbsFreezeInputVisible = true;
    }
    void EditRecipientPhoneDbsFrozen()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewRecipientPhoneDbsFrozen ??= Order.RecipientPhoneDbsFrozen;
        ModificationTrack.IsRecipientPhoneDbsFrozenInputVisible = true;
    }
    void EditStoreIdNormal()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewStoreIdNormal ??= Order.StoreIdNormal;
        ModificationTrack.IsStoreIdNormalInputVisible = true;
    }
    void EditStoreIdFreeze()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewStoreIdFreeze ??= Order.StoreIdFreeze;
        ModificationTrack.IsStoreIdFreezeInputVisible = true;
    }
    void EditStoreIdFrozen()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewStoreIdFrozen ??= Order.StoreIdFrozen;
        ModificationTrack.IsStoreIdFrozenInputVisible = true;
    }
    void EditCVSStoreOutSideNormal()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewCVSStoreOutSideNormal ??= Order.CVSStoreOutSideNormal;
        ModificationTrack.IsCVSStoreOutSideNormalInputVisible = true;
    }
    void EditCVSStoreOutSideFreeze()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewCVSStoreOutSideFreeze ??= Order.CVSStoreOutSideFreeze;
        ModificationTrack.IsCVSStoreOutSideFreezeInputVisible = true;
    }
    void EditCVSStoreOutSideFrozen()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewCVSStoreOutSideFrozen ??= Order.CVSStoreOutSideFrozen;
        ModificationTrack.IsCVSStoreOutSideFrozenInputVisible = true;
    }
    void EditPostalCodeDbsNormal()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewPostalCodeDbsNormal ??= Order.PostalCodeDbsNormal;
        ModificationTrack.IsPostalCodeDbsNormalInputVisible = true;
    }
    void EditPostalCodeDbsFreeze()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewPostalCodeDbsFreeze ??= Order.PostalCodeDbsFreeze;
        ModificationTrack.IsPostalCodeDbsFreezeInputVisible = true;
    }
    void EditPostalCodeDbsFrozen()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewPostalCodeDbsFrozen ??= Order.PostalCodeDbsFrozen;
        ModificationTrack.IsPostalCodeDbsFrozenInputVisible = true;
    }
    void EditCityDbsNormal()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewCityDbsNormal ??= Order.CityDbsNormal;
        ModificationTrack.IsCityDbsNormalInputVisible = true;
    }
    void EditCityDbsFreeze()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewCityDbsFreeze ??= Order.CityDbsFreeze;
        ModificationTrack.IsCityDbsFreezeInputVisible = true;
    }
    void EditCityDbsFrozen()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewCityDbsFrozen ??= Order.CityDbsFrozen;
        ModificationTrack.IsCityDbsFrozenInputVisible = true;
    }
    void EditAddressDetailsDbsNormal()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewAddressDetailsDbsNormal ??= Order.AddressDetailsDbsNormal;
        ModificationTrack.IsAddressDetailsDbsNormalInputVisible = true;
    }
    void EditAddressDetailsDbsFreeze()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewAddressDetailsDbsFreeze ??= Order.AddressDetailsDbsFreeze;
        ModificationTrack.IsAddressDetailsDbsFreezeInputVisible = true;
    }
    void EditAddressDetailsDbsFrozen()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewAddressDetailsDbsFrozen ??= Order.AddressDetailsDbsFrozen;
        ModificationTrack.IsAddressDetailsDbsFrozenInputVisible = true;
    }
    void EditRecipientPhone()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewPhone ??= Order.RecipientPhone;
        ModificationTrack.IsPhoneInputVisible = true;
    }

    void EditRecipientAddress()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewRoad ??= Order.Road;
        ModificationTrack.NewDistrict ??= Order.District;
        ModificationTrack.NewCity ??= Order.City;
        ModificationTrack.NewAddress ??= Order.AddressDetails;
        ModificationTrack.IsAddressInputVisible = true;
    }
    void EditRecipientPostalCode()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewPostalCode ??= Order.PostalCode;
        ModificationTrack.IsPostalCodeInputVisible = true;
    }
    void EditRecipientCity()
    {
        ModificationTrack.IsModified = true;
        ModificationTrack.NewCity ??= Order.City;
        ModificationTrack.IsCityInputVisible = true;
    }

    void SaveCVSStoreId()
    {
        if (ModificationTrack.NewCVSStoreId.IsNullOrWhiteSpace())
        {
            ModificationTrack.IsInvalidCVSStoreId = true;
        }
        else
        {
            ModificationTrack.IsCVSStoreIdModified = true;
            ModificationTrack.IsCVSStoreIdInputVisible = false;
            ModificationTrack.IsInvalidCVSStoreId = false;
        }

    }
    void SaveCVSStoreOutside()
    {
        if (ModificationTrack.NewCVSStoreOutside.IsNullOrWhiteSpace())
        {
            ModificationTrack.IsInvalidCVSStoreOutside = true;
        }
        else
        {
            ModificationTrack.IsCVSStoreOutsideModified = true;
            ModificationTrack.IsCVSStoreOutsideInputVisible = false;
            ModificationTrack.IsInvalidCVSStoreOutside = false;
        }

    }
    void SaveRecipientName()
    {
        if (ModificationTrack.NewName.IsNullOrWhiteSpace())
        {
            ModificationTrack.IsInvalidName = true;
        }
        else
        {
            ModificationTrack.IsNameModified = true;
            ModificationTrack.IsNameInputVisible = false;
            ModificationTrack.IsInvalidName = false;
        }

    }
    void SaveRecipientNameDbsNormal()
    {
        if (ModificationTrack.NewRecipientNameDbsNormal.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidRecipientNameDbsNormal = true;

        else
        {
            ModificationTrack.IsRecipientNameDbsNormalModified = true;
            ModificationTrack.IsRecipientNameDbsNormalInputVisible = false;
            ModificationTrack.IsInvalidRecipientNameDbsNormal = false;
        }
    }
    void SaveRecipientNameDbsFreeze()
    {
        if (ModificationTrack.NewRecipientNameDbsFreeze.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidRecipientNameDbsFreeze = true;

        else
        {
            ModificationTrack.IsRecipientNameDbsFreezeModified = true;
            ModificationTrack.IsRecipientNameDbsFreezeInputVisible = false;
            ModificationTrack.IsInvalidRecipientNameDbsFreeze = false;
        }
    }
    void SaveRecipientNameDbsFrozen()
    {
        if (ModificationTrack.NewRecipientNameDbsFrozen.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidRecipientNameDbsFrozen = true;

        else
        {
            ModificationTrack.IsRecipientNameDbsFrozenModified = true;
            ModificationTrack.IsRecipientNameDbsFrozenInputVisible = false;
            ModificationTrack.IsInvalidRecipientNameDbsFrozen = false;
        }
    }
    void SaveRecipientPhoneDbsNormal()
    {
        if (ModificationTrack.NewRecipientPhoneDbsNormal.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidRecipientPhoneDbsNormal = true;

        else
        {
            ModificationTrack.IsRecipientPhoneDbsNormalModified = true;
            ModificationTrack.IsRecipientPhoneDbsNormalInputVisible = false;
            ModificationTrack.IsInvalidRecipientPhoneDbsNormal = false;
        }
    }
    void SaveRecipientPhoneDbsFreeze()
    {
        if (ModificationTrack.NewRecipientPhoneDbsFreeze.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidRecipientPhoneDbsFreeze = true;

        else
        {
            ModificationTrack.IsRecipientPhoneDbsFreezeModified = true;
            ModificationTrack.IsRecipientPhoneDbsFreezeInputVisible = false;
            ModificationTrack.IsInvalidRecipientPhoneDbsFreeze = false;
        }
    }
    void SaveRecipientPhoneDbsFrozen()
    {
        if (ModificationTrack.NewRecipientPhoneDbsFrozen.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidRecipientPhoneDbsFrozen = true;

        else
        {
            ModificationTrack.IsRecipientPhoneDbsFrozenModified = true;
            ModificationTrack.IsRecipientPhoneDbsFrozenInputVisible = false;
            ModificationTrack.IsInvalidRecipientPhoneDbsFrozen = false;
        }
    }
    void SaveStoreIdNormal()
    {
        if (ModificationTrack.NewStoreIdNormal.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidStoreIdNormal = true;

        else
        {
            ModificationTrack.IsStoreIdNormalModified = true;
            ModificationTrack.IsStoreIdNormalInputVisible = false;
            ModificationTrack.IsInvalidStoreIdNormal = false;
        }
    }
    void SaveStoreIdFreeze()
    {
        if (ModificationTrack.NewStoreIdFreeze.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidStoreIdFreeze = true;

        else
        {
            ModificationTrack.IsStoreIdFreezeModified = true;
            ModificationTrack.IsStoreIdFreezeInputVisible = false;
            ModificationTrack.IsInvalidStoreIdFreeze = false;
        }
    }
    void SaveStoreIdFrozen()
    {
        if (ModificationTrack.NewStoreIdFrozen.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidStoreIdFrozen = true;

        else
        {
            ModificationTrack.IsStoreIdFrozenModified = true;
            ModificationTrack.IsStoreIdFrozenInputVisible = false;
            ModificationTrack.IsInvalidStoreIdFrozen = false;
        }
    }
    void SaveCVSStoreOutSideNormal()
    {
        if (ModificationTrack.NewCVSStoreOutSideNormal.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidCVSStoreOutSideNormal = true;

        else
        {
            ModificationTrack.IsCVSStoreOutSideNormalModified = true;
            ModificationTrack.IsCVSStoreOutSideNormalInputVisible = false;
            ModificationTrack.IsInvalidCVSStoreOutSideNormal = false;
        }
    }
    void SaveCVSStoreOutSideFreeze()
    {
        if (ModificationTrack.NewCVSStoreOutSideFreeze.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidCVSStoreOutSideFreeze = true;

        else
        {
            ModificationTrack.IsCVSStoreOutSideFreezeModified = true;
            ModificationTrack.IsCVSStoreOutSideFreezeInputVisible = false;
            ModificationTrack.IsInvalidCVSStoreOutSideFreeze = false;
        }
    }
    void SaveCVSStoreOutSideFrozen()
    {
        if (ModificationTrack.NewCVSStoreOutSideFrozen.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidCVSStoreOutSideFrozen = true;

        else
        {
            ModificationTrack.IsCVSStoreOutSideFrozenModified = true;
            ModificationTrack.IsCVSStoreOutSideFrozenInputVisible = false;
            ModificationTrack.IsInvalidCVSStoreOutSideFrozen = false;
        }
    }
    void SavePostalCodeDbsNormal()
    {
        if (ModificationTrack.NewPostalCodeDbsNormal.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidPostalCodeDbsNormal = true;

        else
        {
            ModificationTrack.IsPostalCodeDbsNormalModified = true;
            ModificationTrack.IsPostalCodeDbsNormalInputVisible = false;
            ModificationTrack.IsInvalidPostalCodeDbsNormal = false;
        }
    }
    void SavePostalCodeDbsFreeze()
    {
        if (ModificationTrack.NewPostalCodeDbsFreeze.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidPostalCodeDbsFreeze = true;

        else
        {
            ModificationTrack.IsPostalCodeDbsFreezeModified = true;
            ModificationTrack.IsPostalCodeDbsFreezeInputVisible = false;
            ModificationTrack.IsInvalidPostalCodeDbsFreeze = false;
        }
    }
    void SavePostalCodeDbsFrozen()
    {
        if (ModificationTrack.NewPostalCodeDbsFrozen.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidPostalCodeDbsFrozen = true;

        else
        {
            ModificationTrack.IsPostalCodeDbsFrozenModified = true;
            ModificationTrack.IsPostalCodeDbsFrozenInputVisible = false;
            ModificationTrack.IsInvalidPostalCodeDbsFrozen = false;
        }
    }
    void SaveCityDbsNormal()
    {
        if (ModificationTrack.NewCityDbsNormal.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidCityDbsNormal = true;

        else
        {
            ModificationTrack.IsCityDbsNormalModified = true;
            ModificationTrack.IsCityDbsNormalInputVisible = false;
            ModificationTrack.IsInvalidCityDbsNormal = false;
        }
    }
    void SaveCityDbsFreeze()
    {
        if (ModificationTrack.NewCityDbsFreeze.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidCityDbsFreeze = true;

        else
        {
            ModificationTrack.IsCityDbsFreezeModified = true;
            ModificationTrack.IsCityDbsFreezeInputVisible = false;
            ModificationTrack.IsInvalidCityDbsFreeze = false;
        }
    }
    void SaveCityDbsFrozen()
    {
        if (ModificationTrack.NewCityDbsFrozen.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidCityDbsFrozen = true;

        else
        {
            ModificationTrack.IsCityDbsFrozenModified = true;
            ModificationTrack.IsCityDbsFrozenInputVisible = false;
            ModificationTrack.IsInvalidCityDbsFrozen = false;
        }
    }
    void SaveAddressDetailsDbsNormal()
    {
        if (ModificationTrack.NewAddressDetailsDbsNormal.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidAddressDetailsDbsNormal = true;

        else
        {
            ModificationTrack.IsAddressDetailsDbsNormalModified = true;
            ModificationTrack.IsAddressDetailsDbsNormalInputVisible = false;
            ModificationTrack.IsInvalidAddressDetailsDbsNormal = false;
        }
    }
    void SaveAddressDetailsDbsFreeze()
    {
        if (ModificationTrack.NewAddressDetailsDbsFreeze.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidAddressDetailsDbsFreeze = true;

        else
        {
            ModificationTrack.IsAddressDetailsDbsFreezeModified = true;
            ModificationTrack.IsAddressDetailsDbsFreezeInputVisible = false;
            ModificationTrack.IsInvalidAddressDetailsDbsFreeze = false;
        }
    }
    void SaveAddressDetailsDbsFrozen()
    {
        if (ModificationTrack.NewAddressDetailsDbsFrozen.IsNullOrWhiteSpace()) 
            ModificationTrack.IsInvalidAddressDetailsDbsFrozen = true;

        else
        {
            ModificationTrack.IsAddressDetailsDbsFrozenModified = true;
            ModificationTrack.IsAddressDetailsDbsFrozenInputVisible = false;
            ModificationTrack.IsInvalidAddressDetailsDbsFrozen = false;
        }
    }
    void SaveRecipientPhone()
    {
        string pat = @"^\d+$";
        if ((ModificationTrack.NewPhone.IsNullOrWhiteSpace()) || (Regex.IsMatch(ModificationTrack.NewPhone, pat) == false))
        {
            ModificationTrack.IsInvalidPhone = true;
        }
        else
        {
            ModificationTrack.IsPhoneModified = true;
            ModificationTrack.IsPhoneInputVisible = false;
            ModificationTrack.IsInvalidPhone = false;
        }
    }
    void SaveRecipientAddress()
    {
        if (ModificationTrack.NewAddress.IsNullOrWhiteSpace())
        {
            ModificationTrack.IsInvalidAddress = true;
        }
        else
        {
            ModificationTrack.IsAddressModified = true;
            ModificationTrack.IsAddressInputVisible = false;
            ModificationTrack.IsInvalidAddress = false;
        }
    }
    void SaveRecipientPostalCode()
    {
        if (ModificationTrack.NewPostalCode.IsNullOrWhiteSpace())
        {
            ModificationTrack.IsInvalidPostalCode = true;
        }
        else
        {
            ModificationTrack.IsPostalCodeModified = true;
            ModificationTrack.IsPostalCodeInputVisible = false;
            ModificationTrack.IsInvalidPostalCode = false;
        }
    }

    void SaveRecipientCity()
    {
        if (ModificationTrack.NewCity.IsNullOrWhiteSpace())
        {
            ModificationTrack.IsInvalidCity = true;
        }
        else
        {
            ModificationTrack.IsCityModified = true;
            ModificationTrack.IsCityInputVisible = false;
            ModificationTrack.IsInvalidCity = false;
        }
    }
    void CancelChanges()
    {
        ModificationTrack = new();
    }
    protected virtual async Task SaveChangesAsync()
    {
        try
        {
            if (ModificationTrack.IsInvalidName ||
                ModificationTrack.IsInvalidPhone ||
                ModificationTrack.IsInvalidAddress ||
                ModificationTrack.IsInvalidPostalCode ||
                ModificationTrack.IsInvalidCVSStoreId ||
                ModificationTrack.IsInvalidCVSStoreOutside ||
                ModificationTrack.IsInvalidRecipientNameDbsNormal ||
                ModificationTrack.IsInvalidRecipientNameDbsFreeze ||
                ModificationTrack.IsInvalidRecipientNameDbsFrozen ||
                ModificationTrack.IsInvalidRecipientPhoneDbsNormal ||
                ModificationTrack.IsInvalidRecipientPhoneDbsFreeze ||
                ModificationTrack.IsInvalidRecipientPhoneDbsFrozen ||
                ModificationTrack.IsInvalidPostalCodeDbsNormal ||
                ModificationTrack.IsInvalidPostalCodeDbsFreeze ||
                ModificationTrack.IsInvalidPostalCodeDbsFrozen ||
                ModificationTrack.IsInvalidCityDbsNormal ||
                ModificationTrack.IsInvalidCityDbsFreeze ||
                ModificationTrack.IsInvalidCityDbsFrozen ||
                ModificationTrack.IsInvalidAddressDetailsDbsNormal ||
                ModificationTrack.IsInvalidAddressDetailsDbsFreeze ||
                ModificationTrack.IsInvalidAddressDetailsDbsFrozen ||
                ModificationTrack.IsInvalidStoreIdNormal ||
                ModificationTrack.IsInvalidStoreIdFreeze ||
                ModificationTrack.IsInvalidStoreIdFrozen ||
                ModificationTrack.IsInvalidCVSStoreOutSideNormal ||
                ModificationTrack.IsInvalidCVSStoreOutSideFreeze ||
                ModificationTrack.IsInvalidCVSStoreOutSideFrozen
            )
            {
                return;
            }
            if (ModificationTrack.IsNameInputVisible)
            {
                ModificationTrack.IsInvalidName = true;
                return;
            }
            else if (ModificationTrack.IsPhoneInputVisible)
            {
                ModificationTrack.IsInvalidPhone = true;
                return;
            }
            else if (ModificationTrack.IsAddressInputVisible)
            {
                ModificationTrack.IsInvalidAddress = true;
                return;
            }
            else if (ModificationTrack.IsPostalCodeInputVisible)
            {
                ModificationTrack.IsInvalidPostalCode = true;
                return;
            }
            else if (ModificationTrack.IsCityInputVisible)
            {
                ModificationTrack.IsInvalidCity = true;
                return;
            }
            else if (ModificationTrack.IsCVSStoreIdInputVisible)
            {
                ModificationTrack.IsInvalidCVSStoreId = true;
                return;
            }
            else if (ModificationTrack.IsCVSStoreOutsideInputVisible)
            {
                ModificationTrack.IsInvalidCVSStoreOutside = true;
                return;
            }
            else if (ModificationTrack.IsRecipientNameDbsNormalInputVisible)
            {
                ModificationTrack.IsInvalidRecipientNameDbsNormal = true;
                return;
            }
            else if (ModificationTrack.IsRecipientNameDbsFreezeInputVisible)
            {
                ModificationTrack.IsInvalidRecipientNameDbsFreeze = true;
                return;
            }
            else if (ModificationTrack.IsRecipientNameDbsFrozenInputVisible)
            {
                ModificationTrack.IsInvalidRecipientNameDbsFrozen = true;
                return;
            }
            else if (ModificationTrack.IsRecipientPhoneDbsNormalInputVisible)
            {
                ModificationTrack.IsInvalidRecipientPhoneDbsNormal = true;
                return;
            }
            else if (ModificationTrack.IsRecipientPhoneDbsFreezeInputVisible)
            {
                ModificationTrack.IsInvalidRecipientPhoneDbsFreeze = true;
                return;
            }
            else if (ModificationTrack.IsRecipientPhoneDbsFrozenInputVisible)
            {
                ModificationTrack.IsInvalidRecipientPhoneDbsFrozen = true;
                return;
            }
            else if (ModificationTrack.IsPostalCodeDbsNormalInputVisible)
            {
                ModificationTrack.IsInvalidPostalCodeDbsNormal = true;
                return;
            }
            else if (ModificationTrack.IsPostalCodeDbsFreezeInputVisible)
            {
                ModificationTrack.IsInvalidPostalCodeDbsFreeze = true;
                return;
            }
            else if (ModificationTrack.IsPostalCodeDbsFrozenInputVisible)
            {
                ModificationTrack.IsInvalidPostalCodeDbsFrozen = true;
                return;
            }
            else if (ModificationTrack.IsCityDbsNormalInputVisible)
            {
                ModificationTrack.IsInvalidCityDbsNormal = true;
                return;
            }
            else if (ModificationTrack.IsCityDbsFreezeInputVisible)
            {
                ModificationTrack.IsInvalidCityDbsFreeze = true;
                return;
            }
            else if (ModificationTrack.IsCityDbsFrozenInputVisible)
            {
                ModificationTrack.IsInvalidCityDbsFrozen = true;
                return;
            }
            else if (ModificationTrack.IsAddressDetailsDbsNormalInputVisible)
            {
                ModificationTrack.IsInvalidAddressDetailsDbsNormal = true;
                return;
            }
            else if (ModificationTrack.IsAddressDetailsDbsFreezeInputVisible)
            {
                ModificationTrack.IsInvalidAddressDetailsDbsFreeze = true;
                return;
            }
            else if (ModificationTrack.IsAddressDetailsDbsFrozenInputVisible)
            {
                ModificationTrack.IsInvalidAddressDetailsDbsFrozen = true;
                return;
            }
            else if (ModificationTrack.IsStoreIdNormalInputVisible)
            {
                ModificationTrack.IsInvalidStoreIdNormal = true;
                return;
            }
            else if (ModificationTrack.IsStoreIdFreezeInputVisible)
            {
                ModificationTrack.IsInvalidStoreIdFreeze = true;
                return;
            }
            else if (ModificationTrack.IsStoreIdFrozenInputVisible)
            {
                ModificationTrack.IsInvalidStoreIdFrozen = true;
                return;
            }
            else if (ModificationTrack.IsCVSStoreOutSideNormalInputVisible)
            {
                ModificationTrack.IsInvalidCVSStoreOutSideNormal = true;
                return;
            }
            else if (ModificationTrack.IsCVSStoreOutSideFreezeInputVisible)
            {
                ModificationTrack.IsInvalidCVSStoreOutSideFreeze = true;
                return;
            }
            else if (ModificationTrack.IsCVSStoreOutSideFrozenInputVisible)
            {
                ModificationTrack.IsInvalidCVSStoreOutSideFrozen = true;
                return;
            }
            else
            {
                ModificationTrack.IsInvalidName = false;
                ModificationTrack.IsInvalidPhone = false;
                ModificationTrack.IsInvalidAddress = false;
                ModificationTrack.IsInvalidPostalCode = false;
                ModificationTrack.IsInvalidCity = false;
                ModificationTrack.IsInvalidCVSStoreId = false;
                ModificationTrack.IsInvalidCVSStoreOutside = false;
                ModificationTrack.IsInvalidRecipientNameDbsNormal = false;
                ModificationTrack.IsInvalidRecipientNameDbsFreeze = false;
                ModificationTrack.IsInvalidRecipientNameDbsFrozen = false;
                ModificationTrack.IsInvalidRecipientPhoneDbsNormal = false;
                ModificationTrack.IsInvalidRecipientPhoneDbsFreeze = false;
                ModificationTrack.IsInvalidRecipientPhoneDbsFrozen = false;
                ModificationTrack.IsInvalidPostalCodeDbsNormal = false;
                ModificationTrack.IsInvalidPostalCodeDbsFreeze = false;
                ModificationTrack.IsInvalidPostalCodeDbsFrozen = false;
                ModificationTrack.IsInvalidCityDbsNormal = false;
                ModificationTrack.IsInvalidCityDbsFreeze = false;
                ModificationTrack.IsInvalidCityDbsFrozen = false;
                ModificationTrack.IsInvalidAddressDetailsDbsNormal = false;
                ModificationTrack.IsInvalidAddressDetailsDbsFreeze = false;
                ModificationTrack.IsInvalidAddressDetailsDbsFrozen = false;
                ModificationTrack.IsInvalidStoreIdNormal = false;
                ModificationTrack.IsInvalidStoreIdFreeze = false;
                ModificationTrack.IsInvalidStoreIdFrozen = false;
                ModificationTrack.IsInvalidCVSStoreOutSideNormal = false;
                ModificationTrack.IsInvalidCVSStoreOutSideFreeze = false;
                ModificationTrack.IsInvalidCVSStoreOutSideFrozen = false;
            }

            UpdateOrder = _ObjectMapper.Map<OrderDto, CreateOrderDto>(Order);

            UpdateOrder.RecipientName = ModificationTrack.IsNameModified ? 
                                        ModificationTrack.NewName : 
                                        Order.RecipientName;

            UpdateOrder.RecipientPhone = ModificationTrack.IsPhoneModified ? 
                                         ModificationTrack.NewPhone : 
                                         Order.RecipientPhone;

            UpdateOrder.PostalCode = ModificationTrack.IsPostalCodeModified ? 
                                     ModificationTrack.NewPostalCode : 
                                     Order.PostalCode;

            UpdateOrder.City = ModificationTrack.IsCityModified ?
                               ModificationTrack.NewCity : 
                               Order.City;

            UpdateOrder.AddressDetails = ModificationTrack.IsAddressModified ?
                                         ModificationTrack.NewAddress : 
                                         Order.AddressDetails;

            UpdateOrder.StoreId = ModificationTrack.IsCVSStoreIdModified ?
                                  ModificationTrack.NewCVSStoreId :
                                  Order.StoreId;

            UpdateOrder.CVSStoreOutSide = ModificationTrack.IsCVSStoreOutsideModified ?
                                 ModificationTrack.NewCVSStoreOutside :
                                 Order.CVSStoreOutSide;

            UpdateOrder.RecipientNameDbsNormal = ModificationTrack.IsRecipientNameDbsNormalModified ?
                                ModificationTrack.NewRecipientNameDbsNormal :
                                Order.RecipientNameDbsNormal;

            UpdateOrder.RecipientNameDbsFreeze = ModificationTrack.IsRecipientNameDbsFreezeModified ?
                                ModificationTrack.NewRecipientNameDbsFreeze :
                                Order.RecipientNameDbsFreeze;

            UpdateOrder.RecipientNameDbsFrozen = ModificationTrack.IsRecipientNameDbsFrozenModified ?
                                ModificationTrack.NewRecipientNameDbsFrozen :
                                Order.RecipientNameDbsFrozen;

            UpdateOrder.RecipientPhoneDbsNormal = ModificationTrack.IsRecipientPhoneDbsNormalModified ?
                                ModificationTrack.NewRecipientPhoneDbsNormal :
                                Order.RecipientPhoneDbsNormal;

            UpdateOrder.RecipientPhoneDbsFreeze = ModificationTrack.IsRecipientPhoneDbsFreezeModified ?
                                ModificationTrack.NewRecipientPhoneDbsFreeze :
                                Order.RecipientPhoneDbsFreeze;

            UpdateOrder.RecipientPhoneDbsFrozen = ModificationTrack.IsRecipientPhoneDbsFrozenModified ?
                                ModificationTrack.NewRecipientPhoneDbsFrozen :
                                Order.RecipientPhoneDbsFrozen;

            UpdateOrder.PostalCodeDbsNormal = ModificationTrack.IsPostalCodeDbsNormalModified ?
                                ModificationTrack.NewPostalCodeDbsNormal :
                                Order.PostalCodeDbsNormal;

            UpdateOrder.PostalCodeDbsFreeze = ModificationTrack.IsPostalCodeDbsFreezeModified ?
                                ModificationTrack.NewPostalCodeDbsFreeze :
                                Order.PostalCodeDbsFreeze;

            UpdateOrder.PostalCodeDbsFrozen = ModificationTrack.IsPostalCodeDbsFrozenModified ?
                                ModificationTrack.NewPostalCodeDbsFrozen :
                                Order.PostalCodeDbsFrozen;

            UpdateOrder.CityDbsNormal = ModificationTrack.IsCityDbsNormalModified ?
                                ModificationTrack.NewCityDbsNormal :
                                Order.CityDbsNormal;

            UpdateOrder.CityDbsFreeze = ModificationTrack.IsCityDbsFreezeModified ?
                                ModificationTrack.NewCityDbsFreeze :
                                Order.CityDbsFreeze;

            UpdateOrder.CityDbsFrozen = ModificationTrack.IsCityDbsFrozenModified ?
                                ModificationTrack.NewCityDbsFrozen :
                                Order.CityDbsFrozen;

            UpdateOrder.AddressDetailsDbsNormal = ModificationTrack.IsAddressDetailsDbsNormalModified ?
                                ModificationTrack.NewAddressDetailsDbsNormal :
                                Order.AddressDetailsDbsNormal;

            UpdateOrder.AddressDetailsDbsFreeze = ModificationTrack.IsAddressDetailsDbsFreezeModified ?
                                ModificationTrack.NewAddressDetailsDbsFreeze :
                                Order.AddressDetailsDbsFreeze;

            UpdateOrder.AddressDetailsDbsFrozen = ModificationTrack.IsAddressDetailsDbsFrozenModified ?
                                ModificationTrack.NewAddressDetailsDbsFrozen :
                                Order.AddressDetailsDbsFrozen;

            UpdateOrder.StoreIdNormal = ModificationTrack.IsStoreIdNormalModified ?
                                ModificationTrack.NewStoreIdNormal :
                                Order.StoreIdNormal;

            UpdateOrder.StoreIdFreeze = ModificationTrack.IsStoreIdFreezeModified ?
                                ModificationTrack.NewStoreIdFreeze :
                                Order.StoreIdFreeze;

            UpdateOrder.StoreIdFrozen = ModificationTrack.IsStoreIdFrozenModified ?
                                ModificationTrack.NewStoreIdFrozen :
                                Order.StoreIdFrozen;

            UpdateOrder.CVSStoreOutSideNormal = ModificationTrack.IsCVSStoreOutSideNormalModified ?
                                ModificationTrack.NewCVSStoreOutSideNormal :
                                Order.CVSStoreOutSideNormal;

            UpdateOrder.CVSStoreOutSideFreeze = ModificationTrack.IsCVSStoreOutSideFreezeModified ?
                                ModificationTrack.NewCVSStoreOutSideFreeze :
                                Order.CVSStoreOutSideFreeze;

            UpdateOrder.CVSStoreOutSideFrozen = ModificationTrack.IsCVSStoreOutSideFrozenModified ?
                                ModificationTrack.NewCVSStoreOutSideFrozen :
                                Order.CVSStoreOutSideFrozen;

            await loading.Show();
            UpdateOrder.OrderStatus = Order.OrderStatus;
            UpdateOrder.ShouldSendEmail = true;

            Order = await _orderAppService.UpdateAsync(OrderId, UpdateOrder);
            ModificationTrack = new();
            await InvokeAsync(StateHasChanged);
            await loading.Hide();
        }
        catch (BusinessException ex)
        {
            await loading.Hide();
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            await _uiMessageService.Error(L[ex.Code?.ToString()]);
        }
        catch (Exception ex)
        {
            await loading.Hide();
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            await _uiMessageService.Error(ex.GetType().ToString());
        }

    }
    
    public void NavigateToOrderShipmentDetails()
    {
        List<Guid?> orderId = new() { {Order?.Id} };

        string serializedId = Newtonsoft.Json.JsonConvert.SerializeObject(orderId);

        NavigationManager.NavigateTo($"Orders/OrderShippingDetails/{serializedId}");
    }
    
    async Task CancelOrder()
    {
        try
        {
            if (Order?.ShippingStatus == ShippingStatus.WaitingForPayment)
            {
                var confirmed = await _uiMessageService.Confirm(L["AreYouSureToCancelOrder?"]);
                if (confirmed)
                {
                    await loading.Show();
                    await _orderAppService.CancelOrderAsync(OrderId);
                    await GetOrderDetailsAsync();
                    await InvokeAsync(StateHasChanged);
                    await loading.Hide();
                }
            }
        }
        catch (Exception ex)
        {
            await loading.Hide();
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            await _uiMessageService.Error(ex.GetType().ToString());
        }
    }

    private  void OpenShipmentModal(OrderDeliveryDto deliveryOrder)
    {
      OrderDeliveryId=deliveryOrder.Id;
        shipments = new Shipments
        {
            ShippingMethod = deliveryOrder?.DeliveryMethod ?? DeliveryMethod.PostOffice,
            ShippingNumber = deliveryOrder.DeliveryNo
        };

        CreateShipmentModal.Show();
    }
    private async void OrderItemShipped(OrderDeliveryDto deliveryOrder)
    {
        await loading.Show();
        OrderDeliveryId = deliveryOrder.Id;
        await _orderDeliveryAppService.UpdateOrderDeliveryStatus(OrderDeliveryId);
        await GetOrderDetailsAsync();
        await InvokeAsync(StateHasChanged);
        await loading.Hide();

    }
    private async void TestLabel(OrderDeliveryDto deliveryOrder)
    {
        if (deliveryOrder.DeliveryMethod == DeliveryMethod.SevenToEleven1 || deliveryOrder.DeliveryMethod == DeliveryMethod.FamilyMart1)
        {
            await loading.Show();

            var result = await _testLableAppService.TestLableAsync(deliveryOrder.DeliveryMethod == DeliveryMethod.SevenToEleven1 ? "UNIMART" : "FAMI");
            await InvokeAsync(StateHasChanged);
            await loading.Hide();
        }

    }
    private async void CreateOrderLogistics(OrderDeliveryDto deliveryOrder)
    {
        await loading.Show();

        OrderDeliveryId = deliveryOrder.Id;

        if (deliveryOrder.DeliveryMethod is DeliveryMethod.SevenToEleven1 || 
            deliveryOrder.DeliveryMethod is DeliveryMethod.FamilyMart1 ||
            deliveryOrder.DeliveryMethod is DeliveryMethod.SevenToElevenC2C || 
            deliveryOrder.DeliveryMethod is DeliveryMethod.FamilyMartC2C)
        {
            ResponseResultDto result = await _storeLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(Order.Id, deliveryOrder.Id);
            
            if (result.ResponseCode is not "1") await _uiMessageService.Error(result.ResponseMessage);

            #region Commented Code
            //    var htmlString = await _storeLogisticsOrderAppService.GetStoreAsync(Order.Id);
            //    StringBuilder htmlForm = new();
            //    htmlForm.Append(htmlString.HtmlString);
            //    string html = htmlString.HtmlString;
            //    html=UpdateAttributes(html,Order.Id.ToString(),deliveryOrder.Id.ToString());
            //    //int startIndex = htmlString.HtmlString.IndexOf("<script src=\"/Scripts/jquery-1.4.4.js\" type=\"text/javascript\">");
            //    //int endIndex = htmlString.HtmlString.IndexOf("</script>", startIndex);

            //    //int startIndexForm = htmlString.HtmlString.IndexOf("<form id=\"PostForm\" name=\"PostForm\" action=\"/Home/Family\" method=\"POST\">");
            //    //int endIndexForm = htmlString.HtmlString.IndexOf("</form>", startIndexForm);

            //    //if (startIndex != -1 && endIndex != -1)
            //    //{
            //    //    // Extract the script tag
            //    //    string scriptTag = htmlString.HtmlString.Substring(startIndex, endIndex - startIndex + "</script>".Length);

            //    //    // Replace the old src attribute with the new one
            //    //    string newScriptTag = scriptTag.Replace("src=\"/Scripts/jquery-1.4.4.js\"", "src=\"https://logistics-stage.ecpay.com.tw/Scripts/jquery-1.4.4.js\"");

            //    //    // Update the HTML string
            //    //    htmlString.HtmlString.Replace(scriptTag, newScriptTag);
            //    //     html = htmlString.HtmlString.Replace(scriptTag, newScriptTag);

            //    //    // Convert the updated string back to StringBuilder
            //    //    htmlForm = new StringBuilder(html);
            //    //}
            //    //if (startIndexForm != -1 && endIndexForm != -1)
            //    //{
            //    //    // Extract the form tag
            //    //    string formTag = html.Substring(startIndexForm, endIndexForm - startIndexForm + "</form>".Length);

            //    //    // Replace the old action attribute with the new one
            //    //    string newFormTag = formTag.Replace("action=\"/Home/Family\"", "action=\"https://logistics-stage.ecpay.com.tw/Home/Family\"");

            //    //    // Update the HTML string
            //    //    html = html.Replace(formTag, newFormTag);
            //    //    htmlForm = new StringBuilder(html);
            //    //}
            //    //await JSRuntime.InvokeVoidAsync("setCookie", htmlString.CookieName, htmlString.CookieValue,"None",true);
            //    //NavigationManager.NavigateTo($"/map-response?htmlString={Uri.EscapeDataString(htmlForm.ToString())}");
            //    await JSRuntime.InvokeVoidAsync("openPopup", html);
            //    //NavigationManager.NavigateTo($"map-response/{htmlForm}");
            #endregion
        }

        else if (deliveryOrder.DeliveryMethod is DeliveryMethod.TCatDeliveryNormal ||
                 deliveryOrder.DeliveryMethod is DeliveryMethod.TCatDeliveryFreeze ||
                 deliveryOrder.DeliveryMethod is DeliveryMethod.TCatDeliveryFrozen)
        {
            PrintObtResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(Order.Id, deliveryOrder.Id);

            if (response is null || response.Data is null) await _uiMessageService.Error(response.Message);
        }
        
        else if (deliveryOrder.DeliveryMethod is DeliveryMethod.TCatDeliverySevenElevenNormal ||
                 deliveryOrder.DeliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                 deliveryOrder.DeliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFrozen)
        {
            PrintOBTB2SResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(Order.Id, deliveryOrder.Id);

            if (response is null || response.Data is null) await _uiMessageService.Error(response.Message);
        }

        else if (deliveryOrder.DeliveryMethod is DeliveryMethod.DeliveredByStore)
        {
            LogisticProviders? logisticProvider = null; DeliveryMethod? deliveryMethod = null; ItemStorageTemperature? temperature = null;

            List<DeliveryTemperatureCostDto> deliveryTemperatureCosts = await _DeliveryTemperatureCostAppService.GetListAsync();

            foreach (DeliveryTemperatureCostDto entity in deliveryTemperatureCosts)
            {
                if (deliveryOrder.Items.Any(a => a.DeliveryTemperature == entity.Temperature))
                {
                    logisticProvider = entity.LogisticProvider;

                    deliveryMethod = entity.DeliveryMethod;

                    temperature = entity.Temperature;
                }
            }

            if (temperature is ItemStorageTemperature.Normal)
            {
                if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is DeliveryMethod.FamilyMart1 ||
                    logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is DeliveryMethod.SevenToEleven1 ||
                    logisticProvider is LogisticProviders.GreenWorldLogisticsC2C && deliveryMethod is DeliveryMethod.FamilyMartC2C ||
                    logisticProvider is LogisticProviders.GreenWorldLogisticsC2C && deliveryMethod is DeliveryMethod.SevenToElevenC2C)
                {
                    ResponseResultDto result = await _storeLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                    if (result.ResponseCode is not "1") await _uiMessageService.Error(result.ResponseMessage);
                }

                else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is DeliveryMethod.PostOffice ||
                         logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is DeliveryMethod.BlackCat1)
                {
                    ResponseResultDto result = await _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(Order.Id, OrderDeliveryId, deliveryMethod);

                    if (result.ResponseCode is not "1")
                    {
                        await _uiMessageService.Error(result.ResponseMessage);
                        await loading.Hide();
                    }
                }

                else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliveryNormal)
                {
                    PrintObtResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                    if (response is null || response.Data is null) await _uiMessageService.Error(response.Message);
                }

                else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenNormal)
                {
                    PrintOBTB2SResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                    if (response is null || response.Data is null) await _uiMessageService.Error(response.Message);
                }
            }

            else if (temperature is ItemStorageTemperature.Freeze)
            {
                if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is DeliveryMethod.BlackCatFreeze)
                {
                    ResponseResultDto result = await _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(Order.Id, OrderDeliveryId, deliveryMethod);

                    if (result.ResponseCode is not "1")
                    {
                        await _uiMessageService.Error(result.ResponseMessage);
                        await loading.Hide();
                    }
                }

                else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliveryFreeze)
                {
                    PrintObtResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                    if (response is null || response.Data is null) await _uiMessageService.Error(response.Message);
                }

                else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFreeze)
                {
                    PrintOBTB2SResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                    if (response is null || response.Data is null) await _uiMessageService.Error(response.Message);
                }
            }

            else if (temperature is ItemStorageTemperature.Frozen)
            {
                if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliveryFrozen)
                {
                    PrintObtResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                    if (response is null || response.Data is null) await _uiMessageService.Error(response.Message);
                }

                else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFrozen)
                {
                    PrintOBTB2SResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                    if (response is null || response.Data is null) await _uiMessageService.Error(response.Message);
                }

                else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is DeliveryMethod.BlackCatFrozen)
                {
                    ResponseResultDto result = await _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(Order.Id, OrderDeliveryId, deliveryMethod);

                    if (result.ResponseCode is not "1")
                    {
                        await _uiMessageService.Error(result.ResponseMessage);
                        await loading.Hide();
                    }
                }

                else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is DeliveryMethod.SevenToElevenFrozen)
                {
                    ResponseResultDto result = await _storeLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                    if (result.ResponseCode is not "1") await _uiMessageService.Error(result.ResponseMessage);
                }
            }
        }

        else if (deliveryOrder.DeliveryMethod is EnumValues.DeliveryMethod.SelfPickup ||
                       deliveryOrder.DeliveryMethod is EnumValues.DeliveryMethod.HomeDelivery)
        {
            await _storeLogisticsOrderAppService.GenerateDeliveryNumberForSelfPickupAndHomeDeliveryAsync(Order.Id, deliveryOrder.Id);
        }

        else
        {
            ResponseResultDto result = await _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(Order.Id, OrderDeliveryId);
            
            if (result.ResponseCode is not "1")
            {
                await _uiMessageService.Error(result.ResponseMessage);
                await loading.Hide();
            }
        }
        
        await GetOrderDetailsAsync();
        await InvokeAsync(StateHasChanged);
        await loading.Hide();
    }

    public bool CheckForDeliveryMethod(DeliveryMethod deliveryMethod)
    {
        return deliveryMethod is DeliveryMethod.FamilyMartC2C ||
               deliveryMethod is DeliveryMethod.FamilyMart1 ||
               deliveryMethod is DeliveryMethod.SevenToElevenC2C ||
               deliveryMethod is DeliveryMethod.SevenToEleven1 ||
               deliveryMethod is DeliveryMethod.SevenToElevenFrozen ||
               deliveryMethod is DeliveryMethod.PostOffice ||
               deliveryMethod is DeliveryMethod.BlackCat1 ||
               deliveryMethod is DeliveryMethod.BlackCatFreeze ||
               deliveryMethod is DeliveryMethod.BlackCatFrozen ||
               deliveryMethod is DeliveryMethod.TCatDeliveryNormal ||
               deliveryMethod is DeliveryMethod.TCatDeliveryFreeze ||
               deliveryMethod is DeliveryMethod.TCatDeliveryFrozen ||
               deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenNormal ||
               deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFreeze || 
               deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFrozen ||
               deliveryMethod is DeliveryMethod.DeliveredByStore ||
               deliveryMethod is DeliveryMethod.SelfPickup ||
               deliveryMethod is DeliveryMethod.HomeDelivery;
    }

    private async Task<string> GetPaymentStatus()
    {
        if (Order.PaymentMethod is PaymentMethods.CreditCard)
        {
            List<PaymentGatewayDto> paymentGateways = await _PaymentGatewayAppService.GetAllAsync();

            PaymentGatewayDto? ecpay = paymentGateways.FirstOrDefault(w => w.PaymentIntegrationType == PaymentIntegrationType.EcPay);

            if (ecpay is null) return string.Empty;

            RestClientOptions options = new() { MaxTimeout = -1 };

            RestClient client = new(options);

            RestRequest request = new(_Configuration["EcPay:SingleCreditCardTransactionApi"], Method.Post);

            string HashKey = ecpay.HashKey ?? string.Empty;
            
            string HashIV = ecpay.HashIV ?? string.Empty;
            
            string MerchantId = ecpay.MerchantId ?? string.Empty;

            string totalAmount = Order.TotalAmount.ToString(Order.TotalAmount % 1 is 0 ? "0" : string.Empty);

            request.AddHeader("Accept", "text/html");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("MerchantID", MerchantId);
            request.AddParameter("CreditRefundId", (Order.GWSR ?? 0).ToString());
            request.AddParameter("CreditAmount", totalAmount);
            request.AddParameter("CreditCheckCode", "52482296");
            request.AddParameter("CheckMacValue", GenerateCheckMac(HashKey,
                                                                   HashIV,
                                                                   MerchantId,
                                                                   Order.GWSR ?? 0,
                                                                   totalAmount,
                                                                   "52482296"));

            RestResponse response = await client.ExecuteAsync(request);

            PaymentStatus? paymentStatus = JsonSerializer.Deserialize<PaymentStatus>(response.Content);

            if (paymentStatus is null || paymentStatus.RtnValue?.status is null) return string.Empty;

            return paymentStatus.RtnValue?.status;
        }

        if (Order.PaymentMethod is PaymentMethods.BankTransfer ||
            Order.PaymentMethod is PaymentMethods.CashOnDelivery) return L["Paid"];

        return string.Empty;
    }

    public string GenerateCheckMac(string HashKey, string HashIV, string merchantID, int gwsr, string totalAmount, string creditCheckCode)
    {
        Dictionary<string, string> parameters = new ()
        {
            { "MerchantID", merchantID },
            { "CreditRefundId", gwsr.ToString() },
            { "CreditAmount", totalAmount },
            { "CreditCheckCode", creditCheckCode }
        };

        IEnumerable<string>? param = parameters.ToDictionary().Keys
                                      .OrderBy(o => o)
                                      .Select(s => s + "=" + parameters.ToDictionary()[s]);

        string collectionValue = string.Join("&", param);

        collectionValue = $"HashKey={HashKey}" + "&" + collectionValue + $"&HashIV={HashIV}";

        collectionValue = WebUtility.UrlEncode(collectionValue).ToLower();

        return ComputeSHA256Hash(collectionValue);
    }

    public string ComputeSHA256Hash(string rawData)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));

        StringBuilder builder = new();

        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }

        return builder.ToString().ToUpper();
    }

    private string GetRefundChannel()
    {
        if (Order.PaymentMethod is PaymentMethods.CreditCard) return L["AutomaticRefund"];

        if (Order.PaymentMethod is PaymentMethods.BankTransfer || 
            Order.PaymentMethod is PaymentMethods.CashOnDelivery) return L["ManualProcessing"];

        return string.Empty;
    }

    private void CloseShipmentModal()
    {
        CreateShipmentModal.Hide();
    }
    private void CloseRefundModal()
    {
        RefundModal.Hide();
    }
    private async Task ApplyRefundAsync()
    {
        try
        {
            if (refunds.IsRefundOrder)
            {
                await ApplyRefund();

                await RefundModal.Hide();
            }
            else if (refunds.IsRefundItems)
            {
                await loading.Show();
                
                List<Guid>? orderItemIds = [.. Order?.OrderItems.Where(x => x.IsSelected).Select(x => x.Id)];
                
                if (orderItemIds.Count < 1)
                {
                    await _uiMessageService.Error("Please Select Order Item");
                    
                    await loading.Hide();
                
                    return;
                }
                
                if (Order.OrderItems.Count == Order?.OrderItems.Count(c => c.IsSelected))
                {
                    await loading.Hide();

                    await ApplyRefund();

                    await RefundModal.Hide();

                    return;
                }

                refunds.OrderItemIds = orderItemIds;

                await _orderAppService.RefundOrderItems(orderItemIds, OrderId);

                await loading.Hide();

                await RefundModal.Hide();
            }
            else 
            {
                await loading.Show();

                if(refunds.Amount is 0)
                {
                    await _uiMessageService.Error("Please Enter Amount");
                    
                    await loading.Hide();

                    return;
                }

                if(refunds.Amount > (double)Order.TotalAmount) 
                {
                    await _uiMessageService.Error("Enter amount is greater than order amount");

                    await loading.Hide();

                    return;
                }
                
                await _orderAppService.RefundAmountAsync(refunds.Amount, OrderId);
                
                await loading.Hide();

                await RefundModal.Hide();
            }
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            await loading.Hide();
        }
        finally
        {
          await GetOrderDetailsAsync();
        }
    }
    private async Task UpdateCheckState(int checkbox)
    {
        switch (checkbox)
        {
            case 1:
                refunds.IsRefundOrder = true;
                refunds.IsRefundItems = false;
                refunds.IsRefundAmount = false;
                await InvokeAsync(StateHasChanged);
                break;
            case 2:
                refunds.IsRefundItems = true;
                refunds.IsRefundOrder = false;
                refunds.IsRefundAmount = false;
                await InvokeAsync(StateHasChanged);
                break;
            case 3:
                refunds.IsRefundAmount = true;
                refunds.IsRefundOrder = false;
                refunds.IsRefundItems = false;
                await InvokeAsync(StateHasChanged);
                break;
        }
        await InvokeAsync(StateHasChanged);
    }

    private async Task ApplyShipmentAsync()
    {
        try
        {
            await loading.Show();
            
            UpdateOrder.ShippingNumber = shipments.ShippingNumber;
            
            UpdateOrder.DeliveryMethod = shipments.ShippingMethod;
            
            await _orderDeliveryAppService.UpdateShippingDetails(OrderDeliveryId, UpdateOrder);
            
            await CreateShipmentModal.Hide();
            
            await GetOrderDetailsAsync();
            
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await loading.Hide();
        }
    }
    private async void SplitOrder()
    {
        var orderItemIds = Order?.OrderItems.Where(x => x.IsSelected).Select(x => x.Id).ToList();
        await _orderAppService.SplitOrderAsync(orderItemIds, Order.Id);
        NavigationManager.NavigateTo("Orders");

    }
    async void SubmitOrderItemChanges()
    {
        bool isValid = true;

        EditingItems.ForEach(item =>
        {
            item.IsItemPriceError = false;
            item.IsQuantiyError = false;

            if(item.Quantity < 1)
            {
                item.IsQuantiyError = true;
                isValid = false;
            }
            if (item.ItemPrice < 1)
            {
                item.IsItemPriceError = true;
                isValid = false;
            }
        });

        if (!isValid) return;

        await loading.Show();
        await _orderAppService.UpdateOrderItemsAsync(OrderId, EditingItems);
        CancelOrderItemChanges();
        await GetOrderDetailsAsync();
        await loading.Hide();
        await InvokeAsync(StateHasChanged);
    }

    void CancelOrderItemChanges()
    {
        EditingItems = new();
        IsItemsEditMode = false;
        Order.OrderItems.ForEach(item =>
        {
            item.IsSelected = false;
        });
    }

    public void CheckboxChanged(bool e, OrderItemDto item)
    {
        item.IsSelected = e;

        if (Order.ShippingStatus is ShippingStatus.WaitingForPayment ||
            Order.ShippingStatus is ShippingStatus.PrepareShipment ||
            Order.ShippingStatus is ShippingStatus.ToBeShipped)
        {
            if (Order.OrderItems.Count == Order?.OrderItems.Count(c => c.IsSelected))
            {
                refunds.IsRefundItems = false;

                refunds.IsRefundAmount = false;

                refunds.IsRefundOrder = true;

                foreach (OrderItemDto orderItem in Order.OrderItems)
                {
                    orderItem.IsSelected = false;
                }
            }
        }

        StateHasChanged();
    }

    async void ToggleEditMode()
    {
        EditingItems = new();
        var selectedItems = Order.OrderItems.Where(x => x.IsSelected).ToList();
        if(selectedItems.Count > 0)
        {
            selectedItems.ForEach(item =>
            {
                EditingItems.Add(new UpdateOrderItemDto
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    ItemPrice = item.ItemPrice,
                    TotalAmount = item.TotalAmount
                });
            });
            IsItemsEditMode = true;
            await InvokeAsync(StateHasChanged);
        }
    }

    async void ReturnOrder()
    {

        await _orderAppService.ReturnOrderAsync(Order.Id);
        NavigationManager.NavigateTo("Orders");

    }
    async void ExchangeOrder()
    {

        await _orderAppService.ExchangeOrderAsync(Order.Id);
        NavigationManager.NavigateTo("Orders");

    }
    void CalculateTotal(UpdateOrderItemDto item)
    {
        var index = EditingItems.IndexOf(item);
        EditingItems[index].TotalAmount = item.Quantity * item.ItemPrice;
    }

    async Task ApplyRefund()
    {
        try
        {
            bool confimation = await _uiMessageService.Confirm(L["AreYouSureToRefundThisOrder?"]);
          
            if (confimation)
            {
                await loading.Show();
                
                await _refundAppService.CreateAsync(OrderId);
                
                await GetOrderDetailsAsync();
            }
        }
        catch(BusinessException ex)
        {
            await _uiMessageService.Error(L[ex.Code]);
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());

            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await loading.Hide();
        }
    }

    private void OpenRefundModal()
    {
        refunds = new RefundOrder
        {
            IsRefundOrder = true,
        };

        RefundModal.Show();
    }
    private string UpdateAttributes(string htmlString,string orderId,string deliveryId)
    {
        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(htmlString);

        // Update href attributes
        foreach (var link in doc.DocumentNode.SelectNodes("//a[@href]"))
        {
            link.Attributes["href"].Value = UpdateHref(link.Attributes["href"].Value);
        }

        // Update src attributes
        foreach (var element in doc.DocumentNode.SelectNodes("//img[@src] | //script[@src]"))
        {
            element.Attributes["src"].Value = UpdateSrc(element.Attributes["src"].Value);
          
        }
        htmlString = doc.DocumentNode.OuterHtml;
        // Get the updated HTML string
        htmlString = AddNewInputsToForm(htmlString,orderId,deliveryId);

        // Change the form action
        htmlString = UpdateButtonOnclick(htmlString);
        htmlString = ReplaceSaveSubmit(htmlString);

        return htmlString;
    }

    private string UpdateHref(string originalHref)
    {
        // Implement your logic to update href attribute
        return "https://logistics-stage.ecpay.com.tw"+originalHref; // Modify this line based on your requirements
    }

    private string UpdateSrc(string originalSrc)
    {
        // Implement your logic to update src attribute
        return "https://logistics-stage.ecpay.com.tw" +originalSrc; // Modify this line based on your requirements
    }
    private string AddNewInputsToForm(string html, string orderId, string deliveryId)
    {
        // Add new input fields after the existing form content
        string newInputs = "<input id='deliveryId' type='hidden'  name='deliveryId' value='" + deliveryId + "' />";
                newInputs=newInputs+ "<input id='orderId' type='hidden'  name='orderId' value='"+orderId+"' />";

        html = html.Replace("</form>", $"{newInputs}</form>");
        return html;
    }

    private string UpdateButtonOnclick(string html)
    {
        // Update the onclick method for the submit button with ID "submitButton"
        string newOnclick = "SaveSubmitNew('/api/app/store-logistics-order/store-logistics-order');";
        html = System.Text.RegularExpressions.Regex.Replace(html, @"<input[^>]*type\s*=\s*[""']button[""'][^>]*onclick\s*=\s*[""']([^""']*)[""'][^>]*>", match =>
        {
            string originalInput = match.Groups[0].Value;
            string modifiedInput = System.Text.RegularExpressions.Regex.Replace(originalInput, @"onclick\s*=\s*[""'][^""']*[""']", $"onclick=\"{newOnclick}\"");
            return modifiedInput;
        });


        return html;
    }
    private string ReplaceSaveSubmit(string html)
    {

        string newFunction = @"function SaveSubmitNew(url) {
                                debugger;
                                var formData = new FormData(document.getElementById('SubmitForm'));
                                var formDataObject = {};
                                formData.forEach(function(value, key){
                                    formDataObject[key] = value;
                                             });

                                // Convert the plain JavaScript object to a JSON string
                                        var formDataJson = JSON.stringify(formDataObject);
                                        const myHeaders = new Headers();
                                        myHeaders.append('Content-Type', 'application/json');

                                        const raw = formDataJson;

                                    const requestOptions = {
                                                        method: 'POST',
                                                        headers: myHeaders,
                                                        body: raw,
                                                        redirect: 'follow'
                                                            };

                                                    fetch(url, requestOptions)
                                                     .then((response) => {
                                                          if (response.ok) {
                                                       // If response is successful, close the popup window
                                                            window.close();
                                                             return response.text();
                                                              } else {
                                                             throw new Error('Network response was not ok.');
                                                                }
                                                                })
                                                    .then((result) => console.log(result))
                                                    .catch((error) => console.error(error));

                                        };
            </script>";

        // Find the script tag and add the new method inside it
        html = System.Text.RegularExpressions.Regex.Replace(html, @"function SaveSubmit\(url\)[\s\S]*?}\s*<\/script>", newFunction);

        return html;

        

    }

    private async Task ShippingStatusChanged(ShippingStatus selectedValue)
    {
        try
        {
            await loading.Show();

            if (selectedValue is ShippingStatus.PrepareShipment)
            {
                PaymentResult paymentResult = new();
                paymentResult.OrderId = Order.Id;
                var msg= await _orderAppService.HandlePaymentAsync(paymentResult);
                if (!msg.IsNullOrWhiteSpace())
                {
					await _uiMessageService.Error(msg);
				}
                await GetOrderDetailsAsync();
                await OnInitializedAsync();
                StateHasChanged();
                await loading.Hide();

            }
            else if (selectedValue is ShippingStatus.ToBeShipped)
            {

                var result= await _orderAppService.OrderToBeShipped(Order.Id);
				if (!result.InvoiceMsg.IsNullOrWhiteSpace())
				{
					await _uiMessageService.Error(result.InvoiceMsg);
				}
				await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
                await loading.Hide();

            }
            else if (selectedValue is ShippingStatus.Shipped)
            {
                var result= await _orderAppService.OrderShipped(Order.Id);
				if (!result.InvoiceMsg.IsNullOrWhiteSpace())
				{
					await _uiMessageService.Error(result.InvoiceMsg);
				}
				await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
                await loading.Hide();

            }
            else if (selectedValue is ShippingStatus.Completed)
            {
				var result = await _orderAppService.OrderComplete(Order.Id);
				if (!result.InvoiceMsg.IsNullOrWhiteSpace())
				{
					await _uiMessageService.Error(result.InvoiceMsg);
				}
				await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
                await loading.Hide();
            }
            else if (selectedValue is ShippingStatus.Closed)
            {
                await _orderAppService.OrderClosed(Order.Id);
                await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
                await loading.Hide();
            }
            else {
				var result = await _orderAppService.ChangeOrderStatus(Order.Id,selectedValue);
				if (!result.InvoiceMsg.IsNullOrWhiteSpace())
				{
					await _uiMessageService.Error(result.InvoiceMsg);
				}
				await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
                await loading.Hide();

            }
            
            await loading.Hide();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.Message.ToString());
        }
        finally
        {
            await loading.Hide();
        }
    }
    public class StoreCommentsModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string Comment { get; set; }
    }
    #endregion
}
public class ModificationTrack
{
    public bool IsModified { get; set; }
    public bool IsNameInputVisible { get; set; }
    public string NewName { get; set; }
    public bool IsInvalidName { get; set; }
    public bool IsNameModified { get; set; }
    public bool IsPhoneInputVisible { get; set; }
    public string NewPhone { get; set; }
    public bool IsInvalidPhone { get; set; }
    public bool IsPhoneModified { get; set; }
    public bool IsAddressInputVisible { get; set; }
    public bool IsPostalCodeInputVisible { get; set; }
    public string NewRoad { get; set; }
    public string NewDistrict { get; set; }
    public bool IsCityInputVisible { get; set; }
    public string NewCity { get; set; }
    public bool IsInvalidCity { get; set; }
    public string NewAddress { get; set; }
    public string NewPostalCode { get; set; }
    public bool IsInvalidAddress { get; set; }
    public bool IsInvalidPostalCode { get; set; }
    public bool IsAddressModified { get; set; }
    public bool IsPostalCodeModified { get; set; }
    public bool IsCityModified { get; set; }
    public bool IsCVSStoreIdModified { get; set; }
    public string NewCVSStoreId { get; set; }
    public bool IsCVSStoreIdInputVisible { get; set; }
    public bool IsInvalidCVSStoreId { get; set; }
    public bool IsCVSStoreOutsideModified { get; set; }
    public string NewCVSStoreOutside { get; set; }
    public bool IsCVSStoreOutsideInputVisible { get; set; }
    public bool IsInvalidCVSStoreOutside { get; set; }
    public bool IsRecipientNameDbsNormalInputVisible { get; set; }
    public bool IsRecipientNameDbsFreezeInputVisible { get; set; }
    public bool IsRecipientNameDbsFrozenInputVisible { get; set; }
    public bool IsRecipientPhoneDbsNormalInputVisible { get; set; }
    public bool IsRecipientPhoneDbsFreezeInputVisible { get; set; }
    public bool IsRecipientPhoneDbsFrozenInputVisible { get; set; }
    public bool IsStoreIdNormalInputVisible { get; set; }
    public bool IsStoreIdFreezeInputVisible { get; set; }
    public bool IsStoreIdFrozenInputVisible { get; set; }
    public bool IsCVSStoreOutSideNormalInputVisible { get; set; }
    public bool IsCVSStoreOutSideFreezeInputVisible { get; set; }
    public bool IsCVSStoreOutSideFrozenInputVisible { get; set; }
    public bool IsPostalCodeDbsNormalInputVisible { get; set; }
    public bool IsPostalCodeDbsFreezeInputVisible { get; set; }
    public bool IsPostalCodeDbsFrozenInputVisible { get; set; }
    public bool IsCityDbsNormalInputVisible { get; set; }
    public bool IsCityDbsFreezeInputVisible { get; set; }
    public bool IsCityDbsFrozenInputVisible { get; set; }
    public bool IsAddressDetailsDbsNormalInputVisible { get; set; }
    public bool IsAddressDetailsDbsFreezeInputVisible { get; set; }
    public bool IsAddressDetailsDbsFrozenInputVisible { get; set; }
    public bool IsInvalidRecipientNameDbsNormal { get; set; }
    public bool IsInvalidRecipientNameDbsFreeze { get; set; }
    public bool IsInvalidRecipientNameDbsFrozen { get; set; }
    public bool IsInvalidRecipientPhoneDbsNormal { get; set; }
    public bool IsInvalidRecipientPhoneDbsFreeze { get; set; }
    public bool IsInvalidRecipientPhoneDbsFrozen { get; set; }
    public bool IsInvalidStoreIdNormal { get; set; }
    public bool IsInvalidStoreIdFreeze { get; set; }
    public bool IsInvalidStoreIdFrozen { get; set; }
    public bool IsInvalidCVSStoreOutSideNormal { get; set; }
    public bool IsInvalidCVSStoreOutSideFreeze { get; set; }
    public bool IsInvalidCVSStoreOutSideFrozen { get; set; }
    public bool IsInvalidPostalCodeDbsNormal { get; set; }
    public bool IsInvalidPostalCodeDbsFreeze { get; set; }
    public bool IsInvalidPostalCodeDbsFrozen { get; set; }
    public bool IsInvalidCityDbsNormal { get; set; }
    public bool IsInvalidCityDbsFreeze { get; set; }
    public bool IsInvalidCityDbsFrozen { get; set; }
    public bool IsInvalidAddressDetailsDbsNormal { get; set; }
    public bool IsInvalidAddressDetailsDbsFreeze { get; set; }
    public bool IsInvalidAddressDetailsDbsFrozen { get; set; }
    public string NewRecipientNameDbsNormal { get; set; }
    public string NewRecipientNameDbsFreeze { get; set; }
    public string NewRecipientNameDbsFrozen { get; set; }
    public string NewRecipientPhoneDbsNormal { get; set; }
    public string NewRecipientPhoneDbsFreeze { get; set; }
    public string NewRecipientPhoneDbsFrozen { get; set; }
    public string NewStoreIdNormal { get; set; }
    public string NewStoreIdFreeze { get; set; }
    public string NewStoreIdFrozen { get; set; }
    public string NewCVSStoreOutSideNormal { get; set; }
    public string NewCVSStoreOutSideFreeze { get; set; }
    public string NewCVSStoreOutSideFrozen { get; set; }
    public string NewPostalCodeDbsNormal { get; set; }
    public string NewPostalCodeDbsFreeze { get; set; }
    public string NewPostalCodeDbsFrozen { get; set; }
    public string NewCityDbsNormal { get; set; }
    public string NewCityDbsFreeze { get; set; }
    public string NewCityDbsFrozen { get; set; }
    public string NewAddressDetailsDbsNormal { get; set; }
    public string NewAddressDetailsDbsFreeze { get; set; }
    public string NewAddressDetailsDbsFrozen { get; set; }
    public bool IsRecipientNameDbsNormalModified { get; set; }
    public bool IsRecipientNameDbsFreezeModified { get; set; }
    public bool IsRecipientNameDbsFrozenModified { get; set; }
    public bool IsRecipientPhoneDbsNormalModified { get; set; }
    public bool IsRecipientPhoneDbsFreezeModified { get; set; }
    public bool IsRecipientPhoneDbsFrozenModified { get; set; }
    public bool IsStoreIdNormalModified { get; set; }
    public bool IsStoreIdFreezeModified { get; set; }
    public bool IsStoreIdFrozenModified { get; set; }
    public bool IsCVSStoreOutSideNormalModified { get; set; }
    public bool IsCVSStoreOutSideFreezeModified { get; set; }
    public bool IsCVSStoreOutSideFrozenModified { get; set; }
    public bool IsPostalCodeDbsNormalModified { get; set; }
    public bool IsPostalCodeDbsFreezeModified { get; set; }
    public bool IsPostalCodeDbsFrozenModified { get; set; }
    public bool IsCityDbsNormalModified { get; set; }
    public bool IsCityDbsFreezeModified { get; set; }
    public bool IsCityDbsFrozenModified { get; set; }
    public bool IsAddressDetailsDbsNormalModified { get; set; }
    public bool IsAddressDetailsDbsFreezeModified { get; set; }
    public bool IsAddressDetailsDbsFrozenModified { get; set; }
}
public class Shipments
{
    [Required]
    public DeliveryMethod ShippingMethod { get; set; }

   
    public string? ShippingNumber { get; set; }
}
public class RefundOrder
{
    
    public bool IsRefundOrder { get; set; }


    public bool IsRefundItems { get; set; }
    public bool IsRefundAmount { get; set; }
    public Guid? OrderId { get; set; }
    public List<Guid> OrderItemIds { get; set; }
    public double Amount { get; set; }
}

public class PaymentStatus
{
    public string? RtnMsg { get; set; }
    public RtnValue? RtnValue { get; set; }
}

public class RtnValue
{
    public string? TradeID { get; set; }
    public string? amount { get; set; }
    public string? clsamt { get; set; }
    public string? authtime { get; set; }
    public string? status { get; set; }
    public CloseData[]? close_data { get; set; }
}

public class CloseData
{
    public string? status { get; set; }
    public string? sno { get; set; }
    public string? amount { get; set; }
    public string? datetime { get; set; }
}
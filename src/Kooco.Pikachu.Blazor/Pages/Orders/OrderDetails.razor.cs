using Blazorise;
using Blazorise.DataGrid;
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
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Kooco.Pikachu.Assembly;
using DinkToPdf;
using HtmlAgilityPack;
using Kooco.Pikachu.Orders.Interfaces;

namespace Kooco.Pikachu.Blazor.Pages.Orders;

public partial class OrderDetails
{
    #region Inject
    [Parameter]
    public string id { get; set; }
    private static readonly SynchronizedConverter Converter = new SynchronizedConverter(new PdfTools());
    private Guid OrderId { get; set; }
    private CreateUpdateOrderMessageDto StoreCustomerService { get; set; } = new();
    private OrderDto Order { get; set; }
    private decimal? OrderDeliveryCost { get; set; } = 0.00m;
    private CreateOrderDto UpdateOrder { get; set; } = new();
    private StoreCommentsModel StoreComments = new();
    private ModificationTrack ModificationTrack = new();
    private Shipments shipments = new();
    private RefundOrder refunds = new();
    private List<OrderHistoryDto> OrderHistory { get; set; } = new();
    private List<UpdateOrderItemDto> EditingItems { get; set; } = new();
    private IReadOnlyList<OrderMessageDto> CustomerServiceHistory { get; set; } = [];
    private Modal CreateShipmentModal { get; set; }
    private Modal RefundModal { get; set; }
    private bool loading { get; set; } = true;
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
    private bool IsConversationWindowCollapsed { get; set; } = false;
    private string NewMessage { get; set; } = "";
    private List<ConversationMessage> ConversationMessages { get; set; } = new List<ConversationMessage>();

    private string? editingMessageId { get; set; }
    private string editingMessageText { get; set; } = "";

    private void ToggleConversationWindow()
    {
        IsConversationWindowCollapsed = !IsConversationWindowCollapsed;
    }
     async void NavigateToList()
    {
        await JSRuntime.InvokeVoidAsync("historyBack");
    }
    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(NewMessage))
            return;

        try
        {
            loading = true;

            StoreCustomerService.Message = NewMessage;
            StoreCustomerService.OrderId = Order.Id;
            StoreCustomerService.IsMerchant = true;
            StoreCustomerService.Timestamp = DateTime.Now;

            await _OrderMessageAppService.CreateAsync(StoreCustomerService);

            StoreCustomerService = new();
            CustomerServiceHistory = await _OrderMessageAppService.GetOrderMessagesAsync(Order.Id);

            NewMessage = "";
            loading = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            loading = false;
            await HandleErrorAsync(ex);
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Convert CustomerServiceHistory to ConversationMessages
        ConversationMessages = CustomerServiceHistory?.Select(m => new ConversationMessage
        {
            Text = m.Message,
            IsFromRepresentative = m.IsMerchant,
            Timestamp = m.Timestamp,
            Id = m.Id.ToString()
        })?.ToList() ?? new List<ConversationMessage>();
    }

    public class ConversationMessage
    {
        public string Text { get; set; }
        public bool IsFromRepresentative { get; set; }
        public DateTime? Timestamp { get; set; }
        public string Id { get; set; }
    }
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
    string selectedStep = "step1";
    private Dictionary<string, int> stepOrder = new()
    {
        { "step1", 1 },
        { "step2", 2 },
        { "step3", 3 },
        { "step4", 4 },
        { "step5", 5 },
        { "step6", 6 }
    };

    private void UpdateStepByShippingStatus(ShippingStatus status)
    {
        selectedStep = status switch
        {
            ShippingStatus.WaitingForPayment or ShippingStatus.EnterpricePurchase => "step1",
            ShippingStatus.PrepareShipment => "step2",
            ShippingStatus.ToBeShipped => "step3",
            ShippingStatus.Shipped => "step4",
            ShippingStatus.Delivered or ShippingStatus.Return or ShippingStatus.Completed or ShippingStatus.Exchange => "step5",
            ShippingStatus.Closed => "step6",
            _ => "step1" // Default to first step if status is unknown
        };
    }
    protected async override Task OnAfterRenderAsync(bool isFirstRender)
    {
        if (isFirstRender)
        {
            try
            {
                loading = true;
                OrderId = Guid.Parse(id);
                await GetOrderDetailsAsync();

                await base.OnInitializedAsync();
                loading = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                loading = false;
                await HandleErrorAsync(ex);
            }
        }
    }

    public void ChangeStore(ChangeEventArgs e)
    {
        IsShowConvenienceStoreDetails = e.Value is not null && e.Value.ToString() is "convenienceStore" ? true : false;
    }
    private string FormatLocalizedDetails(OrderHistoryDto history)
    {
        try
        {
            if (string.IsNullOrEmpty(history.ActionDetails))
            {
                return L[history.ActionType]; // No details, just return action type
            }

            var parameters = JsonConvert.DeserializeObject<object[]>(history.ActionDetails);
            if (parameters == null || parameters.Length == 0)
            {
                return L[history.ActionType]; // Avoid passing empty params
            }

            // Localize parameters if they are string keys that require translation
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is string paramKey)
                {
                    parameters[i] = L[paramKey]; // Localizing the parameter if it's a string
                }
            }
            return L[history.ActionType, parameters]; // Localize properly
        }
        catch (Exception e)
        {
            return history.ActionDetails; // Debugging purpose
        }
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
    private bool NavigationAllowed(StepNavigationContext context)
    {
        // Determine the expected step based on the current order status
        string expectedStep = GetStepByShippingStatus(Order.ShippingStatus);

        // Allow navigation ONLY if the new step matches the expected step
        return context.NextStepName == expectedStep;
    }

    // 🚀 Helper method to get step name based on `ShippingStatus`
    private string GetStepByShippingStatus(ShippingStatus status)
    {
        return status switch
        {
            ShippingStatus.WaitingForPayment or ShippingStatus.EnterpricePurchase => "step1",
            ShippingStatus.PrepareShipment => "step2",
            ShippingStatus.ToBeShipped => "step3",
            ShippingStatus.Shipped => "step4",
            ShippingStatus.Delivered or ShippingStatus.Completed or ShippingStatus.Return or ShippingStatus.Exchange => "step5",
            ShippingStatus.Closed => "step6",
            _ => "step1"
        };
    }

    async Task GetOrderDetailsAsync()
    {
        Order = await _orderAppService.GetWithDetailsAsync(OrderId) ?? new();
        UpdateStepByShippingStatus(Order.ShippingStatus);
        OrderHistory = await _orderAppService.GetOrderLogsAsync(OrderId);
        if (Order.DeliveryMethod is not DeliveryMethod.SelfPickup &&
            Order.DeliveryMethod is not DeliveryMethod.DeliveredByStore)
            OrderDeliveryCost = Order.DeliveryCost;

        OrderDeliveries = await _orderDeliveryAppService.GetListByOrderAsync(OrderId);

        OrderDeliveries = [.. OrderDeliveries.Where(w => w.Items.Count > 0)];

        PaymentStatus = await GetPaymentStatus();

        CustomerServiceHistory = await _OrderMessageAppService.GetOrderMessagesAsync(Order.Id);

        await InvokeAsync(StateHasChanged);
    }

    bool IsStepActive(string step)
    {
        return stepOrder[step] <= stepOrder[selectedStep];
    }

    async Task SubmitStoreCommentsAsync()
    {
        try
        {
            loading = true;
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
            loading = false;
        }
        catch (Exception ex)
        {
            loading = false;
            await HandleErrorAsync(ex);
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
            loading = true;
            string comment = StoreCustomerService.Message;
            if (comment.IsNullOrWhiteSpace())
            {
                return;
            }

            StoreCustomerService.OrderId = Order.Id;
            StoreCustomerService.IsMerchant = true;
            StoreCustomerService.Timestamp = DateTime.Now;

            await _OrderMessageAppService.CreateAsync(StoreCustomerService);

            StoreCustomerService = new();
            CustomerServiceHistory = await _OrderMessageAppService.GetOrderMessagesAsync(Order.Id);

            loading = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            loading = false;
            await HandleErrorAsync(ex);
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

            loading = true;
            UpdateOrder.OrderStatus = Order.OrderStatus;
            UpdateOrder.ShouldSendEmail = true;

            Order = await _orderAppService.UpdateAsync(OrderId, UpdateOrder);
            ModificationTrack = new();
            await InvokeAsync(StateHasChanged);
            loading = false;
        }
        catch (Exception ex)
        {
            loading = false;
            await HandleErrorAsync(ex);
        }

    }

    public void NavigateToOrderShipmentDetails()
    {
        List<Guid?> orderId = new() { { Order?.Id } };

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
                    loading = true;
                    await _orderAppService.CancelOrderAsync(OrderId);
                    await GetOrderDetailsAsync();
                    await InvokeAsync(StateHasChanged);
                    loading = false;
                }
            }
        }
        catch (Exception ex)
        {
            loading = false;
            await HandleErrorAsync(ex);
        }
    }

    private void OpenShipmentModal(OrderDeliveryDto deliveryOrder)
    {
        OrderDeliveryId = deliveryOrder.Id;
        shipments = new Shipments
        {
            ShippingMethod = deliveryOrder?.DeliveryMethod ?? DeliveryMethod.PostOffice,
            ShippingNumber = deliveryOrder.DeliveryNo
        };

        CreateShipmentModal.Show();
    }
    private async void OrderItemShipped(OrderDeliveryDto deliveryOrder)
    {
        loading = true;
        OrderDeliveryId = deliveryOrder.Id;
        await _orderDeliveryAppService.UpdateOrderDeliveryStatus(OrderDeliveryId);
        await GetOrderDetailsAsync();
        await InvokeAsync(StateHasChanged);
        loading = false;

    }
    private async void TestLabel(OrderDeliveryDto deliveryOrder)
    {
        if (deliveryOrder.DeliveryMethod == DeliveryMethod.SevenToEleven1 || deliveryOrder.DeliveryMethod == DeliveryMethod.FamilyMart1)
        {
            loading = true;

            var result = await _testLableAppService.TestLableAsync(deliveryOrder.DeliveryMethod == DeliveryMethod.SevenToEleven1 ? "UNIMART" : "FAMI");
            await InvokeAsync(StateHasChanged);
            loading = false;
        }

    }
    private async void CreateOrderLogistics(OrderDeliveryDto deliveryOrder)
    {
        loading = true;

        try
        {
            OrderDeliveryId = deliveryOrder.Id;

            if (deliveryOrder.DeliveryMethod is DeliveryMethod.SevenToEleven1 ||
                deliveryOrder.DeliveryMethod is DeliveryMethod.FamilyMart1 ||
                deliveryOrder.DeliveryMethod is DeliveryMethod.SevenToElevenC2C ||
                deliveryOrder.DeliveryMethod is DeliveryMethod.FamilyMartC2C)
            {
                ResponseResultDto result = await _storeLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(Order.Id, deliveryOrder.Id);

                if (result.ResponseCode is not "1")
                {
                    await _uiMessageService.Error(result.ResponseMessage);
                }
                else if (result.ResponseCode is "1")
                {
                    await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                }

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

                if (response is null || response.Data is null)
                {
                    await _uiMessageService.Error(response.Message);
                }
                else if (response.Data is not null)
                {
                    await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                }
            }

            else if (deliveryOrder.DeliveryMethod is DeliveryMethod.TCatDeliverySevenElevenNormal ||
                     deliveryOrder.DeliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                     deliveryOrder.DeliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFrozen)
            {
                PrintOBTB2SResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(Order.Id, deliveryOrder.Id);

                if (response is null || response.Data is null)
                {
                    await _uiMessageService.Error(response.Message);
                }
                else if (response.Data is not null)
                {
                    await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                }
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

                        if (result.ResponseCode is not "1")
                        {
                            await _uiMessageService.Error(result.ResponseMessage);
                        }
                        else if (result.ResponseCode is "1")
                        {
                            await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                        }
                    }

                    else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is DeliveryMethod.PostOffice ||
                             logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is DeliveryMethod.BlackCat1)
                    {
                        ResponseResultDto result = await _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(Order.Id, OrderDeliveryId, deliveryMethod);

                        if (result.ResponseCode is not "1")
                        {
                            await _uiMessageService.Error(result.ResponseMessage);
                            loading = false;
                        }
                        else if (result.ResponseCode is "1")
                        {
                            await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                        }
                    }

                    else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliveryNormal)
                    {
                        PrintObtResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                        if (response is null || response.Data is null)
                        {
                            await _uiMessageService.Error(response.Message);
                        }
                        else if (response.Data is not null)
                        {
                            await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                        }
                    }

                    else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenNormal)
                    {
                        PrintOBTB2SResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                        if (response is null || response.Data is null)
                        {
                            await _uiMessageService.Error(response.Message);
                        }
                        else if (response.Data is not null)
                        {
                            await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                        }
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
                            loading = false;
                        }
                        else if (result.ResponseCode is "1")
                        {
                            await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                        }
                    }

                    else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliveryFreeze)
                    {
                        PrintObtResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                        if (response is null || response.Data is null)
                        {
                            await _uiMessageService.Error(response.Message);
                        }
                        else if (response.Data is not null)
                        {
                            await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                        }
                    }

                    else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFreeze)
                    {
                        PrintOBTB2SResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                        if (response is null || response.Data is null)
                        {
                            await _uiMessageService.Error(response.Message);
                        }
                        else if (response.Data is not null)
                        {
                            await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                        }
                    }
                }

                else if (temperature is ItemStorageTemperature.Frozen)
                {
                    if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliveryFrozen)
                    {
                        PrintObtResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                        if (response is null || response.Data is null)
                        {
                            await _uiMessageService.Error(response.Message);
                        }
                        else if (response.Data is not null)
                        {
                            await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                        }
                    }

                    else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFrozen)
                    {
                        PrintOBTB2SResponse? response = await _storeLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                        if (response is null || response.Data is null)

                        { await _uiMessageService.Error(response.Message); }
                        else if (response.Data is not null)
                        {
                            await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                        }
                    }

                    else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is DeliveryMethod.BlackCatFrozen)
                    {
                        ResponseResultDto result = await _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(Order.Id, OrderDeliveryId, deliveryMethod);

                        if (result.ResponseCode is not "1")
                        {
                            await _uiMessageService.Error(result.ResponseMessage);
                            loading = false;
                        }
                        else if (result.ResponseCode is "1")
                        {
                            await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                        }
                    }

                    else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is DeliveryMethod.SevenToElevenFrozen)
                    {
                        ResponseResultDto result = await _storeLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(Order.Id, deliveryOrder.Id, deliveryMethod);

                        if (result.ResponseCode is not "1")
                        {
                            await _uiMessageService.Error(result.ResponseMessage);
                        }
                        else if (result.ResponseCode is "1")
                        {
                            await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                        }
                    }
                }
            }

            else if (deliveryOrder.DeliveryMethod is EnumValues.DeliveryMethod.SelfPickup ||
                           deliveryOrder.DeliveryMethod is EnumValues.DeliveryMethod.HomeDelivery)
            {
                await _storeLogisticsOrderAppService.GenerateDeliveryNumberForSelfPickupAndHomeDeliveryAsync(Order.Id, deliveryOrder.Id);
                await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
            }

            else
            {
                ResponseResultDto result = await _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(Order.Id, OrderDeliveryId);

                if (result.ResponseCode is not "1")
                {
                    await _uiMessageService.Error(result.ResponseMessage);
                    loading = false;
                }
                else if (result.ResponseCode is "1")
                {
                    await _storeLogisticsOrderAppService.IssueInvoiceAync(Order.Id);
                }
            }

            await GetOrderDetailsAsync();
            loading = false;
            await InvokeAsync(StateHasChanged);

        }
        catch (Exception e)
        {
            await GetOrderDetailsAsync();
            loading = false;
            await InvokeAsync(StateHasChanged);


        }
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

            PaymentStatus? paymentStatus = System.Text.Json.JsonSerializer.Deserialize<PaymentStatus>(response.Content);

            if (paymentStatus is null || paymentStatus.RtnValue?.status is null) return string.Empty;

            return paymentStatus.RtnValue?.status;
        }

        if (Order.PaymentMethod is PaymentMethods.BankTransfer ||
            Order.PaymentMethod is PaymentMethods.CashOnDelivery) return L["Paid"];

        return string.Empty;
    }

    public string GenerateCheckMac(string HashKey, string HashIV, string merchantID, int gwsr, string totalAmount, string creditCheckCode)
    {
        Dictionary<string, string> parameters = new()
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
                loading = true;

                List<Guid>? orderItemIds = [.. Order?.OrderItems.Where(x => x.IsSelected).Select(x => x.Id)];

                if (orderItemIds.Count < 1)
                {
                    await _uiMessageService.Error("Please Select Order Item");

                    loading = false;

                    return;
                }

                if (Order.OrderItems.Count == Order?.OrderItems.Count(c => c.IsSelected))
                {
                    loading = false;

                    await ApplyRefund();

                    await RefundModal.Hide();

                    return;
                }

                refunds.OrderItemIds = orderItemIds;

                await _orderAppService.RefundOrderItems(orderItemIds, OrderId);

                loading = false;

                await RefundModal.Hide();
            }
            else
            {
                loading = true;

                if (refunds.Amount is 0)
                {
                    await _uiMessageService.Error("Please Enter Amount");

                    loading = false;

                    return;
                }

                if (refunds.Amount > (double)Order.TotalAmount)
                {
                    await _uiMessageService.Error("Enter amount is greater than order amount");

                    loading = false;

                    return;
                }

                await _orderAppService.RefundAmountAsync(refunds.Amount, OrderId);

                loading = false;

                await RefundModal.Hide();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
            loading = false;
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
            loading = true;

            UpdateOrder.ShippingNumber = shipments.ShippingNumber;

            UpdateOrder.DeliveryMethod = shipments.ShippingMethod;

            await _orderDeliveryAppService.UpdateShippingDetails(OrderDeliveryId, UpdateOrder);

            await CreateShipmentModal.Hide();

            await GetOrderDetailsAsync();

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            loading = false;
        }
    }
    private async void SplitOrder()
    {
        var orderItemIds = Order?.OrderItems.Where(x => x.IsSelected).Select(x => x.Id).ToList();
        if (orderItemIds.Count == Order?.OrderItems.Count)
        {
            await _uiMessageService.Error(L["Youcannotsplittheorder"]);
            return;

        }
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

            if (item.Quantity < 1)
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

        loading = true;
        await _orderAppService.UpdateOrderItemsAsync(OrderId, EditingItems);
        CancelOrderItemChanges();
        await GetOrderDetailsAsync();
        loading = false;
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
        if (selectedItems.Count > 0)
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
        else
        {
            await _uiMessageService.Info(L["PleaseSelectOrderItem"]);

        }
    }

    async void ReturnOrder()
    {
        var confirmed = await _uiMessageService.Confirm(L["Areyousureyouwanttoreturnthisorder"]);
        if (!confirmed)
        {
            return;
        }
        await _orderAppService.ReturnOrderAsync(Order.Id);
        NavigationManager.NavigateTo("Orders");
        await JSRuntime.InvokeVoidAsync("reloadOrderPage");



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
                loading = true;

                await _refundAppService.CreateAsync(OrderId);

                await GetOrderDetailsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            loading = false;
        }
    }
    public async Task OnPrintShippedLabel(MouseEventArgs e)
    {
        loading = true;

        List<Guid> orderIds = new List<Guid>();
        orderIds.Add(Order.Id);

        Dictionary<string, string> AllPayLogisticsIds = new()
        {
            { "BlackCat1", string.Empty },
            { "BlackCatFrozen", string.Empty },
            { "BlackCatFreeze", string.Empty },
            { "SevenToElevenFrozen", string.Empty },
            { "PostOffice", string.Empty },
            { "FamilyMart1", string.Empty },
            { "SevenToEleven1", string.Empty },
            { "SevenToElevenC2C", string.Empty },
            { "FamilyMartC2C", string.Empty },
            { "TCatDeliveryNormal", string.Empty },
            { "TCatDeliveryFreeze", string.Empty },
            { "TCatDeliveryFrozen", string.Empty },
            { "TCatDeliverySevenElevenNormal", string.Empty },
            { "TCatDeliverySevenElevenFreeze", string.Empty },
            { "TCatDeliverySevenElevenFrozen", string.Empty },
        };

        Dictionary<string, string> allPayLogistic = new()
        {
            { "TCatDeliverySevenElevenNormal", string.Empty },
            { "TCatDeliverySevenElevenFreeze", string.Empty },
            { "TCatDeliverySevenElevenFrozen", string.Empty }
        };

        Dictionary<string, string> DeliveryNumbers = new()
        {
            { "SevenToElevenC2C", string.Empty },
            { "FamilyMartC2C", string.Empty },
            { "TCatDeliveryNormal", string.Empty },
            { "TCatDeliveryFreeze", string.Empty },
            { "TCatDeliveryFrozen", string.Empty },
        };

        foreach (Guid orderId in orderIds)
        {
            OrderDto order = await _orderAppService.GetAsync(orderId);

            List<OrderDeliveryDto> orderDeliveries = await _orderDeliveryAppService.GetListByOrderAsync(orderId);

            foreach (OrderDeliveryDto? delivery in orderDeliveries)
            {
                if (!string.IsNullOrWhiteSpace(delivery.AllPayLogisticsID) &&
                    (!(delivery.DeliveryMethod is EnumValues.DeliveryMethod.SelfPickup ||
                     delivery.DeliveryMethod is EnumValues.DeliveryMethod.HomeDelivery ||
                     delivery.DeliveryMethod is EnumValues.DeliveryMethod.DeliveredByStore)))
                {
                    string? AllPayLogisticsIdsValue = AllPayLogisticsIds.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                    List<string> AllPayLogisticId = AllPayLogisticsIdsValue.IsNullOrEmpty() ? [] : [.. AllPayLogisticsIdsValue.Split(',')];

                    if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                    {
                        AllPayLogisticId.Add(delivery.FileNo);

                        AllPayLogisticsIds.Remove(delivery.DeliveryMethod.ToString());

                        AllPayLogisticsIds.Add(delivery.DeliveryMethod.ToString(), string.Join(",", AllPayLogisticId));

                        if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal ||
                            delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                            delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                        {
                            string? aplValues = allPayLogistic.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                            List<string> aPL = aplValues.IsNullOrEmpty() ? [] : [.. aplValues.Split(',')];

                            aPL.Add(delivery.AllPayLogisticsID);

                            allPayLogistic.Remove(delivery.DeliveryMethod.ToString());

                            allPayLogistic.Add(delivery.DeliveryMethod.ToString(), string.Join(",", aPL));
                        }
                    }

                    else
                    {
                        AllPayLogisticId.Add(delivery.AllPayLogisticsID);

                        AllPayLogisticsIds.Remove(delivery.DeliveryMethod.ToString());

                        AllPayLogisticsIds.Add(delivery.DeliveryMethod.ToString(), string.Join(",", AllPayLogisticId));
                    }

                    if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.FamilyMartC2C ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToElevenC2C ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen)
                    {
                        string? DeliveryNumberValue = DeliveryNumbers.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                        List<string> DeliveryNumber = DeliveryNumberValue.IsNullOrEmpty() ? [] : [.. DeliveryNumberValue.Split(',')];

                        DeliveryNumber.Add(delivery.DeliveryNo);

                        DeliveryNumbers.Remove(delivery.DeliveryMethod.ToString());

                        DeliveryNumbers.Add(delivery.DeliveryMethod.ToString(), string.Join(",", DeliveryNumber));
                    }
                }
            }
        }

        if (Order.DeliveryMethod is DeliveryMethod.SevenToElevenC2C)
        {
            await SevenElevenC2CShippingLabelAsync(AllPayLogisticsIds, DeliveryNumbers);

            loading = false;

            return;
        }

        string MergeTempFolder = Path.Combine(Path.GetTempPath(), "MergeTemp");

        Directory.CreateDirectory(MergeTempFolder);

        var tuple = await _storeLogisticsOrderAppService.OnBatchPrintingShippingLabel(AllPayLogisticsIds, DeliveryNumbers, allPayLogistic);

        string errors = string.Join('\n', tuple.Item3);

        if (!errors.IsNullOrWhiteSpace()) await _uiMessageService.Warn(errors);

        List<string> outputPdfPaths = GeneratePdf(tuple.Item1);

        if (tuple.Item2 is { Count: > 0 }) outputPdfPaths.AddRange(tuple.Item2);

        if (outputPdfPaths is { Count: > 0 })
        {
            MemoryStream combinedPdfStream = CombinePdf(outputPdfPaths);

            await JSRuntime.InvokeVoidAsync("downloadFile", new
            {
                ByteArray = combinedPdfStream.ToArray(),
                FileName = "ShippingLabels.pdf",
                ContentType = "application/pdf"
            });
        }

        Directory.Delete(Path.Combine(Path.GetTempPath(), "MergeTemp"), true);

        loading = false;
    }
    public async Task OnPrintShippedLabel10Cm()
    {
        loading = true;
        List<Guid> orderIds = new List<Guid>();
        orderIds.Add(Order.Id);

        Dictionary<string, string> AllPayLogisticsIds = new()
        {
            { "BlackCat1", string.Empty },
            { "BlackCatFrozen", string.Empty },
            { "BlackCatFreeze", string.Empty },
            { "SevenToElevenFrozen", string.Empty },
            { "PostOffice", string.Empty },
            { "FamilyMart1", string.Empty },
            { "SevenToEleven1", string.Empty },
            { "SevenToElevenC2C", string.Empty },
            { "FamilyMartC2C", string.Empty },
            { "TCatDeliveryNormal", string.Empty },
            { "TCatDeliveryFreeze", string.Empty },
            { "TCatDeliveryFrozen", string.Empty },
            { "TCatDeliverySevenElevenNormal", string.Empty },
            { "TCatDeliverySevenElevenFreeze", string.Empty },
            { "TCatDeliverySevenElevenFrozen", string.Empty },
        };

        Dictionary<string, string> allPayLogistic = new()
        {
            { "TCatDeliverySevenElevenNormal", string.Empty },
            { "TCatDeliverySevenElevenFreeze", string.Empty },
            { "TCatDeliverySevenElevenFrozen", string.Empty }
        };

        Dictionary<string, string> DeliveryNumbers = new()
        {
            { "SevenToElevenC2C", string.Empty },
            { "FamilyMartC2C", string.Empty },
            { "TCatDeliveryNormal", string.Empty },
            { "TCatDeliveryFreeze", string.Empty },
            { "TCatDeliveryFrozen", string.Empty },
        };

        foreach (Guid orderId in orderIds)
        {
            OrderDto order = await _orderAppService.GetAsync(orderId);

            List<OrderDeliveryDto> orderDeliveries = await _orderDeliveryAppService.GetListByOrderAsync(orderId);

            foreach (OrderDeliveryDto? delivery in orderDeliveries)
            {
                if (!string.IsNullOrWhiteSpace(delivery.AllPayLogisticsID) &&
                    (!(delivery.DeliveryMethod is EnumValues.DeliveryMethod.SelfPickup ||
                     delivery.DeliveryMethod is EnumValues.DeliveryMethod.HomeDelivery ||
                     delivery.DeliveryMethod is EnumValues.DeliveryMethod.DeliveredByStore)))
                {
                    string? AllPayLogisticsIdsValue = AllPayLogisticsIds.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                    List<string> AllPayLogisticId = AllPayLogisticsIdsValue.IsNullOrEmpty() ? [] : [.. AllPayLogisticsIdsValue.Split(',')];

                    if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                    {
                        AllPayLogisticId.Add(delivery.FileNo);

                        AllPayLogisticsIds.Remove(delivery.DeliveryMethod.ToString());

                        AllPayLogisticsIds.Add(delivery.DeliveryMethod.ToString(), string.Join(",", AllPayLogisticId));

                        if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal ||
                            delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                            delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                        {
                            string? aplValues = allPayLogistic.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                            List<string> aPL = aplValues.IsNullOrEmpty() ? [] : [.. aplValues.Split(',')];

                            aPL.Add(delivery.AllPayLogisticsID);

                            allPayLogistic.Remove(delivery.DeliveryMethod.ToString());

                            allPayLogistic.Add(delivery.DeliveryMethod.ToString(), string.Join(",", aPL));
                        }
                    }

                    else
                    {
                        AllPayLogisticId.Add(delivery.AllPayLogisticsID);

                        AllPayLogisticsIds.Remove(delivery.DeliveryMethod.ToString());

                        AllPayLogisticsIds.Add(delivery.DeliveryMethod.ToString(), string.Join(",", AllPayLogisticId));
                    }

                    if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.FamilyMartC2C ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToElevenC2C ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen)
                    {
                        string? DeliveryNumberValue = DeliveryNumbers.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                        List<string> DeliveryNumber = DeliveryNumberValue.IsNullOrEmpty() ? [] : [.. DeliveryNumberValue.Split(',')];

                        DeliveryNumber.Add(delivery.DeliveryNo);

                        DeliveryNumbers.Remove(delivery.DeliveryMethod.ToString());

                        DeliveryNumbers.Add(delivery.DeliveryMethod.ToString(), string.Join(",", DeliveryNumber));
                    }
                }
            }
        }

        if (Order.DeliveryMethod is DeliveryMethod.SevenToElevenC2C)
        {
            await SevenElevenC2CShippingLabelAsync(AllPayLogisticsIds, DeliveryNumbers);

            loading = false;

            return;
        }

        string MergeTempFolder = Path.Combine(Path.GetTempPath(), "MergeTemp");

        Directory.CreateDirectory(MergeTempFolder);

        var tuple = await _storeLogisticsOrderAppService.OnBatchPrintingShippingLabel(AllPayLogisticsIds, DeliveryNumbers, allPayLogistic);

        string errors = string.Join('\n', tuple.Item3);

        if (!errors.IsNullOrWhiteSpace()) await _uiMessageService.Warn(errors);

        List<string> outputPdfPaths = GenerateA6Pdf(tuple.Item1);

        if (tuple.Item2 is { Count: > 0 }) outputPdfPaths.AddRange(tuple.Item2);

        if (outputPdfPaths is { Count: > 0 })
        {
            MemoryStream combinedPdfStream = CombinePdf(outputPdfPaths);

            await JSRuntime.InvokeVoidAsync("downloadFile", new
            {
                ByteArray = combinedPdfStream.ToArray(),
                FileName = "ShippingLabels.pdf",
                ContentType = "application/pdf"
            });
        }

        Directory.Delete(Path.Combine(Path.GetTempPath(), "MergeTemp"), true);

        loading = false;
    }
    public List<string> GeneratePdf(List<string> htmls)
    {
        List<string> pdfFilePaths = [];

        CustomAssemblyContext customAssembly = new();

        customAssembly.LoadDinkToPdfDll();

        for (int i = 0; i < htmls.Count; i++)
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "MergeTemp");

                string htmlFilePath = Path.Combine(tempPath, $"outputHTML{i}.html");

                string pdfFilePath = Path.Combine(tempPath, $"output{i}.pdf");

                pdfFilePaths.Add(pdfFilePath);

                File.WriteAllText(htmlFilePath, htmls[i]);

                if (File.Exists(pdfFilePath)) File.Delete(pdfFilePath);

                var doc = new HtmlToPdfDocument
                {
                    GlobalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = DinkToPdf.Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Out = pdfFilePath,

                    },
                    Objects =
                    {
                        new ObjectSettings
                        {
                            Page = htmlFilePath,
                            LoadSettings = new LoadSettings { JSDelay = 5000 },
                            WebSettings = new WebSettings
                            {
                                EnableJavascript = true,
                                DefaultEncoding = "UTF-8",
                                LoadImages = true,

                            }
                        }
                    }
                };

                Converter.Convert(doc);
                Console.WriteLine($"PDF generated successfully: {pdfFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during PDF generation: {ex.Message}");
            }
        }

        return pdfFilePaths;
    }
    public List<string> GenerateA6Pdf(List<string> htmls)
    {
        List<string> pdfFilePaths = new(); // List to store generated PDF file paths
        CustomAssemblyContext customAssembly = new();
        customAssembly.LoadDinkToPdfDll(); // Load the required DLL for PDF conversion

        string tempPath = Path.GetTempPath(); // Get the system's temporary folder path

        for (int i = 0; i < htmls.Count; i++) // Loop through each HTML string provided
        {
            try
            {
                // Parse the HTML content
                HtmlDocument htmlDoc = new();
                htmlDoc.LoadHtml(htmls[i]);

                // Check if <div class="PrintToolsBlock"> exists
                var printToolsBlockDiv = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'PrintToolsBlock')]");
                if (printToolsBlockDiv != null)
                {
                    // Check if <div id="PrintBlock"> exists inside PrintToolsBlock
                    var printBlockDiv = printToolsBlockDiv.SelectSingleNode(".//div[@id='PrintBlock']");
                    if (printBlockDiv != null)
                    {
                        // Check if the number of <img> tags inside PrintBlock is more than 1
                        var imgTags = printBlockDiv.SelectNodes(".//img");
                        if (imgTags != null && imgTags.Count > 1)
                        {
                            // Retrieve the head and body nodes from the HTML document
                            var headNode = htmlDoc.DocumentNode.SelectSingleNode("//head");
                            var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                            // Store the original head content and body class
                            string originalHead = headNode?.OuterHtml ?? "<head></head>";
                            string originalBodyClass = bodyNode?.GetAttributeValue("class", string.Empty) ?? "";

                            for (int j = 0; j < imgTags.Count; j++) // Loop through each image tag
                            {
                                // Define paths for temporary HTML and PDF files
                                string htmlFilePath = Path.Combine(tempPath, $"outputHTML_{i}_{j}.html");
                                string pdfFilePath = Path.Combine(tempPath, $"output_{i}_{j}.pdf");

                                // Create new HTML content containing the image and the original structure
                                string newHtmlContent = $@"
                            <!DOCTYPE html>
                            <html>
                            {originalHead}
                            <body class='{originalBodyClass}'>
                                <div id='PrintBlock'>
                                        < div style ='position: relative; display: inline-block;'> <!-- Corrected display property -->
                                            {imgTags[j].OuterHtml}
                                    </ div >
                                </ div >
                            </ body >
                            </ html > ";

                                // Write the new HTML content to a temporary file
                                File.WriteAllText(htmlFilePath, newHtmlContent);

                                // Delete the existing PDF file if it already exists
                                if (File.Exists(pdfFilePath)) File.Delete(pdfFilePath);

                                // Create a PDF document configuration
                                var doc = new HtmlToPdfDocument
                                {
                                    GlobalSettings = new GlobalSettings
                                    {
                                        ColorMode = ColorMode.Color,
                                        Orientation = DinkToPdf.Orientation.Portrait,
                                        PaperSize = PaperKind.A6,

                                        Margins = new MarginSettings { Top = 3, Left = 0, Bottom = 5, Right = 0 },
                                        Out = pdfFilePath
                                    },
                                    Objects =
                                {
                                    new ObjectSettings
                                    {
                                        Page = htmlFilePath,
                                        LoadSettings = new LoadSettings { JSDelay = 5000 },

                                        WebSettings = new WebSettings
                                        {
                                            EnableJavascript = true,
                                            DefaultEncoding = "UTF-8",
                                            LoadImages = true
                                        }
                                    }
                                }
                                };

                                // Convert the HTML to PDF and add the PDF path to the list
                                Converter.Convert(doc);
                                pdfFilePaths.Add(pdfFilePath);
                                Console.WriteLine($"PDF generated successfully for PrintBlock image: {pdfFilePath}");
                            }
                            continue; // Skip to the next HTML input after processing images
                        }
                    }
                    else
                    {
                        var imgTags = htmlDoc.DocumentNode.SelectNodes("//img[contains(@class, 'ForwardOrderTCat imgGwLoad')]");
                        if (imgTags != null && imgTags.Count > 1) // Process only if more than 1 matching <img> tag exists
                        {
                            // Retrieve the head and body nodes
                            var headNode = htmlDoc.DocumentNode.SelectSingleNode("//head");
                            var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                            // Store the original head content and body attributes
                            string originalHead = headNode?.OuterHtml ?? "<head></head>";
                            string originalBodyClass = bodyNode?.GetAttributeValue("class", string.Empty) ?? "";

                            for (int j = 0; j < imgTags.Count; j++) // Loop through each matching <img> tag
                            {
                                // Define paths for temporary HTML and PDF files
                                string htmlFilePath = Path.Combine(tempPath, $"outputHTML_{i}_{j}.html");
                                string pdfFilePath = Path.Combine(tempPath, $"output_{i}_{j}.pdf");

                                // Create new HTML content with the current <img> tag
                                string newHtmlContent = $@"
                    <!DOCTYPE html>
                    <html>
                    {originalHead}
                    <body class='{originalBodyClass}'>
                        <div id='PrintBlock'>
                            <div style='position: relative; display: inline-block;'>
                                {imgTags[j].OuterHtml}
                            </div>
                        </div>
                    </body>
                    </html>";

                                // Write the new HTML content to a temporary file
                                File.WriteAllText(htmlFilePath, newHtmlContent);

                                // Delete the existing PDF file if it already exists
                                if (File.Exists(pdfFilePath)) File.Delete(pdfFilePath);

                                // Create a PDF document configuration
                                var doc = new HtmlToPdfDocument
                                {
                                    GlobalSettings = new GlobalSettings
                                    {
                                        ColorMode = ColorMode.Color,
                                        Orientation = DinkToPdf.Orientation.Portrait,
                                        PaperSize = PaperKind.A6,
                                        Margins = new MarginSettings { Top = 3, Left = 0, Bottom = 5, Right = 0 },
                                        Out = pdfFilePath
                                    },
                                    Objects =
                        {
                            new ObjectSettings
                            {
                                Page = htmlFilePath,
                                LoadSettings = new LoadSettings { JSDelay = 5000,ZoomFactor=1.5 },
                                WebSettings = new WebSettings
                                {
                                    EnableJavascript = true,
                                    DefaultEncoding = "UTF-8",
                                    LoadImages = true
                                }
                            }
                        }
                                };

                                // Convert the HTML to PDF and add the PDF path to the list
                                Converter.Convert(doc);
                                pdfFilePaths.Add(pdfFilePath);
                                Console.WriteLine($"PDF generated successfully for ForwardOrderTCat image: {pdfFilePath}");
                            }



                        }
                        else // Process any remaining HTML without specific blocks or tables
                        {
                            // Define paths for temporary HTML and PDF files
                            string htmlFilePath = Path.Combine(tempPath, $"outputHTML{i}.html");
                            string pdfFilePath = Path.Combine(tempPath, $"output{i}.pdf");

                            // Add the PDF file path to the list
                            pdfFilePaths.Add(pdfFilePath);

                            // Write the original HTML content to a temporary file
                            File.WriteAllText(htmlFilePath, htmls[i]);

                            // Delete the existing PDF file if it already exists
                            if (File.Exists(pdfFilePath)) File.Delete(pdfFilePath);

                            // Create a PDF document configuration
                            var doc = new HtmlToPdfDocument
                            {
                                GlobalSettings = new GlobalSettings
                                {
                                    ColorMode = ColorMode.Color,
                                    Orientation = DinkToPdf.Orientation.Portrait,
                                    PaperSize = PaperKind.A6,
                                    Out = pdfFilePath
                                },
                                Objects =
                        {
                            new ObjectSettings
                            {
                                Page = htmlFilePath,
                                 LoadSettings = new LoadSettings { JSDelay = 5000,ZoomFactor=1.5 },
                                WebSettings = new WebSettings
                                {
                                    EnableJavascript = true,
                                    DefaultEncoding = "UTF-8",
                                    LoadImages = true
                                }
                            }
                        }
                            };

                            // Convert the HTML to PDF
                            Converter.Convert(doc);
                            Console.WriteLine($"PDF generated successfully: {pdfFilePath}");
                        }
                    }
                }
                else
                {
                    // Existing logic for div_frame or PrintContent tables...
                    var divFrames = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'div_frame')]");
                    if (divFrames == null || divFrames.Count == 0) // Check if there are no div_frames
                    {
                        // Check for <table class="PrintContent">
                        var printContentTables = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'PrintContent')]");
                        if (printContentTables != null && printContentTables.Count > 0)
                        {
                            // Define a temporary directory for merged content
                            tempPath = Path.Combine(tempPath, "MergeTemp");

                            // Retrieve the head and body nodes from the HTML document
                            var headNode = htmlDoc.DocumentNode.SelectSingleNode("//head");
                            var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                            // Store the original head content and body class
                            string originalHead = headNode?.OuterHtml ?? "<head></head>";
                            string originalBodyClass = bodyNode?.GetAttributeValue("class", string.Empty) ?? "";

                            for (int j = 0; j < printContentTables.Count; j++) // Loop through each table
                            {
                                // Define paths for temporary HTML and PDF files
                                string htmlFilePath = Path.Combine(tempPath, $"outputHTML_{i}_{j}.html");
                                string pdfFilePath = Path.Combine(tempPath, $"output_{i}_{j}.pdf");

                                // Create new HTML content containing the table and the original structure
                                string newHtmlContent = $@"
                        <!DOCTYPE html>
                        <html>
                        {originalHead}
                        <body class='
                            {originalBodyClass}'>
                            <div style='position: relative; display: inline - block;'> <!-- Corrected display property -->
                                    {printContentTables[j].OuterHtml}
                            </ div >
                        </ body >
                        </ html > ";

                                // Create the temporary directory if it doesn't exist
                                Directory.CreateDirectory(tempPath);

                                // Write the new HTML content to a temporary file
                                File.WriteAllText(htmlFilePath, newHtmlContent);

                                // Delete the existing PDF file if it already exists
                                if (File.Exists(pdfFilePath)) File.Delete(pdfFilePath);

                                // Create a PDF document configuration
                                var doc = new HtmlToPdfDocument
                                {
                                    GlobalSettings = new GlobalSettings
                                    {
                                        ColorMode = ColorMode.Color,
                                        Orientation = DinkToPdf.Orientation.Portrait,
                                        PaperSize = PaperKind.A6,
                                        Margins = new MarginSettings { Top = 3, Left = 0, Bottom = 5, Right = 0 },
                                        Out = pdfFilePath
                                    },
                                    Objects =
                            {
                                new ObjectSettings
                                {
                                    Page = htmlFilePath,
                                     LoadSettings = new LoadSettings { JSDelay = 5000,ZoomFactor=1.5 },
                                    WebSettings = new WebSettings
                                    {
                                        EnableJavascript = true,
                                        DefaultEncoding = "UTF-8",
                                        LoadImages = true
                                    }
                                }
                            }
                                };

                                // Convert the HTML to PDF and add the PDF path to the list
                                Converter.Convert(doc);
                                pdfFilePaths.Add(pdfFilePath);
                                Console.WriteLine($"PDF generated successfully for PrintContent: {pdfFilePath}");
                            }
                        }
                        else // Process any remaining HTML without specific blocks or tables
                        {
                            // Define paths for temporary HTML and PDF files
                            string htmlFilePath = Path.Combine(tempPath, $"outputHTML{i}.html");
                            string pdfFilePath = Path.Combine(tempPath, $"output{i}.pdf");

                            // Add the PDF file path to the list
                            pdfFilePaths.Add(pdfFilePath);

                            // Write the original HTML content to a temporary file
                            File.WriteAllText(htmlFilePath, htmls[i]);

                            // Delete the existing PDF file if it already exists
                            if (File.Exists(pdfFilePath)) File.Delete(pdfFilePath);

                            // Create a PDF document configuration
                            var doc = new HtmlToPdfDocument
                            {
                                GlobalSettings = new GlobalSettings
                                {
                                    ColorMode = ColorMode.Color,
                                    Orientation = DinkToPdf.Orientation.Portrait,
                                    PaperSize = PaperKind.A6,
                                    Out = pdfFilePath
                                },
                                Objects =
                        {
                            new ObjectSettings
                            {
                                Page = htmlFilePath,
                                LoadSettings = new LoadSettings { JSDelay = 5000 },
                                WebSettings = new WebSettings
                                {
                                    EnableJavascript = true,
                                    DefaultEncoding = "UTF-8",
                                    LoadImages = true
                                }
                            }
                        }
                            };

                            // Convert the HTML to PDF
                            Converter.Convert(doc);
                            Console.WriteLine($"PDF generated successfully: {pdfFilePath}");
                        }
                    }
                    else
                    {
                        //For SevenElevenC2C
                        // Logic for processing divFrames remains unchanged
                        tempPath = Path.Combine(Path.GetTempPath(), "MergeTemp");

                        var headNode = htmlDoc.DocumentNode.SelectSingleNode("//head");
                        var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                        string originalHead = headNode?.OuterHtml ?? "<head></head>";
                        string originalBodyStart = bodyNode?.InnerHtml.Split(new[] { "<div class=\"div_frame\">" }, StringSplitOptions.None)[0] ?? "";

                        // Create a separate PDF for each <div class="div_frame">
                        for (int j = 0; j < divFrames.Count; j++)
                        {
                            string htmlFilePath = Path.Combine(tempPath, $"outputHTML_{i}_{j}.html");
                            string pdfFilePath = Path.Combine(tempPath, $"output_{i}_{j}.pdf");

                            // Build the new HTML content for the current div_frame
                            string newHtmlContent = $@"
                    <!DOCTYPE html>
                    <html>
                    {originalHead}
                    <body class='{bodyNode.GetAttributeValue("class", string.Empty)}'>
                        {originalBodyStart}
                        {divFrames[j].OuterHtml}
                    </body>
                    </html>";

                            // Save the HTML file
                            File.WriteAllText(htmlFilePath, newHtmlContent);

                            // Generate PDF
                            if (File.Exists(pdfFilePath)) File.Delete(pdfFilePath);
                            pdfFilePaths.Add(pdfFilePath);

                            var doc = new HtmlToPdfDocument
                            {
                                GlobalSettings = new GlobalSettings
                                {
                                    ColorMode = ColorMode.Color,
                                    Orientation = DinkToPdf.Orientation.Portrait,
                                    PaperSize = PaperKind.A6,
                                    Margins = new MarginSettings { Top = 3, Left = 0, Bottom = 5, Right = 0 },
                                    Out = pdfFilePath
                                },
                                Objects =
                        {
                            new ObjectSettings
                            {
                                Page = htmlFilePath,
                                LoadSettings = new LoadSettings { JSDelay = 5000 },
                                WebSettings = new WebSettings
                                {
                                    EnableJavascript = true,
                                    DefaultEncoding = "UTF-8",
                                    LoadImages = true,
                                }
                            }
                        }
                            };

                            Converter.Convert(doc);
                            Console.WriteLine($"PDF generated successfully for div_frame: {pdfFilePath}");
                        }



                    }
                }

            }
            catch (Exception ex)
            {
                // Log any errors that occur during PDF generation
                Console.WriteLine($"Error generating PDF: {ex.Message}");
            }
        }

        // Return the list of generated PDF file paths
        return pdfFilePaths;
    }
    public MemoryStream CombinePdf(List<string> inputPdfPaths)
    {
        var memoryStream = new MemoryStream();

        string tempPath = Path.Combine(Path.GetTempPath(), "MergeTemp");

        string outputPdfPath = Path.Combine(tempPath, "combinedPdf.pdf");

        using (var outputDocument = new PdfDocument())
        {
            foreach (var path in inputPdfPaths)
            {
                var inputDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                foreach (var page in inputDocument.Pages)
                {
                    outputDocument.AddPage(page);
                }
            }

            outputDocument.Save(memoryStream);

            Console.WriteLine($"Combined PDF created successfully at: {outputPdfPath}");
        }

        memoryStream.Position = 0;

        return memoryStream;
    }
    private void OpenRefundModal()
    {
        refunds = new RefundOrder
        {
            IsRefundOrder = true,
        };

        RefundModal.Show();
    }
    private string UpdateAttributes(string htmlString, string orderId, string deliveryId)
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
        htmlString = AddNewInputsToForm(htmlString, orderId, deliveryId);

        // Change the form action
        htmlString = UpdateButtonOnclick(htmlString);
        htmlString = ReplaceSaveSubmit(htmlString);

        return htmlString;
    }

    private string UpdateHref(string originalHref)
    {
        // Implement your logic to update href attribute
        return "https://logistics-stage.ecpay.com.tw" + originalHref; // Modify this line based on your requirements
    }

    private string UpdateSrc(string originalSrc)
    {
        // Implement your logic to update src attribute
        return "https://logistics-stage.ecpay.com.tw" + originalSrc; // Modify this line based on your requirements
    }
    private string AddNewInputsToForm(string html, string orderId, string deliveryId)
    {
        // Add new input fields after the existing form content
        string newInputs = "<input id='deliveryId' type='hidden'  name='deliveryId' value='" + deliveryId + "' />";
        newInputs = newInputs + "<input id='orderId' type='hidden'  name='orderId' value='" + orderId + "' />";

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
            loading = true;

            if (selectedValue is ShippingStatus.PrepareShipment)
            {
                PaymentResult paymentResult = new();
                paymentResult.OrderId = Order.Id;
                var msg = await _orderAppService.HandlePaymentAsync(paymentResult);
                if (!msg.IsNullOrWhiteSpace())
                {
                    await _uiMessageService.Error(msg);
                }
                await GetOrderDetailsAsync();
                await OnInitializedAsync();
                StateHasChanged();
                loading = false;

            }
            else if (selectedValue is ShippingStatus.ToBeShipped)
            {

                var result = await _orderAppService.OrderToBeShipped(Order.Id);
                if (!result.InvoiceMsg.IsNullOrWhiteSpace())
                {
                    await _uiMessageService.Error(result.InvoiceMsg);
                }
                await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
                loading = false;

            }
            else if (selectedValue is ShippingStatus.Shipped)
            {
                var result = await _orderAppService.OrderShipped(Order.Id);
                if (!result.InvoiceMsg.IsNullOrWhiteSpace())
                {
                    await _uiMessageService.Error(result.InvoiceMsg);
                }
                await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
                loading = false;

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
                loading = false;
            }
            else if (selectedValue is ShippingStatus.Closed)
            {
                await _orderAppService.OrderClosed(Order.Id);
                await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
                loading = false;
            }
            else
            {
                var result = await _orderAppService.ChangeOrderStatus(Order.Id, selectedValue);
                if (!result.InvoiceMsg.IsNullOrWhiteSpace())
                {
                    await _uiMessageService.Error(result.InvoiceMsg);
                }
                await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
                loading = false;

            }

            loading = false;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            loading = false;
        }
    }

    private async Task SevenElevenC2CShippingLabelAsync(
        Dictionary<string, string> allPayLogisticsIds,
        Dictionary<string, string>? deliveryNumbers)
    {
        var keyValuesParams = await _storeLogisticsOrderAppService.OnSevenElevenC2CShippingLabelAsync(
            allPayLogisticsIds,
            deliveryNumbers);

        await JSRuntime.InvokeVoidAsync(
            "downloadSevenElevenC2C",
            keyValuesParams.GetValueOrDefault("ActionUrl"),
            keyValuesParams.GetValueOrDefault("MerchantID"),
            keyValuesParams.GetValueOrDefault("AllPayLogisticsID"),
            keyValuesParams.GetValueOrDefault("CVSPaymentNo"),
            keyValuesParams.GetValueOrDefault("CVSValidationNo"),
            keyValuesParams.GetValueOrDefault("CheckMacValue"));
    }

    public class StoreCommentsModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string Comment { get; set; }
    }

    private Dictionary<Guid, bool> _deliveryCostRendered = new Dictionary<Guid, bool>();

    private decimal CalculateItemTotal(OrderItemDto item, bool includeDeliveryCost)
    {
        var itemTotal = item.TotalAmount;

        if (Order.DeliveryMethod == DeliveryMethod.DeliveredByStore)
        {
            // Add specific delivery cost based on temperature
            itemTotal += GetDeliveryCost(item.DeliveryTemperature);
        }
        else if (includeDeliveryCost)
        {
            // Only add the order-wide delivery cost for the first applicable item
            itemTotal += OrderDeliveryCost ?? 0.00M;
        }

        return itemTotal;
    }

    private bool ShouldDisplayDeliveryCost(Guid itemId)
    {
        if (!_deliveryCostRendered.ContainsKey(itemId))
        {
            _deliveryCostRendered[itemId] = true;
            return true;
        }

        return false;
    }

    private void StartEditMessage(ConversationMessage message)
    {
        editingMessageId = message.Id;
        editingMessageText = message.Text;
    }

    private void CancelMessageEdit()
    {
        editingMessageId = null;
        editingMessageText = "";
    }

    private async Task SaveMessageEdit(string messageId)
    {
        if (string.IsNullOrWhiteSpace(editingMessageText))
            return;

        try
        {
            loading = true;
            await _OrderMessageAppService.UpdateAsync(Guid.Parse(messageId), new CreateUpdateOrderMessageDto
            {
                Message = editingMessageText,
                OrderId = Order.Id,
                IsMerchant = true,
                Timestamp = DateTime.Now
            });

            CustomerServiceHistory = await _OrderMessageAppService.GetOrderMessagesAsync(Order.Id);
            editingMessageId = null;
            editingMessageText = "";
            loading = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            loading = false;
            await HandleErrorAsync(ex);
        }
    }

    private async Task DeleteMessage(string messageId)
    {
        try
        {
            var confirmed = await _uiMessageService.Confirm(L["AreYouSureToDeleteThisMessage?"]);
            if (!confirmed) return;

            loading = true;
            await _OrderMessageAppService.DeleteAsync(Guid.Parse(messageId));
            CustomerServiceHistory = await _OrderMessageAppService.GetOrderMessagesAsync(Order.Id);
            loading = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            loading = false;
            await HandleErrorAsync(ex);
        }
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
using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Blazor.Pages.Orders
{
    public partial class OrderDetails
    {
        [Parameter]
        public string id { get; set; }
        private Guid OrderId { get; set; }
        private OrderDto Order { get; set; }
        private CreateOrderDto UpdateOrder { get; set; } = new();
        private StoreCommentsModel StoreComments = new();
        private ModificationTrack ModificationTrack = new();
        private Shipments shipments = new();
        private List<UpdateOrderItemDto> EditingItems { get; set; } = new();
        private Modal CreateShipmentModal { get; set; }
        private LoadingIndicator loading { get; set; } = new();
        private bool IsItemsEditMode { get; set; } = false;
        private List<OrderDeliveryDto> OrderDeliveries { get; set; }
        private readonly HashSet<Guid> ExpandedRows = new();
        private OrderDeliveryDto SelectedOrder { get; set; }
        private Guid OrderDeliveryId { get; set; }
        public OrderDetails()
        {
            Order = new();
        }
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
        async Task GetOrderDetailsAsync()
        {
            Order = await _orderAppService.GetWithDetailsAsync(OrderId);
            OrderDeliveries = await _orderDeliveryAppService.GetListByOrderAsync(OrderId);
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

        void EditRecipientName()
        {
            ModificationTrack.IsModified = true;
            ModificationTrack.NewName ??= Order.RecipientName;
            ModificationTrack.IsNameInputVisible = true;
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

        void CancelChanges()
        {
            ModificationTrack = new();
        }
        protected virtual async Task SaveChangesAsync()
        {
            try
            {
                if (ModificationTrack.IsInvalidName || ModificationTrack.IsInvalidPhone || ModificationTrack.IsInvalidAddress)
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
                else
                {
                    ModificationTrack.IsInvalidName = false;
                    ModificationTrack.IsInvalidPhone = false;
                    ModificationTrack.IsInvalidAddress = false;
                }
                UpdateOrder.RecipientName = ModificationTrack.IsNameModified
                ? ModificationTrack.NewName
                : Order.RecipientName;

                UpdateOrder.RecipientPhone = ModificationTrack.IsPhoneModified
                ? ModificationTrack.NewPhone
                : Order.RecipientPhone;

                if (ModificationTrack.IsAddressModified)
                {
                    UpdateOrder.City = ModificationTrack.NewCity;
                    UpdateOrder.District = ModificationTrack.NewDistrict;
                    UpdateOrder.Road = ModificationTrack.NewRoad;
                    UpdateOrder.AddressDetails = ModificationTrack.NewAddress;
                }
                else
                {
                    UpdateOrder.City = Order.City;
                    UpdateOrder.District = Order.District;
                    UpdateOrder.Road = Order.Road;
                    UpdateOrder.AddressDetails = Order.AddressDetails;
                }
                await loading.Show();
                UpdateOrder.OrderStatus = Order.OrderStatus;
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
            var id = Order?.Id;
            NavigationManager.NavigateTo($"Orders/OrderShippingDetails/{id}");
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
        private void CloseShipmentModal()
        {
            CreateShipmentModal.Hide();
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
                if (Order.PaymentMethod != PaymentMethods.CreditCard)
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.RefundIsOnlyAvailableForCreditCardPayments]);
                    return;
                }
                var confimation = await _uiMessageService.Confirm(L["AreYouSureToRefundThisOrder?"]);
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

        public class StoreCommentsModel
        {
            public Guid? Id { get; set; }

            [Required(ErrorMessage = "This Field Is Required")]
            public string Comment { get; set; }
        }

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
        public string NewRoad { get; set; }
        public string NewDistrict { get; set; }
        public string NewCity { get; set; }
        public string NewAddress { get; set; }
        public bool IsInvalidAddress { get; set; }
        public bool IsAddressModified { get; set; }
    }
    public class Shipments
    {
        [Required]
        public DeliveryMethod ShippingMethod { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string? ShippingNumber { get; set; }
    }
}

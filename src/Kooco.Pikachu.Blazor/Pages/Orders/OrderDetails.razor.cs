using Blazorise;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;

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
        private bool isModalVisible = false;
        private Validations CreateValidationsRef;
        private Modal CreateShipmentModal { get; set; }
        protected async override Task OnInitializedAsync()
        {
            try
            {
                OrderId = Guid.Parse(id);
                await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                Console.WriteLine(ex.Message);
            }
        }

        async Task GetOrderDetailsAsync()
        {
            Order = await _orderAppService.GetWithDetailsAsync(OrderId);
            await InvokeAsync(StateHasChanged);
        }

        async Task SubmitStoreCommentsAsync()
        {
            try
            {
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
            }
            catch (BusinessException ex)
            {
                await _uiMessageService.Error(L[ex.Code]);
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                Console.WriteLine(ex.ToString());
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

                if (
                    ModificationTrack.IsInvalidName
                    || ModificationTrack.IsInvalidPhone
                    || ModificationTrack.IsInvalidAddress
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
                UpdateOrder.OrderStatus = Order.OrderStatus;
                Order = await _orderAppService.UpdateAsync(OrderId, UpdateOrder);
                ModificationTrack = new();
                await InvokeAsync(StateHasChanged);
            }
            catch (BusinessException ex)
            {
                Console.WriteLine(ex.ToString());
                await _uiMessageService.Error(ex.Code?.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await _uiMessageService.Error(ex.GetType().ToString());
            }

        }
        public void NavigateToOrderShipmentDetails()
        {
            var id = Order?.Id;
            NavigationManager.NavigateTo($"Orders/OrderShippingDetails/{id}");
        }
        protected virtual async Task CancelOrder()
        {
            var confirmed = await _uiMessageService.Confirm("Are you sure to cancel the Order?");
            if (confirmed)
            {
                //Delete the 'admin' role here.

                if (Order?.ShippingStatus == ShippingStatus.WaitingForPayment)
                {
                    UpdateOrder.City = Order.City;
                    UpdateOrder.District = Order.District;
                    UpdateOrder.Road = Order.Road;
                    UpdateOrder.AddressDetails = Order.AddressDetails;
                    UpdateOrder.RecipientName = Order.RecipientName;
                    UpdateOrder.RecipientPhone = Order.RecipientPhone;
                    UpdateOrder.OrderStatus = OrderStatus.Closed;
                    await _orderAppService.UpdateAsync(OrderId, UpdateOrder);
                    await InvokeAsync(StateHasChanged);
                }
            }
        }
        private void OpenShipmentModal()
        {
            // CreateValidationsRef.ClearAll();

            // NewAuthor = new CreateAuthorDto();
            CreateShipmentModal.Show();
        }
        private void CloseShipmentModal()
        {
            CreateShipmentModal.Hide();
        }
        private async Task ApplyShipmentAsync()
        {
                UpdateOrder.ShippingNumber = shipments.ShippingNumber;
                UpdateOrder.DeliveryMethod = shipments.ShippingMethod;
                await _orderAppService.UpdateShippingDetails(OrderId, UpdateOrder);
                CreateShipmentModal.Hide();
                await InvokeAsync(StateHasChanged);
            
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
        [Required]
        public string? ShippingNumber { get; set; }
        }

    }

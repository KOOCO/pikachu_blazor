using Kooco.Pikachu.Migrations;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
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
        private StoreCommentsModel StoreComments = new();
        private ModificationTrack ModificationTrack = new();
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
                if(StoreComments.Id != null)
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
            ModificationTrack.NewName = Order.RecipientName;

            ModificationTrack.IsNameInputVisible = true;
        }

        void EditRecipientPhone()
        {
            ModificationTrack.IsModified = true;
            ModificationTrack.NewPhone = Order.RecipientPhone;

            ModificationTrack.IsPhoneInputVisible = true;
        }
        void EditRecipientAddress()
        {
            ModificationTrack.IsModified = true;
            ModificationTrack.NewRoad = Order.Road;
            ModificationTrack.NewDistrict = Order.District;
            ModificationTrack.NewCity = Order.City;
            ModificationTrack.NewAddress = Order.AddressDetails;

            ModificationTrack.IsAddressInputVisible = true;
        }

        void CancelChanges()
        {
            ModificationTrack = new();
        }   
    }

    public class StoreCommentsModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string Comment { get; set; }
    }

    public class ModificationTrack
    {
        public bool IsModified { get; set; }

        public bool IsNameInputVisible { get; set; }
        public string NewName { get; set; }
        public bool IsValidName { get; set; }
        public bool IsNameModified { get; set; }

        public bool IsPhoneInputVisible { get; set; }
        public string NewPhone { get; set; }
        public bool IsValidPhone { get; set; }
        public bool IsPhoneModified { get; set; }

        public bool IsAddressInputVisible { get; set; }
        public string NewRoad { get; set; }
        public string NewDistrict { get; set; }
        public string NewCity { get; set; }
        public string NewAddress { get; set; }
        public bool IsValidAddress { get;set; }
        public bool IsAddressModified { get; set; }
    }
}

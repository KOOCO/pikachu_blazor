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
    }

    public class StoreCommentsModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string Comment { get; set; }
    }
}

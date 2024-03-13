using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using static System.Net.WebRequestMethods;

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
        string? CheckoutForm { get; set; } = null;

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
                UpdateOrder.PostalCode = "403702";//Order.PostalCode;
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
        private async void OrderItemShipped(OrderDeliveryDto deliveryOrder)
        {
            await loading.Show();
            OrderDeliveryId = deliveryOrder.Id;
            await _orderDeliveryAppService.UpdateOrderDeliveryStatus(OrderDeliveryId);
            await GetOrderDetailsAsync();
            await InvokeAsync(StateHasChanged);
            await loading.Hide();

        }
        private async void CreateOrderLogistics(OrderDeliveryDto deliveryOrder)
        {
            await loading.Show();
            OrderDeliveryId = deliveryOrder.Id;
            if (deliveryOrder.DeliveryMethod == DeliveryMethod.SevenToEleven1 || deliveryOrder.DeliveryMethod == DeliveryMethod.FamilyMart1)
            {
                var htmlString = await _storeLogisticsOrderAppService.GetStoreAsync(Order.Id, OrderDeliveryId);
                StringBuilder htmlForm = new();
                htmlForm.Append(htmlString.HtmlString);
                string html = htmlString.HtmlString;
                html=UpdateAttributes(html,Order.Id.ToString(),deliveryOrder.Id.ToString());
                //int startIndex = htmlString.HtmlString.IndexOf("<script src=\"/Scripts/jquery-1.4.4.js\" type=\"text/javascript\">");
                //int endIndex = htmlString.HtmlString.IndexOf("</script>", startIndex);

                //int startIndexForm = htmlString.HtmlString.IndexOf("<form id=\"PostForm\" name=\"PostForm\" action=\"/Home/Family\" method=\"POST\">");
                //int endIndexForm = htmlString.HtmlString.IndexOf("</form>", startIndexForm);

                //if (startIndex != -1 && endIndex != -1)
                //{
                //    // Extract the script tag
                //    string scriptTag = htmlString.HtmlString.Substring(startIndex, endIndex - startIndex + "</script>".Length);

                //    // Replace the old src attribute with the new one
                //    string newScriptTag = scriptTag.Replace("src=\"/Scripts/jquery-1.4.4.js\"", "src=\"https://logistics-stage.ecpay.com.tw/Scripts/jquery-1.4.4.js\"");

                //    // Update the HTML string
                //    htmlString.HtmlString.Replace(scriptTag, newScriptTag);
                //     html = htmlString.HtmlString.Replace(scriptTag, newScriptTag);

                //    // Convert the updated string back to StringBuilder
                //    htmlForm = new StringBuilder(html);
                //}
                //if (startIndexForm != -1 && endIndexForm != -1)
                //{
                //    // Extract the form tag
                //    string formTag = html.Substring(startIndexForm, endIndexForm - startIndexForm + "</form>".Length);

                //    // Replace the old action attribute with the new one
                //    string newFormTag = formTag.Replace("action=\"/Home/Family\"", "action=\"https://logistics-stage.ecpay.com.tw/Home/Family\"");

                //    // Update the HTML string
                //    html = html.Replace(formTag, newFormTag);
                //    htmlForm = new StringBuilder(html);
                //}
                //await JSRuntime.InvokeVoidAsync("setCookie", htmlString.CookieName, htmlString.CookieValue,"None",true);
                //NavigationManager.NavigateTo($"/map-response?htmlString={Uri.EscapeDataString(htmlForm.ToString())}");
                await JSRuntime.InvokeVoidAsync("openPopup", html);
                //NavigationManager.NavigateTo($"map-response/{htmlForm}");
            }
            else {
              var result=  await _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(Order.Id, OrderDeliveryId);
                if (result.ResponseCode != "1")
                {
                    await _uiMessageService.Error(result.ResponseMessage);

                }
               
            }
            await GetOrderDetailsAsync();
            await InvokeAsync(StateHasChanged);
            await loading.Hide();

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
            string newFunction = "function SaveSubmitNew(url){" +
                "" +
                "" +
                "const myHeaders = new Headers();\r\nmyHeaders.append(\"Content-Type\", \"application/json\");\r\n\r\nconst raw = JSON.stringify({\r\n  \"address\": \"新竹市東區建中一路52號1樓\",\r\n  \"storeaddress\": \"新竹市東區建中一路52號1樓\",\r\n  \"storeid\": \"131386\",\r\n  \"storename\": \"建盛門市\",\r\n  \"outside\": \"0\",\r\n  \"deliveryId\": \"5eefb622-d497-44c4-8f2a-909c55e6abd8\",\r\n  \"orderId\": \"054f0fed-d2c7-26cd-c8b6-3a11253970fe\"\r\n});\r\n\r\nconst requestOptions = {\r\n  method: \"POST\",\r\n  headers: myHeaders,\r\n  body: raw,\r\n  redirect: \"follow\"\r\n};\r\n\r\nfetch(\"https://localhost:44374/api/app/store-logistics-order/store-logistics-order\", requestOptions)\r\n  .then((response) => response.text())\r\n  .then((result) => console.log(result))\r\n  .catch((error) => console.error(error));}</script>";
//            string newFunction = @"function SaveSubmitNew(url) {
//                                debugger;
//                                var formData = new FormData(document.getElementById('SubmitForm'));
//                                var formDataObject = {};
//                                formData.forEach(function(value, key){
//                                    formDataObject[key] = value;
//                                             });

            //                    // Convert the plain JavaScript object to a JSON string
            //                            var formDataJson = JSON.stringify(formDataObject);
            //                            const myHeaders = new Headers();
            //                            myHeaders.append('Content-Type', 'application/json');

            //                            const raw = formDataJson;

            //                        const requestOptions = {
            //                                            method: 'POST',
            //                                            headers: myHeaders,
            //                                            body: raw,
            //                                            redirect: 'follow'
            //                                                };

            //                                        fetch(url, requestOptions)
            //                                        .then((response) => response.text())
            //                                        .then((result) => console.log(result))
            //                                        .catch((error) => console.error(error));

            //                            };
            //</script>";

            // Find the script tag and add the new method inside it
            html = System.Text.RegularExpressions.Regex.Replace(html, @"function SaveSubmit\(url\)[\s\S]*?}\s*<\/script>", newFunction);

            return html;

            

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

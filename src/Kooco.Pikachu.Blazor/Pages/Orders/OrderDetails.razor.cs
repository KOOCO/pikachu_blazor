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
        private RefundOrder refunds = new();
        private List<UpdateOrderItemDto> EditingItems { get; set; } = new();
        private Modal CreateShipmentModal { get; set; }
        private Modal RefundModal { get; set; }
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
                if (ModificationTrack.IsInvalidName || ModificationTrack.IsInvalidPhone || ModificationTrack.IsInvalidAddress||ModificationTrack.IsInvalidPostalCode)
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
                else
                {
                    ModificationTrack.IsInvalidName = false;
                    ModificationTrack.IsInvalidPhone = false;
                    ModificationTrack.IsInvalidAddress = false;
                    ModificationTrack.IsInvalidPostalCode = false;
                }
                UpdateOrder.RecipientName = ModificationTrack.IsNameModified
                ? ModificationTrack.NewName
                : Order.RecipientName;

                UpdateOrder.RecipientPhone = ModificationTrack.IsPhoneModified
                ? ModificationTrack.NewPhone
                : Order.RecipientPhone;
                UpdateOrder.PostalCode = ModificationTrack.IsPostalCodeModified
                ? ModificationTrack.NewPostalCode
                : Order.PostalCode;
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
                //UpdateOrder.PostalCode = Order.PostalCode;
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
            if (deliveryOrder.DeliveryMethod == DeliveryMethod.SevenToEleven1 || deliveryOrder.DeliveryMethod == DeliveryMethod.FamilyMart1 ||
               deliveryOrder.DeliveryMethod == DeliveryMethod.SevenToElevenC2C || deliveryOrder.DeliveryMethod == DeliveryMethod.FamilyMart1)
            {
                var result = await _storeLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(Order.Id, deliveryOrder.Id);
                if (result.ResponseCode != "1")
                {
                    await _uiMessageService.Error(result.ResponseMessage);

                }
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
            }
            else
            {
                var result = await _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(Order.Id, OrderDeliveryId);
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
                  await  loading.Show();
                    var orderItemIds = Order?.OrderItems.Where(x => x.IsSelected).Select(x => x.Id).ToList();
                    if (orderItemIds.Count < 1)
                    {
                        await _uiMessageService.Error("Please Select Order Item");
                        await loading.Hide();
                        return;
                    }
                    refunds.OrderItemIds=orderItemIds;
                    await _orderAppService.RefundOrderItems(orderItemIds, OrderId);
                    await loading.Hide();
                    await RefundModal.Hide();

                }
                else {
                    await loading.Show();
                 if(refunds.Amount==0)
                    {
                        await _uiMessageService.Error("Please Enter Amount");
                        await loading.Hide();
                        return;

                    }
                 if(refunds.Amount>(double)Order.TotalAmount) {

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
               
                
               
                if (selectedValue == ShippingStatus.PrepareShipment )
                {
                    PaymentResult paymentResult = new PaymentResult();
                    paymentResult.OrderId = Order.Id;
                    await _orderAppService.HandlePaymentAsync(paymentResult);
                    await GetOrderDetailsAsync();
                    await base.OnInitializedAsync();
                    await loading.Hide();

                }
                else if (selectedValue == ShippingStatus.Shipped )
                {
                    await _orderAppService.OrderShipped(Order.Id);
                    await GetOrderDetailsAsync();
                    await base.OnInitializedAsync();
                    await loading.Hide();

                }
                else if (selectedValue == ShippingStatus.Completed)
                {
                    await _orderAppService.OrderComplete(Order.Id);
                    await GetOrderDetailsAsync();
                    await base.OnInitializedAsync();
                    await loading.Hide();
                }
                else if (selectedValue == ShippingStatus.Closed )
                {
                    await _orderAppService.OrderClosed(Order.Id);
                    await GetOrderDetailsAsync();
                    await base.OnInitializedAsync();
                    await loading.Hide();
                }
                await loading.Hide();
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());

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
}

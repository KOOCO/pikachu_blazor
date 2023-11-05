using Blazorise.LoadingIndicator;
using Kooco.Pikachu.AutomaticEmails;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantEmailing
{
    public partial class CreateAutomaticEmails
    {
        LoadingIndicator Loading;
        AutomaticEmailCreateUpdateDto Model { get; set; }
        List<KeyValueDto> GroupBuys { get; set; }
        string? Recipient { get; set; }
        
        public CreateAutomaticEmails()
        {
            Model = new();
            GroupBuys = new();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                GroupBuys = await _groupBuyAppService.GetGroupBuyLookupAsync();
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        void HandleRecipientInputKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" || e.Key == ",")
            {
                if (!Recipient.IsNullOrWhiteSpace() && !Model.RecipientsList.Any(x => x == Recipient))
                {
                    Recipient = Recipient?.TrimEnd(',');

                    var email = new EmailAddressAttribute();
                    if (!email.IsValid(Recipient))
                    {
                        return;
                    }
                    Model.RecipientsList.Add(Recipient);
                }
                Recipient = string.Empty;
            }
        }

        private void HandleRecipientDelete(string item)
        {
            Model.RecipientsList.Remove(item);
        }

        async Task CreateAutomaticEmailAsync()
        {
            try
            {
                var isValid = await ValidateForm();

                if (!isValid)
                {
                    return;
                }

                await Loading.Show();
                await _automaticEmailAppService.CreateAsync(Model);
                NavigationManager.NavigateTo("/AutomaticEmailing");
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
                await Loading.Hide();
            }
        }

        private async Task<bool> ValidateForm()
        {
            if (Model.TradeName.IsNullOrEmpty())
            {
                await _uiMessageService.Error(L["InvalidTradeName"]);
                return false;
            }
            if(Model.RecipientsList == null || Model.RecipientsList.Count == 0)
            {
                await _uiMessageService.Error(L["InvalidRecipients"]);
                return false;
            }
            if(Model.StartDate == null)
            {
                await _uiMessageService.Error(L["InvalidStartDate"]);
                return false;
            }
            if(Model.EndDate == null)
            {
                await _uiMessageService.Error(L["InvalidEndDate"]);
                return false;
            }
            if(Model.SendTime == null)
            {
                await _uiMessageService.Error(L["InvalidSendTime"]);
                return false;
            }
            if(Model.GroupBuyIds == null || Model.GroupBuyIds.Count == 0)
            {
                await _uiMessageService.Error(L["InvalidGroupBuys"]);
                return false;
            }
            return true;
        }
    }
}

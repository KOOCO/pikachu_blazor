using Blazorise;
using DeviceDetectorNET;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Campaigns;

public partial class CreateCampaign
{
    private CreateCampaignDto Entity { get; set; } = new();
    private List<string> TargetAudienceOptions { get; } = CampaignConsts.TargetAudience.Values;
    private List<DeliveryMethod> DeliveryMethodOptions { get; } = [.. Enum.GetValues<DeliveryMethod>()];
    private IReadOnlyList<KeyValueDto> GroupBuyOptions { get; set; } = [];
    private IReadOnlyList<KeyValueDto> ProductOptions { get; set; } = [];
    private bool Loading { get; set; } = false;

    private Validations ValidationsRef;
    private readonly ValidationMessageStore _messageStore;
    private readonly EditContext _editContext;

    public CreateCampaign()
    {
        _editContext = new(Entity);
        _messageStore = new(_editContext);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                GroupBuyOptions = await GroupBuyAppService.GetGroupBuyLookupAsync();
                ProductOptions = await ItemAppService.GetAllItemsLookupAsync();
            }
            catch(Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    void Cancel()
    {
        NavigationManager.NavigateTo("/Campaigns");
    }

    async Task Save()
    {
        Loading = true;
        bool isValid = await ValidationsRef.ValidateAll();
        await Validate();

        if (!isValid || _editContext.GetValidationMessages().Any())
        {
            await Notify.Error(L["ValidationErrorsDescription"], L["ValidationErrors"]);
            Loading = false;
            return;
        }

        await Message.Success("Success");
        Loading = false;
    }

    async Task Validate()
    {
        _messageStore.Clear();
        var validationResult = await CreateCampaignValidator.ValidateAsync(Entity);

        foreach (var error in validationResult.Errors)
        {
            var fieldIdentifier = new FieldIdentifier(Entity, error.PropertyName);
            _messageStore.Add(fieldIdentifier, error.ErrorMessage);
        }

        _editContext.NotifyValidationStateChanged();
    }

    string ValidationClass(string propertyName)
    {
        var fieldIdentifier = new FieldIdentifier(Entity, propertyName);
        return _editContext.GetValidationMessages(fieldIdentifier).Any() ? "is-invalid" : string.Empty;
    }
}
using Blazorise;
using DeviceDetectorNET;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Campaigns;

public partial class CreateCampaign
{
    [Parameter] public Guid? Id { get; set; }
    private CreateCampaignDto Entity { get; set; } = new();
    private IReadOnlyList<string> TargetAudienceOptions { get; set; } = [];
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

        _editContext.OnFieldChanged += (sender, e) =>
        {
            _messageStore.Clear(e.FieldIdentifier);
            _editContext.NotifyValidationStateChanged();
        };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                GroupBuyOptions = await GroupBuyAppService.GetGroupBuyLookupAsync();
                ProductOptions = await ItemAppService.GetAllItemsLookupAsync();
                var memberTags = await MemberTagAppService.GetMemberTagNamesAsync();
                TargetAudienceOptions = [.. CampaignConsts.TargetAudience.Values, .. memberTags];
            }
            catch (Exception ex)
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
        try
        {
            Loading = true;

            if (!await Validate())
            {
                return;
            }
            await CampaignAppService.CreateAsync(Entity);
            await Message.Success("Success");
            Loading = false;
            Cancel();
        }
        catch (Exception ex)
        {
            Loading = false;
            await HandleErrorAsync(ex);
        }
    }

    async Task<bool> Validate()
    {
        _messageStore.Clear();
        var validationResult = await CreateCampaignValidator.ValidateAsync(Entity);

        foreach (var error in validationResult.Errors)
        {
            var segments = error.PropertyName.Split('.', 2);

            FieldIdentifier field;

            if (segments.Length == 2)
            {
                field = segments[0] switch
                {
                    "Discount" => new FieldIdentifier(Entity.Discount, segments[1]),
                    "AddOnProduct" => new FieldIdentifier(Entity.AddOnProduct, segments[1]),
                    "ShoppingCredit" => new FieldIdentifier(Entity.ShoppingCredit, segments[1]),
                    _ => new FieldIdentifier(Entity, error.PropertyName)
                };
            }
            else
            {
                field = new FieldIdentifier(Entity, error.PropertyName);
            }

            _messageStore.Add(field, error.ErrorMessage);
        }

        _editContext.NotifyValidationStateChanged();

        bool isValid = await ValidationsRef.ValidateAll();

        if (!isValid || _editContext.GetValidationMessages().Any())
        {
            await Notify.Error(L["ValidationErrorsDescription"], L["ValidationErrors"]);
            Loading = false;
        }

        return Loading;
    }

    string ValidationClass(string propertyName)
    {
        var fieldIdentifier = new FieldIdentifier(Entity, propertyName);
        return _editContext.GetValidationMessages(fieldIdentifier).Any() ? "is-invalid" : string.Empty;
    }
}
using Blazored.TextEditor;
using Blazorise;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.TenantEmailing;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantEmailing;

public partial class CreateEdm
{
    private CreateEdmDto Entity { get; set; } = new();
    private IReadOnlyList<KeyValueDto> CampaignOptions { get; set; } = [];
    private IReadOnlyList<KeyValueDto> GroupBuyOptions { get; set; } = [];
    private IReadOnlyList<string> MemberTagOptions { get; set; } = [];
    private IReadOnlyList<EdmTemplateType> TemplateTypeOptions { get; set; } = [.. Enum.GetValues<EdmTemplateType>()];
    private IReadOnlyList<EdmSendFrequency> SendFrequencyOptions { get; set; } = [.. Enum.GetValues<EdmSendFrequency>()];
    private string SelectedTab { get; set; } = EdmTemplateType.Customize.ToString();
    private bool Loading { get; set; }

    private Validations ValidationsRef;
    private readonly ValidationMessageStore _messageStore;
    private readonly EditContext _editContext;
    private BlazoredTextEditor MessageHtml;

    public CreateEdm()
    {
        _editContext = new(Entity);
        _messageStore = new(_editContext);

        _editContext.OnFieldChanged += (sender, e) =>
        {
            _messageStore.Clear(e.FieldIdentifier);
            _editContext.NotifyValidationStateChanged();
        };
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            GroupBuyOptions = await GroupBuyAppService.GetAllGroupBuyLookupAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    void OnTabChanged(string tab)
    {
        SelectedTab = tab;
        Entity.TemplateType = Enum.Parse<EdmTemplateType>(tab);
    }

    string ValidationClass(string propertyName)
    {
        var fieldIdentifier = new FieldIdentifier(Entity, propertyName);
        return _editContext.GetValidationMessages(fieldIdentifier).Any() ? "is-invalid" : string.Empty;
    }

    async Task Save()
    {
        try
        {
            Loading = true;
            await InvokeAsync(StateHasChanged);

            Entity.Message = await MessageHtml.GetHTML();

            if (!await Validate())
            {
                return;
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    async Task<bool> Validate()
    {
        _messageStore.Clear();
        var validationResult = await CreateEdmValidator.ValidateAsync(Entity);

        foreach (var error in validationResult.Errors)
        {
            var field = new FieldIdentifier(Entity, error.PropertyName);
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
}
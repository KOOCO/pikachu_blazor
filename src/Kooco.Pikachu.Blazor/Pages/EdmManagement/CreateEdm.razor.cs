using Blazored.TextEditor;
using Blazorise;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.EdmManagement;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.EdmManagement;

public partial class CreateEdm
{
    [Parameter] public Guid? Id { get; set; }
    private CreateEdmDto Entity { get; set; } = new();
    private IReadOnlyList<KeyValueDto> CampaignOptions { get; set; } = [];
    private IReadOnlyList<KeyValueDto> GroupBuyOptions { get; set; } = [];
    private IReadOnlyList<KeyValueDto> FilteredGroupBuyOptions { get; set; } = [];
    private IReadOnlyList<string> MemberTagOptions { get; set; } = [];
    private IReadOnlyList<string> FilteredMemberTagOptions { get; set; } = [];
    private IReadOnlyList<EdmTemplateType> TemplateTypeOptions { get; set; } = [.. Enum.GetValues<EdmTemplateType>()];
    private IReadOnlyList<EdmSendFrequency> SendFrequencyOptions { get; set; } = [.. Enum.GetValues<EdmSendFrequency>()];
    private CampaignDto SelectedCampaign { get; set; }
    private bool CanSelectAllMembers { get; set; }
    private string SelectedTab { get; set; }
    private bool Loading { get; set; }

    private Validations ValidationsRef;
    private readonly ValidationMessageStore _messageStore;
    private readonly EditContext _editContext;
    private BlazoredTextEditor MessageHtml;
    private bool _htmlLoaded = false;
    private bool _htmlNeedsLoading = false;
    private bool _firstLoad = true;

    public CreateEdm()
    {
        _editContext = new(Entity);
        _messageStore = new(_editContext);

        _editContext.OnFieldChanged += (sender, e) =>
        {
            _messageStore.Clear(e.FieldIdentifier);
            _editContext.NotifyValidationStateChanged();
        };

        OnTabChanged(EdmTemplateType.Customize.ToString());
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await CheckPolicyAsync();

            GroupBuyOptions = await GroupBuyAppService.GetAllGroupBuyLookupAsync();
            CampaignOptions = await CampaignAppService.GetLookupAsync();
            MemberTagOptions = await MemberTagAppService.GetMemberTagNamesAsync();

            FilteredGroupBuyOptions = [.. GroupBuyOptions];
            FilteredMemberTagOptions = [.. MemberTagOptions];

            if (Id.HasValue)
            {
                var edm = await EdmAppService.GetAsync(Id.Value);
                Entity = ObjectMapper.Map<EdmDto, CreateEdmDto>(edm);
                OnTabChanged(Entity.TemplateType.ToString());
                StateHasChanged();
                await ValidationsRef.ClearAll();

                _htmlNeedsLoading = true;
                _firstLoad = Entity.TemplateType == EdmTemplateType.Campaign;
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if ((_htmlNeedsLoading || firstRender) && !_htmlLoaded)
        {
            if (Id.HasValue && !string.IsNullOrWhiteSpace(Entity?.Message))
            {
                await Task.Delay(5);
                await MessageHtml.LoadHTMLContent(Entity.Message);
                _htmlLoaded = true;
                _htmlNeedsLoading = false;
                StateHasChanged();
            }
        }
    }

    async Task CheckPolicyAsync()
    {
        bool isGranted = Id.HasValue
                ? await AuthorizationService.IsGrantedAsync(PikachuPermissions.EdmManagement.Edit)
                : await AuthorizationService.IsGrantedAsync(PikachuPermissions.EdmManagement.Create);

        if (!isGranted)
        {
            await Message.Error(L["YouAreNotAuthorized"]);
            Cancel();
        }
    }

    void OnTabChanged(string tab)
    {
        SelectedTab = tab;

        var templateType = Enum.Parse<EdmTemplateType>(tab);

        if (templateType != Entity.TemplateType)
        {
            Entity.TemplateType = templateType;

            Entity.CampaignId = null;

            ResetOptions();

            Entity.Subject = Entity.TemplateType == EdmTemplateType.ShoppingCart
                ? L["EdmTemplateTypeShoppingCartSubject"]
                : Entity.Subject = string.Empty;

            if (!_firstLoad)
            {
                ClearCampaignFields();
            }

            MessageHtml?.LoadHTMLContent(string.Empty);
        }
    }

    async Task LoadDefaultTemplate()
    {
        if (!Entity.TemplateType.HasValue) return;

        var templateType = Entity.TemplateType!.Value;

        var template = EdmTemplateConsts.GetTemplate(templateType).Replace("\n", "");

        if (templateType == EdmTemplateType.Campaign)
        {
            if (!Entity.CampaignId.HasValue) return;

            template = template
                .Replace(EdmTemplatePlaceholders.PromotionModule, EdmTemplateConsts.GetTemplate(SelectedCampaign.PromotionModule))
                .Replace("\n", "");
        }

        await MessageHtml.LoadHTMLContent(template);

        StateHasChanged();
    }

    async Task OnCampaignChanged(Guid? campaignId)
    {
        _editContext.NotifyFieldChanged(new FieldIdentifier(Entity, nameof(Entity.CampaignId)));

        SelectedCampaign = null!;
        if (campaignId.HasValue)
        {
            SelectedCampaign = await CampaignAppService.GetAsync(campaignId.Value, true);
        }

        ClearCampaignFields();
        ResetOptions();
    }

    void ClearCampaignFields()
    {
        if (_firstLoad)
        {
            _firstLoad = false;
            return;
        }

        CanSelectAllMembers = true;
        Entity.ApplyToAllMembers = null;
        Entity.GroupBuyId = null;
        Entity.MemberTags = [];
    }

    void ResetOptions()
    {
        CanSelectAllMembers = true;
        FilteredMemberTagOptions = [.. MemberTagOptions];
        FilteredGroupBuyOptions = [.. GroupBuyOptions];

        if (Entity.TemplateType == EdmTemplateType.Campaign && SelectedCampaign != null)
        {
            Entity.Subject = SelectedCampaign.Name;

            CanSelectAllMembers = SelectedCampaign.TargetAudience.Contains(CampaignConsts.TargetAudience.All)
                || SelectedCampaign.TargetAudience.Contains(CampaignConsts.TargetAudience.AllMembers);

            FilteredMemberTagOptions = CanSelectAllMembers
                ? [.. MemberTagOptions]
                : [.. MemberTagOptions.Where(tag => SelectedCampaign.TargetAudience.Any(audience => tag == audience))];

            FilteredGroupBuyOptions = SelectedCampaign.ApplyToAllGroupBuys
                ? [.. GroupBuyOptions]
                : [.. GroupBuyOptions.Where(gb => SelectedCampaign.GroupBuys.Any(cgb => cgb.GroupBuyId == gb.Id))];

            Entity.MemberTags = Entity.MemberTags
                .Where(tag => FilteredMemberTagOptions.Contains(tag));

            Entity.GroupBuyId = FilteredGroupBuyOptions
                .Where(gbo => gbo.Id == Entity.GroupBuyId)
                .FirstOrDefault()?.Id;

            StateHasChanged();
        }
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

            if (Id.HasValue)
            {
                await EdmAppService.UpdateAsync(Id.Value, Entity);
            }
            else
            {
                await EdmAppService.CreateAsync(Entity);
            }

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

    string ValidationClass(string propertyName)
    {
        var fieldIdentifier = new FieldIdentifier(Entity, propertyName);
        return _editContext.GetValidationMessages(fieldIdentifier).Any() ? "is-invalid" : string.Empty;
    }

    void Cancel()
    {
        NavigationManager.NavigateTo("/Edm");
    }

    async Task InsertPlaceholder(string placeholder)
    {
        if (MessageHtml != null)
        {
            await MessageHtml.InsertText(placeholder);
            StateHasChanged();
        }
    }
}
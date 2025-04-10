﻿using Blazorise;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Identity;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.Tenants;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Blazor;
using Volo.Abp.TenantManagement.Localization;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement;

public partial class CustomTenantManagement
{
    #region Inject
    private List<string> TenantContactTitles = ["Mr.", "Ms."];
    private const int MaxTextCount = 60; //input max length
    private const int MaxAllowedFilesPerUpload = 10;
    private const int TotalMaxAllowedFiles = 50;
    private const int MaxAllowedFileSize = 1024 * 1024 * 10;
    private readonly List<string> ValidFileExtensions = new() { ".jpg", ".png", ".svg", ".jpeg", ".webp" };
    private FilePicker FilePickerCustom { get; set; }
    private FilePicker BannerPickerCustom { get; set; }
    public Guid? TenantOwnerId { get; set; }
    public TenantStatus Status { get; set; }
    IReadOnlyList<KeyValueDto> UsersList = [];
    public int ShareProfitPercentage { get; set; }
    public string LogoUrl { get; set; }
    public string BannerUrl { get; set; }
    public string ShortCode { get; set; }
    public string EntryUrl { get; set; }
    public string? TenantUrl { get; set; }
    public string TenantContactPerson { get; set; }
    public string TenantContactTitle { get; set; }
    public string TenantContactEmail { get; set; }
    public string? Domain { get; set; }

    private readonly IUiMessageService _uiMessageService;
    private readonly ImageContainerManager _imageContainerManager;
    private readonly IIdentityUserAppService _identityUserAppService;
    private readonly IMyTenantAppService _myTenantAppService;
    private readonly IStringLocalizer<PikachuResource> _L;
    #endregion

    public CustomTenantManagement(
        IUiMessageService uiMessageService,
        ImageContainerManager imageContainerManager,
        IIdentityUserAppService identityUserAppService,
        IMyTenantAppService myTenantAppService,
        IStringLocalizer<PikachuResource> L
    )
    {
        LocalizationResource = typeof(AbpTenantManagementResource);
        ObjectMapperContext = typeof(AbpTenantManagementBlazorModule);
        CreatePolicyName = TenantManagementPermissions.Tenants.Create;
        UpdatePolicyName = TenantManagementPermissions.Tenants.Update;
        DeletePolicyName = TenantManagementPermissions.Tenants.Delete;
        ManageFeaturesPolicyName = TenantManagementPermissions.Tenants.ManageFeatures;
        _uiMessageService = uiMessageService;
        _imageContainerManager = imageContainerManager;
        _identityUserAppService = identityUserAppService;
        _myTenantAppService = myTenantAppService;
        _L = L;
    }
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        EntryUrl = _configuration["EntryUrl"]?.TrimEnd('/');
        UsersList = await MyIdentityUserAppService.GetCategorizedLookupAsync(UserTypes.Backend) ?? [];
    }

    public void OnTenantContactTitleChange(ChangeEventArgs e)
    {
        TenantContactTitle = Convert.ToString(e.Value);
    }

    protected override async Task CreateEntityAsync()
    {
        if (base.NewEntity.AdminPassword is null ||
            base.NewEntity.AdminPassword.Length < 6 ||
            !Regex.IsMatch(base.NewEntity.AdminPassword, @"\W") ||
            !Regex.IsMatch(base.NewEntity.AdminPassword, "[a-z]") ||
            !Regex.IsMatch(base.NewEntity.AdminPassword, "[A-Z]"))
        {
            await _uiMessageService.Error("Passwords must be at least 6 characters., Passwords must have at least one non alphanumeric character., Passwords must have at least one lowercase ('a'-'z')., Passwords must have at least one uppercase ('A'-'Z').");

            return;
        }

        base.NewEntity.SetProperty("ShareProfitPercent", ShareProfitPercentage);
        base.NewEntity.SetProperty("TenantOwner", TenantOwnerId);
        base.NewEntity.SetProperty("Status", Status);
        base.NewEntity.SetProperty("BannerUrl", BannerUrl);
        base.NewEntity.SetProperty("TenantContactPerson", TenantContactPerson);
        base.NewEntity.SetProperty("TenantContactTitle", TenantContactTitle);
        base.NewEntity.SetProperty("TenantContactEmail", TenantContactEmail);
        base.NewEntity.SetProperty(Constant.Domain, Domain);
        base.NewEntity.SetProperty(Constant.TenantUrl, TenantUrl);

        if (ShortCode == null)
        {
            await _uiMessageService.Warn(_L["TenantShortCodeRequired"]);
            return;
        }
        if (!ValidateUrls().IsNullOrEmpty())
        {
            await _uiMessageService.Warn(_L[ValidateUrls()]);
            return;
        }
        var check = await _myTenantAppService.CheckShortCodeForCreateAsync(ShortCode);
        if (check)
        {
            await _uiMessageService.Warn(L["Short Code Already Exsist"]);
            return;
        }

        base.NewEntity.SetProperty("ShortCode", ShortCode);
        await base.CreateEntityAsync();
        LogoUrl = null;
        ShareProfitPercentage = 0;
        TenantOwnerId = null;
        Status = 0;
        BannerUrl = null;
        ShortCode = null;
        TenantContactPerson = null;
        TenantContactTitle = null;
        TenantContactEmail = null;
        Domain = null;
    }
    protected override async ValueTask SetTableColumnsAsync()
    {
        var columns = (await GetExtensionTableColumnsAsync(
            TenantManagementModuleExtensionConsts.ModuleName,
            TenantManagementModuleExtensionConsts.EntityNames.Tenant)).ToList();


        TenantManagementTableColumns
            .AddRange(new TableColumn[]
            {
                new TableColumn
                {
                    Title = L["Actions"],
                    Actions = EntityActions.Get<Volo.Abp.TenantManagement.Blazor.Pages.TenantManagement.TenantManagement>(),
                },
                new TableColumn
                {
                    Title = L["TenantName"],
                    Sortable = true,
                    Data = nameof(TenantDto.Name),
                },
               new TableColumn{
               Title="Logo",
               Sortable=false,
               //Data=columns[2].Data,
               Component=typeof(CustomTableColumn)


               },
                new TableColumn{
               Title=L["Banner"],
               Sortable=false,
               //Data=columns[2].Data,
               Component=typeof(BannerTableColumn)


               },
                   new TableColumn
                {
                    Title = _L["Owner"] ,
                    Sortable = true,
                   ///*Data*/=columns[0].Data,
                    Component=typeof(OwnerNameTableColumn)
                },
                   new TableColumn
                {
                    Title =_L["TenantContactPerson"] ,
                    Sortable = true,
                    Data=columns[1].Data
                },
                   new TableColumn
                {
                    Title = _L["TenantContactEmail"] ,
                    Sortable = true,
                    Data=columns[3].Data
                },

                  new TableColumn
                {
                    Title = _L["ShareProfit%"],
                    Sortable = true,
                   Data=columns[4].Data,
                },
                      new TableColumn
                {
                    Title =_L["Link"] ,
                    Sortable = true,
                   Data=columns[9].Data,
                },
                new() {
                    Title = _L["Domain"] ,
                    Sortable = true,
                    Component = typeof(DomainTableColumn)
                },
                    new TableColumn
                {
                    Title =_L["ShortCode"] ,
                    Sortable = true,
                   Data=columns[8].Data,
                },
                    new TableColumn
                {
                    Title = _L["Status"] ,
                    Sortable = true,
                   Data=Lo["Enum:TenantStatus."+columns[6].Data],
                },
            });

        //TableColumns.Get<Volo.Abp.TenantManagement.Blazor.Pages.TenantManagement.TenantManagement>().Add(confirmedColumn);
    }

    protected override Task OpenCreateModalAsync()
    {
        LogoUrl = null;
        ShareProfitPercentage = 0;
        TenantOwnerId = null;
        Status = 0;
        BannerUrl = null;
        ShortCode = null;
        TenantContactPerson = null;
        TenantContactTitle = null;
        TenantContactEmail = null;
        Domain = null;
        TenantUrl = null;
        return base.OpenCreateModalAsync();

    }
    protected override async Task UpdateEntityAsync()
    {
        if (ShortCode == null)
        {
            await _uiMessageService.Warn(L["Short Code Can't Null"]);
            return;
        }
        var check = await _myTenantAppService.CheckShortCodeForUpdate(ShortCode, base.EditingEntityId);
        if (check)
        {
            await _uiMessageService.Warn(L["Short Code Already Exsist"]);
            return;
        }
        if (!ValidateUrls().IsNullOrEmpty())
        {
            await _uiMessageService.Warn(_L[ValidateUrls()]);
            return;
        }
        base.EditingEntity.ExtraProperties.Remove("LogoUrl");
        base.EditingEntity.ExtraProperties.Remove("ShareProfitPercent");
        base.EditingEntity.ExtraProperties.Remove("TenantOwner");
        base.EditingEntity.ExtraProperties.Remove("Status");
        base.EditingEntity.ExtraProperties.Remove("BannerUrl");
        base.EditingEntity.ExtraProperties.Remove("TenantContactPerson");
        base.EditingEntity.ExtraProperties.Remove("TenantContactTitle");
        base.EditingEntity.ExtraProperties.Remove("TenantContactEmail");
        base.EditingEntity.ExtraProperties.Remove(Constant.Domain);
        base.EditingEntity.ExtraProperties.Remove("ShortCode");
        base.EditingEntity.ExtraProperties.Remove(Constant.TenantUrl);

        base.EditingEntity.SetProperty("LogoUrl", LogoUrl);
        base.EditingEntity.SetProperty("ShareProfitPercent", ShareProfitPercentage);
        base.EditingEntity.SetProperty("TenantOwner", TenantOwnerId);
        base.EditingEntity.SetProperty("Status", Status);
        base.EditingEntity.SetProperty("BannerUrl", BannerUrl);
        base.EditingEntity.SetProperty("ShortCode", ShortCode);
        base.EditingEntity.SetProperty("TenantContactPerson", TenantContactPerson);
        base.EditingEntity.SetProperty("TenantContactTitle", TenantContactTitle);
        base.EditingEntity.SetProperty("TenantContactEmail", TenantContactEmail);
        base.EditingEntity.SetProperty(Constant.Domain, Domain);
        base.EditingEntity.SetProperty(Constant.TenantUrl, TenantUrl);
        LogoUrl = null;
        ShareProfitPercentage = 0;
        TenantOwnerId = null;
        Status = 0;
        await base.UpdateEntityAsync();
    }

    protected override Task OpenEditModalAsync(TenantDto row)
    {
        TenantOwnerId = row.GetProperty<Guid?>("TenantOwner");
        ShareProfitPercentage = row.GetProperty<int>("ShareProfitPercent");
        LogoUrl = row.GetProperty<string>("LogoUrl");
        Status = row.GetProperty<TenantStatus>("Status");
        BannerUrl = row.GetProperty<string>("BannerUrl");
        ShortCode = row.GetProperty<string>("ShortCode");
        TenantUrl = row.GetProperty<string>("TenantUrl");
        TenantContactPerson = row.GetProperty<string>("TenantContactPerson");
        TenantContactTitle = row.GetProperty<string>("TenantContactTitle");
        TenantContactEmail = row.GetProperty<string>("TenantContactEmail");
        Domain = row.GetProperty<string>(Constant.Domain);
        return base.OpenEditModalAsync(row);
    }
    async Task OnFileUploadAsync(FileChangedEventArgs e)
    {
        if (e.Files.Count() > MaxAllowedFilesPerUpload)
        {
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesExceedMaxAllowedPerUpload]);
            await FilePickerCustom.Clear();
            return;
        }

        var count = 0;
        try
        {
            foreach (var file in e.Files)
            {
                if (!ValidFileExtensions.Contains(Path.GetExtension(file.Name).ToLower()))
                {
                    await FilePickerCustom.RemoveFile(file);
                    return;
                }
                if (file.Size > MaxAllowedFileSize)
                {
                    count++;
                    await FilePickerCustom.RemoveFile(file);
                    return;
                }
                string newFileName = Path.ChangeExtension(
                      Guid.NewGuid().ToString().Replace("-", ""),
                      Path.GetExtension(file.Name));
                var stream = file.OpenReadStream(long.MaxValue);
                try
                {
                    var memoryStream = new MemoryStream();

                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                    int sortNo = 0;
                    base.NewEntity.SetProperty("LogoUrl", url);
                    LogoUrl = url;

                    //await LogoPicker.Clear();
                }
                finally
                {
                    stream.Close();
                }
            }
            if (count > 0)
            {
                await _uiMessageService.Error(count + ' ' + L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    async Task OnBannerUploadAsync(FileChangedEventArgs e)
    {
        if (e.Files.Count() > MaxAllowedFilesPerUpload)
        {
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesExceedMaxAllowedPerUpload]);
            await BannerPickerCustom.Clear();
            return;
        }

        var count = 0;
        try
        {
            foreach (var file in e.Files)
            {
                if (!ValidFileExtensions.Contains(Path.GetExtension(file.Name).ToLower()))
                {
                    await BannerPickerCustom.RemoveFile(file);
                    return;
                }
                if (file.Size > MaxAllowedFileSize)
                {
                    count++;
                    await BannerPickerCustom.RemoveFile(file);
                    return;
                }
                string newFileName = Path.ChangeExtension(
                      Guid.NewGuid().ToString().Replace("-", ""),
                      Path.GetExtension(file.Name));
                var stream = file.OpenReadStream(long.MaxValue);
                try
                {
                    var memoryStream = new MemoryStream();

                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                    int sortNo = 0;
                    base.NewEntity.SetProperty("BannerUrl", url);
                    BannerUrl = url;

                    //await LogoPicker.Clear();
                }
                finally
                {
                    stream.Close();
                }
            }
            if (count > 0)
            {
                await _uiMessageService.Error(count + ' ' + L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    private string ValidateUrls()
    {
        // Check if the domain is not empty
        if (string.IsNullOrWhiteSpace(Domain))
        {
            return _L["DomainIsRequired"];
        }

        // Check if the domain is not empty
        if (string.IsNullOrWhiteSpace(TenantUrl))
        {
            return _L["TenantUrlIsRequired"];
        }

        // Check if the domain is a valid URL with http or https
        if (!Uri.TryCreate(Domain, UriKind.Absolute, out var domainUriResult) ||
            (domainUriResult.Scheme != Uri.UriSchemeHttp && domainUriResult.Scheme != Uri.UriSchemeHttps))
        {
            return _L["InvalidUrlWithField", nameof(Domain)];
        }

        if (!Uri.TryCreate(TenantUrl, UriKind.Absolute, out var tenantUriResult) ||
            (tenantUriResult.Scheme != Uri.UriSchemeHttp && tenantUriResult.Scheme != Uri.UriSchemeHttps))
        {
            return _L["InvalidUrlWithField", nameof(TenantUrl)];
        }

        return null;
    }
}


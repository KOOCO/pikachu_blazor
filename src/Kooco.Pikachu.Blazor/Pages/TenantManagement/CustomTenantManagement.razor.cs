

using Blazorise;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Blazor.Pages.ItemManagement;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.EntityActions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.FeatureManagement.Blazor.Components;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Blazor.Pages.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Blazor;
using Volo.Abp.TenantManagement.Localization;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement
{
  

    public partial class CustomTenantManagement
    {
        private const int MaxTextCount = 60; //input max length
        private const int MaxAllowedFilesPerUpload = 10;
        private const int TotalMaxAllowedFiles = 50;
        private const int MaxAllowedFileSize = 1024 * 1024 * 10;
        private readonly List<string> ValidFileExtensions = new() { ".jpg", ".png", ".svg", ".jpeg", ".webp" };
        private FilePicker FilePickerCustom { get; set; }
        private FilePicker BannerPickerCustom { get; set; }
        public Guid? TenantOwnerId { get; set; }
        public TenantStatus Status { get; set; }
        IReadOnlyList<IdentityUserDto> UsersList = Array.Empty<IdentityUserDto>();
        public int ShareProfitPercentage { get; set; }
        public string LogoUrl { get; set; }
        public string BannerUrl { get; set; }
        private readonly IUiMessageService _uiMessageService;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly IIdentityUserAppService _identityUserAppService;
        public CustomTenantManagement(IUiMessageService uiMessageService,
            ImageContainerManager imageContainerManager,
            IIdentityUserAppService identityUserAppService)
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
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var users=  await _identityUserAppService.GetListAsync(new GetIdentityUsersInput() {MaxResultCount=1000,SkipCount=0 });
            UsersList = users?.Items.ToList();
        
        }

        protected override async Task CreateEntityAsync() {
            base.NewEntity.SetProperty("ShareProfitPercent", ShareProfitPercentage);
            base.NewEntity.SetProperty("TenantOwner", TenantOwnerId);
            base.NewEntity.SetProperty("Status", Status);
            base.NewEntity.SetProperty("BannerUrl", BannerUrl);
            await base.CreateEntityAsync();
            LogoUrl = null;
            ShareProfitPercentage = 0;
            TenantOwnerId = null;
            Status = 0;
            BannerUrl = null;
           
        }
        protected override ValueTask SetTableColumnsAsync()
        {
          
            var columns = (GetExtensionTableColumns(
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
                   Title="Banner",
                   Sortable=false,
                   //Data=columns[2].Data,
                   Component=typeof(BannerTableColumn)


                   },
                       new TableColumn
                    {
                        Title ="Owner" ,
                        Sortable = true,
                       ///*Data*/=columns[0].Data,
                        Component=typeof(OwnerNameTableColumn)
                    },
                      new TableColumn
                    {
                        Title ="Share Profit %" ,
                        Sortable = true,
                       Data=columns[1].Data,
                    },
                        new TableColumn
                    {
                        Title ="Status" ,
                        Sortable = true,
                       Data=Lo["Enum:TenantStatus."+columns[3].Data],
                    },
                });

            //TableColumns.Get<Volo.Abp.TenantManagement.Blazor.Pages.TenantManagement.TenantManagement>().Add(confirmedColumn);

            return new ValueTask();
           


        }
        protected override Task OpenCreateModalAsync()
        {
            LogoUrl = null;
            ShareProfitPercentage = 0;
            TenantOwnerId = null;
            Status = 0;
            BannerUrl = null;
           return base.OpenCreateModalAsync();
        
        }
        protected override  Task UpdateEntityAsync()
        {
            base.EditingEntity.ExtraProperties.Remove("LogoUrl") ;
            base.EditingEntity.ExtraProperties.Remove("ShareProfitPercent");
            base.EditingEntity.ExtraProperties.Remove("TenantOwner");
            base.EditingEntity.ExtraProperties.Remove("Status");
            base.EditingEntity.ExtraProperties.Remove("BannerUrl");
            base.EditingEntity.SetProperty("LogoUrl", LogoUrl);
            base.EditingEntity.SetProperty("ShareProfitPercent", ShareProfitPercentage);
            base.EditingEntity.SetProperty("TenantOwner", TenantOwnerId);
            base.EditingEntity.SetProperty("Status", Status);
            base.EditingEntity.SetProperty("BannerUrl",BannerUrl);
            LogoUrl = null;
            ShareProfitPercentage = 0;
            TenantOwnerId = null;
            Status = 0;
            return  base.UpdateEntityAsync();
           

        }
        protected override Task OpenEditModalAsync(TenantDto row) {

            TenantOwnerId = row.GetProperty<Guid?>("TenantOwner");
            ShareProfitPercentage=row.GetProperty<int>("ShareProfitPercent");
            LogoUrl = row.GetProperty<string>("LogoUrl");
            Status = row.GetProperty<TenantStatus>("Status");
            BannerUrl = row.GetProperty<string>("BannerUrl");
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

                        //await FilePickerCustom.Clear();
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

                        //await FilePickerCustom.Clear();
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

    }
   
}

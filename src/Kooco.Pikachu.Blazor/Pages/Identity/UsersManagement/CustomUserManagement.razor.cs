using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Identity;
using Kooco.Pikachu.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Blazor.Pages.Identity;
using Volo.Abp.ObjectExtending;

namespace Kooco.Pikachu.Blazor.Pages.Identity.UsersManagement
{
    public partial class CustomUserManagement
    {
        #region Inject
        private List<TableColumn> CustomUserManagementTableColumns => base.TableColumns.Get<UserManagement>();

        TextRole _passwordTextRole = TextRole.Password;

        private GetCategorizedListDto Filters { get; set; } = new() { UserTypes = UserTypes.Backend };
        #endregion

        #region Methods
        public void ChangePassword(TextRole? textRole)
        {
            if (textRole is null)
            {
                ChangePassword(_passwordTextRole == TextRole.Password ? TextRole.Text : TextRole.Password);
                ShowPassword = !ShowPassword;
            }

            else _passwordTextRole = textRole.Value;
        }

        protected override async ValueTask SetTableColumnsAsync()
        {
            CustomUserManagementTableColumns
                .AddRange(new TableColumn[]
                {
                    new TableColumn
                    {
                        Title = L["Actions"],
                        Actions = EntityActions.Get<UserManagement>(),
                    },
                    new TableColumn
                    {
                        Title = l["DisplayName:UserName"],
                        Data = nameof(IdentityUserDto.UserName),
                        Sortable = true,
                    },
                    new TableColumn
                    {
                        Title = l["DisplayName:name"],
                        Data = nameof(IdentityUserDto.Name),
                        Sortable = true,
                    },
                    new TableColumn
                    {
                        Title = L["EmailAddress"],
                        Data = nameof(IdentityUserDto.Email),
                        Sortable = true,
                    },
                    new TableColumn
                    {
                        Title = L["PhoneNumber"],
                        Data = nameof(IdentityUserDto.PhoneNumber),
                        Sortable = true,
                    }
                });

            var columns = await GetExtensionTableColumnsAsync(IdentityModuleExtensionConsts.ModuleName,
                IdentityModuleExtensionConsts.EntityNames.User);
            CustomUserManagementTableColumns.AddRange(columns);
        }

        protected override async Task CreateEntityAsync()
        {
            if (base.NewEntity.Name.IsNullOrEmpty()) await _UiMessageService.Error(l[PikachuResource.NameCannotBeEmpty]);

            else await base.CreateEntityAsync();
            await GetCategorizedListAsync();
        }

        protected async override Task UpdateEntityAsync()
        {
            if (base.EditingEntity.Name.IsNullOrEmpty()) await _UiMessageService.Error(l[PikachuResource.NameCannotBeEmpty]);

            else await base.UpdateEntityAsync();
            await GetCategorizedListAsync();
        }

        protected override async Task OnSearchTextChanged(string value)
        {
            Filters.Filter = value;
            CurrentPage = 1;
            await GetCategorizedListAsync();
        }

        private async Task GetCategorizedListAsync()
        {
            try
            {
                var data = await MyIdentityUserAppService.GetCategorizedListAsync(
                    new GetCategorizedListDto
                    {
                        SkipCount = (CurrentPage - 1) * PageSize,
                        MaxResultCount = PageSize,
                        Sorting = CurrentSorting,
                        Filter = Filters.Filter,
                        UserTypes = UserTypes.Backend
                    });
                Entities = data.Items;
                TotalCount = (int)data.TotalCount;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private new async Task OnDataGridReadAsync(DataGridReadDataEventArgs<IdentityUserDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.SortField + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;

            await GetCategorizedListAsync();

            await InvokeAsync(StateHasChanged);
        }
        #endregion
    }
}
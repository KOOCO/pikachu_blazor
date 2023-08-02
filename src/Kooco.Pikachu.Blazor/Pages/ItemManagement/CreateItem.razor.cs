using Blazorise;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement 
{
    public partial class CreateItem
    {
        protected Validations CreateValidationsRef;
        protected CreateUpdateItemDto NewEntity = new();
        IReadOnlyList<ItemDto> itemList = Array.Empty<ItemDto>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (CreateValidationsRef != null)
                await CreateValidationsRef.ClearAll();
        }

        protected virtual async Task CreateEntityAsync()
        {
            try
            {
                    await AppService.CreateAsync(NewEntity);
                    NavigationManager.NavigateTo("Items");
            }
            catch(Exception ex)
            {
                //if the environment is debug then throw the exception
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    throw new UserFriendlyException("Unable to Save", ex.Source, ex.Message);
                else
                    throw new UserFriendlyException("Unable to Save");



            }
        }
    }
}

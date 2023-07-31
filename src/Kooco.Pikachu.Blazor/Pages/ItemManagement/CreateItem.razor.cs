using Blazorise;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                //var validate = true;
                //if (CreateValidationsRef != null)
                //{
                //    validate = await CreateValidationsRef.ValidateAll();
                //}
                //if (validate)
                //{
                    await AppService.CreateAsync(NewEntity);
                    NavigationManager.NavigateTo("ItemList");
                //}
            }
            catch(Exception ex)
            {
            }

        }
    }
}

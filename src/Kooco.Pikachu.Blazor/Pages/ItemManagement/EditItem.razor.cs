using Volo.Abp.Application.Services;
using Blazorise;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class EditItem
    {
        protected Validations CreateValidationsRef;
        protected CreateUpdateItemDto EditingEntity = new();
        IReadOnlyList<ItemDto> itemList = Array.Empty<ItemDto>();
        [Parameter] 
        public string Id {get;set;}
        public Guid EditingEntityId { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (CreateValidationsRef != null)
                await CreateValidationsRef.ClearAll();

            EditingEntityId = Guid.Parse(Id);

            var entityDto = await AppService.GetAsync(EditingEntityId);
            EditingEntity = new CreateUpdateItemDto
            {
                ItemName = entityDto.ItemName,
                SKU = entityDto.SKU,
                SellingPrice = entityDto.SellingPrice,
                Returnable = entityDto.Returnable,
                OpeningStock = entityDto.OpeningStock,
                ItemDescription = entityDto.ItemDescription
            };
        }

        protected virtual async Task EditEntityAsync()
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
                    await AppService.UpdateAsync(EditingEntityId, EditingEntity);
                    NavigationManager.NavigateTo("ItemList");
                //}
            }
            catch { }

        }
    }
}

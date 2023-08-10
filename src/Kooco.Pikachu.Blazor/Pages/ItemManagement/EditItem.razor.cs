using Volo.Abp.Application.Services;
using Blazorise;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class EditItem
    {
        protected Validations CreateValidationsRef;
        protected UpdateItemDto EditingEntity = new();
        [Parameter]
        public string Id { get; set; }
        public Guid EditingEntityId { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (CreateValidationsRef != null)
                await CreateValidationsRef.ClearAll();

            EditingEntityId = Guid.Parse(Id);

            var entityDto = await AppService.GetAsync(EditingEntityId);
            EditingEntity = new UpdateItemDto
            {
                ItemName = entityDto.ItemName,
                Returnable = entityDto.Returnable,
                ItemDescription = entityDto.ItemDescription
            };
        }

        protected virtual async Task EditEntityAsync()
        {
            try
            {
                await AppService.UpdateAsync(EditingEntityId, EditingEntity);
                NavigationManager.NavigateTo("Items");
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Unable to Save");
            }
        }
    }
}

using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace Kooco.Pikachu.SetItems
{
    public class SetItemAppServiceTests : PikachuApplicationTestBase
    {
        private readonly ISetItemAppService _setItemAppService;

        public SetItemAppServiceTests()
        {
            _setItemAppService = GetRequiredService<ISetItemAppService>();
        }

        [Fact]
        public async Task CreateAsync_Should_Create()
        {
            var input = GetInput();
            input.SetItemDetails.Clear();
            input.Images.Clear();

            var setItem = await _setItemAppService.CreateAsync(input);
            setItem.ShouldNotBeNull();
            setItem.Id.ShouldNotBe(Guid.Empty);
            setItem.SetItemName.ShouldBe(input.SetItemName);
            setItem.SetItemBadge.ShouldBe(input.SetItemBadge);
            setItem.SetItemDescriptionTitle.ShouldBe(input.SetItemDescriptionTitle);
            setItem.Description.ShouldBe(input.Description);
            setItem.SetItemMainImageURL.ShouldBe(input.SetItemMainImageURL);
            setItem.SetItemPrice.ShouldBe(input.SetItemPrice);
            setItem.LimitQuantity.ShouldBe(input.LimitQuantity);
            setItem.LimitAvaliableTimeStart.ShouldBe(input.LimitAvaliableTimeStart);
            setItem.LimitAvaliableTimeEnd.ShouldBe(input.LimitAvaliableTimeEnd);
            setItem.ShareProfit.ShouldBe(input.ShareProfit);
            setItem.IsFreeShipping.ShouldBe(input.IsFreeShipping);
            setItem.ItemStorageTemperature.ShouldBe(input.ItemStorageTemperature);
            setItem.SaleableQuantity.ShouldBe(input.SaleableQuantity);
            setItem.GroupBuyPrice.ShouldBe(input.GroupBuyPrice);
            setItem.SetItemDetails.Count.ShouldBe(0);
            setItem.Images.Count.ShouldBe(0);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_Exception_On_Same_Name()
        {
            var input = GetInput();
            input.SetItemDetails.Clear();
            input.Images.Clear();

            var setItem = await _setItemAppService.CreateAsync(input);
            setItem.SetItemName.ShouldBe(input.SetItemName);

            var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
                await _setItemAppService.CreateAsync(input)
                );
            exception.Code
                .ShouldNotBeNull()
                .ShouldContain(PikachuDomainErrorCodes.ItemWithSameNameAlreadyExists);

            setItem.SetItemDetails.Count.ShouldBe(0);
            setItem.Images.Count.ShouldBe(0);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_With_Item_Details()
        {
            var input = GetInput();
            input.Images.Clear();

            var setItem = await _setItemAppService.CreateAsync(input);
            setItem.ShouldNotBeNull();
            setItem.Id.ShouldNotBe(Guid.Empty);

            setItem.SetItemDetails.Count.ShouldBe(1);
            setItem.Images.Count.ShouldBe(0);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_With_Images()
        {
            var input = GetInput();
            input.SetItemDetails.Clear();

            var setItem = await _setItemAppService.CreateAsync(input);
            setItem.ShouldNotBeNull();
            setItem.Id.ShouldNotBe(Guid.Empty);

            setItem.SetItemDetails.Count.ShouldBe(0);
            setItem.Images.Count.ShouldBe(2);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_With_Item_Details_And_Images()
        {
            var input = GetInput();

            var setItem = await _setItemAppService.CreateAsync(input);
            setItem.ShouldNotBeNull();
            setItem.Id.ShouldNotBe(Guid.Empty);

            setItem.SetItemDetails.Count.ShouldBe(1);
            setItem.Images.Count.ShouldBe(2);
        }

        [Fact]
        public async Task UpdateAsync_Should_Not_Throw_Exception_On_Same_Name_For_Same_Entity()
        {
            var input = GetInput();
            input.SetItemDetails.Clear();
            input.Images.Clear();

            var setItem = await _setItemAppService.CreateAsync(input);
            setItem.SetItemName.ShouldBe(input.SetItemName);

            setItem.SetItemDetails.Count.ShouldBe(0);
            setItem.Images.Count.ShouldBe(0);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_Exception_On_Same_Name()
        {
            var input = GetInput();
            input.SetItemDetails.Clear();
            input.Images.Clear();

            var setItem = await _setItemAppService.CreateAsync(input);
            setItem.SetItemName.ShouldBe(input.SetItemName);
            setItem.SetItemDetails.ShouldBeEmpty();
            setItem.Images.ShouldBeEmpty();

            input.SetItemName = "Set item 2";
            var duplicateSetItem = await _setItemAppService.CreateAsync(input);

            var setItemId = duplicateSetItem.Id;
            input = GetInput();

            var exception = await Assert.ThrowsAsync<BusinessException>(() =>
                _setItemAppService.UpdateAsync(setItemId, input)
            );

            exception.Code
                .ShouldNotBeNull()
                .ShouldContain(PikachuDomainErrorCodes.ItemWithSameNameAlreadyExists);
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_SetItem()
        {
            var input = GetInput();
            var setItem = await _setItemAppService.CreateAsync(input);
            setItem.ShouldNotBeNull();

            var updateInput = GetInput();
            updateInput.SetItemName = "Updated Set Item";
            updateInput.SetItemPrice = 150;

            var updatedSetItem = await _setItemAppService.UpdateAsync(setItem.Id, updateInput);

            updatedSetItem.SetItemName.ShouldBe(updateInput.SetItemName);
            updatedSetItem.SetItemPrice.ShouldBe(updateInput.SetItemPrice);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_Exception_For_NonExisting_Id()
        {
            var input = GetInput();
            var nonExistingId = Guid.NewGuid();

            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _setItemAppService.UpdateAsync(nonExistingId, input)
                );

            exception.Message.ShouldContain("SetItem");
        }

        [Fact]
        public async Task GetAsync_Should_Return_SetItem()
        {
            var input = GetInput();
            var setItem = await _setItemAppService.CreateAsync(input);

            var retrievedSetItem = await _setItemAppService.GetAsync(setItem.Id);

            retrievedSetItem.ShouldNotBeNull();
            retrievedSetItem.Id.ShouldBe(setItem.Id);
            retrievedSetItem.SetItemName.ShouldBe(setItem.SetItemName);
        }

        #region INPUT
        private static CreateUpdateSetItemDto GetInput()
        {
            var input = new CreateUpdateSetItemDto()
            {
                SetItemName = "Set Item",
                SetItemBadge = "1",
                SetItemDescriptionTitle = "Sample Description Title",
                Description = "This is a sample description",
                SetItemMainImageURL = "https://example.com/sample-image.jpg",
                SetItemPrice = 100,
                LimitQuantity = 10,
                LimitAvaliableTimeStart = DateTime.UtcNow,
                LimitAvaliableTimeEnd = DateTime.UtcNow.AddDays(7),
                ShareProfit = 5,
                IsFreeShipping = true,
                ItemStorageTemperature = ItemStorageTemperature.Normal,
                SaleableQuantity = 50,
                GroupBuyPrice = 90,
                SetItemDetails = [
                    new()
                    {
                        ItemId = Guid.Parse("170a29a7-95cf-4897-b60c-30e1f654ab0a"),
                        Quantity = 1,
                        Attribute1Value = "Attribute 1",
                        Attribute2Value = "Attribute 2",
                        Attribute3Value = "Attribute 3"
                    }],
                Images = [
                    new()
                    {
                        Name = "Image 1.png",
                        BlobImageName = "Image 1.png",
                        ImageUrl = "https://www.google.com",
                        SortNo = 1
                    },
                    new()
                    {
                        Name = "Image 2.png",
                        BlobImageName = "Image 2.png",
                        ImageUrl = "https://www.google.com",
                        SortNo = 2
                    }]
            };

            return input;
        }
        #endregion
    }
}

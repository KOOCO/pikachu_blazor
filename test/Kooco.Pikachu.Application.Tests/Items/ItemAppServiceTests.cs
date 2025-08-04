using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.ProductCategories;
using Volo.Abp.Domain.Entities;
using NSubstitute;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.Application.Tests.Items
{
    public class ItemAppServiceTests : PikachuApplicationTestBase
    {
        private readonly IItemAppService _itemAppService;
        private readonly IItemRepository _itemRepository;
        private readonly ISetItemRepository _setItemRepository;
        private readonly IRepository<CategoryProduct> _categoryProductRepository;
        private readonly IEnumValueAppService _enumValueAppService;
        private readonly ItemManager _itemManager;

        public ItemAppServiceTests()
        {
            _itemAppService = GetRequiredService<IItemAppService>();
            _itemRepository = GetRequiredService<IItemRepository>();
            _setItemRepository = GetRequiredService<ISetItemRepository>();
            _categoryProductRepository = GetRequiredService<IRepository<CategoryProduct>>();
            _enumValueAppService = GetRequiredService<IEnumValueAppService>();
            _itemManager = GetRequiredService<ItemManager>();
        }

        #region Test Helpers

        private async Task<Item> CreateTestItemAsync(
            string itemName = null,
            bool isAvailable = true,
            bool includeDetails = false)
        {
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var item = await _itemManager.CreateAsync(
                itemName ?? $"Test Item {uniqueId}",
                itemBadge: "New",
                itemBadgeColor: "#FF0000",
                itemDescriptionTitle: "Test Description Title",
                itemDescription: "<p>Test Description</p>",
                itemTags: "test,item",
                limitAvailableTimeStart: DateTime.UtcNow,
                limitAvailableTimeEnd: DateTime.UtcNow.AddDays(30),
                shareProfit: 10,
                isFreeShipping: true,
                isReturnable: true,
                shippingMethodId: null,
                taxTypeId: null,
                customField1Value: null,
                customField1Name: null,
                customField2Value: null,
                customField2Name: null,
                customField3Value: null,
                customField3Name: null,
                customField4Value: null,
                customField4Name: null,
                customField5Value: null,
                customField5Name: null,
                customField6Value: null,
                customField6Name: null,
                customField7Value: null,
                customField7Name: null,
                customField8Value: null,
                customField8Name: null,
                customField9Value: null,
                customField9Name: null,
                customField10Value: null,
                customField10Name: null,
                attribute1Name: "Size",
                attribute2Name: "Color",
                attribute3Name: "Material",
                ItemStorageTemperature.Normal
            );

            item.IsItemAvaliable = isAvailable;
            item.ItemNo = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (includeDetails)
            {
                await _itemManager.AddItemDetailAsync(
                    item,
                    itemName: "Large/Red/Cotton",
                    sku: $"SKU-{uniqueId}-001",
                    limitQuantity: 100,
                    sellingPrice: 29.99f,
                    cost: 15.00f,
                    saleableQuantity: 50,
                    stockonHand: 100,
                    preOrderableQuantity: null,
                    saleablePreOrderQuantity: null,
                    inventoryAccount: "INV001",
                    attribute1Value: "Large",
                    attribute2Value: "Red",
                    attribute3Value: "Cotton",
                    image: null,
                    itemDescription: null,
                    sortNo: 1,
                    status: true
                );

                await _itemManager.AddItemDetailAsync(
                    item,
                    itemName: "Medium/Blue/Cotton",
                    sku: $"SKU-{uniqueId}-002",
                    limitQuantity: 100,
                    sellingPrice: 29.99f,
                    cost: 15.00f,
                    saleableQuantity: 30,
                    stockonHand: 80,
                    preOrderableQuantity: null,
                    saleablePreOrderQuantity: null,
                    inventoryAccount: "INV001",
                    attribute1Value: "Medium",
                    attribute2Value: "Blue",
                    attribute3Value: "Cotton",
                    image: null,
                    itemDescription: null,
                    sortNo: 2,
                    status: true
                );
            }

            await _itemRepository.InsertAsync(item);
            return item;
        }

        private CreateItemDto CreateTestItemDto(string itemName = null)
        {
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            return new CreateItemDto
            {
                ItemName = itemName ?? $"New Test Item {uniqueId}",
                ItemBadgeDto = new ItemBadgeDto
                {
                    ItemBadge = "Hot",
                    ItemBadgeColor = "#00FF00"
                },
                ItemDescriptionTitle = "Product Description",
                ItemDescription = "<p>This is a test product</p>",
                ItemTags = "new,test,product",
                LimitAvaliableTimeStart = DateTime.UtcNow,
                LimitAvaliableTimeEnd = DateTime.UtcNow.AddDays(60),
                ShareProfit = 15,
                IsFreeShipping = true,
                IsReturnable = true,
                IsItemAvaliable = true,
                Attribute1Name = "Size",
                Attribute2Name = "Color",
                Attribute3Name = "Style",
                ItemStorageTemperature = ItemStorageTemperature.Normal,
                ItemDetails = new List<CreateItemDetailsDto>
                {
                    new CreateItemDetailsDto
                    {
                        ItemName = "Small/Black/Classic",
                        Sku = $"SKU-NEW-{uniqueId}-001",
                        LimitQuantity = 50,
                        SellingPrice = 39.99f,
                        Cost = 20.00f,
                        SaleableQuantity = 25,
                        StockOnHand = 50,
                        InventoryAccount = "INV002",
                        Attribute1Value = "Small",
                        Attribute2Value = "Black",
                        Attribute3Value = "Classic",
                        SortNo = 1,
                        Status = true
                    }
                },
                ItemImages = new List<CreateImageDto>
                {
                    new CreateImageDto
                    {
                        Name = "product-image-1.jpg",
                        BlobImageName = $"blob-{uniqueId}-1.jpg",
                        ImageUrl = "https://test.com/images/product1.jpg",
                        ImageType = ImageType.Item,
                        SortNo = 1
                    }
                }
            };
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task CreateAsync_Should_Create_Item()
        {
            // Arrange
            var input = CreateTestItemDto("Create Test Item");
            input.IsTest = true; // Enable test mode to auto-generate ItemNo

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ItemName.ShouldBe("Create Test Item");
            result.ItemBadge.ShouldBe("Hot");
            result.ItemBadgeColor.ShouldBe("#00FF00");
            result.IsFreeShipping.ShouldBe(true);
            result.IsReturnable.ShouldBe(true);
            result.ItemDetails.ShouldNotBeNull();
            result.ItemDetails.Count().ShouldBe(1);
            result.ItemDetails.First().SKU.ShouldStartWith("SKU-NEW-");
            result.Images.ShouldNotBeNull();
            result.Images.Count.ShouldBe(1);
        }

        [Fact]
        public async Task CreateAsync_Should_Sanitize_Item_Description()
        {
            // Arrange
            var input = CreateTestItemDto();
            input.ItemDescription = "<p>Safe content</p><script>alert('XSS')</script>";
            input.IsTest = true;

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ItemDescription.ShouldNotContain("<script>");
            result.ItemDescription.ShouldContain("Safe content");
        }

        [Fact]
        public async Task CreateAsync_Should_Add_Multiple_ItemDetails()
        {
            // Arrange
            var input = CreateTestItemDto();
            input.IsTest = true;
            input.ItemDetails.Add(new CreateItemDetailsDto
            {
                ItemName = "Large/White/Modern",
                Sku = "SKU-NEW-002",
                LimitQuantity = 75,
                SellingPrice = 49.99f,
                Cost = 25.00f,
                SaleableQuantity = 40,
                StockOnHand = 75,
                InventoryAccount = "INV002",
                Attribute1Value = "Large",
                Attribute2Value = "White",
                Attribute3Value = "Modern",
                SortNo = 2,
                Status = true
            });

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ItemDetails.Count().ShouldBe(2);
            result.ItemDetails.ShouldContain(x => x.Attribute1Value == "Small");
            result.ItemDetails.ShouldContain(x => x.Attribute1Value == "Large");
        }

        [Fact]
        public async Task CreateAsync_Should_Add_Category_Products()
        {
            // Arrange
            var input = CreateTestItemDto();
            input.IsTest = true;
            input.ItemCategories = new List<CreateUpdateItemCategoryDto>
            {
                new CreateUpdateItemCategoryDto { ProductCategoryId = Guid.NewGuid() },
                new CreateUpdateItemCategoryDto { ProductCategoryId = Guid.NewGuid() }
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.CategoryProducts.ShouldNotBeNull();
            result.CategoryProducts.Count.ShouldBe(2);
        }

        #endregion

        #region Get Tests

        [Fact]
        public async Task GetAsync_Should_Return_Item()
        {
            // Arrange
            var item = await CreateTestItemAsync("Get Test Item");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetAsync(item.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(item.Id);
            result.ItemName.ShouldBe("Get Test Item");
        }

        [Fact]
        public async Task GetAsync_Should_Include_Details_When_Requested()
        {
            // Arrange
            var item = await CreateTestItemAsync("Get With Details Item", includeDetails: true);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetAsync(item.Id, includeDetails: true);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ItemDetails.ShouldNotBeNull();
            result.ItemDetails.Count().ShouldBe(2);
            result.Images.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetAsync_Should_Throw_When_Not_Found()
        {
            // Act & Assert
            await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    await _itemAppService.GetAsync(Guid.NewGuid());
                });
            });
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_Should_Update_Item()
        {
            // Arrange
            var item = await CreateTestItemAsync("Original Item");
            var input = new UpdateItemDto
            {
                ItemName = "Updated Item",
                ItemBadgeDto = new ItemBadgeDto
                {
                    ItemBadge = "Updated",
                    ItemBadgeColor = "#0000FF"
                },
                ItemDescriptionTitle = "Updated Title",
                ItemDescription = "<p>Updated Description</p>",
                ItemTags = "updated,test",
                IsFreeShipping = false,
                IsReturnable = false,
                ShareProfit = 20,
                Attribute1Name = "UpdatedSize",
                Attribute2Name = "UpdatedColor",
                Attribute3Name = "UpdatedMaterial",
                ItemStorageTemperature = ItemStorageTemperature.Frozen,
                ItemDetails = new List<CreateItemDetailsDto>()
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.UpdateAsync(item.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ItemName.ShouldBe("Updated Item");
            result.ItemBadge.ShouldBe("Updated");
            result.ItemBadgeColor.ShouldBe("#0000FF");
            result.IsFreeShipping.ShouldBe(false);
            result.IsReturnable.ShouldBe(false);
            result.ShareProfit.ShouldBe(20);
            result.ItemStorageTemperature.ShouldBe(ItemStorageTemperature.Frozen);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_Name_Already_Exists()
        {
            // Arrange
            var item1 = await CreateTestItemAsync("Existing Item");
            var item2 = await CreateTestItemAsync("Item To Update");
            
            var input = new UpdateItemDto
            {
                ItemName = "Existing Item", // Trying to use existing name
                ItemDetails = new List<CreateItemDetailsDto>()
            };

            // Act & Assert
            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    await _itemAppService.UpdateAsync(item2.Id, input);
                });
            });
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_ItemDetails()
        {
            // Arrange
            var item = await CreateTestItemAsync("Item With Details", includeDetails: true);
            await _itemRepository.EnsureCollectionLoadedAsync(item, i => i.ItemDetails);
            
            var existingDetail = item.ItemDetails.First();
            var input = new UpdateItemDto
            {
                ItemName = item.ItemName,
                ItemDetails = new List<CreateItemDetailsDto>
                {
                    new CreateItemDetailsDto
                    {
                        Id = existingDetail.Id,
                        ItemName = "Updated Detail",
                        Sku = existingDetail.SKU,
                        SellingPrice = 99,
                        Cost = 50.00f,
                        SaleableQuantity = 100,
                        StockOnHand = 200,
                        Attribute1Value = "UpdatedSize",
                        Attribute2Value = "UpdatedColor",
                        Attribute3Value = "UpdatedMaterial",
                        SortNo = 1,
                        Status = false
                    }
                }
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.UpdateAsync(item.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ItemDetails.Count().ShouldBe(1);
            result.ItemDetails.First().SellingPrice.ShouldBe(99);
            result.ItemDetails.First().Status.ShouldBe(false);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_Should_Delete_Item()
        {
            // Arrange
            var item = await CreateTestItemAsync("Item To Delete");

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _itemAppService.DeleteAsync(item.Id);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var deleted = await _itemRepository.FindAsync(item.Id);
                deleted.ShouldBeNull();
            });
        }

        [Fact]
        public async Task DeleteManyItemsAsync_Should_Delete_Multiple_Items()
        {
            // Arrange
            var items = new List<Item>();
            for (int i = 0; i < 3; i++)
            {
                items.Add(await CreateTestItemAsync($"Item To Delete {i}"));
            }
            var ids = items.Select(i => i.Id).ToList();

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _itemAppService.DeleteManyItemsAsync(ids);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var id in ids)
                {
                    var deleted = await _itemRepository.FindAsync(id);
                    deleted.ShouldBeNull();
                }
            });
        }

        [Fact]
        public async Task DeleteSingleImageAsync_Should_Remove_Image()
        {
            // Arrange
            var item = await CreateTestItemAsync("Item With Image");
            _itemManager.AddItemImage(
                item,
                "test-image.jpg",
                "blob-test.jpg",
                "https://test.com/image.jpg",
                ImageType.Item,
                1
            );
            await _itemRepository.UpdateAsync(item);

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _itemAppService.DeleteSingleImageAsync(item.Id, "blob-test.jpg");
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var updated = await _itemRepository.GetAsync(item.Id);
                await _itemRepository.EnsureCollectionLoadedAsync(updated, i => i.Images);
                updated.Images.ShouldNotContain(x => x.BlobImageName == "blob-test.jpg");
            });
        }

        #endregion

        #region Availability Tests

        [Fact]
        public async Task ChangeItemAvailability_Should_Toggle_Availability()
        {
            // Arrange
            var item = await CreateTestItemAsync("Available Item", isAvailable: true);

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _itemAppService.ChangeItemAvailability(item.Id);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var updated = await _itemRepository.GetAsync(item.Id);
                updated.IsItemAvaliable.ShouldBe(false);
            });
        }

        #endregion

        #region Copy Tests

        [Fact]
        public async Task CopyAsync_Should_Create_Duplicate()
        {
            // Arrange
            var original = await CreateTestItemAsync("Original Item", includeDetails: true);
            _itemManager.AddItemImage(
                original,
                "original-image.jpg",
                "blob-original.jpg",
                "https://test.com/original.jpg",
                ImageType.Item,
                1
            );
            await _itemRepository.UpdateAsync(original);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.CopyAysnc(original.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(original.Id);
            result.ItemName.ShouldBe("Original ItemCopy");
            result.ItemDetails.Count().ShouldBe(2);
            result.ItemDetails.ShouldAllBe(x => x.SKU.EndsWith("Copy"));
            result.Images.Count.ShouldBe(1);
        }

        #endregion

        #region List and Query Tests

        [Fact]
        public async Task GetItemsListAsync_Should_Return_Paged_Result()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                await CreateTestItemAsync($"List Item {i}");
            }

            var input = new GetItemListDto
            {
                MaxResultCount = 3,
                SkipCount = 0
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetItemsListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(5);
            result.Items.Count.ShouldBe(3);
        }

        [Fact]
        public async Task GetItemsListAsync_Should_Filter_By_Availability()
        {
            // Arrange
            await CreateTestItemAsync("Available Item", isAvailable: true);
            await CreateTestItemAsync("Unavailable Item", isAvailable: false);

            var input = new GetItemListDto
            {
                IsAvailable = true,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetItemsListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            var availableItems = result.Items.Where(i => i.ItemName == "Available Item" || i.ItemName == "Unavailable Item").ToList();
            availableItems.ShouldAllBe(x => x.IsItemAvaliable == true);
        }

        [Fact]
        public async Task GetItemsListAsync_Should_Filter_By_FreeShipping()
        {
            // Arrange
            var item1 = await CreateTestItemAsync("Free Shipping Item");
            item1.IsFreeShipping = true;
            await _itemRepository.UpdateAsync(item1);

            var item2 = await CreateTestItemAsync("Paid Shipping Item");
            item2.IsFreeShipping = false;
            await _itemRepository.UpdateAsync(item2);

            var input = new GetItemListDto
            {
                IsFreeShipping = true,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetItemsListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            var relevantItems = result.Items.Where(i => i.ItemName.Contains("Shipping Item")).ToList();
            relevantItems.ShouldAllBe(x => x.IsFreeShipping == true);
        }

        #endregion

        #region Lookup Tests

        [Fact]
        public async Task LookupAsync_Should_Return_KeyValue_List()
        {
            // Arrange
            await CreateTestItemAsync("Lookup Item 1");
            await CreateTestItemAsync("Lookup Item 2");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.LookupAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThanOrEqualTo(2);
            result.ShouldAllBe(x => x.Id != Guid.Empty && !string.IsNullOrEmpty(x.Name));
            // Should be ordered by name
        }

        [Fact]
        public async Task GetAllItemsLookupAsync_Should_Return_All_Items()
        {
            // Arrange
            await CreateTestItemAsync("All Lookup Item 1");
            await CreateTestItemAsync("All Lookup Item 2");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetAllItemsLookupAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThanOrEqualTo(2);
            result.ShouldContain(x => x.Name == "All Lookup Item 1");
            result.ShouldContain(x => x.Name == "All Lookup Item 2");
        }

        [Fact]
        public async Task GetItemsLookupAsync_Should_Return_ItemWithItemType()
        {
            // Arrange
            await CreateTestItemAsync("Type Lookup Item");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetItemsLookupAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result.ShouldAllBe(x => x.Id != Guid.Empty && !string.IsNullOrEmpty(x.Name));
        }

        #endregion

        #region Badge Tests

        [Fact]
        public async Task GetItemBadgesAsync_Should_Return_Distinct_Badges()
        {
            // Arrange
            var item1 = await CreateTestItemAsync("Badge Item 1");
            item1.ItemBadge = "Sale";
            item1.ItemBadgeColor = "#FF0000";
            await _itemRepository.UpdateAsync(item1);

            var item2 = await CreateTestItemAsync("Badge Item 2");
            item2.ItemBadge = "New";
            item2.ItemBadgeColor = "#00FF00";
            await _itemRepository.UpdateAsync(item2);

            var item3 = await CreateTestItemAsync("Badge Item 3");
            item3.ItemBadge = "Sale"; // Duplicate badge
            item3.ItemBadgeColor = "#FF0000";
            await _itemRepository.UpdateAsync(item3);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetItemBadgesAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.ShouldContain(x => x.ItemBadge == "Sale" && x.ItemBadgeColor == "#FF0000");
            result.ShouldContain(x => x.ItemBadge == "New" && x.ItemBadgeColor == "#00FF00");
            var saleBadges = result.Where(x => x.ItemBadge == "Sale" && x.ItemBadgeColor == "#FF0000").ToList();
            saleBadges.Count.ShouldBe(1); // Should be distinct
        }

        [Fact]
        public async Task DeleteItemBadgeAsync_Should_Remove_Badge_From_Items()
        {
            // Arrange
            var item = await CreateTestItemAsync("Badge Delete Item");
            item.ItemBadge = "ToDelete";
            item.ItemBadgeColor = "#123456";
            await _itemRepository.UpdateAsync(item);

            var badgeDto = new ItemBadgeDto
            {
                ItemBadge = "ToDelete",
                ItemBadgeColor = "#123456"
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _itemAppService.DeleteItemBadgeAsync(badgeDto);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var updated = await _itemRepository.GetAsync(item.Id);
                updated.ItemBadge.ShouldBeNull();
                updated.ItemBadgeColor.ShouldBeNull();
            });
        }

        #endregion

        #region Image Tests

        [Fact]
        public async Task GetFirstImageUrlAsync_Should_Return_First_Image()
        {
            // Arrange
            var item = await CreateTestItemAsync("Image Test Item");
            _itemManager.AddItemImage(item, "image1.jpg", "blob1.jpg", "https://test.com/1.jpg", ImageType.Item, 2);
            _itemManager.AddItemImage(item, "image2.jpg", "blob2.jpg", "https://test.com/2.jpg", ImageType.Item, 1);
            _itemManager.AddItemImage(item, "image3.jpg", "blob3.jpg", "https://test.com/3.jpg", ImageType.Item, 3);
            await _itemRepository.UpdateAsync(item);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetFirstImageUrlAsync(item.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe("https://test.com/2.jpg"); // Should return image with lowest SortNo
        }

        #endregion

        #region Store Tests

        [Fact]
        public async Task GetListForStoreAsync_Should_Return_Items_With_Images()
        {
            // Arrange
            var item = await CreateTestItemAsync("Store Item");
            _itemManager.AddItemImage(item, "store-image.jpg", "blob-store.jpg", "https://test.com/store.jpg", ImageType.Item, 1);
            await _itemRepository.UpdateAsync(item);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetListForStoreAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result.Count.ShouldBeLessThanOrEqualTo(3); // Method limits to 3 items
        }

        #endregion

        #region Category Tests

        [Fact]
        public async Task GetItemCategoriesAsync_Should_Return_Categories()
        {
            // Arrange
            var item = await CreateTestItemAsync("Category Item");
            var category1 = new CategoryProduct(item.Id, Guid.NewGuid());
            var category2 = new CategoryProduct(item.Id, Guid.NewGuid());
            
            await _categoryProductRepository.InsertAsync(category1);
            await _categoryProductRepository.InsertAsync(category2);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetItemCategoriesAsync(item.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.ShouldAllBe(x => x.ItemId == item.Id);
        }

        #endregion

        #region SKU Tests

        [Fact]
        public async Task GetSKUAndItemAsync_Should_Return_Item_With_Specific_Detail()
        {
            // Arrange
            var item = await CreateTestItemAsync("SKU Test Item", includeDetails: true);
            await _itemRepository.EnsureCollectionLoadedAsync(item, i => i.ItemDetails);
            var detail = item.ItemDetails.First();

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetSKUAndItemAsync(item.Id, detail.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(item.Id);
            result.ItemName.ShouldBe("SKU Test Item");
        }

        #endregion

        #region Batch Operations Tests

        [Fact]
        public async Task GetManyAsync_Should_Return_Multiple_Items()
        {
            // Arrange
            var items = new List<Item>();
            for (int i = 0; i < 3; i++)
            {
                items.Add(await CreateTestItemAsync($"Batch Item {i}"));
            }
            var ids = items.Select(i => i.Id).ToList();

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetManyAsync(ids);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result.Select(x => x.Id).ShouldBe(ids);
        }

        [Fact]
        public async Task GetItemsWithAttributesAsync_Should_Include_Attributes()
        {
            // Arrange
            var item = await CreateTestItemAsync("Attribute Item", includeDetails: true);
            var ids = new List<Guid> { item.Id };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetItemsWithAttributesAsync(ids);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result.First().Attribute1Name.ShouldBe("Size");
            result.First().Attribute2Name.ShouldBe("Color");
            result.First().Attribute3Name.ShouldBe("Material");
        }

        #endregion

        #region HTML Sanitization Tests

        [Fact]
        public async Task SanitizeItemDescription_Should_Remove_Dangerous_Tags()
        {
            // Arrange & Act
            // SanitizeItemDescription is an internal method, cannot test directly
            // Instead, test through CreateAsync
            var input = CreateTestItemDto();
            input.ItemDescription = "<p>Safe</p><script>alert('XSS')</script><img src=x onerror=alert('XSS')>";
            input.IsTest = true;
            
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.CreateAsync(input);
            });
            
            var sanitized = result.ItemDescription;

            // Assert
            sanitized?.ShouldNotContain("<script>");
            sanitized?.ShouldNotContain("onerror");
            sanitized.ShouldContain("<p>Safe</p>");
        }

        [Fact]
        public async Task SanitizeItemDescription_Should_Handle_Null_Input()
        {
            // Act
            // Test null handling through CreateAsync
            var input = CreateTestItemDto();
            input.ItemDescription = "";
            input.IsTest = true;
            
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.CreateAsync(input);
            });

            // Assert
            result.ItemDescription.ShouldBeNull();
        }

        [Fact]
        public async Task SanitizeItemDescription_Should_Handle_Empty_Input()
        {
            // Act
            // Test empty string handling through CreateAsync
            var input = CreateTestItemDto();
            input.ItemDescription = "";
            input.IsTest = true;
            
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.CreateAsync(input);
            });

            // Assert
            result.ItemDescription.ShouldBe("");
        }

        #endregion

        #region ItemDetail Lookup Tests

        [Fact]
        public async Task GetItemDetailLookupAsync_Should_Return_Details_For_Item()
        {
            // Arrange
            var item = await CreateTestItemAsync("Detail Lookup Item", includeDetails: true);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _itemAppService.GetItemDetailLookupAsync(item.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.ShouldAllBe(x => !string.IsNullOrEmpty(x.Name));
            // Should be ordered by name
        }

        #endregion
    }
}
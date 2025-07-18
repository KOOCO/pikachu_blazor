using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.ShopCarts;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Xunit;

namespace Kooco.Pikachu.Application.Tests.ShopCarts
{
    public class ShopCartAppServiceAdvancedTests : PikachuApplicationTestBase
    {
        private readonly IShopCartAppService _shopCartAppService;
        private readonly IShopCartRepository _shopCartRepository;
        private readonly IRepository<GroupBuy, Guid> _groupBuyRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<ItemDetails, Guid> _itemDetailRepository;
        private readonly IRepository<SetItem, Guid> _setItemRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        public ShopCartAppServiceAdvancedTests()
        {
            _shopCartAppService = GetRequiredService<IShopCartAppService>();
            _shopCartRepository = GetRequiredService<IShopCartRepository>();
            _groupBuyRepository = GetRequiredService<IRepository<GroupBuy, Guid>>();
            _itemRepository = GetRequiredService<IRepository<Item, Guid>>();
            _itemDetailRepository = GetRequiredService<IRepository<ItemDetails, Guid>>();
            _setItemRepository = GetRequiredService<IRepository<SetItem, Guid>>();
            _userRepository = GetRequiredService<IRepository<IdentityUser, Guid>>();
        }

        #region Test Helpers

        private async Task<IdentityUser> CreateTestUserAsync(string name = null)
        {
            var uniqueId = Guid.NewGuid().ToString("N");
            var user = new IdentityUser(
                Guid.NewGuid(),
                name ?? "testuser" + uniqueId,
                $"testuser{uniqueId}@test.com"
            );
            await _userRepository.InsertAsync(user);
            return user;
        }

        private async Task<GroupBuy> CreateTestGroupBuyAsync(string name = null)
        {
            var groupBuy = new GroupBuy
            {
                Id = Guid.NewGuid(),
                GroupBuyName = name ?? "Test Group Buy " + Guid.NewGuid().ToString("N").Substring(0, 8),
                ShortCode = "TGB" + Guid.NewGuid().ToString("N").Substring(0, 5),
                GroupBuyNo = 123456,
                Status = "AwaitingRelease",
                EntryURL = "test-url-" + Guid.NewGuid().ToString("N").Substring(0, 8)
            };
            await _groupBuyRepository.InsertAsync(groupBuy);
            return groupBuy;
        }

        private async Task<(Item item, List<ItemDetails> details)> CreateTestItemWithMultipleDetailsAsync()
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                ItemName = "Multi-Detail Item"
            };
            await _itemRepository.InsertAsync(item);

            var details = new List<ItemDetails>();
            var colors = new[] { "Red", "Blue", "Green" };
            var sizes = new[] { "S", "M", "L" };

            foreach (var color in colors)
            {
                foreach (var size in sizes)
                {
                    var detail = new ItemDetails
                    {
                        ItemId = item.Id,
                        ItemName = $"{item.ItemName} - {color} - {size}",
                        SKU = $"MDI001-{color[0]}{size}",
                        // GroupBuyPrice = 900 + (sizes.ToList().IndexOf(size) * 100),
                        SellingPrice = 1200 + (sizes.ToList().IndexOf(size) * 100),
                        StockOnHand = 50
                    };
                    await _itemDetailRepository.InsertAsync(detail);
                    details.Add(detail);
                }
            }

            return (item, details);
        }

        #endregion

        #region Complex Cart Operations

        [Fact]
        public async Task AddCartItemAsync_Should_Update_Existing_Item_Quantity()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, details) = await CreateTestItemWithMultipleDetailsAsync();
            var detail = details.First();

            // First add item with quantity 2
            await WithUnitOfWorkAsync(async () =>
            {
                var input = new CreateCartItemDto
                {
                    ItemId = item.Id,
                    ItemDetailId = detail.Id,
                    Quantity = 2,
                    GroupBuyPrice = (int)detail.SellingPrice,
                    SellingPrice = detail.SellingPrice
                };
                await _shopCartAppService.AddCartItemAsync(user.Id, groupBuy.Id, input);
            });

            // Act - Add same item with quantity 3 (should update to 3, not add)
            var result = await WithUnitOfWorkAsync(async () =>
            {
                var input = new CreateCartItemDto
                {
                    ItemId = item.Id,
                    ItemDetailId = detail.Id,
                    Quantity = 3,
                    GroupBuyPrice = (int)detail.SellingPrice,
                    SellingPrice = detail.SellingPrice
                };
                return await _shopCartAppService.AddCartItemAsync(user.Id, groupBuy.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.CartItems.Count.ShouldBe(1);
            result.CartItems[0].Quantity.ShouldBe(3); // Updated to 3, not 5
        }

        [Fact]
        public async Task CreateAsync_With_SetItem_Should_Work()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var setItem = await CreateTestSetItemAsync();

            var input = new CreateShopCartDto
            {
                UserId = user.Id,
                GroupBuyId = groupBuy.Id,
                CartItems = new List<CreateCartItemDto>
                {
                    new CreateCartItemDto
                    {
                        SetItemId = setItem.Id,
                        Quantity = 1,
                        GroupBuyPrice = (int)setItem.SetItemPrice,
                        SellingPrice = (int)setItem.SellingPrice
                    }
                }
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.CartItems.Count.ShouldBe(1);
            result.CartItems[0].SetItemId.ShouldBe(setItem.Id);
            result.CartItems[0].ItemId.ShouldBeNull();
            result.CartItems[0].ItemDetailId.ShouldBeNull();
        }

        [Fact]
        public async Task UpdateShopCartAsync_Should_Handle_Complex_Updates()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item1, details1) = await CreateTestItemWithMultipleDetailsAsync();
            var (item2, details2) = await CreateTestItemWithMultipleDetailsAsync();

            // Create cart with multiple items
            var shopCart = await WithUnitOfWorkAsync(async () =>
            {
                var input = new CreateShopCartDto
                {
                    UserId = user.Id,
                    GroupBuyId = groupBuy.Id,
                    CartItems = new List<CreateCartItemDto>
                    {
                        new CreateCartItemDto
                        {
                            ItemId = item1.Id,
                            ItemDetailId = details1[0].Id,
                            Quantity = 2,
                            GroupBuyPrice = (int)details1[0].SellingPrice,
                            SellingPrice = (int)details1[0].SellingPrice
                        },
                        new CreateCartItemDto
                        {
                            ItemId = item1.Id,
                            ItemDetailId = details1[1].Id,
                            Quantity = 3,
                            GroupBuyPrice = (int)details1[1].SellingPrice,
                            SellingPrice = (int)details1[1].SellingPrice
                        },
                        new CreateCartItemDto
                        {
                            ItemId = item2.Id,
                            ItemDetailId = details2[0].Id,
                            Quantity = 1,
                            GroupBuyPrice = (int)details2[0].SellingPrice,
                            SellingPrice = (int)details2[0].SellingPrice
                        }
                    }
                };
                return await _shopCartAppService.CreateAsync(input);
            });

            // Prepare update: remove one item, update another, add new
            var updatedCartItems = new List<CartItemWithDetailsDto>
            {
                // Keep and update first item
                new CartItemWithDetailsDto
                {
                    Id = shopCart.CartItems[0].Id,
                    ItemId = item1.Id,
                    ItemDetailId = details1[0].Id,
                    Quantity = 5, // Updated quantity
                    GroupBuyPrice = (int)details1[0].SellingPrice,
                    SellingPrice = (int)details1[0].SellingPrice
                },
                // Remove second item (not included)
                // Keep third item as is
                new CartItemWithDetailsDto
                {
                    Id = shopCart.CartItems[2].Id,
                    ItemId = item2.Id,
                    ItemDetailId = details2[0].Id,
                    Quantity = 1,
                    GroupBuyPrice = (int)details2[0].SellingPrice,
                    SellingPrice = (int)details2[0].SellingPrice
                },
                // Add new item
                new CartItemWithDetailsDto
                {
                    ItemId = item2.Id,
                    ItemDetailId = details2[1].Id,
                    Quantity = 2,
                    GroupBuyPrice = (int)details2[1].SellingPrice,
                    SellingPrice = (int)details2[1].SellingPrice
                }
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _shopCartAppService.UpdateShopCartAsync(shopCart.Id, updatedCartItems);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var updated = await _shopCartAppService.GetAsync(shopCart.Id);
                updated.CartItems.Count.ShouldBe(3); // 1 removed, 1 added
                updated.CartItems.First(ci => ci.Id == shopCart.CartItems[0].Id).Quantity.ShouldBe(5);
                updated.CartItems.Any(ci => ci.ItemDetailId == details2[1].Id).ShouldBeTrue();
            });
        }

        private async Task<SetItem> CreateTestSetItemAsync()
        {
            var setItem = new SetItem
            {
                Id = Guid.NewGuid(),
                SetItemName = "Test Set Item " + Guid.NewGuid().ToString("N").Substring(0, 8),
                SetItemNo = "TSI" + Guid.NewGuid().ToString("N").Substring(0, 5),
                SetItemPrice = 2000,
                SellingPrice = 2500
            };
            await _setItemRepository.InsertAsync(setItem);
            return setItem;
        }

        #endregion

        #region GetListWithDetails Tests

        [Fact]
        public async Task GetListWithDetailsAsync_Should_Return_Detailed_Info()
        {
            // Arrange
            var users = new List<IdentityUser>();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, details) = await CreateTestItemWithMultipleDetailsAsync();

            for (int i = 0; i < 3; i++)
            {
                users.Add(await CreateTestUserAsync($"detailuser{i}"));
            }

            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var user in users)
                {
                    var input = new CreateShopCartDto
                    {
                        UserId = user.Id,
                        GroupBuyId = groupBuy.Id,
                        CartItems = new List<CreateCartItemDto>
                        {
                            new CreateCartItemDto
                            {
                                ItemId = item.Id,
                                ItemDetailId = details[0].Id,
                                Quantity = 2,
                                GroupBuyPrice = (int)details[0].SellingPrice,
                                SellingPrice = (int)details[0].SellingPrice
                            }
                        }
                    };
                    await _shopCartAppService.CreateAsync(input);
                }
            });

            var listInput = new GetShopCartListDto
            {
                GroupBuyId = groupBuy.Id,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.GetListWithDetailsAsync(listInput);
            });

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(3);
            result.Items.Count.ShouldBe(3);
        }

        #endregion

        #region GetCartItemsList Tests

        [Fact]
        public async Task GetCartItemsListAsync_Should_Return_Cart_Items()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, details) = await CreateTestItemWithMultipleDetailsAsync();

            var shopCart = await WithUnitOfWorkAsync(async () =>
            {
                var input = new CreateShopCartDto
                {
                    UserId = user.Id,
                    GroupBuyId = groupBuy.Id,
                    CartItems = details.Take(3).Select(d => new CreateCartItemDto
                    {
                        ItemId = item.Id,
                        ItemDetailId = d.Id,
                        Quantity = 1,
                        GroupBuyPrice = (int)d.SellingPrice,
                        SellingPrice = (int)d.SellingPrice
                    }).ToList()
                };
                return await _shopCartAppService.CreateAsync(input);
            });

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.GetCartItemsListAsync(shopCart.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
        }

        #endregion

        #region Product Lookup Tests

        [Fact]
        public async Task GetGroupBuyProductsLookupAsync_Should_Return_Products()
        {
            // Arrange
            var groupBuy = await CreateTestGroupBuyAsync();
            
            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.GetGroupBuyProductsLookupAsync(groupBuy.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<ItemWithItemTypeDto>>();
        }

        [Fact]
        public async Task GetItemWithDetailsAsync_Should_Return_Item_Details()
        {
            // Arrange
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, details) = await CreateTestItemWithMultipleDetailsAsync();

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.GetItemWithDetailsAsync(groupBuy.Id, item.Id, ItemType.Item);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(item.Id);
        }

        #endregion

        #region Edge Cases and Error Scenarios

        [Fact]
        public async Task AddCartItemAsync_With_Zero_Quantity_Should_Handle_Gracefully()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, details) = await CreateTestItemWithMultipleDetailsAsync();

            var input = new CreateCartItemDto
            {
                ItemId = item.Id,
                ItemDetailId = details[0].Id,
                Quantity = 0,
                // GroupBuyPrice = details[0].SellingPrice,
                SellingPrice = (int)details[0].SellingPrice
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.AddCartItemAsync(user.Id, groupBuy.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.CartItems.Any(ci => ci.Quantity == 0).ShouldBeTrue();
        }

        [Fact]
        public async Task CreateAsync_With_Mixed_Items_And_SetItems()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, details) = await CreateTestItemWithMultipleDetailsAsync();
            var setItem = await CreateTestSetItemAsync();

            var input = new CreateShopCartDto
            {
                UserId = user.Id,
                GroupBuyId = groupBuy.Id,
                CartItems = new List<CreateCartItemDto>
                {
                    new CreateCartItemDto
                    {
                        ItemId = item.Id,
                        ItemDetailId = details[0].Id,
                        Quantity = 2,
                        GroupBuyPrice = (int)details[0].SellingPrice,
                        SellingPrice = (int)details[0].SellingPrice
                    },
                    new CreateCartItemDto
                    {
                        SetItemId = setItem.Id,
                        Quantity = 1,
                        GroupBuyPrice = (int)setItem.SetItemPrice,
                        SellingPrice = (int)setItem.SellingPrice
                    }
                }
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.CartItems.Count.ShouldBe(2);
            result.CartItems.Count(ci => ci.ItemId.HasValue).ShouldBe(1);
            result.CartItems.Count(ci => ci.SetItemId.HasValue).ShouldBe(1);
        }

        [Fact]
        public async Task GetListAsync_With_Sorting_Should_Work()
        {
            // Arrange
            var groupBuy = await CreateTestGroupBuyAsync();
            var users = new List<IdentityUser>();

            for (int i = 0; i < 3; i++)
            {
                users.Add(await CreateTestUserAsync());
                await Task.Delay(100); // Ensure different creation times
            }

            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var user in users)
                {
                    var input = new CreateShopCartDto
                    {
                        UserId = user.Id,
                        GroupBuyId = groupBuy.Id
                    };
                    await _shopCartAppService.CreateAsync(input);
                }
            });

            var listInput = new GetShopCartListDto
            {
                Sorting = "CreationTime ASC",
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.GetListAsync(listInput);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(3);
            // First item should have earliest creation time
            for (int i = 1; i < result.Items.Count; i++)
            {
                result.Items[i].CreationTime.ShouldBeGreaterThanOrEqualTo(result.Items[i - 1].CreationTime);
            }
        }

        #endregion
    }
}
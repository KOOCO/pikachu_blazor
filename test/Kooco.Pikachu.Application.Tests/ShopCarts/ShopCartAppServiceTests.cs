using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.ShopCarts;
using NSubstitute;
using Shouldly;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Xunit;

namespace Kooco.Pikachu.Application.Tests.ShopCarts
{
    public class ShopCartAppServiceTests : PikachuApplicationTestBase
    {
        private readonly IShopCartAppService _shopCartAppService;
        private readonly IShopCartRepository _shopCartRepository;
        private readonly IRepository<GroupBuy, Guid> _groupBuyRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<ItemDetails, Guid> _itemDetailRepository;
        private readonly IRepository<SetItem, Guid> _setItemRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IGroupBuyAppService _groupBuyAppService;

        public ShopCartAppServiceTests()
        {
            _shopCartAppService = GetRequiredService<IShopCartAppService>();
            _shopCartRepository = GetRequiredService<IShopCartRepository>();
            _groupBuyRepository = GetRequiredService<IRepository<GroupBuy, Guid>>();
            _itemRepository = GetRequiredService<IRepository<Item, Guid>>();
            _itemDetailRepository = GetRequiredService<IRepository<ItemDetails, Guid>>();
            _setItemRepository = GetRequiredService<IRepository<SetItem, Guid>>();
            _userRepository = GetRequiredService<IRepository<IdentityUser, Guid>>();
            _groupBuyAppService = GetRequiredService<IGroupBuyAppService>();
        }

        #region Test Helpers

        private async Task<IdentityUser> CreateTestUserAsync()
        {
            var user = new IdentityUser(
                Guid.NewGuid(),
                "testuser" + Guid.NewGuid().ToString("N"),
                "testuser" + Guid.NewGuid().ToString("N") + "@test.com"
            );
            await _userRepository.InsertAsync(user);
            return user;
        }

        private async Task<GroupBuy> CreateTestGroupBuyAsync()
        {
            var groupBuy = new GroupBuy
            {
                GroupBuyName = "Test Group Buy",
                ShortCode = "TGB001",
                GroupBuyNo = 123456,
                Status = "AwaitingRelease",
                EntryURL = "test-url"
            };
            await _groupBuyRepository.InsertAsync(groupBuy);
            return groupBuy;
        }

        private async Task<(Item item, ItemDetails detail)> CreateTestItemAsync()
        {
            var item = new Item
            {
                ItemName = "Test Item"
            };
            await _itemRepository.InsertAsync(item);

            var detail = new ItemDetails
            {
                ItemId = item.Id,
                ItemName = "Test Item Detail",
                SKU = "TST001-D1",
                SellingPrice = 1200,
                StockOnHand = 100
            };
            await _itemDetailRepository.InsertAsync(detail);

            return (item, detail);
        }

        private async Task<SetItem> CreateTestSetItemAsync()
        {
            var setItem = new SetItem
            {
                SetItemName = "Test Set Item",
                SetItemNo = "TSI001",
                SetItemPrice = 2000,
                SellingPrice = 2500
            };
            await _setItemRepository.InsertAsync(setItem);
            return setItem;
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task CreateAsync_Should_Create_New_ShopCart()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            
            var input = new CreateShopCartDto
            {
                UserId = user.Id,
                GroupBuyId = groupBuy.Id,
                CartItems = new List<CreateCartItemDto>()
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(user.Id);
            result.GroupBuyId.ShouldBe(groupBuy.Id);
            result.CartItems.ShouldNotBeNull();
            result.CartItems.Count.ShouldBe(0);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_ShopCart_With_Items()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, detail) = await CreateTestItemAsync();
            
            var input = new CreateShopCartDto
            {
                UserId = user.Id,
                GroupBuyId = groupBuy.Id,
                CartItems = new List<CreateCartItemDto>
                {
                    new CreateCartItemDto
                    {
                        ItemId = item.Id,
                        ItemDetailId = detail.Id,
                        Quantity = 2,
                        GroupBuyPrice = (int)detail.SellingPrice,
                        SellingPrice = (int)detail.SellingPrice
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
            result.CartItems[0].ItemId.ShouldBe(item.Id);
            result.CartItems[0].ItemDetailId.ShouldBe(detail.Id);
            result.CartItems[0].Quantity.ShouldBe(2);
        }

        [Fact]
        public async Task CreateAsync_Should_Merge_Duplicate_Items()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, detail) = await CreateTestItemAsync();
            
            var input = new CreateShopCartDto
            {
                UserId = user.Id,
                GroupBuyId = groupBuy.Id,
                CartItems = new List<CreateCartItemDto>
                {
                    new CreateCartItemDto
                    {
                        ItemId = item.Id,
                        ItemDetailId = detail.Id,
                        Quantity = 2,
                        GroupBuyPrice = (int)detail.SellingPrice,
                        SellingPrice = (int)detail.SellingPrice
                    },
                    new CreateCartItemDto
                    {
                        ItemId = item.Id,
                        ItemDetailId = detail.Id,
                        Quantity = 3,
                        GroupBuyPrice = (int)detail.SellingPrice,
                        SellingPrice = (int)detail.SellingPrice
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
            result.CartItems[0].Quantity.ShouldBe(5); // 2 + 3
        }

        #endregion

        #region Get Tests

        [Fact]
        public async Task GetAsync_Should_Return_ShopCart()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            
            var shopCart = await WithUnitOfWorkAsync(async () =>
            {
                var input = new CreateShopCartDto
                {
                    UserId = user.Id,
                    GroupBuyId = groupBuy.Id
                };
                return await _shopCartAppService.CreateAsync(input);
            });

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.GetAsync(shopCart.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(shopCart.Id);
            result.UserId.ShouldBe(user.Id);
            result.GroupBuyId.ShouldBe(groupBuy.Id);
        }

        [Fact]
        public async Task GetAsync_Should_Throw_When_Not_Found()
        {
            // Act & Assert
            await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    await _shopCartAppService.GetAsync(Guid.NewGuid());
                });
            });
        }

        #endregion

        #region Find Tests

        [Fact]
        public async Task FindByUserIdAsync_Should_Return_ShopCart()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            
            await WithUnitOfWorkAsync(async () =>
            {
                var input = new CreateShopCartDto
                {
                    UserId = user.Id,
                    GroupBuyId = groupBuy.Id
                };
                await _shopCartAppService.CreateAsync(input);
            });

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.FindByUserIdAsync(user.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(user.Id);
        }

        [Fact]
        public async Task FindByUserIdAsync_Should_Return_Null_When_Not_Found()
        {
            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.FindByUserIdAsync(Guid.NewGuid());
            });

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task FindByUserIdAndGroupBuyIdAsync_Should_Return_ShopCart()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            
            await WithUnitOfWorkAsync(async () =>
            {
                var input = new CreateShopCartDto
                {
                    UserId = user.Id,
                    GroupBuyId = groupBuy.Id
                };
                await _shopCartAppService.CreateAsync(input);
            });

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.FindByUserIdAndGroupBuyIdAsync(user.Id, groupBuy.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(user.Id);
            result.GroupBuyId.ShouldBe(groupBuy.Id);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_Should_Delete_ShopCart()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            
            var shopCart = await WithUnitOfWorkAsync(async () =>
            {
                var input = new CreateShopCartDto
                {
                    UserId = user.Id,
                    GroupBuyId = groupBuy.Id
                };
                return await _shopCartAppService.CreateAsync(input);
            });

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _shopCartAppService.DeleteAsync(shopCart.Id);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var deleted = await _shopCartRepository.FindAsync(shopCart.Id);
                deleted.ShouldBeNull();
            });
        }

        [Fact]
        public async Task DeleteByUserIdAsync_Should_Delete_User_ShopCart()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            
            await WithUnitOfWorkAsync(async () =>
            {
                var input = new CreateShopCartDto
                {
                    UserId = user.Id,
                    GroupBuyId = groupBuy.Id
                };
                await _shopCartAppService.CreateAsync(input);
            });

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _shopCartAppService.DeleteByUserIdAsync(user.Id);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var deleted = await _shopCartRepository.FindByUserIdAsync(user.Id);
                deleted.ShouldBeNull();
            });
        }

        [Fact]
        public async Task DeleteByUserIdAndGroupBuyIdAsync_Should_Delete_Specific_ShopCart()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            
            await WithUnitOfWorkAsync(async () =>
            {
                var input = new CreateShopCartDto
                {
                    UserId = user.Id,
                    GroupBuyId = groupBuy.Id
                };
                await _shopCartAppService.CreateAsync(input);
            });

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _shopCartAppService.DeleteByUserIdAndGroupBuyIdAsync(user.Id, groupBuy.Id);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var deleted = await _shopCartRepository.FindByUserIdAndGroupBuyIdAsync(user.Id, groupBuy.Id);
                deleted.ShouldBeNull();
            });
        }

        #endregion

        #region Cart Item Tests

        [Fact]
        public async Task AddCartItemAsync_Should_Add_Item_To_Existing_Cart()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, detail) = await CreateTestItemAsync();
            
            await WithUnitOfWorkAsync(async () =>
            {
                var input = new CreateShopCartDto
                {
                    UserId = user.Id,
                    GroupBuyId = groupBuy.Id
                };
                await _shopCartAppService.CreateAsync(input);
            });

            var cartItemInput = new CreateCartItemDto
            {
                ItemId = item.Id,
                ItemDetailId = detail.Id,
                Quantity = 3,
                GroupBuyPrice = (int)detail.SellingPrice,
                SellingPrice = (int)detail.SellingPrice
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.AddCartItemAsync(user.Id, groupBuy.Id, cartItemInput);
            });

            // Assert
            result.ShouldNotBeNull();
            result.CartItems.Count.ShouldBe(1);
            result.CartItems[0].Quantity.ShouldBe(3);
        }

        [Fact]
        public async Task AddCartItemAsync_Should_Create_Cart_If_Not_Exists()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, detail) = await CreateTestItemAsync();
            
            var cartItemInput = new CreateCartItemDto
            {
                ItemId = item.Id,
                ItemDetailId = detail.Id,
                Quantity = 2,
                GroupBuyPrice = (int)detail.SellingPrice,
                SellingPrice = (int)detail.SellingPrice
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.AddCartItemAsync(user.Id, groupBuy.Id, cartItemInput);
            });

            // Assert
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(user.Id);
            result.GroupBuyId.ShouldBe(groupBuy.Id);
            result.CartItems.Count.ShouldBe(1);
        }

        [Fact]
        public async Task UpdateCartItemAsync_Should_Update_Quantity()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, detail) = await CreateTestItemAsync();
            
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
                            ItemId = item.Id,
                            ItemDetailId = detail.Id,
                            Quantity = 2,
                            GroupBuyPrice = (int)detail.SellingPrice,
                            SellingPrice = (int)detail.SellingPrice
                        }
                    }
                };
                return await _shopCartAppService.CreateAsync(input);
            });

            var cartItemId = shopCart.CartItems[0].Id;
            var updateInput = new CreateCartItemDto
            {
                Quantity = 5
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.UpdateCartItemAsync(cartItemId, updateInput);
            });

            // Assert
            result.ShouldNotBeNull();
            result.CartItems[0].Quantity.ShouldBe(5);
        }

        [Fact]
        public async Task DeleteCartItemAsync_Should_Remove_Item()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, detail) = await CreateTestItemAsync();
            
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
                            ItemId = item.Id,
                            ItemDetailId = detail.Id,
                            Quantity = 2,
                            GroupBuyPrice = (int)detail.SellingPrice,
                            SellingPrice = (int)detail.SellingPrice
                        }
                    }
                };
                return await _shopCartAppService.CreateAsync(input);
            });

            var cartItemId = shopCart.CartItems[0].Id;

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.DeleteCartItemAsync(cartItemId);
            });

            // Assert
            result.ShouldNotBeNull();
            result.CartItems.Count.ShouldBe(0);
        }

        #endregion

        #region Clear Cart Tests

        [Fact]
        public async Task ClearCartItemsAsync_Should_Clear_Multiple_Carts()
        {
            // Arrange
            var user1 = await CreateTestUserAsync();
            var user2 = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, detail) = await CreateTestItemAsync();
            
            var shopCartIds = new List<Guid>();
            
            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var user in new[] { user1, user2 })
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
                                ItemDetailId = detail.Id,
                                Quantity = 1,
                                GroupBuyPrice = (int)detail.SellingPrice,
                                SellingPrice = (int)detail.SellingPrice
                            }
                        }
                    };
                    var cart = await _shopCartAppService.CreateAsync(input);
                    shopCartIds.Add(cart.Id);
                }
            });

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _shopCartAppService.ClearCartItemsAsync(shopCartIds);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var id in shopCartIds)
                {
                    var cart = await _shopCartAppService.GetAsync(id);
                    cart.CartItems.Count.ShouldBe(0);
                }
            });
        }

        [Fact]
        public async Task ClearCartItemsAsync_By_UserId_GroupBuyId_Should_Clear_Cart()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, detail) = await CreateTestItemAsync();
            
            await WithUnitOfWorkAsync(async () =>
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
                            ItemDetailId = detail.Id,
                            Quantity = 2,
                            GroupBuyPrice = (int)detail.SellingPrice,
                            SellingPrice = (int)detail.SellingPrice
                        }
                    }
                };
                await _shopCartAppService.CreateAsync(input);
            });

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _shopCartAppService.ClearCartItemsAsync(user.Id, groupBuy.Id);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var cart = await _shopCartAppService.FindByUserIdAndGroupBuyIdAsync(user.Id, groupBuy.Id);
                cart.ShouldNotBeNull();
                cart.CartItems.Count.ShouldBe(0);
            });
        }

        #endregion

        #region List Tests

        [Fact]
        public async Task GetListAsync_Should_Return_Paged_Result()
        {
            // Arrange
            var users = new List<IdentityUser>();
            var groupBuy = await CreateTestGroupBuyAsync();
            
            for (int i = 0; i < 5; i++)
            {
                users.Add(await CreateTestUserAsync());
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
                MaxResultCount = 3,
                SkipCount = 0
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.GetListAsync(listInput);
            });

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(5);
            result.Items.Count.ShouldBe(3);
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_GroupBuyId()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy1 = await CreateTestGroupBuyAsync();
            var groupBuy2 = await CreateTestGroupBuyAsync();
            
            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var gb in new[] { groupBuy1, groupBuy2 })
                {
                    var input = new CreateShopCartDto
                    {
                        UserId = user.Id,
                        GroupBuyId = gb.Id
                    };
                    await _shopCartAppService.CreateAsync(input);
                }
            });

            var listInput = new GetShopCartListDto
            {
                GroupBuyId = groupBuy1.Id,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.GetListAsync(listInput);
            });

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(1);
            result.Items[0].GroupBuyId.ShouldBe(groupBuy1.Id);
        }

        #endregion

        #region Verify Cart Items Tests

        [Fact]
        public async Task VerifyCartItemsAsync_Should_Return_Valid_Items()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();
            var (item, detail) = await CreateTestItemAsync();
            
            await WithUnitOfWorkAsync(async () =>
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
                            ItemDetailId = detail.Id,
                            Quantity = 2,
                            GroupBuyPrice = (int)detail.SellingPrice,
                            SellingPrice = (int)detail.SellingPrice
                        }
                    }
                };
                await _shopCartAppService.CreateAsync(input);
            });

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _shopCartAppService.VerifyCartItemsAsync(user.Id, groupBuy.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThanOrEqualTo(0);
        }

        #endregion
    }
}
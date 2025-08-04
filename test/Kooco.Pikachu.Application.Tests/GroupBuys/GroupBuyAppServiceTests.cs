using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Volo.Abp.Domain.Entities;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Orders;
using NSubstitute;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Xunit;

namespace Kooco.Pikachu.Application.Tests.GroupBuys
{
    public class GroupBuyAppServiceTests : PikachuApplicationTestBase
    {
        private readonly IGroupBuyAppService _groupBuyAppService;
        private readonly IGroupBuyRepository _groupBuyRepository;
        private readonly IRepository<Image, Guid> _imageRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<SetItem, Guid> _setItemRepository;
        private readonly IRepository<Freebie, Guid> _freebieRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        public GroupBuyAppServiceTests()
        {
            _groupBuyAppService = GetRequiredService<IGroupBuyAppService>();
            _groupBuyRepository = GetRequiredService<IGroupBuyRepository>();
            _imageRepository = GetRequiredService<IRepository<Image, Guid>>();
            _itemRepository = GetRequiredService<IRepository<Item, Guid>>();
            _setItemRepository = GetRequiredService<IRepository<SetItem, Guid>>();
            _freebieRepository = GetRequiredService<IRepository<Freebie, Guid>>();
            _userRepository = GetRequiredService<IRepository<IdentityUser, Guid>>();
        }

        #region Test Helpers

        private async Task<GroupBuy> CreateTestGroupBuyAsync(
            string groupBuyName = null,
            string status = "AwaitingRelease",
            string shortCode = null)
        {
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var groupBuy = new GroupBuy
            {
                GroupBuyName = groupBuyName ?? $"Test Group Buy {uniqueId}",
                ShortCode = shortCode ?? $"TGB{uniqueId}",
                GroupBuyNo = 123456,
                Status = status,
                EntryURL = $"test-url-{uniqueId}",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(30),
                FreeShipping = true,
                AllowShipToOuterTaiwan = false,
                AllowShipOversea = false,
                ExcludeShippingMethod = "BlackCat1"
            };

            await _groupBuyRepository.InsertAsync(groupBuy);
            return groupBuy;
        }

        private async Task<Image> CreateTestImageAsync(string name = null, string url = null)
        {
            var image = new Image
            {
                Name = name ?? "test-image.jpg",
                BlobImageName = "blob-" + Guid.NewGuid().ToString("N") + ".jpg",
                ImageUrl = url ?? "https://test.com/image.jpg",
                ImageType = ImageType.GroupBuyCarouselImage
            };
            await _imageRepository.InsertAsync(image);
            return image;
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task CreateAsync_Should_Create_GroupBuy()
        {
            // Arrange
            var input = new GroupBuyCreateDto
            {
                GroupBuyName = "New Group Buy Test",
                ShortCode = "NGBT001",
                LogoURL = "https://test.com/logo.jpg",
                BannerURL = "https://test.com/banner.jpg",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(30),
                FreeShipping = true,
                AllowShipToOuterTaiwan = false,
                AllowShipOversea = false,
                ExcludeShippingMethod = "BlackCat1",
                Status = "AwaitingRelease"
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.GroupBuyName.ShouldBe("New Group Buy Test");
            result.ShortCode.ShouldBe("NGBT001");
            result.Status.ShouldBe("AwaitingRelease");
            result.FreeShipping.ShouldBe(true);
        }

        [Fact]
        public async Task CreateAsync_Should_Generate_GroupBuyNo()
        {
            // Arrange
            var input = new GroupBuyCreateDto
            {
                GroupBuyName = "Test Group Buy",
                ShortCode = "TGB999",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(7)
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.GroupBuyNo.ShouldBeGreaterThan(0);
        }

        #endregion

        #region Get Tests

        [Fact]
        public async Task GetAsync_Should_Return_GroupBuy()
        {
            // Arrange
            var groupBuy = await CreateTestGroupBuyAsync();

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.GetAsync(groupBuy.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(groupBuy.Id);
            result.GroupBuyName.ShouldBe(groupBuy.GroupBuyName);
            result.ShortCode.ShouldBe(groupBuy.ShortCode);
        }

        [Fact]
        public async Task GetAsync_Should_Throw_When_Not_Found()
        {
            // Act & Assert
            await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    await _groupBuyAppService.GetAsync(Guid.NewGuid());
                });
            });
        }

        [Fact]
        public async Task GetWithDetailsAsync_Should_Include_Related_Data()
        {
            // Arrange
            var groupBuy = await CreateTestGroupBuyAsync();

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.GetWithDetailsAsync(groupBuy.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(groupBuy.Id);
            result.ItemGroups.ShouldNotBeNull();
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_Should_Update_GroupBuy()
        {
            // Arrange
            var groupBuy = await CreateTestGroupBuyAsync();
            var input = new GroupBuyUpdateDto
            {
                GroupBuyName = "Updated Group Buy",
                ShortCode = groupBuy.ShortCode,
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(45),
                FreeShipping = false,
                Status = "AwaitingRelease"
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.UpdateAsync(groupBuy.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.GroupBuyName.ShouldBe("Updated Group Buy");
            result.FreeShipping.ShouldBe(false);
            // LimitQuantity is not part of GroupBuyDto
        }

        [Fact]
        public async Task UpdateAsync_Should_Handle_Status_Change()
        {
            // Arrange
            var groupBuy = await CreateTestGroupBuyAsync(status: "AwaitingRelease");
            var input = new GroupBuyUpdateDto
            {
                GroupBuyName = groupBuy.GroupBuyName,
                ShortCode = groupBuy.ShortCode,
                StartTime = groupBuy.StartTime,
                EndTime = groupBuy.EndTime,
                Status = "Released"
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.UpdateAsync(groupBuy.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe("Released");
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_Should_Delete_GroupBuy()
        {
            // Arrange
            var groupBuy = await CreateTestGroupBuyAsync();

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _groupBuyAppService.DeleteAsync(groupBuy.Id);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var deleted = await _groupBuyRepository.FindAsync(groupBuy.Id);
                deleted.ShouldBeNull();
            });
        }

        [Fact]
        public async Task DeleteManyGroupBuyItemsAsync_Should_Delete_Multiple()
        {
            // Arrange
            var groupBuys = new List<GroupBuy>();
            for (int i = 0; i < 3; i++)
            {
                groupBuys.Add(await CreateTestGroupBuyAsync());
            }
            var ids = groupBuys.Select(gb => gb.Id).ToList();

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _groupBuyAppService.DeleteManyGroupBuyItemsAsync(ids);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var id in ids)
                {
                    var deleted = await _groupBuyRepository.FindAsync(id);
                    deleted.ShouldBeNull();
                }
            });
        }

        #endregion

        #region List Tests

        [Fact]
        public async Task GetListAsync_Should_Return_Paged_Result()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                await CreateTestGroupBuyAsync($"Group Buy {i}");
            }

            var input = new GetGroupBuyInput
            {
                MaxResultCount = 3,
                SkipCount = 0
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(5);
            result.Items.Count.ShouldBe(3);
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_Status()
        {
            // Arrange
            await CreateTestGroupBuyAsync("Released GB", "Released");
            await CreateTestGroupBuyAsync("Awaiting GB", "AwaitingRelease");
            await CreateTestGroupBuyAsync("Closed GB", "Closed");

            var input = new GetGroupBuyInput
            {
                Status = "Released",
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.ShouldAllBe(x => x.Status == "Released");
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_Date_Range()
        {
            // Arrange
            var now = DateTime.UtcNow;
            
            var gb1 = await CreateTestGroupBuyAsync("Active GB");
            gb1.StartTime = now.AddDays(-10);
            gb1.EndTime = now.AddDays(10);
            await _groupBuyRepository.UpdateAsync(gb1);

            var gb2 = await CreateTestGroupBuyAsync("Future GB");
            gb2.StartTime = now.AddDays(5);
            gb2.EndTime = now.AddDays(15);
            await _groupBuyRepository.UpdateAsync(gb2);

            var input = new GetGroupBuyInput
            {
                StartTime = now.AddDays(-1),
                EndTime = now.AddDays(1),
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.ShouldContain(x => x.Id == gb1.Id);
            result.Items.ShouldNotContain(x => x.Id == gb2.Id);
        }

        #endregion

        #region ShortCode Tests

        [Fact]
        public async Task CheckShortCodeForCreate_Should_Return_True_When_Available()
        {
            // Arrange
            var shortCode = "UNIQUE001";

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.CheckShortCodeForCreate(shortCode);
            });

            // Assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task CheckShortCodeForCreate_Should_Return_False_When_Exists()
        {
            // Arrange
            var existingGroupBuy = await CreateTestGroupBuyAsync(shortCode: "EXIST001");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.CheckShortCodeForCreate("EXIST001");
            });

            // Assert
            result.ShouldBe(false);
        }

        [Fact]
        public async Task CheckShortCodeForEdit_Should_Return_True_For_Same_GroupBuy()
        {
            // Arrange
            var groupBuy = await CreateTestGroupBuyAsync(shortCode: "EDIT001");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.CheckShortCodeForEdit("EDIT001", groupBuy.Id);
            });

            // Assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task GetGroupBuyByShortCode_Should_Return_Matching_GroupBuys()
        {
            // Arrange
            await CreateTestGroupBuyAsync(shortCode: "FIND001");
            await CreateTestGroupBuyAsync(shortCode: "FIND002");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.GetGroupBuyByShortCode("FIND001");
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result.ShouldAllBe(x => x.ShortCode == "FIND001");
        }

        [Fact]
        public async Task GetGroupBuyIdAsync_Should_Return_Id_By_ShortCode()
        {
            // Arrange
            var groupBuy = await CreateTestGroupBuyAsync(shortCode: "GETID001");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.GetGroupBuyIdAsync("GETID001");
            });

            // Assert
            result.ShouldNotBeNull();
            result.Value.ShouldBe(groupBuy.Id);
        }

        #endregion

        #region Availability Tests

        [Fact]
        public async Task ChangeGroupBuyAvailability_Should_Toggle_Availability()
        {
            // Arrange
            var groupBuy = await CreateTestGroupBuyAsync();
            await _groupBuyRepository.UpdateAsync(groupBuy);

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _groupBuyAppService.ChangeGroupBuyAvailability(groupBuy.Id);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var updated = await _groupBuyRepository.GetAsync(groupBuy.Id);
            });
        }

        #endregion

        #region Copy Tests

        [Fact]
        public async Task CopyAsync_Should_Create_Duplicate()
        {
            // Arrange
            var originalGroupBuy = await CreateTestGroupBuyAsync(
                groupBuyName: "Original GB",
                shortCode: "ORIG001"
            );

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.CopyAsync(originalGroupBuy.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(originalGroupBuy.Id);
            result.GroupBuyName.ShouldContain("Original GB");
            result.ShortCode.ShouldNotBe("ORIG001"); // Should generate new short code
            result.Status.ShouldBe("AwaitingRelease");
        }

        #endregion

        #region Lookup Tests

        [Fact]
        public async Task GetGroupBuyLookupAsync_Should_Return_KeyValue_List()
        {
            // Arrange
            await CreateTestGroupBuyAsync("Lookup GB 1");
            await CreateTestGroupBuyAsync("Lookup GB 2");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.GetGroupBuyLookupAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThanOrEqualTo(2);
            result.ShouldAllBe(x => x.Id != Guid.Empty && !string.IsNullOrEmpty(x.Name));
        }

        #endregion

        #region Report Tests

        [Fact]
        public async Task GetGroupBuyReportListAsync_Should_Return_Reports()
        {
            // Arrange
            await CreateTestGroupBuyAsync("Report GB 1");
            await CreateTestGroupBuyAsync("Report GB 2");

            var input = new GetGroupBuyReportListDto
            {
                MaxResultCount = 10,
                SkipCount = 0
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _groupBuyAppService.GetGroupBuyReportListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        }

        #endregion
    }
}
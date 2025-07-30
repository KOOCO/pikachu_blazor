using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.InventoryManagement;
using Kooco.Pikachu.Items;
using MiniExcelLibs;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.Application.Tests.InventoryManagement
{
    public class InventoryAppServiceRealTests : PikachuApplicationTestBase
    {
        private readonly IInventoryAppService _inventoryAppService;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<ItemDetails, Guid> _itemDetailRepository;

        public InventoryAppServiceRealTests()
        {
            _inventoryAppService = GetRequiredService<IInventoryAppService>();
            _itemRepository = GetRequiredService<IRepository<Item, Guid>>();
            _itemDetailRepository = GetRequiredService<IRepository<ItemDetails, Guid>>();
        }

        #region Test Helpers

        private async Task<(Item item, ItemDetails detail)> CreateTestItemWithDetailsAsync(
            string itemName = null,
            string sku = null,
            int stockOnHand = 100,
            int saleableQuantity = 80)
        {
            var item = new Item
            {
                ItemName = itemName ?? $"Test Item {Guid.NewGuid().ToString().Substring(0, 8)}"
            };
            await _itemRepository.InsertAsync(item);

            var detail = new ItemDetails
            {
                ItemId = item.Id,
                ItemName = item.ItemName + " Detail",
                SKU = sku ?? $"TEST-SKU-{Guid.NewGuid().ToString().Substring(0, 8)}",
                StockOnHand = stockOnHand,
                SaleableQuantity = saleableQuantity,
                PreOrderableQuantity = 20,
                SaleablePreOrderQuantity = 15,
                InventoryAccount = "Main Warehouse"
            };
            await _itemDetailRepository.InsertAsync(detail);

            return (item, detail);
        }

        #endregion

        #region Basic Tests

        [Fact]
        public async Task GetAsync_Should_Return_Inventory_When_Item_Exists()
        {
            // Arrange
            var (item, detail) = await CreateTestItemWithDetailsAsync();

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetAsync(item.Id, detail.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ItemId.ShouldBe(item.Id);
            result.ItemDetailId.ShouldBe(detail.Id);
            result.ItemName.ShouldContain(item.ItemName);
            result.Sku.ShouldBe(detail.SKU);
            result.CurrentStock.ShouldBe(detail.StockOnHand ?? 0);
            result.AvailableStock.ShouldBe((int)detail.SaleableQuantity);
        }

        [Fact]
        public async Task GetAsync_Should_Throw_When_Item_Not_Found()
        {
            // Arrange
            var nonExistentItemId = Guid.NewGuid();
            var nonExistentDetailId = Guid.NewGuid();

            // Act & Assert
            await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    await _inventoryAppService.GetAsync(nonExistentItemId, nonExistentDetailId);
                });
            });
        }

        [Fact]
        public async Task GetListAsync_Should_Return_Empty_When_No_Items()
        {
            // Arrange
            var input = new GetInventoryDto
            {
                Filter = "NonExistentItem" + Guid.NewGuid(),
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(0);
            result.Items.Count.ShouldBe(0);
        }

        [Fact]
        public async Task GetListAsync_Should_Return_Items_With_Pagination()
        {
            // Arrange
            var items = new List<(Item, ItemDetails)>();
            for (int i = 0; i < 5; i++)
            {
                items.Add(await CreateTestItemWithDetailsAsync(
                    itemName: $"Pagination Test Item {i}",
                    sku: $"PAGE-{i:D3}",
                    stockOnHand: 100 + (i * 10)
                ));
            }

            var input = new GetInventoryDto
            {
                MaxResultCount = 3,
                SkipCount = 0,
                Filter = "Pagination Test"
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(5);
            result.Items.Count.ShouldBeLessThanOrEqualTo(3);
        }

        #endregion

        #region Lookup Tests

        [Fact]
        public async Task GetWarehouseLookupAsync_Should_Return_Distinct_Warehouses()
        {
            // Arrange
            await CreateTestItemWithDetailsAsync();

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetWarehouseLookupAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<string>>();
            // The list might contain warehouses from previously created test data
            result.Distinct().Count().ShouldBe(result.Count); // All should be unique
        }

        [Fact]
        public async Task GetSkuLookupAsync_Should_Return_Distinct_SKUs()
        {
            // Arrange
            var testSku = $"LOOKUP-SKU-{Guid.NewGuid().ToString().Substring(0, 8)}";
            await CreateTestItemWithDetailsAsync(sku: testSku);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetSkuLookupAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<string>>();
            result.ShouldContain(testSku);
            result.Distinct().Count().ShouldBe(result.Count); // All should be unique
        }

        [Fact]
        public async Task GetAttributesLookupAsync_Should_Return_Distinct_Attributes()
        {
            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetAttributesLookupAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<string>>();
            result.Distinct().Count().ShouldBe(result.Count); // All should be unique
        }

        #endregion

        #region Excel Export Tests

        [Fact]
        public async Task GetListAsExcelAsync_Should_Export_With_Empty_List()
        {
            // Arrange
            var emptyList = new List<InventoryDto>();

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetListAsExcelAsync(emptyList);
            });

            // Assert
            result.ShouldNotBeNull();
            result.FileName.ShouldContain(".xlsx");
            result.ContentType.ShouldBe("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            
            // Verify the stream is valid
            using var memoryStream = new MemoryStream();
            await result.GetStream().CopyToAsync(memoryStream);
            memoryStream.Length.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task GetListAsExcelAsync_Should_Export_With_Data()
        {
            // Arrange
            var items = new List<InventoryDto>
            {
                new InventoryDto
                {
                    ItemName = "Excel Test Item 1",
                    Sku = "EXCEL-001",
                    Warehouse = "Test Warehouse",
                    CurrentStock = 100,
                    AvailableStock = 90,
                    PreOrderQuantity = 10,
                    AvailablePreOrderQuantity = 8
                },
                new InventoryDto
                {
                    ItemName = "Excel Test Item 2",
                    Sku = "EXCEL-002",
                    Warehouse = "Test Warehouse",
                    CurrentStock = 50,
                    AvailableStock = 45,
                    PreOrderQuantity = 5,
                    AvailablePreOrderQuantity = 4
                }
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetListAsExcelAsync(items);
            });

            // Assert
            result.ShouldNotBeNull();
            result.FileName.ShouldContain(".xlsx");
            
            // Verify the excel content
            using var memoryStream = new MemoryStream();
            await result.GetStream().CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            
            var excelData = memoryStream.Query<Dictionary<string, object>>().ToList();
            excelData.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GetListAsExcelAsync_Without_Items_Should_Export_All()
        {
            // Arrange
            await CreateTestItemWithDetailsAsync(itemName: "Excel Export All Test");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetListAsExcelAsync(null);
            });

            // Assert
            result.ShouldNotBeNull();
            result.FileName.ShouldContain(".xlsx");
            
            using var memoryStream = new MemoryStream();
            await result.GetStream().CopyToAsync(memoryStream);
            memoryStream.Length.ShouldBeGreaterThan(0);
        }

        #endregion

        #region Filter Tests

        [Fact]
        public async Task GetListAsync_Should_Filter_By_Warehouse()
        {
            // Arrange
            var warehouse = "Filter Test Warehouse " + Guid.NewGuid().ToString().Substring(0, 8);
            var (item, detail) = await CreateTestItemWithDetailsAsync();
            detail.InventoryAccount = warehouse;
            await _itemDetailRepository.UpdateAsync(detail);

            var input = new GetInventoryDto
            {
                Warehouse = warehouse,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            if (result.Items.Any())
            {
                result.Items.ShouldAllBe(x => x.Warehouse == warehouse);
            }
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_Stock_Range()
        {
            // Arrange
            var items = new List<(Item, ItemDetails)>();
            for (int i = 0; i < 3; i++)
            {
                items.Add(await CreateTestItemWithDetailsAsync(
                    itemName: $"Stock Range Test {i}",
                    stockOnHand: 50 + (i * 50) // 50, 100, 150
                ));
            }

            var input = new GetInventoryDto
            {
                MinCurrentStock = 75,
                MaxCurrentStock = 125,
                Filter = "Stock Range Test",
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            if (result.Items.Any())
            {
                result.Items.ShouldAllBe(x => x.CurrentStock >= 75 && x.CurrentStock <= 125);
            }
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_Available_Stock()
        {
            // Arrange
            var items = new List<(Item, ItemDetails)>();
            for (int i = 0; i < 3; i++)
            {
                items.Add(await CreateTestItemWithDetailsAsync(
                    itemName: $"Available Stock Test {i}",
                    saleableQuantity: 20 + (i * 30) // 20, 50, 80
                ));
            }

            var input = new GetInventoryDto
            {
                MinAvailableStock = 40,
                MaxAvailableStock = 90,
                Filter = "Available Stock Test",
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            if (result.Items.Any())
            {
                result.Items.ShouldAllBe(x => x.AvailableStock >= 40 && x.AvailableStock <= 90);
            }
        }

        #endregion

        #region Sorting Tests

        [Fact]
        public async Task GetListAsync_Should_Support_Sorting()
        {
            // Arrange
            var baseItemName = $"Sort Test {Guid.NewGuid().ToString().Substring(0, 8)}";
            for (int i = 0; i < 3; i++)
            {
                await CreateTestItemWithDetailsAsync(
                    itemName: $"{baseItemName} {3 - i}", // Create in reverse order
                    stockOnHand: (3 - i) * 100
                );
            }

            var input = new GetInventoryDto
            {
                Filter = baseItemName,
                Sorting = "ItemName ASC",
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _inventoryAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            if (result.Items.Count > 1)
            {
                // Verify ascending order
                for (int i = 1; i < result.Items.Count; i++)
                {
                    result.Items[i].ItemName.CompareTo(result.Items[i - 1].ItemName).ShouldBeGreaterThanOrEqualTo(0);
                }
            }
        }

        #endregion
    }
}
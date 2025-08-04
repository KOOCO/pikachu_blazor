using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Items.Dtos;
using NSubstitute;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.ProductCategories
{
    public class ProductCategoryAppServiceAdvancedTests : PikachuApplicationTestBase
    {
        private readonly IProductCategoryAppService _productCategoryAppService;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IRepository<ProductCategory, Guid> _repository;

        public ProductCategoryAppServiceAdvancedTests()
        {
            _productCategoryAppService = GetRequiredService<IProductCategoryAppService>();
            _productCategoryRepository = GetRequiredService<IProductCategoryRepository>();
            _repository = GetRequiredService<IRepository<ProductCategory, Guid>>();
        }

        #region Test Helpers

        private async Task<ProductCategory> CreateTestCategoryWithImagesAsync(
            string name,
            string zhName,
            int imageCount = 3)
        {
            var category = new ProductCategory(
                Guid.NewGuid(),
                name,
                zhName,
                $"Description for {name}",
                null
            );

            for (int i = 1; i <= imageCount; i++)
            {
                category.AddProductCategoryImage(
                    Guid.NewGuid(),
                    $"https://test.com/image{i}.jpg",
                    $"image{i}.jpg",
                    $"Image {i}",
                    i
                );
            }

            await _repository.InsertAsync(category);
            return category;
        }

        private async Task<List<ProductCategory>> CreateCategoryHierarchyAsync()
        {
            var categories = new List<ProductCategory>();

            // Create main categories
            var electronics = new ProductCategory(Guid.NewGuid(), "Electronics", "電子產品", "Electronic products", null);
            var clothing = new ProductCategory(Guid.NewGuid(), "Clothing", "服裝", "Clothing and apparel", null);
            
            await _repository.InsertAsync(electronics);
            await _repository.InsertAsync(clothing);
            categories.Add(electronics);
            categories.Add(clothing);

            // Create sub categories for electronics
            var computers = new ProductCategory(Guid.NewGuid(), "Computers", "電腦", "Computer products", electronics.Id);
            var phones = new ProductCategory(Guid.NewGuid(), "Phones", "手機", "Mobile phones", electronics.Id);
            var accessories = new ProductCategory(Guid.NewGuid(), "Accessories", "配件", "Electronic accessories", electronics.Id);
            
            await _repository.InsertAsync(computers);
            await _repository.InsertAsync(phones);
            await _repository.InsertAsync(accessories);
            categories.AddRange(new[] { computers, phones, accessories });

            // Create sub categories for clothing
            var mens = new ProductCategory(Guid.NewGuid(), "Men's Clothing", "男裝", "Men's apparel", clothing.Id);
            var womens = new ProductCategory(Guid.NewGuid(), "Women's Clothing", "女裝", "Women's apparel", clothing.Id);
            
            await _repository.InsertAsync(mens);
            await _repository.InsertAsync(womens);
            categories.AddRange(new[] { mens, womens });

            return categories;
        }

        #endregion

        #region Image Limit Tests

        [Fact]
        public async Task CreateAsync_Should_Respect_Max_Image_Limit()
        {
            // Arrange
            var input = new CreateProductCategoryDto
            {
                Name = "Test Category",
                ZHName = "測試分類",
                Description = "Test",
                ProductCategoryImages = new List<CreateUpdateProductCategoryImageDto>(),
                CategoryProducts = new List<CreateUpdateCategoryProductDto>()
            };

            // Add images up to the limit (assuming limit is 10 from ProductCategoryConsts)
            for (int i = 1; i <= 15; i++) // Try to add more than limit
            {
                input.ProductCategoryImages.Add(new CreateUpdateProductCategoryImageDto
                {
                    Url = $"https://test.com/image{i}.jpg",
                    BlobName = $"image{i}.jpg",
                    Name = $"Image {i}",
                    SortNo = i
                });
            }

            // Act & Assert
            await Should.ThrowAsync<ProductCategoryImagesMaxLimitException>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    return await _productCategoryAppService.CreateAsync(input);
                });
            });
        }

        #endregion

        #region Localization Tests

        [Fact]
        public async Task GetMainProductCategoryLookupAsync_Should_Return_Localized_Names_For_Chinese()
        {
            // Arrange
            await CreateCategoryHierarchyAsync();
            
            // Set culture to Chinese
            Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-CN");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetMainProductCategoryLookupAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            var electronics = result.FirstOrDefault(x => x.Id != Guid.Empty && x.Name == "電子產品");
            electronics.ShouldNotBeNull();
            
            var clothing = result.FirstOrDefault(x => x.Id != Guid.Empty && x.Name == "服裝");
            clothing.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetMainProductCategoryLookupAsync_Should_Return_English_Names_For_English()
        {
            // Arrange
            await CreateCategoryHierarchyAsync();
            
            // Set culture to English
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetMainProductCategoryLookupAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            var electronics = result.FirstOrDefault(x => x.Id != Guid.Empty && x.Name == "Electronics");
            electronics.ShouldNotBeNull();
            
            var clothing = result.FirstOrDefault(x => x.Id != Guid.Empty && x.Name == "Clothing");
            clothing.ShouldNotBeNull();
        }

        #endregion

        #region Complex Update Scenarios

        [Fact]
        public async Task UpdateAsync_Moving_Category_To_Different_Parent_Should_Work()
        {
            // Arrange
            var categories = await CreateCategoryHierarchyAsync();
            var electronics = categories.First(x => x.Name == "Electronics");
            var clothing = categories.First(x => x.Name == "Clothing");
            var accessories = categories.First(x => x.Name == "Accessories");

            var updateDto = new UpdateProductCategoryDto
            {
                Name = accessories.Name,
                ZHName = accessories.ZHName,
                Description = "Moved to clothing",
                MainCategoryId = clothing.Id, // Move from electronics to clothing
                ProductCategoryImages = new List<CreateUpdateProductCategoryImageDto>(),
                CategoryProducts = new List<CreateUpdateCategoryProductDto>()
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.UpdateAsync(accessories.Id, updateDto);
            });

            // Assert
            result.ShouldNotBeNull();
            result.MainCategoryId.ShouldBe(clothing.Id);

            // Verify sub categories
            var electronicsSubCategories = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetSubCategoryListAsync(electronics.Id);
            });
            electronicsSubCategories.ShouldNotContain(x => x.Id == accessories.Id);

            var clothingSubCategories = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetSubCategoryListAsync(clothing.Id);
            });
            clothingSubCategories.ShouldContain(x => x.Id == accessories.Id);
        }

        [Fact]
        public async Task UpdateAsync_Making_SubCategory_Main_Category_Should_Work()
        {
            // Arrange
            var categories = await CreateCategoryHierarchyAsync();
            var computers = categories.First(x => x.Name == "Computers");

            var updateDto = new UpdateProductCategoryDto
            {
                Name = "Computers & IT",
                ZHName = "電腦與資訊",
                Description = "Now a main category",
                MainCategoryId = null, // Remove parent
                ProductCategoryImages = new List<CreateUpdateProductCategoryImageDto>(),
                CategoryProducts = new List<CreateUpdateCategoryProductDto>()
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.UpdateAsync(computers.Id, updateDto);
            });

            // Assert
            result.ShouldNotBeNull();
            result.MainCategoryId.ShouldBeNull();
            result.Name.ShouldBe("Computers & IT");

            // Verify it appears in main categories
            var mainCategories = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetMainProductCategoryLookupAsync();
            });
            mainCategories.ShouldContain(x => x.Id == computers.Id);
        }

        #endregion

        #region Batch Operations

        [Fact]
        public async Task Batch_Create_Categories_Should_Work()
        {
            // Arrange
            var categoryNames = new[]
            {
                ("Books", "書籍"),
                ("Music", "音樂"),
                ("Sports", "運動"),
                ("Home & Garden", "家居園藝"),
                ("Toys", "玩具")
            };

            var createdCategories = new List<ProductCategoryDto>();

            // Act
            foreach (var (name, zhName) in categoryNames)
            {
                var input = new CreateProductCategoryDto
                {
                    Name = name,
                    ZHName = zhName,
                    Description = $"{name} category",
                    ProductCategoryImages = new List<CreateUpdateProductCategoryImageDto>(),
                    CategoryProducts = new List<CreateUpdateCategoryProductDto>()
                };

                var result = await WithUnitOfWorkAsync(async () =>
                {
                    return await _productCategoryAppService.CreateAsync(input);
                });

                createdCategories.Add(result);
            }

            // Assert
            createdCategories.Count.ShouldBe(5);
            createdCategories.ShouldAllBe(x => x.Id != Guid.Empty);
            
            // Verify all were created
            var allCategories = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetProductCategoryLookupAsync();
            });

            foreach (var (name, _) in categoryNames)
            {
                allCategories.ShouldContain(x => x.Name == name);
            }
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task CreateAsync_With_Only_ZHName_Should_Work()
        {
            // Arrange
            var input = new CreateProductCategoryDto
            {
                Name = null,
                ZHName = "中文分類",
                Description = "Category with only Chinese name",
                ProductCategoryImages = new List<CreateUpdateProductCategoryImageDto>(),
                CategoryProducts = new List<CreateUpdateCategoryProductDto>()
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBeNull();
            result.ZHName.ShouldBe("中文分類");
        }

        [Fact]
        public async Task CreateAsync_With_Only_Name_Should_Work()
        {
            // Arrange
            var input = new CreateProductCategoryDto
            {
                Name = "English Category",
                ZHName = null,
                Description = "Category with only English name",
                ProductCategoryImages = new List<CreateUpdateProductCategoryImageDto>(),
                CategoryProducts = new List<CreateUpdateCategoryProductDto>()
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe("English Category");
            result.ZHName.ShouldBeNull();
        }

        [Fact]
        public async Task GetDefaultImageUrlAsync_With_No_Images_Should_Return_Null()
        {
            // Arrange
            var category = new ProductCategory(
                Guid.NewGuid(),
                "No Images",
                "無圖片",
                "Category without images",
                null
            );
            await _repository.InsertAsync(category);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetDefaultImageUrlAsync(category.Id);
            });

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task UpdateAsync_With_Duplicate_CategoryProducts_Should_Add_Once()
        {
            // Arrange
            var category = await CreateTestCategoryWithImagesAsync("Test", "測試", 0);
            var itemId = Guid.NewGuid();

            var updateDto = new UpdateProductCategoryDto
            {
                Name = category.Name,
                ZHName = category.ZHName,
                Description = category.Description,
                CategoryProducts = new List<CreateUpdateCategoryProductDto>
                {
                    new CreateUpdateCategoryProductDto { ItemId = itemId },
                    new CreateUpdateCategoryProductDto { ItemId = itemId }, // Duplicate
                    new CreateUpdateCategoryProductDto { ItemId = itemId }  // Another duplicate
                },
                ProductCategoryImages = new List<CreateUpdateProductCategoryImageDto>()
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.UpdateAsync(category.Id, updateDto);
            });

            // Assert
            result.ShouldNotBeNull();
            result.CategoryProducts.Count.ShouldBe(1);
            result.CategoryProducts[0].ItemId.ShouldBe(itemId);
        }

        #endregion

        #region Performance Tests

        [Fact]
        public async Task GetListAsync_With_Large_Dataset_Should_Handle_Pagination()
        {
            // Arrange - Create many categories
            for (int i = 1; i <= 50; i++)
            {
                await CreateTestCategoryWithImagesAsync($"Category {i}", $"分類 {i}", 0);
            }

            // Act - Get different pages
            var page1 = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetListAsync(new GetProductCategoryListDto
                {
                    SkipCount = 0,
                    MaxResultCount = 10,
                    Sorting = "Name"
                });
            });

            var page2 = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetListAsync(new GetProductCategoryListDto
                {
                    SkipCount = 10,
                    MaxResultCount = 10,
                    Sorting = "Name"
                });
            });

            var page3 = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetListAsync(new GetProductCategoryListDto
                {
                    SkipCount = 20,
                    MaxResultCount = 10,
                    Sorting = "Name"
                });
            });

            // Assert
            page1.TotalCount.ShouldBeGreaterThanOrEqualTo(50);
            page1.Items.Count.ShouldBe(10);
            page2.Items.Count.ShouldBe(10);
            page3.Items.Count.ShouldBe(10);

            // Ensure no overlap between pages
            var page1Ids = page1.Items.Select(x => x.Id).ToList();
            var page2Ids = page2.Items.Select(x => x.Id).ToList();
            var page3Ids = page3.Items.Select(x => x.Id).ToList();

            page1Ids.Intersect(page2Ids).ShouldBeEmpty();
            page2Ids.Intersect(page3Ids).ShouldBeEmpty();
            page1Ids.Intersect(page3Ids).ShouldBeEmpty();
        }

        #endregion
    }
}
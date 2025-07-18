using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Items.Dtos;
using NSubstitute;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.ProductCategories
{
    public class ProductCategoryAppServiceTests : PikachuApplicationTestBase
    {
        private readonly IProductCategoryAppService _productCategoryAppService;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IRepository<ProductCategory, Guid> _repository;
        private readonly ImageContainerManager _mockImageContainerManager;

        public ProductCategoryAppServiceTests()
        {
            _productCategoryAppService = GetRequiredService<IProductCategoryAppService>();
            _productCategoryRepository = GetRequiredService<IProductCategoryRepository>();
            _repository = GetRequiredService<IRepository<ProductCategory, Guid>>();
            _mockImageContainerManager = Substitute.For<ImageContainerManager>();
        }

        #region Test Helpers

        private async Task<ProductCategory> CreateTestCategoryAsync(
            string name = "Test Category",
            string zhName = "測試分類",
            string description = "Test Description",
            Guid? mainCategoryId = null)
        {
            var category = new ProductCategory(
                Guid.NewGuid(),
                name,
                zhName,
                description,
                mainCategoryId
            );

            await _repository.InsertAsync(category);
            return category;
        }

        private CreateProductCategoryDto GetCreateDto(
            string name = "New Category",
            string zhName = "新分類",
            string description = "New Description",
            Guid? mainCategoryId = null)
        {
            return new CreateProductCategoryDto
            {
                Name = name,
                ZHName = zhName,
                Description = description,
                MainCategoryId = mainCategoryId,
                ProductCategoryImages = new List<CreateUpdateProductCategoryImageDto>(),
                CategoryProducts = new List<CreateUpdateCategoryProductDto>()
            };
        }

        private UpdateProductCategoryDto GetUpdateDto(
            string name = "Updated Category",
            string zhName = "更新分類",
            string description = "Updated Description",
            Guid? mainCategoryId = null)
        {
            return new UpdateProductCategoryDto
            {
                Name = name,
                ZHName = zhName,
                Description = description,
                MainCategoryId = mainCategoryId,
                ProductCategoryImages = new List<CreateUpdateProductCategoryImageDto>(),
                CategoryProducts = new List<CreateUpdateCategoryProductDto>()
            };
        }

        #endregion

        #region CRUD Tests

        [Fact]
        public async Task CreateAsync_Should_Create_ProductCategory()
        {
            // Arrange
            var input = GetCreateDto();

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe("New Category");
            result.ZHName.ShouldBe("新分類");
            result.Description.ShouldBe("New Description");
        }

        [Fact]
        public async Task CreateAsync_With_Images_Should_Create_Category_With_Images()
        {
            // Arrange
            var input = GetCreateDto();
            input.ProductCategoryImages = new List<CreateUpdateProductCategoryImageDto>
            {
                new CreateUpdateProductCategoryImageDto
                {
                    Url = "https://test.com/image1.jpg",
                    BlobName = "image1.jpg",
                    Name = "Test Image 1",
                    SortNo = 1
                },
                new CreateUpdateProductCategoryImageDto
                {
                    Url = "https://test.com/image2.jpg",
                    BlobName = "image2.jpg",
                    Name = "Test Image 2",
                    SortNo = 2
                }
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ProductCategoryImages.Count.ShouldBe(2);
            result.ProductCategoryImages[0].Name.ShouldBe("Test Image 1");
            result.ProductCategoryImages[1].Name.ShouldBe("Test Image 2");
        }

        [Fact]
        public async Task CreateAsync_With_CategoryProducts_Should_Create_Category_With_Products()
        {
            // Arrange
            var input = GetCreateDto();
            var itemId1 = Guid.NewGuid();
            var itemId2 = Guid.NewGuid();

            input.CategoryProducts = new List<CreateUpdateCategoryProductDto>
            {
                new CreateUpdateCategoryProductDto { ItemId = itemId1 },
                new CreateUpdateCategoryProductDto { ItemId = itemId2 }
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.CreateAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.CategoryProducts.Count.ShouldBe(2);
            result.CategoryProducts.Select(x => x.ItemId).ShouldContain(itemId1);
            result.CategoryProducts.Select(x => x.ItemId).ShouldContain(itemId2);
        }

        [Fact]
        public async Task GetAsync_Should_Return_ProductCategory()
        {
            // Arrange
            var category = await CreateTestCategoryAsync();

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetAsync(category.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(category.Id);
            result.Name.ShouldBe("Test Category");
            result.ZHName.ShouldBe("測試分類");
        }

        [Fact]
        public async Task GetAsync_With_IncludeDetails_Should_Return_Category_With_Details()
        {
            // Arrange
            var category = await CreateTestCategoryAsync();
            category.AddProductCategoryImage(
                Guid.NewGuid(),
                "https://test.com/image.jpg",
                "image.jpg",
                "Test Image",
                1
            );
            await _repository.UpdateAsync(category);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetAsync(category.Id, includeDetails: true);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ProductCategoryImages.Count.ShouldBe(1);
            result.ProductCategoryImages[0].Name.ShouldBe("Test Image");
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_ProductCategory()
        {
            // Arrange
            var category = await CreateTestCategoryAsync();
            var input = GetUpdateDto();

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.UpdateAsync(category.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Updated Category");
            result.ZHName.ShouldBe("更新分類");
            result.Description.ShouldBe("Updated Description");
        }

        [Fact]
        public async Task UpdateAsync_Should_Replace_Images_And_Products()
        {
            // Arrange
            var category = await CreateTestCategoryAsync();
            
            // Add initial image
            category.AddProductCategoryImage(
                Guid.NewGuid(),
                "https://test.com/old-image.jpg",
                "old-image.jpg",
                "Old Image",
                1
            );
            category.AddCategoryProduct(Guid.NewGuid());
            await _repository.UpdateAsync(category);

            var input = GetUpdateDto();
            input.ProductCategoryImages = new List<CreateUpdateProductCategoryImageDto>
            {
                new CreateUpdateProductCategoryImageDto
                {
                    Url = "https://test.com/new-image.jpg",
                    BlobName = "new-image.jpg",
                    Name = "New Image",
                    SortNo = 1
                }
            };

            var newItemId = Guid.NewGuid();
            input.CategoryProducts = new List<CreateUpdateCategoryProductDto>
            {
                new CreateUpdateCategoryProductDto { ItemId = newItemId }
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.UpdateAsync(category.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ProductCategoryImages.Count.ShouldBe(1);
            result.ProductCategoryImages[0].Name.ShouldBe("New Image");
            result.CategoryProducts.Count.ShouldBe(1);
            result.CategoryProducts[0].ItemId.ShouldBe(newItemId);
        }

        [Fact]
        public async Task DeleteAsync_Should_Delete_ProductCategory()
        {
            // Arrange
            var category = await CreateTestCategoryAsync();

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _productCategoryAppService.DeleteAsync(category.Id);
            });

            // Assert
            var deletedCategory = await WithUnitOfWorkAsync(async () =>
            {
                return await _repository.FindAsync(category.Id);
            });
            deletedCategory.ShouldBeNull();
        }

        #endregion

        #region List and Query Tests

        [Fact]
        public async Task GetListAsync_Should_Return_Paged_Categories()
        {
            // Arrange
            await CreateTestCategoryAsync("Category 1", "分類1");
            await CreateTestCategoryAsync("Category 2", "分類2");
            await CreateTestCategoryAsync("Category 3", "分類3");

            var input = new GetProductCategoryListDto
            {
                MaxResultCount = 2,
                SkipCount = 0
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
            result.Items.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GetListAsync_With_Filter_Should_Return_Filtered_Categories()
        {
            // Arrange
            await CreateTestCategoryAsync("Electronics", "電子產品");
            await CreateTestCategoryAsync("Clothing", "服裝");
            await CreateTestCategoryAsync("Food Electronics", "食品電子");

            var input = new GetProductCategoryListDto
            {
                Filter = "Electronics",
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.ShouldAllBe(x => x.Name.Contains("Electronics"));
        }

        [Fact]
        public async Task GetSubCategoryListAsync_Should_Return_Sub_Categories()
        {
            // Arrange
            var mainCategory = await CreateTestCategoryAsync("Main Category", "主分類");
            await CreateTestCategoryAsync("Sub Category 1", "子分類1", mainCategoryId: mainCategory.Id);
            await CreateTestCategoryAsync("Sub Category 2", "子分類2", mainCategoryId: mainCategory.Id);
            await CreateTestCategoryAsync("Other Category", "其他分類");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetSubCategoryListAsync(mainCategory.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.ShouldAllBe(x => x.MainCategoryId == mainCategory.Id);
        }

        #endregion

        #region Lookup Tests

        [Fact]
        public async Task GetProductCategoryLookupAsync_Should_Return_All_Categories()
        {
            // Arrange
            await CreateTestCategoryAsync("Category A", "分類A");
            await CreateTestCategoryAsync("Category B", "分類B");
            await CreateTestCategoryAsync("Category C", "分類C");

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetProductCategoryLookupAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThanOrEqualTo(3);
            result.ShouldContain(x => x.Name == "Category A");
            result.ShouldContain(x => x.Name == "Category B");
            result.ShouldContain(x => x.Name == "Category C");
        }

        [Fact]
        public async Task GetMainProductCategoryLookupAsync_Should_Return_Only_Main_Categories()
        {
            // Arrange
            var mainCategory1 = await CreateTestCategoryAsync("Main 1", "主分類1");
            var mainCategory2 = await CreateTestCategoryAsync("Main 2", "主分類2");
            await CreateTestCategoryAsync("Sub 1", "子分類1", mainCategoryId: mainCategory1.Id);
            await CreateTestCategoryAsync("Sub 2", "子分類2", mainCategoryId: mainCategory2.Id);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetMainProductCategoryLookupAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            var mainCategories = result.Where(x => x.Name == "Main 1" || x.Name == "Main 2").ToList();
            mainCategories.Count.ShouldBe(2);
            result.ShouldNotContain(x => x.Name.StartsWith("Sub"));
        }

        #endregion

        #region Image Upload Tests

        [Fact]
        public async Task UploadImagesAsync_Should_Upload_And_Process_Images()
        {
            // Arrange
            var input = new List<CreateUpdateProductCategoryImageDto>
            {
                new CreateUpdateProductCategoryImageDto
                {
                    Base64 = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }),
                    BlobName = "old-image1.jpg",
                    Name = "image1.jpg"
                },
                new CreateUpdateProductCategoryImageDto
                {
                    Base64 = Convert.ToBase64String(new byte[] { 5, 6, 7, 8 }),
                    BlobName = "old-image2.jpg",
                    Name = "image2.jpg"
                }
            };

            // Act
            var result = await _productCategoryAppService.UploadImagesAsync(input, deleteExisting: false);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.ShouldAllBe(x => !x.BlobName.StartsWith("old-"));
            result.ShouldAllBe(x => !string.IsNullOrEmpty(x.Url));
        }

        [Fact]
        public async Task GetDefaultImageUrlAsync_Should_Return_First_Image_Url()
        {
            // Arrange
            var category = await CreateTestCategoryAsync();
            category.AddProductCategoryImage(
                Guid.NewGuid(),
                "https://test.com/image1.jpg",
                "image1.jpg",
                "Image 1",
                1
            );
            category.AddProductCategoryImage(
                Guid.NewGuid(),
                "https://test.com/image2.jpg",
                "image2.jpg",
                "Image 2",
                2
            );
            await _repository.UpdateAsync(category);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetDefaultImageUrlAsync(category.Id);
            });

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe("https://test.com/image1.jpg");
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task CreateAsync_With_Null_Input_Should_Throw()
        {
            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(async () =>
            {
                await _productCategoryAppService.CreateAsync(null);
            });
        }

        [Fact]
        public async Task UpdateAsync_With_NonExistent_Id_Should_Throw()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var input = GetUpdateDto();

            // Act & Assert
            await Should.ThrowAsync<Exception>(async () =>
            {
                await _productCategoryAppService.UpdateAsync(nonExistentId, input);
            });
        }

        [Fact]
        public async Task DeleteAsync_With_NonExistent_Id_Should_Throw()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Should.ThrowAsync<Exception>(async () =>
            {
                await _productCategoryAppService.DeleteAsync(nonExistentId);
            });
        }

        #endregion

        #region Complex Scenarios

        [Fact]
        public async Task Create_Category_Hierarchy_Should_Work_Correctly()
        {
            // Arrange & Act
            var mainCategory = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.CreateAsync(GetCreateDto("Electronics", "電子產品"));
            });

            var subCategory1 = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.CreateAsync(
                    GetCreateDto("Computers", "電腦", mainCategoryId: mainCategory.Id));
            });

            var subCategory2 = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.CreateAsync(
                    GetCreateDto("Mobile Phones", "手機", mainCategoryId: mainCategory.Id));
            });

            // Get sub categories
            var subCategories = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.GetSubCategoryListAsync(mainCategory.Id);
            });

            // Assert
            subCategories.Count.ShouldBe(2);
            subCategories.ShouldContain(x => x.Name == "Computers");
            subCategories.ShouldContain(x => x.Name == "Mobile Phones");
        }

        [Fact]
        public async Task Update_Category_Should_Preserve_Hierarchy()
        {
            // Arrange
            var mainCategory = await CreateTestCategoryAsync("Main", "主");
            var subCategory = await CreateTestCategoryAsync("Sub", "子", mainCategoryId: mainCategory.Id);

            // Act
            var updateDto = GetUpdateDto("Updated Sub", "更新子");
            updateDto.MainCategoryId = mainCategory.Id;

            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _productCategoryAppService.UpdateAsync(subCategory.Id, updateDto);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Updated Sub");
            result.MainCategoryId.ShouldBe(mainCategory.Id);
        }

        #endregion
    }
}
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Validation;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kooco.Pikachu.Items;

public class ItemAppServiceTests : PikachuApplicationTestBase
{
    private readonly IItemAppService _itemAppService;
    private readonly ItemManager _itemManager;
    private readonly IItemRepository _itemRepository;

    public ItemAppServiceTests()
    {
        _itemAppService = GetRequiredService<IItemAppService>();
        _itemManager = GetRequiredService<ItemManager>();
        _itemRepository= GetRequiredService<IItemRepository>();
    }
    #region ItemName Tests
    [Fact]
    public async Task Should_Throw_Exception_If_Name_Exceeds_60_Characters()
    {
        // Arrange

        var input = new CreateItemDto
        {
            ItemName = new string('A', 61), // 61 characters


            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _itemAppService.CreateAsync(input);
        });

        exception.Message.ShouldContain("itemName length must be equal to or lower than 60! (Parameter 'itemName')");
    }

    [Fact]
    public async Task Should_Create_Item_If_Name_Is_Within_Limit()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "Valid Name", // Within limit
            //ItemNo = "12345",

            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true
        };

        // Act
        var result = await _itemManager.CreateAsync(
            input.ItemName,
            input.ItemBadge,
            input.ItemDescriptionTitle,
            input.ItemDescription,
            input.ItemTags,
            input.LimitAvaliableTimeStart,
            input.LimitAvaliableTimeEnd,
            input.ShareProfit,
            input.IsFreeShipping,
            input.IsReturnable,
            input.ShippingMethodId,
            input.TaxTypeId,
            input.CustomField1Value,
            input.CustomField1Name,
            input.CustomField2Value,
            input.CustomField2Name,
            input.CustomField3Value,
            input.CustomField3Name,
            input.CustomField4Value,
            input.CustomField4Name,
            input.CustomField5Value,
            input.CustomField5Name,
            input.CustomField6Value,
            input.CustomField6Name,
            input.CustomField7Value,
            input.CustomField7Name,
            input.CustomField8Value,
            input.CustomField8Name,
            input.CustomField9Value,
            input.CustomField9Name,
            input.CustomField10Value,
            input.CustomField10Name,
            input.Attribute1Name,
            input.Attribute2Name,
            input.Attribute3Name,
            input.ItemStorageTemperature
            );
        result.ItemNo = 12345;
        result = await _itemRepository.InsertAsync(result);
        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.ItemName.ShouldBe(input.ItemName);
    }

    [Fact]
    public async Task Should_Allow_Special_Characters_In_Item_Name()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "Item @#$%&*!", // Special characters included

            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true
        };

        // Act

        var result = await _itemAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.ItemName.ShouldBe(input.ItemName);
    }
    [Fact]
    public async Task Should_Throw_Exception_When_Item_Name_Is_Empty()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "", // Empty value

            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true,
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {

            await _itemAppService.CreateAsync(input);
        });

        exception.Code.ShouldContain("Item Name Can not be null");
    }
    [Fact]
    public async Task Should_Throw_Exception_When_Item_Name_Is_Duplicate()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "Valid Name",

            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true
        };




        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _itemAppService.CreateAsync(input);
            await _itemAppService.CreateAsync(input);
        });

        exception.Code.ShouldContain("Item with same Name already exists");
    }
    #endregion

    #region ItemDescriptionTitle Tests
    [Fact]
    public async Task Should_Throw_Exception_When_ItemDescriptionTitle_Length_Exceeds_60_Characters()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ValidItemName",
            ItemDescriptionTitle = new string('A', 61), // 61 characters (invalid)
            ItemNo = "12345",
            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _itemAppService.CreateAsync(input);
        });

        exception.Message.ShouldContain("ItemDescriptionTitle length must be equal to or lower than 60! (Parameter 'ItemDescriptionTitle')");
    }

    [Fact]
    public async Task Should_Allow_ItemDescriptionTitle_Within_60_Characters()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ValidItemName",
            ItemDescriptionTitle = "This is a valid description within the 60 character limit.", // Valid description (60 characters)
            
            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest=true
        };

        // Act
        var result = await _itemAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.ItemDescription.ShouldBe(input.ItemDescription);
    }
    [Fact]
    public async Task Should_Allow_Special_Characters_In_ItemDescriptionTitle()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "Item", 
            ItemDescriptionTitle = "Item @#$%&*!", // Special characters included

            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true
        };

        // Act

        var result = await _itemAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.ItemDescriptionTitle.ShouldBe(input.ItemDescriptionTitle);
    }
    #endregion

    #region ItemDescription Tests
    [Fact]
    public async Task Should_Allow_Multi_Line_Text_In_ItemDescription()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ValidItemName",
            ItemDescription = "<p>This is the first line.<br>This is the second line.<br>This is the third line.</p>", // Multi-line HTML
           
            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true
        };

        // Act
        var result = await _itemAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.ItemDescription.ShouldBe(input.ItemDescription);
    }

    [Fact]
    public async Task Should_Allow_Multi_Line_Text_With_Preserved_Line_Breaks_In_ItemDescription()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ValidItemName",
            ItemDescription = "<p>Line 1<br>Line 2<br>Line 3</p>", // Multi-line with <br> tags
            
            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest=true
        };

        // Act
        var result = await _itemAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.ItemDescription.ShouldBe(input.ItemDescription); // Check if the description matches input with line breaks
    }
    [Fact]
    public async Task Should_Sanitize_And_Remove_XSS_Tags_From_ItemDescription()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ValidItemName",
            ItemDescription = "<script>alert('XSS');</script>This is safe text.<img src='invalid.jpg' onerror='alert(1)' />", // Dangerous HTML tags
            ItemNo = "12345",
            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest=true
        };

        // Act
        var result = await _itemAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        // Ensure the script tag and event handler in the image are sanitized out
        result.ItemDescription.ShouldNotContain("<script>");
        result.ItemDescription.ShouldNotContain("onerror");
        result.ItemDescription.ShouldContain("This is safe text.");
    }

    #endregion
    #region ItemDetails Test
    [Fact]
    public async Task Should_Throw_Exception_When_Sku_Is_Duplicate()
    {
        // Arrange
        var existingItemDetails = new CreateItemDetailsDto
        {
            ItemName = "Item1",
            Sku = "SKU123", // Existing SKU
            SellingPrice = 100.0f,
            GroupBuyPrice = 90.0f,
            Cost = 50.0f,
            InventoryAccount = "Account1",
            SaleableQuantity = 10,
            Status = true,
            SortNo = 1
        };
        var newItemDetails = new CreateItemDetailsDto
        {
            ItemName = "Item2",
            Sku = "SKU123", // Duplicate SKU
            SellingPrice = 150.0f,
            GroupBuyPrice = 130.0f,
            Cost = 70.0f,
            InventoryAccount = "Account2",
            SaleableQuantity = 20,
            Status = true,
            SortNo = 2
        };
        var existingItemDto = new CreateItemDto
        {
            ItemName = "Item1",
            ItemDescription = "Description of Item 1",
            IsTest=true,
            ItemDetails = new List<CreateItemDetailsDto> { existingItemDetails, newItemDetails }
        };

        // Act & Assert: Ensure that the second item throws an exception due to duplicate SKU
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _itemAppService.CreateAsync(existingItemDto);
        });
        exception.Code.ShouldContain("Pikachu:ItemWithSKUAlreadyExists");
    }
    [Fact]
    public async Task Should_Throw_Exception_When_Sku_Is_Null()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ItemWithNullSku",
            ItemDescription = "Description of Item with null SKU",
            IsTest=true,
            ItemDetails = new List<CreateItemDetailsDto>
        {
            new CreateItemDetailsDto
            {
                Sku = null, // Null SKU
                ItemName = "ItemWithNullSkuDetail",
                SellingPrice = 100.0f,
                GroupBuyPrice = 90.0f,
                Cost = 50.0f,
                InventoryAccount = "Account1",
                SaleableQuantity = 10,
                Status = true,
                SortNo = 1
            }
        }
        };

        // Act & Assert: Ensure that SKU being null throws an exception
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _itemAppService.CreateAsync(input);
        });

        exception.Message.ShouldContain("Value cannot be null. (Parameter 'SKU')");
    }
    [Fact]
    public async Task Should_Throw_Exception_When_ItemDetailName_Is_Null()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ItemWithNullItemDetailName",
            ItemDescription = "Description of Item with null Item Detail Name",
            IsTest = true,
            ItemDetails = new List<CreateItemDetailsDto>
        {
            new CreateItemDetailsDto
            {
                Sku = "ItemWithNullItemDetailName",
                ItemName = null,
                SellingPrice = 100.0f,
                GroupBuyPrice = 90.0f,
                Cost = 50.0f,
                InventoryAccount = "Account1",
                SaleableQuantity = 10,
                Status = true,
                SortNo = 1
            }
        }
        };

        // Act & Assert: Ensure that ItemName being null throws an exception
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _itemAppService.CreateAsync(input);
        });

        exception.Message.ShouldContain("Value cannot be null. (Parameter 'ItemName')");
    }
    #endregion
    #region SellingPrice,GroupbuyPrice,Cost
    [Theory]
    [InlineData(-10.00f, 5.0f, 3.0f)] // Negative SellingPrice
    [InlineData(10.0f, -5.5f, 3.0f)] // Negative GroupBuyPrice
    [InlineData(10.0f, 5.5f, -3.0f)] // Negative Cost
    public async Task Should_Throw_Exception_When_Prices_Are_Negative(float sellingPrice, float groupBuyPrice, float cost)
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ItemWithNegativePrice",
            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest =true,
            ItemDetails = new List<CreateItemDetailsDto>
        {
            new CreateItemDetailsDto
            {
                Sku = "SKU_NEG",
                ItemName = "Negative Price Item",
                SellingPrice = sellingPrice,
                GroupBuyPrice = groupBuyPrice,
                Cost = cost,
                InventoryAccount = "INV001",
                SaleableQuantity = 10,
                Status = true,
                SortNo = 1
            }
        }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _itemAppService.CreateAsync(input);
        });

        exception.ValidationErrors.ShouldContain(x=>x.ErrorMessage== "Price must be a positive number.");
    }
    [Theory]
    [InlineData(10.123f, 5.50f, 3.99f)] // Invalid SellingPrice
    [InlineData(10.99f, 5.999f, 3.50f)] // Invalid GroupBuyPrice
    [InlineData(10.99f, 5.50f, 3.9999f)] // Invalid Cost
    public async Task Should_Throw_Exception_When_Prices_Have_More_Than_Two_Decimal_Places(float sellingPrice, float groupBuyPrice, float cost)
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ItemWithInvalidDecimals",
            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true,
            ItemDetails = new List<CreateItemDetailsDto>
        {
            new CreateItemDetailsDto
            {
                Sku = "SKU_DEC",
                ItemName = "ItemWithInvalidDecimals",
                SellingPrice = sellingPrice,
                GroupBuyPrice = groupBuyPrice,
                Cost = cost,
                InventoryAccount = "INV002",
                SaleableQuantity = 5,
                Status = true,
                SortNo = 2
            }
        }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _itemAppService.CreateAsync(input);
        });

        exception.ValidationErrors.ShouldContain(x=>x.ErrorMessage=="Price can have up to 2 decimal places.");
    }

    #endregion
    #region Quantities Test
    [Theory]
    [InlineData(-1, 10, 5, 100, 50)] // Negative SaleableQuantity
    [InlineData(10, -1, 5, 100, 50)] // Negative PreOrderableQuantity
    [InlineData(10, 10, -5, 100, 50)] // Negative SaleablePreOrderQuantity
    [InlineData(10, 10, 5, -100, 50)] // Negative LimitQuantity
    [InlineData(10, 10, 5, 100, -50)] // Negative StockOnHand
    public async Task Should_Throw_Exception_When_Values_Are_Negative(float saleableQuantity, float preOrderableQuantity,
    float saleablePreOrderQuantity, int limitQuantity, int stockOnHand)
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ItemWithNegativeValues",
            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true,
            ItemDetails = new List<CreateItemDetailsDto>
        {
            new CreateItemDetailsDto
            {
                Sku = "SKU_NEG",
                ItemName = "ItemWithNegativeValues",
                SaleableQuantity = saleableQuantity,
                PreOrderableQuantity = preOrderableQuantity,
                SaleablePreOrderQuantity = saleablePreOrderQuantity,
                LimitQuantity = limitQuantity,
                StockOnHand = stockOnHand,
                Status = true,
                SortNo = 1
            }
        }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _itemAppService.CreateAsync(input);
        });

        exception.ValidationErrors.ShouldContain(x=>x.ErrorMessage=="Quantity must be a positive number and less than 99,999.");
    }
    [Fact]
    public async Task Should_Throw_Exception_When_Values_Are_Empty_Or_Null()
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ItemWithNullValues",
            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true,
            ItemDetails = new List<CreateItemDetailsDto>
        {
            new CreateItemDetailsDto
            {
                Sku = "SKU_NULL",
                ItemName = "ItemWithNullValues",
                SaleableQuantity = 0, // Null SaleableQuantity
                PreOrderableQuantity = null, // Null PreOrderableQuantity
                SaleablePreOrderQuantity = null, // Null SaleablePreOrderQuantity
                LimitQuantity = null, // Null LimitQuantity
                StockOnHand = null, // Null StockOnHand
                Status = true,
                SortNo = 1
            }
        }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _itemAppService.CreateAsync(input);
        });

        exception.Message.ShouldContain("Price must be a positive number.");
    }
    [Theory]
    [InlineData(100000, 10, 5, 100, 50)] // SaleableQuantity exceeds 99,999
    [InlineData(10, 100000, 5, 100, 50)] // PreOrderableQuantity exceeds 99,999
    [InlineData(10, 10, 100000, 100, 50)] // SaleablePreOrderQuantity exceeds 99,999
    [InlineData(10, 10, 5, 100000, 50)] // LimitQuantity exceeds 99,999
    [InlineData(10, 10, 5, 100, 100000)] // StockOnHand exceeds 99,999
    public async Task Should_Throw_Exception_When_Values_Exceed_Upper_Limit(float saleableQuantity, float preOrderableQuantity,
    float saleablePreOrderQuantity, int limitQuantity, int stockOnHand)
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ItemWithExceedingValues",
            ItemTags = "Tag1,Tag2,Tag3",
            ShareProfit = 10.5f,
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true,
            ItemDetails = new List<CreateItemDetailsDto>
        {
            new CreateItemDetailsDto
            {
                Sku = "SKU_EXCEED",
                ItemName = "ItemWithExceedingValues",
                SaleableQuantity = saleableQuantity,
                PreOrderableQuantity = preOrderableQuantity,
                SaleablePreOrderQuantity = saleablePreOrderQuantity,
                LimitQuantity = limitQuantity,
                StockOnHand = stockOnHand,
                Status = true,
                SortNo = 1
            }
        }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _itemAppService.CreateAsync(input);
        });

        exception.ValidationErrors.ShouldContain(x => x.ErrorMessage == "Quantity must be a positive number and less than 99,999.");
    }

    #endregion
    #region ShareProfit Test
    [Theory]
    [InlineData(-1)] // Value less than 0
    [InlineData(101)] // Value greater than 100
    public async Task Should_Throw_Exception_When_ShareProfit_Is_Out_Of_Range(float shareProfit)
    {
        // Arrange
        var input = new CreateItemDto
        {
            ItemName = "ItemWithInvalidShareProfit",
            ItemTags = "Tag1,Tag2,Tag3",
           
            ShippingMethodId = 1,
            TaxTypeId = 2,
            IsItemAvaliable = true,
            IsTest = true,
            ShareProfit = shareProfit,
        
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _itemAppService.CreateAsync(input);
        });

        exception.ValidationErrors.ShouldContain(x=>x.ErrorMessage=="Profit must be a greate than equal to 0 and less than equal to 100.");
    }

    #endregion
}


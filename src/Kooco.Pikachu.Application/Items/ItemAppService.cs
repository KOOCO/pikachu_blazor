using System;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public class ItemAppService : CrudAppService<Item, ItemDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateItemDto>,
    IItemAppService
{
    protected override string GetPolicyName { get; set; } = PikachuPermissions.Item.Default;
    protected override string GetListPolicyName { get; set; } = PikachuPermissions.Item.Default;
    protected override string CreatePolicyName { get; set; } = PikachuPermissions.Item.Create;
    protected override string UpdatePolicyName { get; set; } = PikachuPermissions.Item.Update;
    protected override string DeletePolicyName { get; set; } = PikachuPermissions.Item.Delete;

    private readonly IItemRepository _repository;

    public ItemAppService(IItemRepository repository) : base(repository)
    {
        _repository = repository;
    }

    //protected override async Task<IQueryable<Item>> CreateFilteredQueryAsync(ItemGetListInput input)
    //{
    //    // TODO: AbpHelper generated
    //    return (await base.CreateFilteredQueryAsync(input))
    //        .WhereIf(input.ItemNo != null, x => x.ItemNo == input.ItemNo)
    //        .WhereIf(!input.ItemName.IsNullOrWhiteSpace(), x => x.ItemName.Contains(input.ItemName))
    //        .WhereIf(!input.ItemDescription.IsNullOrWhiteSpace(), x => x.ItemDescription.Contains(input.ItemDescription))
    //        .WhereIf(input.SellingPrice != null, x => x.SellingPrice == input.SellingPrice)
    //        .WhereIf(!input.SalesAccount.IsNullOrWhiteSpace(), x => x.SalesAccount.Contains(input.SalesAccount))
    //        .WhereIf(input.Returnable != null, x => x.Returnable == input.Returnable)
    //        .WhereIf(!input.BrandName.IsNullOrWhiteSpace(), x => x.BrandName.Contains(input.BrandName))
    //        .WhereIf(!input.ManufactorName.IsNullOrWhiteSpace(), x => x.ManufactorName.Contains(input.ManufactorName))
    //        .WhereIf(input.PackageWeight != null, x => x.PackageWeight == input.PackageWeight)
    //        .WhereIf(input.PackageLength != null, x => x.PackageLength == input.PackageLength)
    //        .WhereIf(input.PackageHeight != null, x => x.PackageHeight == input.PackageHeight)
    //        .WhereIf(input.DiemensionsUnit != null, x => x.DiemensionsUnit == input.DiemensionsUnit)
    //        .WhereIf(input.WeightUnit != null, x => x.WeightUnit == input.WeightUnit)
    //        .WhereIf(!input.TaxName.IsNullOrWhiteSpace(), x => x.TaxName.Contains(input.TaxName))
    //        .WhereIf(input.TaxPercentage != null, x => x.TaxPercentage == input.TaxPercentage)
    //        .WhereIf(!input.TaxType.IsNullOrWhiteSpace(), x => x.TaxType.Contains(input.TaxType))
    //        .WhereIf(!input.PurchaseTaxName.IsNullOrWhiteSpace(), x => x.PurchaseTaxName.Contains(input.PurchaseTaxName))
    //        .WhereIf(input.PurchaseTaxPercentage != null, x => x.PurchaseTaxPercentage == input.PurchaseTaxPercentage)
    //        .WhereIf(!input.ProductType.IsNullOrWhiteSpace(), x => x.ProductType.Contains(input.ProductType))
    //        .WhereIf(input.Source != null, x => x.Source == input.Source)
    //        .WhereIf(input.ReferenceID != null, x => x.ReferenceID == input.ReferenceID)
    //        .WhereIf(input.LastSyncTime != null, x => x.LastSyncTime == input.LastSyncTime)
    //        .WhereIf(!input.Status.IsNullOrWhiteSpace(), x => x.Status.Contains(input.Status))
    //        .WhereIf(input.Unit != null, x => x.Unit == input.Unit)
    //        .WhereIf(!input.SKU.IsNullOrWhiteSpace(), x => x.SKU.Contains(input.SKU))
    //        .WhereIf(!input.UPC.IsNullOrWhiteSpace(), x => x.UPC.Contains(input.UPC))
    //        .WhereIf(!input.EAN.IsNullOrWhiteSpace(), x => x.EAN.Contains(input.EAN))
    //        .WhereIf(!input.ISBN.IsNullOrWhiteSpace(), x => x.ISBN.Contains(input.ISBN))
    //        .WhereIf(!input.PartNumber.IsNullOrWhiteSpace(), x => x.PartNumber.Contains(input.PartNumber))
    //        .WhereIf(input.PurchasePrice != null, x => x.PurchasePrice == input.PurchasePrice)
    //        .WhereIf(!input.PurchaseAccount.IsNullOrWhiteSpace(), x => x.PurchaseAccount.Contains(input.PurchaseAccount))
    //        .WhereIf(!input.PurchaseDescription.IsNullOrWhiteSpace(), x => x.PurchaseDescription.Contains(input.PurchaseDescription))
    //        .WhereIf(!input.InventoryAccount.IsNullOrWhiteSpace(), x => x.InventoryAccount.Contains(input.InventoryAccount))
    //        .WhereIf(input.ReorderLevel != null, x => x.ReorderLevel == input.ReorderLevel)
    //        .WhereIf(!input.PreferredVendor.IsNullOrWhiteSpace(), x => x.PreferredVendor.Contains(input.PreferredVendor))
    //        .WhereIf(!input.WarehouseName.IsNullOrWhiteSpace(), x => x.WarehouseName.Contains(input.WarehouseName))
    //        .WhereIf(input.OpeningStock != null, x => x.OpeningStock == input.OpeningStock)
    //        .WhereIf(input.OpeningStockValue != null, x => x.OpeningStockValue == input.OpeningStockValue)
    //        .WhereIf(input.StockOnHand != null, x => x.StockOnHand == input.StockOnHand)
    //        .WhereIf(input.IsComboProduct != null, x => x.IsComboProduct == input.IsComboProduct)
    //        .WhereIf(!input.ItemType.IsNullOrWhiteSpace(), x => x.ItemType.Contains(input.ItemType))
    //        .WhereIf(!input.ItemCategory.IsNullOrWhiteSpace(), x => x.ItemCategory.Contains(input.ItemCategory))
    //        .WhereIf(!input.CustomeField1Name.IsNullOrWhiteSpace(), x => x.CustomeField1Name.Contains(input.CustomeField1Name))
    //        .WhereIf(!input.CustomeField1Value.IsNullOrWhiteSpace(), x => x.CustomeField1Value.Contains(input.CustomeField1Value))
    //        .WhereIf(!input.CustomeField2Name.IsNullOrWhiteSpace(), x => x.CustomeField2Name.Contains(input.CustomeField2Name))
    //        .WhereIf(!input.CustomeField2Value.IsNullOrWhiteSpace(), x => x.CustomeField2Value.Contains(input.CustomeField2Value))
    //        .WhereIf(!input.CustomeField3Name.IsNullOrWhiteSpace(), x => x.CustomeField3Name.Contains(input.CustomeField3Name))
    //        .WhereIf(!input.CustomeField3Value.IsNullOrWhiteSpace(), x => x.CustomeField3Value.Contains(input.CustomeField3Value))
    //        .WhereIf(!input.CustomeField4Name.IsNullOrWhiteSpace(), x => x.CustomeField4Name.Contains(input.CustomeField4Name))
    //        .WhereIf(!input.CustomeField4Value.IsNullOrWhiteSpace(), x => x.CustomeField4Value.Contains(input.CustomeField4Value))
    //        .WhereIf(!input.CustomeField5Name.IsNullOrWhiteSpace(), x => x.CustomeField5Name.Contains(input.CustomeField5Name))
    //        .WhereIf(!input.CustomeField5Value.IsNullOrWhiteSpace(), x => x.CustomeField5Value.Contains(input.CustomeField5Value))
    //        .WhereIf(!input.CustomeField6Name.IsNullOrWhiteSpace(), x => x.CustomeField6Name.Contains(input.CustomeField6Name))
    //        .WhereIf(!input.CustomeField6Value.IsNullOrWhiteSpace(), x => x.CustomeField6Value.Contains(input.CustomeField6Value))
    //        .WhereIf(!input.CustomeField7Name.IsNullOrWhiteSpace(), x => x.CustomeField7Name.Contains(input.CustomeField7Name))
    //        .WhereIf(!input.CustomeField7Value.IsNullOrWhiteSpace(), x => x.CustomeField7Value.Contains(input.CustomeField7Value))
    //        .WhereIf(!input.CustomeField8Name.IsNullOrWhiteSpace(), x => x.CustomeField8Name.Contains(input.CustomeField8Name))
    //        .WhereIf(!input.CustomeField8Value.IsNullOrWhiteSpace(), x => x.CustomeField8Value.Contains(input.CustomeField8Value))
    //        .WhereIf(!input.CustomeField9Name.IsNullOrWhiteSpace(), x => x.CustomeField9Name.Contains(input.CustomeField9Name))
    //        .WhereIf(!input.CustomeField9Value.IsNullOrWhiteSpace(), x => x.CustomeField9Value.Contains(input.CustomeField9Value))
    //        .WhereIf(!input.CustomeField10Name.IsNullOrWhiteSpace(), x => x.CustomeField10Name.Contains(input.CustomeField10Name))
    //        .WhereIf(!input.CustomeField10Value.IsNullOrWhiteSpace(), x => x.CustomeField10Value.Contains(input.CustomeField10Value))
    //        ;
    //}
}

using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.DeliveryTempratureCosts;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.Orders;
using Microsoft.Extensions.Localization;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Validation.Localization;

namespace Kooco.Pikachu.GroupBuys
{
    [RemoteService(IsEnabled = false)]
    public class GroupBuyAppService : ApplicationService, IGroupBuyAppService
    {
        private readonly IGroupBuyRepository _groupBuyRepository;
        private readonly GroupBuyManager _groupBuyManager;
        private readonly IRepository<Image, Guid> _imageRepository;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly IFreebieRepository _freebieRepository;
        private readonly IDataFilter _dataFilter;
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<DeliveryTemperatureCost, Guid> _temperatureRepository;
        private readonly IStringLocalizer<PikachuResource> _l;
      
        public GroupBuyAppService(
            IGroupBuyRepository groupBuyRepository,
            GroupBuyManager groupBuyManager,
            ImageContainerManager imageContainerManager,
            IFreebieRepository freebieRepository,
            IRepository<Image, Guid> imageRepository,
            IDataFilter dataFilter,
            IOrderRepository orderRepository,
            IRepository<DeliveryTemperatureCost, Guid> temperatureRepository,
            IStringLocalizer<PikachuResource> l
            )
        {
            _groupBuyManager = groupBuyManager;
            _groupBuyRepository = groupBuyRepository;
            _imageContainerManager = imageContainerManager;
            _imageRepository = imageRepository;
            _dataFilter = dataFilter;
            _freebieRepository = freebieRepository;
            _orderRepository = orderRepository;
            _temperatureRepository = temperatureRepository;
            _l = l;
        }

        public async Task<GroupBuyDto> CreateAsync(GroupBuyCreateDto input)
        {
            GroupBuy result = await _groupBuyManager.CreateAsync(input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine,
                                                        input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan,
                                                        input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                        input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                        input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy,
                                                        input.NotifyMessage, input.ExcludeShippingMethod, input.IsDefaultPaymentGateWay, input.PaymentMethod, input.GroupBuyCondition, input.CustomerInformation,
                                                        input.CustomerInformationDescription, input.GroupBuyConditionDescription, input.ExchangePolicyDescription, input.ShortCode, input.IsEnterprise,input.FreeShippingThreshold,input.SelfPickupDeliveryTime,
                                                        input.BlackCatDeliveryTime,input.HomeDeliveryDeliveryTime,input.DeliveredByStoreDeliveryTime,input.TaxType);

            if (input.ItemGroups != null && input.ItemGroups.Any())
            {
                foreach (var group in input.ItemGroups)
                {
                    var itemGroup = _groupBuyManager.AddItemGroup(
                        result,
                        group.SortOrder,
                        group.GroupBuyModuleType
                        );

                    if (group.ItemDetails != null && group.ItemDetails.Any())
                    {
                        foreach (var item in group.ItemDetails)
                        {
                            _groupBuyManager.AddItemGroupDetail(
                                itemGroup,
                                item.SortOrder,
                                item.ItemId,
                                item.SetItemId,
                                item.ItemType,
                                item.DisplayText
                                );
                        }
                    }
                }
            }

            await _groupBuyRepository.InsertAsync(result);

            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(result);
        }
        public async Task<GroupBuyDto> CopyAsync(Guid Id)
        {
            var query = await _groupBuyRepository.GetQueryableAsync();
            var input = await _groupBuyRepository.GetWithDetailsAsync(Id);
            var count = query.Where(x => x.GroupBuyName.Contains(input.GroupBuyName)).Count();
            var Name = input.GroupBuyName + "(" + count + ")";
            var ShortCode = "";
            var result = await _groupBuyManager.CreateAsync(input.GroupBuyNo, input.Status, Name, input.EntryURL, input.EntryURL2, input.SubjectLine,
                                                        input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan,
                                                        input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                        input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                        input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy,
                                                        input.NotifyMessage, input.ExcludeShippingMethod, input.IsDefaultPaymentGateWay, input.PaymentMethod, input.GroupBuyCondition, input.CustomerInformation,
                                                        input.CustomerInformationDescription, input.GroupBuyConditionDescription, input.ExchangePolicyDescription, ShortCode, input.IsEnterprise,input.FreeShippingThreshold,
                                                        input.SelfPickupDeliveryTime,input.BlackCatDeliveryTime,input.HomeDeliveryDeliveryTime,input.DeliveredByStoreDeliveryTime,input.TaxType);

            if (input.ItemGroups != null && input.ItemGroups.Any())
            {
                foreach (var group in input.ItemGroups)
                {
                    var itemGroup = _groupBuyManager.AddItemGroup(
                        result,
                        group.SortOrder,
                        group.GroupBuyModuleType
                        );

                    if (group.ItemGroupDetails != null && group.ItemGroupDetails.Any())
                    {
                        foreach (var item in group.ItemGroupDetails)
                        {
                            _groupBuyManager.AddItemGroupDetail(
                                itemGroup,
                                item.SortOrder,
                                item.ItemId,
                                item.SetItemId,
                                item.ItemType,
                                item.DisplayText
                                );
                        }
                    }
                }
            }

            await _groupBuyRepository.InsertAsync(result);

            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(result);
        }
        public async Task DeleteAsync(Guid id)
        {
            var check = (await _orderRepository.GetQueryableAsync()).Any(x => x.GroupBuyId == id);
                if(check)
                {
                throw new UserFriendlyException(_l["Groupbuyhaveorderitsnotdeleteable"]);
            
            }
            await _groupBuyRepository.DeleteAsync(id);
        }

        public async Task<GroupBuyDto> GetAsync(Guid id, bool includeDetails = false)
        {
            var item = await _groupBuyRepository.GetAsync(id);
            if (includeDetails)
            {
                await _groupBuyRepository.EnsureCollectionLoadedAsync(item, i => i.ItemGroups);
            }

            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(item);
        }
        public async Task<GroupBuyDto> GetWithDetailsAsync(Guid id)
        {
            var item = await _groupBuyRepository.GetWithDetailsAsync(id);
            return item is null
                ? throw new BusinessException(PikachuDomainErrorCodes.EntityWithGivenIdDoesnotExist)
                : ObjectMapper.Map<GroupBuy, GroupBuyDto>(item);
        }

        public async Task DeleteGroupBuyItemAsync(Guid id, Guid GroupBuyID)
        {
            var groupbuy = await _groupBuyRepository.GetAsync(GroupBuyID);
            await _groupBuyRepository.EnsureCollectionLoadedAsync(groupbuy, i => i.ItemGroups);
            var itemGroup = groupbuy.ItemGroups.Where(i => i.Id == id).First();
            groupbuy.ItemGroups.Remove(itemGroup);
            await _groupBuyRepository.UpdateAsync(groupbuy);
        }
        public async Task<PagedResultDto<GroupBuyDto>> GetListAsync(GetGroupBuyInput input)
        {
            var sorting = "CreationTime DESC";
            var count = await _groupBuyRepository.GetGroupBuyCountAsync(input.FilterText, input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine
                                                         , input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan
                                                         , input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                         input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                         input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy,
                                                         input.NotifyMessage
                                                         );
            var result = await _groupBuyRepository.GetGroupBuyListAsync(input.FilterText, input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine,
                                                        input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan,
                                                        input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                        input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                        input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy,
                                                        input.NotifyMessage, sorting, input.MaxResultCount, input.SkipCount);
            return new PagedResultDto<GroupBuyDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<GroupBuyList>, List<GroupBuyDto>>(result)
            };
        }

        public async Task<GroupBuyDto> UpdateAsync(Guid id, GroupBuyUpdateDto input)
        {
            bool sameName = await _groupBuyRepository.AnyAsync(x => x.GroupBuyName == input.GroupBuyName && x.Id != id);

            if (sameName) throw new BusinessException(PikachuDomainErrorCodes.ItemWithSameNameAlreadyExists);

            GroupBuy groupBuy = await _groupBuyRepository.GetWithDetailsAsync(id);

            groupBuy = ObjectMapper.Map<GroupBuyUpdateDto, GroupBuy>(input);

            if (input?.ItemGroups != null)
            {
                foreach (var group in input.ItemGroups)
                {
                    if (group.Id.HasValue)
                    {
                        var itemGroup = groupBuy.ItemGroups.FirstOrDefault(x => x.Id == group.Id);
                        if (itemGroup != null)
                        {
                            itemGroup.SortOrder = group.SortOrder;
                            itemGroup.GroupBuyModuleType = group.GroupBuyModuleType;
                            if (group.ItemDetails.Count == 0)
                            {
                                groupBuy.ItemGroups.Remove(itemGroup);
                            }
                            ProcessItemDetails(itemGroup, group.ItemDetails);
                        }
                    }
                    else
                    {
                        var itemGroup = _groupBuyManager.AddItemGroup(
                                groupBuy,
                                group.SortOrder,
                                group.GroupBuyModuleType
                         );

                        ProcessItemDetails(itemGroup, group.ItemDetails);
                    }
                }
            }

            await _groupBuyRepository.UpdateAsync(groupBuy);

            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(groupBuy);
        }
        private void ProcessItemDetails(GroupBuyItemGroup itemGroup, ICollection<GroupBuyItemGroupDetailCreateUpdateDto> itemDetails)
        {
            itemGroup.ItemGroupDetails?.Clear();
            foreach (var item in itemDetails)
            {
                _groupBuyManager.AddItemGroupDetail(
                    itemGroup,
                    item.SortOrder,
                    item.ItemId,
                    item.SetItemId,
                    item.ItemType,
                    item.DisplayText
                );
            }
        }
        public async Task DeleteManyGroupBuyItemsAsync(List<Guid> groupBuyIds)
        {
            foreach (var id in groupBuyIds)
            {
                var check = (await _orderRepository.GetQueryableAsync()).Any(x => x.GroupBuyId == id);
                if (check)
                {
                    throw new UserFriendlyException(_l["Groupbuyhaveorderitsnotdeleteable"]);

                }
                var images = await _imageRepository.GetListAsync(x => x.TargetId == id);
                if (images == null) continue;

                foreach (var image in images)
                {
                    await _imageContainerManager.DeleteAsync(image.BlobImageName);
                }
            }
            await _groupBuyRepository.DeleteManyAsync(groupBuyIds);
        }

        public async Task ChangeGroupBuyAvailability(Guid groupBuyID)
        {
            var groupBuy = await _groupBuyRepository.FindAsync(x => x.Id == groupBuyID);
            groupBuy.IsGroupBuyAvaliable = !groupBuy.IsGroupBuyAvaliable;
            await _groupBuyRepository.UpdateAsync(groupBuy);
        }

        public async Task<GroupBuyItemGroupDto> GetGroupBuyItemGroupAsync(Guid id)
        {
            var itemGroup = await _groupBuyRepository.GetGroupBuyItemGroupAsync(id);
            return ObjectMapper.Map<GroupBuyItemGroup, GroupBuyItemGroupDto>(itemGroup);
        }

        public async Task<List<KeyValueDto>> GetGroupBuyLookupAsync()
        {
            var groupbuys = (await _groupBuyRepository.GetQueryableAsync())
                            .Where(g => g.IsGroupBuyAvaliable)
                            .Select(x => new GroupBuyList { Id = x.Id, GroupBuyName = x.GroupBuyName })
                            .ToList();
            return ObjectMapper.Map<List<GroupBuyList>, List<KeyValueDto>>(groupbuys);
        }
        public async Task<List<KeyValueDto>> GetAllGroupBuyLookupAsync()
        {
            var groupbuys = (await _groupBuyRepository.GetListAsync()).Select(x=>new GroupBuyList {Id=x.Id,GroupBuyName=x.GroupBuyName }).ToList();
            return ObjectMapper.Map<List<GroupBuyList>, List<KeyValueDto>>(groupbuys);
        }
        /// <summary>
        /// This Method Returns the Desired Result For the Store Front End.
        /// Do not change unless you want to make changes in the Store Front End Code
        /// </summary>
        /// <returns></returns>
        public async Task<GroupBuyDto> GetForStoreAsync(Guid id)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var item = await _groupBuyRepository.GetAsync(id);
                return ObjectMapper.Map<GroupBuy, GroupBuyDto>(item);

            }
        }

        /// <summary>
        /// This Method Returns the Desired Result For the Store Front End.
        /// Do not change unless you want to make changes in the Store Front End Code
        /// </summary>
        /// <returns></returns>
        public async Task<GroupBuyItemGroupWithCountDto> GetPagedItemGroupAsync(Guid id, int skipCount)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var item = await _groupBuyRepository.GetPagedItemGroupAsync(id, skipCount);
                return item == null ?
                    throw new BusinessException(PikachuDomainErrorCodes.EntityWithGivenIdDoesnotExist)
                    : ObjectMapper.Map<GroupBuyItemGroupWithCount, GroupBuyItemGroupWithCountDto>(item);

            }
        }

        /// <summary>
        /// This Method Returns the Desired Result For the Store Front End.
        /// Do not change unless you want to make changes in the Store Front End Code
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetCarouselImagesAsync(Guid id)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var data = await _imageRepository.GetListAsync(x => x.TargetId == id && x.ImageType == ImageType.GroupBuyCarouselImage);
                return data.Select(i => i.ImageUrl).ToList();
            }
        }

        /// <summary>
        /// This Method Returns the Desired Result For the Store Front End.
        /// Do not change unless you want to make changes in the Store Front End Code
        /// </summary>
        /// <returns></returns>
        public async Task<List<FreebieDto>> GetFreebieForStoreAsync(Guid groupBuyId)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                List<Freebie> freebie = await _freebieRepository.GetFreebieStoreAsync(groupBuyId);
                return ObjectMapper.Map<List<Freebie>, List<FreebieDto>>(freebie);
            }
        }

        public async Task<bool> CheckShortCodeForCreate(string shortCode)
        {

            var query = await _groupBuyRepository.GetQueryableAsync();
            var check = query.Any(x => x.ShortCode == shortCode);
            return check;
        }

        public async Task<bool> CheckShortCodeForEdit(string shortCode, Guid Id)
        {

            var query = await _groupBuyRepository.GetQueryableAsync();
            var check = query.Any(x => x.ShortCode == shortCode && x.Id != Id);
            return check;
        }

        public async Task<List<GroupBuyDto>> GetGroupBuyByShortCode(string ShortCode)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var query = await _groupBuyRepository.GetQueryableAsync();
                var groupbuy = query.Where(x => x.ShortCode == ShortCode).ToList();
                return ObjectMapper.Map<List<GroupBuy>, List<GroupBuyDto>>(groupbuy);
            }

        }

        public async Task<PagedResultDto<GroupBuyReportDto>> GetGroupBuyReportListAsync(GetGroupBuyReportListDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(GroupBuyReport.GroupBuyName);
            }
            var totalCount = await _groupBuyRepository.GetGroupBuyReportCountAsync();

            var items = await _groupBuyRepository.GetGroupBuyReportListAsync(input.SkipCount, input.MaxResultCount, input.Sorting);

            return new PagedResultDto<GroupBuyReportDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<GroupBuyReport>, List<GroupBuyReportDto>>(items)
            };
        }
        public async Task<PagedResultDto<GroupBuyReportDto>> GetGroupBuyTenantReportListAsync(GetGroupBuyReportListDto input)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                if (input.Sorting.IsNullOrWhiteSpace())
                {
                    input.Sorting = nameof(GroupBuyReport.GroupBuyName);
                }
                var totalCount = await _groupBuyRepository.GetGroupBuyTenantReportCountAsync();

                var items = await _groupBuyRepository.GetGroupBuyTenantReportListAsync(input.SkipCount, input.MaxResultCount, input.Sorting);

                return new PagedResultDto<GroupBuyReportDto>
                {
                    TotalCount = totalCount,
                    Items = ObjectMapper.Map<List<GroupBuyReport>, List<GroupBuyReportDto>>(items)
                };
            }
        }

        public async Task<GroupBuyDto> GetGroupBuyofTenant(string ShortCode, Guid TenantId)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var query = await _groupBuyRepository.GetQueryableAsync();
                var groupbuy = query.Where(x => x.ShortCode == ShortCode && x.TenantId == TenantId).FirstOrDefault();
                return ObjectMapper.Map<GroupBuy, GroupBuyDto>(groupbuy);
            }

        }

        public async Task<GroupBuyDto> GetWithItemGroupsAsync(Guid id)
        {
            var groupbuy = await _groupBuyRepository.GetWithItemGroupsAsync(id);
            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(groupbuy);
        }

        public async Task<GroupBuyReportDetailsDto> GetGroupBuyReportDetailsAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, OrderStatus? orderStatus = null)
        {
            var data = await _groupBuyRepository.GetGroupBuyReportDetailsAsync(id, startDate, endDate, orderStatus);
            return ObjectMapper.Map<GroupBuyReportDetails, GroupBuyReportDetailsDto>(data);
        }

        public async Task<GroupBuyReportDetailsDto> GetGroupBuyTenantReportDetailsAsync(Guid id)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var data = await _groupBuyRepository.GetGroupBuyReportDetailsAsync(id);
                return ObjectMapper.Map<GroupBuyReportDetails, GroupBuyReportDetailsDto>(data);
            }
        }

        public async Task<IRemoteStreamContent> GetTenantsListAsExcelFileAsync(Guid id)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var items = await _orderRepository.GetListAsync(0, int.MaxValue, nameof(Order.CreationTime), null, id, new List<Guid>());

                // Create a dictionary for localized headers
                var headers = new Dictionary<string, string>
                {
                    { "OrderNo", _l["OrderNo"] },
                    { "OrderDate", _l["OrderDate"] },
                    { "CustomerName", _l["CustomerName"] },
                    { "Email", _l["Email"] },
                    { "OrderStatus", _l["OrderStatus"] },
                    { "ShippingStatus", _l["ShippingStatus"] },
                    { "PaymentMethod", _l["PaymentMethod"] },
                    { "CheckoutAmount", _l["CheckoutAmount"] }
                };

                var excelData = items.Select(x => new Dictionary<string, object>
                {
                    { headers["OrderNo"], x.OrderNo },
                    { headers["OrderDate"], x.CreationTime.ToString("MM/d/yyyy h:mm:ss tt") },
                    { headers["CustomerName"], x.CustomerName },
                    { headers["Email"], x.CustomerEmail },
                    { headers["OrderStatus"], _l[x.OrderStatus.ToString()] },
                    { headers["ShippingStatus"], _l[x.ShippingStatus.ToString()] },
                    { headers["PaymentMethod"], _l[x.PaymentMethod.ToString()] },
                    { headers["CheckoutAmount"], "$ " + x.TotalAmount.ToString("N2") }
                });

                var memoryStream = new MemoryStream();
                await memoryStream.SaveAsAsync(excelData);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new RemoteStreamContent(memoryStream, "InventroyReport.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        public async Task<IRemoteStreamContent> GetListAsExcelFileAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, bool isChinese = false)
        {
            var groupBuy = await _groupBuyRepository.FirstOrDefaultAsync(x => x.Id == id);
            var items = await _orderRepository.GetListAsync(x => x.GroupBuyId == id
                            && (!startDate.HasValue || startDate.Value <= x.CreationTime)
                            && (!endDate.HasValue || endDate.Value > x.CreationTime));
            var data = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);

            if (groupBuy.ProtectPrivacyData)
            {
                data = data.HideCredentials();
            }

            // Create a dictionary for localized headers
            var headers = isChinese ? new Dictionary<string, string>
            {
                { "OrderNo", "訂單號" },
                { "OrderDate", "訂單日期" },
                { "CustomerName", "客戶名稱" },
                { "Email", "電子郵件" },
                { "OrderStatus", "訂單狀態" },
                { "ShippingStatus", "運輸狀態" },
                { "PaymentMethod", "付款方式" },
                { "CheckoutAmount", "結帳金額" }
            } :
            new Dictionary<string, string>
            {
                { "OrderNo", _l["OrderNo"] },
                { "OrderDate", _l["OrderDate"] },
                { "CustomerName", _l["CustomerName"] },
                { "Email", _l["Email"] },
                { "OrderStatus", _l["OrderStatus"] },
                { "ShippingStatus", _l["ShippingStatus"] },
                { "PaymentMethod", _l["PaymentMethod"] },
                { "CheckoutAmount", _l["CheckoutAmount"] }
            };

            var excelData = data.Select(x => new Dictionary<string, object>
            {
                { headers["OrderNo"], x.OrderNo },
                { headers["OrderDate"], x.CreationTime.ToString("MM/d/yyyy h:mm:ss tt") },
                { headers["CustomerName"], x.CustomerName },
                { headers["Email"], x.CustomerEmail },
                { headers["OrderStatus"], _l[x.OrderStatus.ToString()] },
                { headers["ShippingStatus"], _l[x.ShippingStatus.ToString()] },
                { headers["PaymentMethod"], _l[x.PaymentMethod.ToString()] },
                { headers["CheckoutAmount"], "$ " + x.TotalAmount.ToString("N2") }
            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(excelData);
            memoryStream.Seek(0, SeekOrigin.Begin);

            string fileName = groupBuy?.GroupBuyName ?? "GroupBuyReport";
            return new RemoteStreamContent(memoryStream, $"{fileName}.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<IRemoteStreamContent> GetAttachmentAsync(Guid id, Guid? tenantId, DateTime sendTime, RecurrenceType recurrenceType)
        {
            using (CurrentTenant.Change(tenantId))
            {
                var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, sendTime.Hour, sendTime.Minute, sendTime.Second);
                DateTime endDate = date.AddMinutes(-1);
                var startDate = recurrenceType switch
                {
                    RecurrenceType.Daily => date.AddDays(-1),
                    RecurrenceType.Weekly => date.AddDays(-7),
                    _ => throw new ArgumentException("Invalid RecurrenceType"),
                };
                return await GetListAsExcelFileAsync(id, startDate, endDate, true);
            }
        }

        public async Task UpdateSortOrderAsync(Guid id, List<GroupBuyItemGroupCreateUpdateDto> itemGroups)
        {
            var groupbuy = await _groupBuyRepository.GetAsync(id);
            await _groupBuyRepository.EnsureCollectionLoadedAsync(groupbuy, g => g.ItemGroups);

            foreach (var item in itemGroups)
            {
                var itemGroup = groupbuy.ItemGroups.FirstOrDefault(x => x.Id == item.Id);
                if (itemGroup != null)
                {
                    itemGroup.SortOrder = item.SortOrder;
                }
            }

            await _groupBuyRepository.UpdateAsync(groupbuy);
        }

        public async Task<DeliveryTemperatureCostDto> GetTemperatureCostAsync(ItemStorageTemperature itemStorageTemperature)
        {
            var query = await _temperatureRepository.GetQueryableAsync();
            var cost = query.Where(x => x.Temperature == itemStorageTemperature).FirstOrDefault();
            return ObjectMapper.Map<DeliveryTemperatureCost, DeliveryTemperatureCostDto>(cost);
        }
    }
}

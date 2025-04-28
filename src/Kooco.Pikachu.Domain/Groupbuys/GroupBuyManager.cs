using JetBrains.Annotations;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Groupbuys;

public class GroupBuyManager : DomainService
{
    #region Inject
    private readonly IGroupBuyRepository _groupBuyRepository;
    #endregion

    #region Constructor
    public GroupBuyManager(
        IGroupBuyRepository groupBuyRepository
    )
    {
        _groupBuyRepository = groupBuyRepository;
    }
    #endregion

    #region Methods
    public async Task<GroupBuy> CreateAsync(
        int groupBuyNo,
        string status,
        string groupBuyName,
        string entryURL,
        string entryURL2,
        string subjectLine,
        string shortName,
        string logoURL,
        string bannerURL,
        DateTime? startTime,
        DateTime? endTime,
        bool freeShipping,
        bool allowShipToOuterTaiwan,
        bool allowShipOversea,
        DateTime? expectShippingDateFrom,
        DateTime? expectShippingDateTo,
        int moneyTransferValidDayBy,
        int? moneyTransferValidDays,
        bool issueInvoice,
        bool autoIssueTriplicateInvoice,
        string? invoiceNote,
        bool protectPrivacyData,
        string? inviteCode,
        int profitShare,
        int? metaPixelNo,
        string? fBID,
        string? iGID,
        string? lineID,
        string? gAID,
        string? gTM,
        string? warningMessage,
        string? orderContactInfo,
        string? exchangePolicy,
        string? notifyMessage,
        string? excludeShippingMethod,
        bool isDefaultPaymentGateway,
        string? paymentMethod,
        string? groupbuyCondition,
        string? customerInformation,
        string? customerInformationDescription,
        string? groupBuyConditionDescription,
        string? exchangePolicyDescription,
        string shortCode,
        bool isEnterprise,
        int? freeShippingThreshold ,
        string? selfPickupDeliveryTime,
        string? blackCatDeliveryTime,
        string? homeDeliveryDeliveryTime,
        string? deliveredByStoreDeliveryTime,
        TaxType taxType,
        ProductType? productType,
        ColorScheme? colorScheme,
        string? primaryColor,
        string? secondaryColor,
        string? backgroundColor,
        string? secondaryBackgroundColor,
        string? alertColor,
        string? blockColor,
        ProductDetailsDisplayMethod? productDetailsDisplayMethod,
        string? notificationBar
    )
    {
        GroupBuy? sameName = await _groupBuyRepository.FirstOrDefaultAsync(x => x.GroupBuyName == groupBuyName);

        if (sameName is not null) throw new BusinessException(PikachuDomainErrorCodes.GroupBuyWithSameNameAlreadyExists);

        return new GroupBuy(GuidGenerator.Create(), groupBuyNo, status, groupBuyName, entryURL, entryURL2, subjectLine, shortName, logoURL, bannerURL, startTime, endTime, freeShipping,
            allowShipToOuterTaiwan, allowShipOversea, expectShippingDateFrom, expectShippingDateTo, moneyTransferValidDayBy, moneyTransferValidDays,
            issueInvoice, autoIssueTriplicateInvoice, invoiceNote, protectPrivacyData, inviteCode, profitShare, metaPixelNo, fBID, iGID, lineID, gAID, gTM,
            warningMessage, orderContactInfo, exchangePolicy, notifyMessage, excludeShippingMethod, isDefaultPaymentGateway, paymentMethod, groupbuyCondition, 
            customerInformation,customerInformationDescription,groupBuyConditionDescription,exchangePolicyDescription,shortCode, isEnterprise,freeShippingThreshold,selfPickupDeliveryTime,
            blackCatDeliveryTime,homeDeliveryDeliveryTime,deliveredByStoreDeliveryTime,taxType, productType, colorScheme, primaryColor, secondaryColor, backgroundColor, secondaryBackgroundColor, alertColor, blockColor,
            productDetailsDisplayMethod, notificationBar);
    }


    public void RemoveItemGroups(
        [NotNull] GroupBuy @groupBuy,
        List<Guid?> itemGroupsIds
        )
    {
        if (itemGroupsIds != null && itemGroupsIds.Any())
        {
            foreach (var itemGroup in @groupBuy.ItemGroups)
            {
                if (!itemGroupsIds.Contains(itemGroup.Id))
                {
                    @groupBuy.ItemGroups.Remove(itemGroup);
                }
            }
        }
    }

    public void AddItemGroupDetail(
        GroupBuyItemGroup itemGroup,
        int sortOrder,
        Guid? itemId,
        Guid? setItemId,
        ItemType itemType,
        string displayText,
        int? moduleNumber,
        Guid? itemDetailId
        ) 
    {
        itemGroup.GroupBuyItemGroupDetails(
            GuidGenerator.Create(),
            itemGroup.Id,
            sortOrder,
            itemId,
            setItemId,
            itemType,
            displayText,
            moduleNumber,
            itemDetailId
            );
    }
    public GroupBuyItemGroup AddItemGroup(
        GroupBuy groupBuy,
        int sortOrder,
        GroupBuyModuleType groupBuyModuleType,
        string? additionalInfo,
        string? productGroupModuleTitle,
        string? productGroupModuleImageSize,
        string? title,
        string? text,
        string? url
    )
    {
        return groupBuy.AddItemGroup(
            GuidGenerator.Create(),
            groupBuy.Id,
            sortOrder,
            groupBuyModuleType,
            additionalInfo,
            productGroupModuleTitle,
            productGroupModuleImageSize,
            title,
            text,
            url
        );
    }
    #endregion
}

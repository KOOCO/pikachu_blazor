using System;
using System.Collections.Generic;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Domain.Entities.Auditing;
using Kooco.Pikachu.EnumValues;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.GroupBuys;

public class GroupBuy : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 團購編號  GroupBuysNo
    /// </summary>
    public int GroupBuyNo { get; set; }

    public string ShortCode { get; set; }
    /// <summary>
    /// 團購狀態  Status
    /// </summary>
    public string Status { get; set; }
    /// <summary>
    /// 團購名稱  GroupBuyName
    /// </summary>
    public string GroupBuyName { get; set; }

    /// <summary>
    /// 團購入口網址  EntryURL
    /// </summary>
    public string? EntryURL { get; set; }

    /// <summary>
    /// 團購入口網址2  EntryURL2
    /// </summary>
    public string? EntryURL2 { get; set; }
    /// <summary>
    /// 副標題 SubTitle
    /// </summary>
    public string? SubjectLine { get; set; }

    /// <summary>
    /// 簡短名稱 ShortName
    /// </summary>
    public string? ShortName { get; set; }

    /// <summary>
    /// 通知欄 NotificationBar
    /// </summary>
    public string? NotificationBar { get; set; }

    /// <summary>
    /// Logo圖片網址 LogoURL
    /// </summary>

    public string? LogoURL { get; set; }

    /// <summary>
    /// Banner圖片網址 BannerURL
    /// </summary>
    public string? BannerURL { get; set; }

    /// <summary>
    /// 團購開始時間 StartTime
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 團購結束時間 EndTime
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 免運費 FreeShipping
    /// </summary>
    public bool FreeShipping { get; set; }
    //TODO: Find a better way to handle this
    /// <summary>
    /// 不可選用的運送選項 ExcludeShippingMethod
    /// </summary>
    //public Array? ExcludeShippingMethod { get; set; }

    /// <summary>
    /// 允許寄送到外島 AllowShipToOuterTaiwan
    /// </summary>
    public bool AllowShipToOuterTaiwan { get; set; }

    /// <summary>
    /// 允許寄送到海外 AllowShipOversea
    /// </summary>
    public bool AllowShipOversea { get; set; }

    /// <summary>
    /// 預計開始配送日期 ExpectShippingDateFrom
    /// </summary>
    public DateTime? ExpectShippingDateFrom { get; set; }

    /// <summary>
    /// 預計結束配送日期 ExpectShippingDateTo
    /// </summary>
    public DateTime? ExpectShippingDateTo { get; set; }

    //todo add payment method
    /// <summary>
    /// 可接受的付款方式 AcceptePaymentMethod
    /// </summary>
    //public Array? AcceptePaymentMethod { get; set; }

    /// <summary>
    /// 匯款有效日期依照 MoneyTransferValidDayBy
    /// </summary>
    public int MoneyTransferValidDayBy { get; set; }

    /// <summary>
    /// 匯款有效天數 MoneyTransferValidDays
    /// </summary>
    public int? MoneyTransferValidDays { get; set; }

    /// <summary>
    /// 開立發票 IssueInvoice
    /// </summary>
    public bool IssueInvoice { get; set; }

    /// <summary>
    /// 自動開立三聯單 AutoIssueTriplicateInvoice
    /// </summary>
    public bool AutoIssueTriplicateInvoice { get; set; }

    /// <summary>
    /// 電子發票備註 InvoiceNote
    /// </summary>
    public string? InvoiceNote { get; set; }

    /// <summary>
    /// 隱藏部分個資 ProtectPrivacyData
    /// </summary>
    public bool ProtectPrivacyData { get; set; }

    /// <summary>
    /// 團購邀請碼 InviteCode
    /// </summary>
    public string? InviteCode { get; set; }

    /// <summary>
    /// 分潤 ShareProfit
    /// </summary>
    public int ProfitShare { get; set; }

    /// <summary>
    /// Meta Pixel No
    /// </summary>
    public int? MetaPixelNo { get; set; }

    /// <summary>
    /// Facebook ID
    /// </summary>
    public string? FBID { get; set; }

    /// <summary>
    /// Instagram ID
    /// </summary>
    public string? IGID { get; set; }

    /// <summary>
    /// Line ID
    /// </summary>
    public string? LineID { get; set; }

    /// <summary>
    /// Google Analytics ID
    /// </summary>
    public string? GAID { get; set; }

    /// <summary>
    /// Google Tag Manager ID
    /// </summary>
    public string? GTM { get; set; }

    /// <summary>
    /// 團購注意事項 WarningMessage
    /// </summary>
    public string? WarningMessage { get; set; }

    /// <summary>
    /// 訂購人資訊 OrderContactInfo
    /// </summary>
    public string? OrderContactInfo { get; set; }

    /// <summary>
    /// 退換貨政策 ExchangePolicy
    /// </summary>
    public string? ExchangePolicy { get; set; }
    public string? ExchangePolicyDescription { get; set; }

    /// <summary>
    /// 通知信件文案 NotifyMessage
    /// </summary>
    public string? NotifyMessage { get; set; }
    public string? ExcludeShippingMethod { get; set; }
    public bool IsDefaultPaymentGateWay { get; set; }
    public string? PaymentMethod { get; set; }
    public string? GroupBuyCondition { get; set; }
    public string? GroupBuyConditionDescription { get; set; }
    public string? CustomerInformation { get; set; }
    public string? CustomerInformationDescription { get; set; }
    public bool IsGroupBuyAvaliable { get; set; }
    public decimal? SalesAmount { get; set; }
    public string? SelfPickupDeliveryTime { get; set; }
    public string? BlackCatDeliveryTime { get; set; }
    public string? HomeDeliveryDeliveryTime { get; set; }
    public string? DeliveredByStoreDeliveryTime { get; set; }
    public TaxType TaxType { get; set; }
    public int? FreeShippingThreshold { get; set; }
    /// <summary>
    /// 預設使用的發貨倉庫 Default Warehouse used for shipping
    /// </summary>
    //public Warehouse DefaultWarehouse { get; set; }
    public bool IsEnterprise { get; set; }
    public ProductType? ProductType { get; set; }
    public string? FacebookLink { get; set; }
    public string? InstagramLink { get; set; }
    public string? LINELink { get; set; }
    public GroupBuyTemplateType? TemplateType { get; set; }
    public ColorScheme? ColorSchemeType { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? BackgroundColor { get; set; }
    public string? SecondaryBackgroundColor { get; set; }
    public string? AlertColor { get; set; }
    public string? BlockColor { get; set; }
    public ProductDetailsDisplayMethod? ProductDetailsDisplayMethod { get; set; }
    public ICollection<GroupBuyItemGroup> ItemGroups { get; set; }

    public GroupBuy()
    {

    }

    public GroupBuy(Guid id, int groupNo, string status,
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
        Id = id;
        GroupBuyNo = groupNo;
        Status = status;
        GroupBuyName = groupBuyName;
        EntryURL = entryURL?.TrimEnd('/') + "/" + Id; 
        EntryURL2 = entryURL2;
        SubjectLine = subjectLine;
        ShortName = shortName;
        LogoURL = logoURL;
        BannerURL = bannerURL;
        StartTime = startTime;
        EndTime = endTime;
        FreeShipping = freeShipping;
        AllowShipOversea = allowShipOversea;
        AllowShipToOuterTaiwan = allowShipToOuterTaiwan;
        ExpectShippingDateFrom = expectShippingDateFrom;
        ExpectShippingDateTo = expectShippingDateTo;
        MoneyTransferValidDayBy = moneyTransferValidDayBy;
        MoneyTransferValidDays = moneyTransferValidDays;
        IssueInvoice = issueInvoice;
        AutoIssueTriplicateInvoice = autoIssueTriplicateInvoice;
        InvoiceNote = invoiceNote;
        InviteCode = inviteCode;
        ProtectPrivacyData = protectPrivacyData;
        ProfitShare = profitShare;
        MetaPixelNo = metaPixelNo;
        FBID = fBID;
        IGID = iGID;
        LineID = lineID;
        GAID = gAID;
        GTM = gTM;
        WarningMessage = warningMessage;
        OrderContactInfo = orderContactInfo;
        ExchangePolicy = exchangePolicy;
        NotifyMessage = notifyMessage;
        ExcludeShippingMethod = excludeShippingMethod;
        IsDefaultPaymentGateWay = isDefaultPaymentGateway;
        PaymentMethod = paymentMethod;
        GroupBuyCondition = groupbuyCondition;
        CustomerInformation = customerInformation;
        CustomerInformationDescription = customerInformationDescription;
        GroupBuyConditionDescription = groupBuyConditionDescription;
        ExchangePolicyDescription = exchangePolicyDescription;
        ShortCode = shortCode;
        IsEnterprise = isEnterprise;
        FreeShippingThreshold = freeShippingThreshold;
        ItemGroups = new List<GroupBuyItemGroup>();
        SelfPickupDeliveryTime = selfPickupDeliveryTime;
        HomeDeliveryDeliveryTime = homeDeliveryDeliveryTime;
        BlackCatDeliveryTime = blackCatDeliveryTime;
        DeliveredByStoreDeliveryTime = deliveredByStoreDeliveryTime;
        TaxType = taxType;
        ProductType = productType;
        ColorSchemeType = colorScheme;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        BackgroundColor = backgroundColor;
        SecondaryBackgroundColor = secondaryBackgroundColor;
        AlertColor = alertColor;
        BlockColor = blockColor;
        ProductDetailsDisplayMethod = productDetailsDisplayMethod;
        NotificationBar = notificationBar;
    }

    public GroupBuyItemGroup AddItemGroup(
        Guid id,
        Guid groupBuyId,
        int sortOrder,
        GroupBuyModuleType groupBuyModuleType,
        string? additionalInfo,
        string? productGroupModuleTitle,
        string? productGroupModuleImageSize
    )
    {
        var groupBuyItemGroup = new GroupBuyItemGroup(
            id,
            groupBuyId,
            sortOrder,
            groupBuyModuleType,
            additionalInfo,
            productGroupModuleTitle,
            productGroupModuleImageSize
        );
        ItemGroups.Add(groupBuyItemGroup);
        return groupBuyItemGroup;
    }


}

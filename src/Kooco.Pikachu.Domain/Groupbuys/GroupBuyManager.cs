using JetBrains.Annotations;
using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Groupbuys
{
    public class GroupBuyManager : DomainService
    {
        private readonly IGroupBuyRepositroy _groupBuyRepositroy;

        public GroupBuyManager(IGroupBuyRepositroy groupBuyRepositroy)
        {
            _groupBuyRepositroy = groupBuyRepositroy;


        }

        public async Task<GroupBuy> CreateAsync(int groupBuyNo,
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
          string? exchangePolicyDescription


            )
        {

            return new GroupBuy(GuidGenerator.Create(), groupBuyNo, status, groupBuyName, entryURL, entryURL2, subjectLine, shortName, logoURL, bannerURL, startTime, endTime, freeShipping,
                allowShipToOuterTaiwan, allowShipOversea, expectShippingDateFrom, expectShippingDateTo, moneyTransferValidDayBy, moneyTransferValidDays,
                issueInvoice, autoIssueTriplicateInvoice, invoiceNote, protectPrivacyData, inviteCode, profitShare, metaPixelNo, fBID, iGID, lineID, gAID, gTM,
                warningMessage, orderContactInfo, exchangePolicy, notifyMessage, excludeShippingMethod, isDefaultPaymentGateway, paymentMethod, groupbuyCondition, customerInformation,customerInformationDescription,groupBuyConditionDescription,exchangePolicyDescription);

        }

        public async Task<GroupBuy> UpdateAsync(Guid Id, int groupBuyNo,
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
              [CanBeNull] string concurrencyStamp = null)
        {
            var groupBuy = await _groupBuyRepositroy.GetAsync(Id);
            groupBuy.GroupBuyNo = groupBuyNo;
            groupBuy.Status = status;
            groupBuy.GroupBuyName = groupBuyName;
            groupBuy.EntryURL = entryURL;
            groupBuy.EntryURL2 = entryURL2;
            groupBuy.SubjectLine = subjectLine;
            groupBuy.ShortName = shortName;
            groupBuy.LogoURL = logoURL;
            groupBuy.BannerURL = bannerURL;
            groupBuy.StartTime = startTime;
            groupBuy.EndTime = endTime;
            groupBuy.FreeShipping = freeShipping;
            groupBuy.AllowShipToOuterTaiwan = allowShipToOuterTaiwan;
            groupBuy.AllowShipOversea = allowShipOversea;
            groupBuy.ExpectShippingDateFrom = expectShippingDateFrom;
            groupBuy.ExpectShippingDateTo = expectShippingDateTo;
            groupBuy.MoneyTransferValidDayBy = moneyTransferValidDayBy;
            groupBuy.MoneyTransferValidDays = moneyTransferValidDays;
            groupBuy.IssueInvoice = issueInvoice;
            groupBuy.AutoIssueTriplicateInvoice = autoIssueTriplicateInvoice;
            groupBuy.InvoiceNote = invoiceNote;
            groupBuy.ProtectPrivacyData = protectPrivacyData;
            groupBuy.InviteCode = inviteCode;
            groupBuy.ProfitShare = profitShare;
            groupBuy.MetaPixelNo = metaPixelNo;
            groupBuy.FBID = fBID;
            groupBuy.IGID = iGID;
            groupBuy.IGID = lineID;
            groupBuy.GAID = gAID;
            groupBuy.GTM = gTM;
            groupBuy.WarningMessage = warningMessage;
            groupBuy.OrderContactInfo = orderContactInfo;
            groupBuy.ExchangePolicy = exchangePolicy;
            groupBuy.NotifyMessage = notifyMessage;
            groupBuy.ExcludeShippingMethod = excludeShippingMethod;
            groupBuy.IsDefaultPaymentGateWay = isDefaultPaymentGateway;
            groupBuy.PaymentMethod = paymentMethod;
            groupBuy.GroupBuyCondition = groupbuyCondition;
            groupBuy.CustomerInformation = customerInformation;

            return await _groupBuyRepositroy.UpdateAsync(groupBuy);
        }

        public void AddItemGroupDetail(
            GroupBuyItemGroup itemGroup,
            int sortOrder,
            string? itemDescription,
            Guid? itemId,
            Guid? imageId
            
            ) 
        {
            itemGroup.GroupBuyItemGroupDetails(
                GuidGenerator.Create(),
                itemGroup.Id,
                sortOrder,
                itemDescription,
                itemId,
                imageId
                );
        }
        public GroupBuyItemGroup AddItemGroup(
            GroupBuy groupBuy,
            int sortOrder,
            string? title
            )
        {
            return groupBuy.AddItemGroup(
                GuidGenerator.Create(),
                groupBuy.Id,
                sortOrder,
                title
                );
        }
    }
}

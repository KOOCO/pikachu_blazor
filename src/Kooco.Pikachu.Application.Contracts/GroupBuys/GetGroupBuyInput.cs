using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.GroupBuys
{
    public class GetGroupBuyInput:PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }
        public int? GroupBuyNo { get; set; }
     
        public string? Status { get; set; }
    
        public string? GroupBuyName { get; set; }

     
        public string? EntryURL { get; set; }

        
        public string? EntryURL2 { get; set; }
    
        public string? SubjectLine { get; set; }

     
        public string? ShortName { get; set; }

      
        public string? LogoURL { get; set; }

       
        public string? BannerURL { get; set; }

    
        public DateTime? StartTime { get; set; }

        
        public DateTime? EndTime { get; set; }

        public bool? FreeShipping { get; set; }
       

        public bool? AllowShipToOuterTaiwan { get; set; }

     
        public bool? AllowShipOversea { get; set; }

     
        public DateTime? ExpectShippingDateFrom { get; set; }

       
        public DateTime? ExpectShippingDateTo { get; set; }

      
        public int? MoneyTransferValidDayBy { get; set; }

   
        public int? MoneyTransferValidDays { get; set; }

     
        public bool? IssueInvoice { get; set; }

     
        public bool? AutoIssueTriplicateInvoice { get; set; }

        public string? InvoiceNote { get; set; }

  
        public bool? ProtectPrivacyData { get; set; }


        public string? InviteCode { get; set; }

      
        public int? ProfitShare { get; set; }

       
        public int? MetaPixelNo { get; set; }

   
        public string? FBID { get; set; }

     
        public string? IGID { get; set; }

        public string? LineID { get; set; }

     
        public string? GAID { get; set; }

   
        public string? GTM { get; set; }

     
        public string? WarningMessage { get; set; }

        
        public string? OrderContactInfo { get; set; }

     
        public string? ExchangePolicy { get; set; }

        public string? NotifyMessage { get; set; }

    }
}

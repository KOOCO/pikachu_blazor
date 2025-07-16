using Kooco.Pikachu.EnumValues;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Kooco.Pikachu.GroupBuys.Interfaces
{
    /// <summary>
    /// Service responsible for GroupBuy reporting and Excel export functionality
    /// Extracted from GroupBuyAppService to follow Single Responsibility Principle
    /// </summary>
    public interface IGroupBuyReportingService : IApplicationService
    {
        /// <summary>
        /// Export GroupBuy overview data to Excel
        /// </summary>
        Task<IRemoteStreamContent> ExportGroupBuyOverviewExcelAsync(Guid groupBuyId);

        /// <summary>
        /// Export GroupBuy product rankings to Excel
        /// </summary>
        Task<IRemoteStreamContent> ExportGroupBuyProductRankingExcelAsync(Guid groupBuyId);

        /// <summary>
        /// Export GroupBuy order data to Excel
        /// </summary>
        Task<IRemoteStreamContent> ExportGroupBuyOrderExcelAsync(Guid groupBuyId);

        /// <summary>
        /// Export GroupBuy delivery data to Excel
        /// </summary>
        Task<IRemoteStreamContent> ExportGroupBuyDeliveryExcelAsync(Guid groupBuyId);

        /// <summary>
        /// Generate GroupBuy sales report
        /// </summary>
        Task<GroupBuyReportDto> GenerateGroupBuySalesReportAsync(Guid groupBuyId);

        /// <summary>
        /// Generate GroupBuy inventory report
        /// </summary>
        Task<GroupBuyInventoryReportDto> GenerateGroupBuyInventoryReportAsync(Guid groupBuyId);

        /// <summary>
        /// Generate GroupBuy revenue report
        /// </summary>
        Task<GroupBuyRevenueReportDto> GenerateGroupBuyRevenueReportAsync(Guid groupBuyId);

        /// <summary>
        /// Export GroupBuy member data to Excel
        /// </summary>
        Task<IRemoteStreamContent> ExportGroupBuyMemberExcelAsync(Guid groupBuyId);

        /// <summary>
        /// Export GroupBuy payment data to Excel
        /// </summary>
        Task<IRemoteStreamContent> ExportGroupBuyPaymentExcelAsync(Guid groupBuyId);

        /// <summary>
        /// Export GroupBuy refund data to Excel
        /// </summary>
        Task<IRemoteStreamContent> ExportGroupBuyRefundExcelAsync(Guid groupBuyId);

        /// <summary>
        /// Get GroupBuy report list with pagination
        /// </summary>
        Task<PagedResultDto<GroupBuyReportDto>> GetGroupBuyReportListAsync(GetGroupBuyReportListDto input);

        /// <summary>
        /// Get GroupBuy report details with filtering options
        /// </summary>
        Task<GroupBuyReportDetailsDto> GetGroupBuyReportDetailsAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, OrderStatus? orderStatus = null);

        /// <summary>
        /// Export GroupBuy list data to Excel with filtering
        /// </summary>
        Task<IRemoteStreamContent> GetListAsExcelFileAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Get GroupBuy tenant report list with pagination
        /// </summary>
        Task<PagedResultDto<GroupBuyReportDto>> GetGroupBuyTenantReportListAsync(GetGroupBuyReportListDto input);

        /// <summary>
        /// Get GroupBuy tenant report details
        /// </summary>
        Task<GroupBuyReportDetailsDto> GetGroupBuyTenantReportDetailsAsync(Guid id);

        /// <summary>
        /// Export tenants list data to Excel
        /// </summary>
        Task<IRemoteStreamContent> GetTenantsListAsExcelFileAsync(Guid id);
    }
}
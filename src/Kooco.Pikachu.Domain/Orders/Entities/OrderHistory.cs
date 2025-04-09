using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Orders.Entities;

/// <summary>
/// 訂單歷史記錄
/// </summary>
public class OrderHistory : FullAuditedEntity<Guid>
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public Guid OrderId { get; set; }  // The associated order

    /// <summary>
    /// 操作類型
    /// 例如：建立、更新、退款請求等
    /// </summary>
    public string ActionType { get; set; } // E.g., Created, Updated, RefundRequested, etc.

    /// <summary>
    /// 操作詳情
    /// JSON 或純文字形式的詳細資訊
    /// </summary>
    public string ActionDetails { get; set; } // JSON or plain text for details

    /// <summary>
    /// 編輯者用戶識別碼
    /// 第三方訂單可為空
    /// </summary>
    public Guid? EditorUserId { get; set; } // Nullable for third-party orders

    /// <summary>
    /// 編輯者用戶名稱
    /// 無用戶資料時可為空
    /// </summary>
    public string? EditorUserName { get; set; } // Nullable if no user data is available

    /// <summary>
    /// 預設建構函式
    /// </summary>
    public OrderHistory() { }

    /// <summary>
    /// 建立訂單歷史記錄的建構函式
    /// </summary>
    /// <param name="id">歷史記錄識別碼</param>
    /// <param name="orderId">訂單識別碼</param>
    /// <param name="actionType">操作類型</param>
    /// <param name="actionDetails">操作詳情</param>
    /// <param name="editorUserId">編輯者用戶識別碼</param>
    /// <param name="editorUserName">編輯者用戶名稱</param>
    public OrderHistory(
        Guid id,
        Guid orderId,
        string actionType,
        string actionDetails,
        Guid? editorUserId,
        string? editorUserName) : base(id)
    {
        OrderId = orderId;
        ActionType = actionType;
        ActionDetails = actionDetails;
        EditorUserId = editorUserId;
        EditorUserName = editorUserName;
    }

    /// <summary>
    /// 訂單導覽屬性
    /// </summary>
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; }
}

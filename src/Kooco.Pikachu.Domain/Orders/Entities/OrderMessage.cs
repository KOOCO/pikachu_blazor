using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Orders.Entities;

/// <summary>
/// 訂單訊息
/// </summary>
public class OrderMessage : AuditedEntity<Guid>
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public Guid OrderId { get; set; } // Required: ID of the order associated with the message

    /// <summary>
    /// 發送者識別碼（可選：客戶或商家的識別碼）
    /// </summary>
    public Guid? SenderId { get; set; } // Optional: ID of the sender (either customer or merchant)

    /// <summary>
    /// 訊息內容
    /// </summary>
    public string Message { get; set; } // Required: Content of the message

    /// <summary>
    /// 訊息發送時間（可選：若未提供，伺服器將預設為目前時間）
    /// </summary>
    public DateTime? Timestamp { get; set; } // Optional: Time the message was sent (server will default to current time if not provided)

    /// <summary>
    /// 是否為商家發送（若為true表示商家，false表示客戶）
    /// </summary>
    public bool IsMerchant { get; set; } // Required: Indicates if the sender is the merchant (true) or customer (false)

    /// <summary>
    /// 發送者名稱
    /// </summary>
    [NotMapped]
    public string SenderName { get; set; }

    public bool IsRead { get; set; } // Indicates if the message has been read

    /// <summary>
    /// 預設建構函式
    /// </summary>
    public OrderMessage() { }

    /// <summary>
    /// 建立訂單訊息的建構函式
    /// </summary>
    /// <param name="id">訊息識別碼</param>
    /// <param name="orderId">訂單識別碼</param>
    /// <param name="senderId">發送者識別碼</param>
    /// <param name="message">訊息內容</param>
    /// <param name="isMerchant">是否為商家發送</param>
    public OrderMessage(Guid id, Guid orderId, Guid? senderId, string message, bool isMerchant) : base(id)
    {
        SenderId = senderId;
        OrderId = orderId;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        IsMerchant = isMerchant;
        Timestamp = DateTime.UtcNow; // Default to current UTC time if no timestamp provided
    }
}

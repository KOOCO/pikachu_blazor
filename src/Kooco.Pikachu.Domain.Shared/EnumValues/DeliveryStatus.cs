namespace Kooco.Pikachu.EnumValues;

/// <summary>
/// 訂單配送狀態的枚舉。
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// 處理中：表示訂單正在處理中。
    /// </summary>
    Processing,

    /// <summary>
    /// 已發貨：表示訂單已經發貨。
    /// </summary>
    Shipped,

    /// <summary>
    /// 待發貨：表示訂單準備發貨。
    /// </summary>
    ToBeShipped,

    /// <summary>
    /// 已送達：表示訂單已經送達。
    /// </summary>
    Delivered,

    /// <summary>
    /// 已完成：表示訂單已經完成。
    /// </summary>
    Completed,
}
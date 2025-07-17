using System;
using System.Threading;

namespace Kooco.Pikachu;

public static class TestDataGenerator
{
    private static long _itemNoCounter = DateTime.Now.Ticks;
    
    public static string GenerateUniqueItemName(string baseName = "TestItem")
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        var randomPart = Guid.NewGuid().ToString("N")[..6];
        return $"{baseName}_{timestamp}_{randomPart}";
    }
    
    public static long GenerateUniqueItemNo()
    {
        return Interlocked.Increment(ref _itemNoCounter);
    }
    
    public static string GenerateUniqueSku(string prefix = "SKU")
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        var randomPart = Guid.NewGuid().ToString("N")[..4];
        return $"{prefix}_{timestamp}_{randomPart}";
    }
    
    public static string GenerateUniqueEmail()
    {
        var randomPart = Guid.NewGuid().ToString("N")[..8];
        return $"test_{randomPart}@example.com";
    }
    
    public static int GenerateUniqueGroupBuyNo()
    {
        return (int)(DateTime.Now.Ticks % int.MaxValue);
    }
    
    public static string GenerateUniqueShortCode()
    {
        return DateTime.Now.ToString("yyyyMMddHH") + new Random().Next(10, 99);
    }
}
using System;

namespace Kooco.Pikachu.Items
{
    /// <summary>
    /// Helper class to generate unique test data to avoid database constraint violations
    /// </summary>
    public static class TestDataGenerator
    {
        private static readonly Random _random = new Random();
        
        /// <summary>
        /// Generates a unique item name by appending timestamp and random number
        /// </summary>
        public static string GenerateUniqueItemName(string baseName = "TestItem")
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var randomSuffix = _random.Next(1000, 9999);
            return $"{baseName}_{timestamp}_{randomSuffix}";
        }
        
        /// <summary>
        /// Generates a unique SKU by appending timestamp and random number
        /// </summary>
        public static string GenerateUniqueSku(string baseName = "SKU")
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var randomSuffix = _random.Next(1000, 9999);
            return $"{baseName}_{timestamp}_{randomSuffix}";
        }
        
        /// <summary>
        /// Generates a unique group buy name
        /// </summary>
        public static string GenerateUniqueGroupBuyName(string baseName = "TestGroupBuy")
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var randomSuffix = _random.Next(1000, 9999);
            return $"{baseName}_{timestamp}_{randomSuffix}";
        }
        
        /// <summary>
        /// Generates a unique set item name
        /// </summary>
        public static string GenerateUniqueSetItemName(string baseName = "TestSetItem")
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var randomSuffix = _random.Next(1000, 9999);
            return $"{baseName}_{timestamp}_{randomSuffix}";
        }
        
        /// <summary>
        /// Generates a unique email address for testing
        /// </summary>
        public static string GenerateUniqueEmail(string baseName = "test")
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var randomSuffix = _random.Next(1000, 9999);
            return $"{baseName}_{timestamp}_{randomSuffix}@example.com";
        }
        
        /// <summary>
        /// Generates a unique phone number for testing
        /// </summary>
        public static string GenerateUniquePhoneNumber()
        {
            var randomNumber = _random.Next(100000000, 999999999);
            return $"09{randomNumber.ToString().Substring(0, 8)}";
        }
        
        /// <summary>
        /// Generates a unique short code
        /// </summary>
        public static string GenerateUniqueShortCode(string baseName = "SC")
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString().Substring(5); // Last 8 digits
            var randomSuffix = _random.Next(100, 999);
            return $"{baseName}{timestamp}{randomSuffix}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu
{
    /// <summary>
    /// list of putting constant key word
    /// </summary>
    public static class Constant
    {
        public const string TenantOwner = "TenantOwner";
        public const string TenantContactPerson = "TenantContactPerson";
        public const string TenantContactTitle = "TenantContactTitle";
        public const string TenantContactEmail = "TenantContactEmail";
        public const string ShareProfitPercent = "ShareProfitPercent";
        public const string Logo = "LogoUrl";
        public const string Status = "Status";
        public const string BannerUrl = "BannerUrl";
        public const string ShortCode = "ShortCode";
        public const string TenantUrl = "TenantUrl";
        public const string Domain = "Domain";

        public const string Birthday = "Birthday";
        public const string FacebookId = "FacebookId";
        public const string LineId = "LineId";
        public const string GoogleId = "GoogleId";
        public const string MobileNumber = "MobileNumber";
        public const string Gender = "Gender";


        public const int MaxImageSizeInBytes = 512000;
        public static readonly List<string> ValidImageExtensions = [".jpg", ".png", ".svg", ".jpeg", ".webp"];
        public static readonly List<string> ValidFaviconExtensions = [".ico"];

        public static readonly List<string> TenantContactTitles = ["Mr.", "Ms."];
        public static IReadOnlyList<string> Genders = ["Male", "Female", "Other"];
    }
}

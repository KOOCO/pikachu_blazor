using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kooco.Pikachu.Members;

public static class MemberConsts
{
    public const string Role = "MEMBER";
    public const string MemberAndUserRole = "MEMBER AND USER";

    public const string MembersManagement = "MembersManagement";

    public const string Male = "Male";
    public const string Female = "Female";
    public const string MaleAvatarUrl = "images/Male_Avatar.png";
    public const string FemaleAvatarUrl = "images/Female_Avatar.png";

    public static string GetAvatarUrl(string? gender)
    {
        return gender == Female
            ? FemaleAvatarUrl
            : MaleAvatarUrl;
    }

    public static class Members
    {
        public const string Default = MembersManagement + ".Members";
        public const string Edit = Default + ".Edit";
    }

    public static class ShopCarts
    {
        public const string Default = MembersManagement + ".ShopCarts";
    }

    public static class UserAddresses
    {
        public const string Default = MembersManagement + ".UserAddresses";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string SetIsDefault = Default + ".SetIsDefault";
    }

    public static class UserShoppingCredits
    {
        public const string Default = MembersManagement + ".UserShoppingCredits";
    }

    public static class MemberTags
    {
        public const string New = "New";
        public const string Existing = "Existing";
        public const string Blacklisted = "Blacklisted";

        public static readonly List<string> Names =
            [
                New,
                Existing,
                Blacklisted
            ];
    }
}

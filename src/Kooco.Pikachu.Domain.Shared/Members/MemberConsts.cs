namespace Kooco.Pikachu.Members;

public static class MemberConsts
{
    public const string All = "All";
    public const string NewMembers = "NewMembers";
    public const string ExistingMembers = "ExistingMembers";

    public const string Role = "MEMBER";

    public const string MembersManagement = "MembersManagement";

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
    }
}

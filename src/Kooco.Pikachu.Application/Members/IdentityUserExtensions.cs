using System;
using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.Users;

public static class IdentityUserExtensions
{
    public static void SetBirthday(this IdentityUser user, DateTime? birthday)
    {
        user.RemoveProperty(Constant.Birthday);
        user.SetProperty(Constant.Birthday, birthday);
    }

    public static DateTime? GetBirthday(this IdentityUser user)
    {
        return user.GetProperty<DateTime?>(Constant.Birthday);
    }

    public static DateTime? GetBirthday(this IdentityUserDto user)
    {
        return user.GetProperty<DateTime?>(Constant.Birthday);
    }

    public static void SetMobileNumber(this IdentityUser user, string? mobileNumber)
    {
        user.RemoveProperty(Constant.MobileNumber);
        user.SetProperty(Constant.MobileNumber, mobileNumber);
    }

    public static string? GetMobileNumber(this IdentityUser user)
    {
        return user.GetProperty<string?>(Constant.MobileNumber);
    }

    public static string? GetMobileNumber(this IdentityUserDto user)
    {
        return user.GetProperty<string?>(Constant.MobileNumber);
    }

    public static void SetGender(this IdentityUser user, string? gender)
    {
        user.RemoveProperty(Constant.Gender);
        user.SetProperty(Constant.Gender, gender);
    }

    public static string? GetGender(this IdentityUser user)
    {
        return user.GetProperty<string?>(Constant.Gender);
    }

    public static string? GetGender(this IdentityUserDto user)
    {
        return user.GetProperty<string?>(Constant.Gender);
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Kooco.Pikachu.Blazor;
public static class PikachuBlazorDefaults
{
    public static bool TryGetPage(this NavigationManager navigation, out int currentPage)
    {
        var uri = navigation.ToAbsoluteUri(navigation.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(nameof(Page), out var pageValue))
        {
            return int.TryParse(pageValue, out currentPage);
        }

        currentPage = default;
        return false;
    }
    public static void NavigateTo(this NavigationManager navigation, [NotNull] string[] segments, string? previousPage = default, Guid id = default, int currentPage = default)
    {
        if (segments is null)
        {
            throw new ArgumentNullException(nameof(segments), "Segments cannot be null");
        }

        if (segments.Length == default)
        {
            throw new ArgumentException("Segments cannot be empty", nameof(segments));
        }

        var path = $"/{string.Join("/", segments)}";
        if (previousPage != default) path = $"{path}/{previousPage}";
        if (id != default) path = $"{path}/{id}";
        if (currentPage != default) path = $"{path}?{nameof(Page)}={currentPage}";
        navigation.NavigateTo(path);
    }
}
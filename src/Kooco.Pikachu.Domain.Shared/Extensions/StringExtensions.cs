﻿using System;

namespace Kooco.Pikachu.Extensions;

public static class StringExtensions
{
    public static bool IsValidUrl(this string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}

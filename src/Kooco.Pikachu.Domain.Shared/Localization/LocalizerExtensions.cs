using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

public static class LocalizerExtensions
{
    /// <summary>
    /// Localizes a key and replaces parameters.
    /// Supports both dictionary-based named placeholders and positional arrays.
    /// Always returns the localization value or the key if not found.
    /// </summary>
    public static string WithParams(this IStringLocalizer localizer, string key, object? parameters)
    {
        if (string.IsNullOrWhiteSpace(key))
            return key ?? string.Empty; // Return key as fallback

        LocalizedString localized = localizer[key];
        string text = localized.Value ?? key;

        if (parameters == null)
            return text;

        if (parameters is Dictionary<string, string> dict)
        {
            foreach (var kv in dict)
            {
                text = text.Replace("{" + kv.Key + "}", kv.Value ?? string.Empty);
            }
            return text;
        }

        if (parameters is object[] arr && arr.Length > 0)
        {
            try
            {
                text = string.Format(text, arr);
            }
            catch (FormatException)
            {
                return text;
            }
        }

        return text;
    }
}
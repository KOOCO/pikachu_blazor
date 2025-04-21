
using System;

namespace Kooco.Pikachu.Items
{
    public class ItemConsts
    {
        public const int MaxItemNameLength = 60;
        public const int MaxDescriptionTitleLength = 60;
        public const int MaxItemBadgeLength = 4;

        public const string DefaultBadgeColor = "#D6D6D6";

        public static string GetContrastingTextColor(string bgColor)
        {
            if (string.IsNullOrEmpty(bgColor)) return "#000"; // fallback

            // Remove the '#' if present
            var color = bgColor.TrimStart('#');

            if (color.Length == 6)
            {
                var r = Convert.ToInt32(color.Substring(0, 2), 16);
                var g = Convert.ToInt32(color.Substring(2, 2), 16);
                var b = Convert.ToInt32(color.Substring(4, 2), 16);

                // Calculate brightness (simple luminance formula)
                var brightness = (r * 299 + g * 587 + b * 114) / 1000;

                return brightness > 125 ? "#000" : "#fff";
            }

            // fallback
            return "#000";
        }

    }
}

using Kooco.Pikachu.Orders;
using System.Collections.Generic;

namespace Kooco.Pikachu.GroupBuys
{
    public static class HideCredentialsAppService
    {
        public static List<OrderDto> HideCredentials(this List<OrderDto> data)
        {
            data.ForEach(item =>
            {
                item.CustomerName = item.CustomerName.HideCredentials();
                item.CustomerEmail = item.CustomerEmail.HideCredentials();

                item.RecipientName = item.RecipientName.HideCredentials();
                item.RecipientEmail = item.RecipientEmail.HideCredentials();
            });
            return data;
        }

        public static List<GroupBuyReportOrderDto> HideCredentials(this List<GroupBuyReportOrderDto> data)
        {
            data.ForEach(item =>
            {
                item.CustomerName = item.CustomerName.HideCredentials();
                item.CustomerEmail = item.CustomerEmail.HideCredentials();
            });
            return data;
        }

        public static string HideCredentials(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char[] chars = input.ToCharArray();
            int endIndex = input.IndexOf('@');

            // If '@' is not found or it's the first character, process the whole string
            if (endIndex <= 0)
            {
                endIndex = chars.Length;
            }

            for (int i = 2; i < endIndex; i += 4)
            {
                if (i + 1 < endIndex)
                {
                    chars[i] = '*';
                    chars[i + 1] = '*';
                }
                else if (i < endIndex)
                {
                    chars[i] = '*';
                }
            }

            return new string(chars);

        }
    }
}

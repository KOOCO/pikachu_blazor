using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.GroupBuys
{
    public static class GroupBuyConsts
    {

        private const string DefaultSorting = "{0}GroupBuyName asc";
        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "GroupBuy." : string.Empty);
        }
    }
}

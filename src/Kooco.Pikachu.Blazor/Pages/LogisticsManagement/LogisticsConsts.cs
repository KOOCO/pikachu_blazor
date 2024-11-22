using System.Collections.Generic;

namespace Kooco.Pikachu.Blazor.Pages.LogisticsManagement
{
    public static class LogisticsConsts
    {
        public static List<string> LogisticsTypes = new() { "B2C", "C2C" };
        public static List<string> LogisticsSubTypes = new() { "711", "全家" };
        public static List<string> MainIslands = new()
        {
           "台北市",
           "新北市",
           "桃園市",
           "新竹縣",
           "苗栗縣",
           "台中市",
           "彰化縣",
           "南投縣",
           "雲林縣",
           "嘉義縣",
           "台南市",
           "高雄市",
           "屏東縣",
           "宜蘭縣",
           "花蓮縣",
           "台東縣",
           "基隆市"
        };
        public static List<string> OuterIslands = new() { "澎湖縣", "金門縣", "連江縣" };
    }
}

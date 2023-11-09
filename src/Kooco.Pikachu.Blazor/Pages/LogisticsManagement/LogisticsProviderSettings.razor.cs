using Blazorise.LoadingIndicator;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.Blazor.Pages.LogisticsManagement
{
    public partial class LogisticsProviderSettings
    {
        GreenWorldLogistics GreenWorld { get; set; } = new();
        HomeDelivery HomeDelivery { get; set; } = new();
        LoadingIndicator Loading { get; set; }

        readonly List<string> LogisticsTypes = new() { "B2C", "C2C" };
        readonly List<string> LogisticsSubTypes = new() { "711", "全家" };
        readonly List<string> MainIslands = new() { "Taipei", "Taoyuan", "Hsinchu", "Taichung", "Tainan", "Kaohsiung" };
        readonly List<string> OuterIslands = new() { "Penghu", "Kinmen", "Mazu"};
        void UpdateGreenWorldLogisticsAsync()
        {

        }
        void UpdateHomeDeliveryAsync()
        {

        }

        void HandleTagDelete(string item)
        {
            GreenWorld.LogisticsSubTypesList.Remove(item);
            if(GreenWorld.LogisticsSubTypesList.Count > 0)
            {
                GreenWorld.LogisticsSubTypes = JsonConvert.SerializeObject(GreenWorld.LogisticsSubTypes);
            }
            else
            {
                GreenWorld.LogisticsSubTypes = string.Empty;
            }
        }

        void OnSelectedValueChanged(string value)
        {
            GreenWorld.LogisticsType = value;
            GreenWorld.LogisticsSubTypesList = new();
            LogisticsSubTypes.ForEach(item => GreenWorld.LogisticsSubTypesList.Add($"{item}({value})"));
            GreenWorld.LogisticsSubTypes = JsonConvert.SerializeObject(GreenWorld.LogisticsSubTypes);
        }
    }

    public class GreenWorldLogistics
    {
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string StoreCode { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string HashKey { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string HashIV { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string SenderName { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string SenderPhoneNumber { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string LogisticsType { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string LogisticsSubTypes { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public List<string> LogisticsSubTypesList { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string FreeShippingThreshold { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string Freight { get; set; }

        public GreenWorldLogistics()
        {
            LogisticsSubTypesList = new();
        }
    }

    public class HomeDelivery
    {
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string CustomTitle { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string FreeShippingThreshold { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string Freight { get; set; }
    }
}

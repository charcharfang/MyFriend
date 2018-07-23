using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace MyFriend.Shop
{
    public class Govlan : ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "https://m.govlan.com/api/2.0/charging/getChargingPile";
            string data = "chargingStationId=" + staid;

            var ret = Utils.PostData(url, data);

            return ret;
        }

        public string GetStations()
        {
            string url = "https://m.govlan.com/api/2.0/govlan/getCity";
            string data = "token=&userId=0";
            var cityjson = Utils.PostData(url, data);
            StringBuilder sb = new StringBuilder();

            var cities = (JsonConvert.DeserializeObject(cityjson) as JToken)["data"];
            var cities1 = cities["cities"];
            var cities2 = cities["hotCities"];

            List<string> allcities = new List<string>();
            foreach (var city in cities1)
            {
                var ccode = city["cityCode"].ToString();
                if (false == allcities.Contains(ccode))
                {
                    allcities.Add(ccode);
                }
            }
            foreach (var city in cities2)
            {
                var ccode = city["cityCode"].ToString();
                if (false == allcities.Contains(ccode))
                {
                    allcities.Add(ccode);
                }
            }

            foreach (var cityCode in allcities)
            {
                url = "https://m.govlan.com/api/2.0/charging/screeningStation";
                data = "cityCode=" + cityCode + "&connectorType=&equipmentType=&lat=36&lng=117&operation=&page=1&pageSize=999999&sortBy=0&tag=&token=&userId=0";

                string json = Utils.PostData(url, data);

                sb.AppendLine(json);

            }

            return sb.ToString();
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            string[] lines = bigtext.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<List<string>> list = new List<List<string>>();
            foreach (var line in lines)
            {
                var stations = (JsonConvert.DeserializeObject(line) as JToken)["data"]["entities"] as JArray;

                for (int i = 0; i < stations.Count; i++)
                {
                    var s = stations[i];
                    list.Add(new List<string>() { Convert.ToString(s["chargingStationId"]), Convert.ToString(s["chargingStationName"]), Convert.ToString(s["chargingPile"]["quickCharge"]["total"]), Convert.ToString(s["chargingPile"]["slowCharge"]["total"]), Convert.ToString(s["electricityFee"]) });
                }
            }
            return list;
        }
    }
}

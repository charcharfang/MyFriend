using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyFriend.Shop
{
    public class CarEnergyNet : ICharge
    {
        
        public string GetStationDetail(string staid)
        {
            string url = "http://wx.carenergynet.cn/w/stations/"+staid+"?consId=&key=";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var ret = wc.DownloadString(url);
                return JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);
            }
        }

        public string GetStations()
        {
            string url = "http://wx.carenergynet.cn/w/stations?first=0&limit=999999&lat=36&lng=117";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var ret = wc.DownloadString(url);
                return ret;
            }
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["data"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                var s = stations[i];
                list.Add(new List<string>() { Convert.ToString(s["stationId"]), Convert.ToString(s["stationName"]),  Convert.ToString(s["dcPileTotal"]), Convert.ToString(s["acPileTotal"]) });
            }

            return list;
        }
    }
}

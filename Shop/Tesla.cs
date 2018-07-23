using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace MyFriend.Shop
{
    public class Tesla:ICharge
    {
        public string GetStationDetail(string staid)
        {
            return "在电站列表中就有详情";
        }

        public string GetStations()
        {
            string url = "https://wechat.teslamotors.com/findus.txt";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                return wc.DownloadString(url);
            }
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken) as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["id"].ToString(), stations[i]["title"].ToString(), stations[i]["chargers"].ToString() });
            }

            return list;
        }
    }
}

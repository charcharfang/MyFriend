using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace MyFriend.Shop
{
    public class StarCharge : ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "https://wx.starcharge.com/api/xcx.getStubGroup?FRAMEparams=%7B%22stubGroupId%22:%22"+staid+"%22,%22lat%22:36,%22lng%22:117%7D";
            string data = "{}";
            var ret = Utils.PostData(url, data);
            ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            return ret;
        }

        private string GetStationsByCity(string city)
        {
            string url = "https://wx.starcharge.com/api/xcx.findStubGroup?FRAMEparams=%7B%22screenItems%22:%22%22,%22lng%22:36,%22lat%22:117,%22city%22:%22"+city+"%22%7D";
            string data = "{}";
            var ret = Utils.PostData(url, data);

            return ret;
        }

        public string GetStations()
        {
            //这个方式，没有城市信息
            //string url = "https://wx.starcharge.com/api/xcx.findStubGroup?FRAMEparams=%7B%22city%22:%22%22,%22screenItems%22:%22%22,%22orderClause%22:0,%22lng%22:117,%22lat%22:36,%22page%22:1,%22pagecount%22:999999%7D";

            string url = "https://wx.starcharge.com/api/weChat.cityList?FRAMEparams=%7B%7D";
            string data = "{}";
            StringBuilder sb = new StringBuilder();

            var ret = Utils.PostData(url, data);
            var citylist = (JsonConvert.DeserializeObject(ret) as JToken)["data"] as JArray;
            foreach(var city in citylist)
            {
                var id = Convert.ToString(city["id"]);
                var name = Convert.ToString(city["name"]);
                var cityinfo = GetStationsByCity(id);

                sb.AppendLine(id);
                sb.AppendLine(name);
                sb.AppendLine(cityinfo);
            }

            return sb.ToString();
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            string[] all = bigtext.Split(new char[] { '\r', '\n' },StringSplitOptions.RemoveEmptyEntries);

            for (int c = 0; c < all.Length; c += 3)
            {
                if (all[c + 0] == "500300")
                {
                    c = c - 1;
                    continue;
                }
                var stations = (JsonConvert.DeserializeObject(all[c+2]) as JToken)["data"] as JArray;

                for (int i = 0; i < stations.Count; i++)
                {
                    var s = stations[i];
                    list.Add(new List<string>() {all[c+1], Convert.ToString(s["id"]), Convert.ToString(s["name"]), Convert.ToString(s["address"]), Convert.ToString(s["stubDcCnt"]), Convert.ToString(s["stubAcCnt"]) });
                }
            }
            return list;
        }
    }
}

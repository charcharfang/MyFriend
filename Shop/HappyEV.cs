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
    public class HappyEV:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://chargecoolv2.happyev.com:8080/happyevadmin/station/queryPipeList.htm";
            string data = "pagesize=20&stationid="+staid+"&token=20180702095531439785&userid=20180702095529087971";

            var ret = Utils.PostData(url, data);
            return ret;
        }

        public string GetStations()
        {
            string url = "http://chargecoolv2.happyev.com:8080/happyevadmin/station/queryStationsByKeywords.htm";
            string data = "keywords=&pagesize=999999";//此处就是漏洞，keywords不写就是全国

            var ret = Utils.PostData(url, data);
            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["body"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                var s = stations[i];
                list.Add(new List<string>() { Convert.ToString(s["stationid"]),Convert.ToString(s["stationname"]), Convert.ToString(s["stationaddress"]), Convert.ToString(s["fastpipenum"]), Convert.ToString(s["slowpipenum"]) });
            }

            return list;
        }
    }
}

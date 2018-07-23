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
    public class CamelRay:ICharge
    {
        public string GetStationDetail(string staid)
        {
            return "未做";
        }

        public string GetStations()
        {
            string url = "http://wechat.camelray.com/system/data";
            string data = "clientType=html&latitude=22.57024&longitude=113.8629&method=getChaStation&module=irecharge&ms=1531019478441&pageindex=1&pagesize=10&searchkey=%E7%94%B5&service=ChargeForWX&sign=b6737a9c805df37cb1127dd19e087d55";

            var ret = Utils.PostData(url, data);

            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["data"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["staid"].ToString(), stations[i]["staname"].ToString(), stations[i]["address"].ToString(), stations[i]["quantity"].ToString()});
            }

            return list;
        }
    }
}

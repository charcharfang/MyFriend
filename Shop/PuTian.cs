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
    public class PuTian : ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://183.78.183.241:18080/chargeService/station/getStationDetail";
            string data = "message=%7B%22stationCode%22%3A%22"+staid+"%22%7D";

            var ret = Utils.PostData(url, data);
            ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            return ret;
        }

        public string GetStations()
        {
            string url = "http://183.78.183.241:18080/chargeService/station/getAllStations";
            string data = "message=%7B%7D";

            var ret = Utils.PostData(url, data);
            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["stations"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                var s = stations[i];
                list.Add(new List<string>() { Convert.ToString(s["cityCode"]), Convert.ToString(s["stationCode"]), Convert.ToString(s["stationName"]), Convert.ToString(s["totalACs"]), Convert.ToString(s["totalDCs"]) });
            }

            return list;
        }
    }
}

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
    public class EVChargeOnline:ICharge
    {
        public string GetStationDetail(string staid)
        {
            //微信公众号
            if (staid.IndexOf("/") < 0)
            {
                return "格式：operatorId/stadionId";
            }
            var tmp = staid.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string url = String.Format("https://wp.evchargeonline.com/station/detail?timestamp=1531017669151&operatorId={0}&stationId={1}&longitude=117&latitude=36&isAjax=1", tmp[0], tmp[1]);
            using (WebClient wc = new WebClient())
            {
                var ret = wc.DownloadString(url);
                ret = ret.Replace("\\\"", "\"").Substring(1);
                ret = ret.Remove(ret.Length - 1, 1);
                ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);
                return ret;
            }
        }

        public string GetStations()
        {
            //官网
            //最后那个level=1，是全国的，越大，显示的桩越多
            string url = "https://www.evchargeonline.com/site/station/map?timestamp=1531015605115&operatorIds=&equipmentTypes=&payChannels=&isNewNantionalStandard=-1&isOpenToPublic=-1&isParkingFree=-1&longitude=121&latitude=31&radius=125790&aggregationLevel=99999";
            using (WebClient wc = new WebClient())
            {
                var ret = wc.DownloadString(url);
                ret = ret.Replace("\\\"", "\"").Substring(1);
                ret = ret.Remove(ret.Length - 1, 1);
                return ret;
            }
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["data"]["stations"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                var s = stations[i];
                list.Add(new List<string>() { Convert.ToString(s["stationId"]), Convert.ToString(s["address"]), Convert.ToString(s["operatorId"]), Convert.ToString(s["publicStationQty"]), Convert.ToString(s["privateStationQty"]),});
            }

            return list;
        }
    }
}

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
    public class ZTEV : ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://szompr.ztev.com.cn:8827/chargeApp/ztev/App_QueryStationDetail";
            string data = "{\"stationID\":\"" + staid + "\"}";

            var ret = Utils.PostData(url, data, contentType: "application/json");
            ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            return ret;
        }

        public string GetStations()
        {
            string url = "http://szompr.ztev.com.cn:8827/chargeApp/ztev/App_QueryStationByMap";
            string data = "{\"stationType\":\"\",\"sortType\":1,\"posX\" : 119.123456,\"posY\" : 33.123456,\"chargeType\":\"\"}";

            var ret = Utils.PostData(url, data,contentType:"application/json");

            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["result"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["stationID"].ToString(), stations[i]["name"].ToString(), stations[i]["sumposnum"].ToString()});
            }

            return list;
        }
    }
}

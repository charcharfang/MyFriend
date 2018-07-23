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
    public class DuDuChong:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "https://app.api.duduchong.com/api/app/plugs/"+staid+"/areas.json";
            string url2 = "https://app.api.duduchong.com/api/app/plugs/"+staid+".json";
            string ret = String.Empty;

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("device", "ver=158&client=ios&pushtype=1&lan=lan&device_id=a324asdflkj234234lkjslkfh2342412&cityCode=(null)&lat=23.11&lng=113.30&cityName=");
                ret = wc.DownloadString(url);
                string json = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);
                
                var ret2 = wc.DownloadString(url2);
                string json2 = JToken.Parse(ret2).ToString(Newtonsoft.Json.Formatting.Indented);

                return Utils.Decode(json + "\r\n" + json2);
            }
        }

        public string GetStations()
        {
            string url = "https://app.api.duduchong.com/api/app/plugs/near_list.json?lat=23.11&lon=113.30&radius=6km";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("device", "ver=158&client=ios&pushtype=1&lan=lan&device_id=a324asdflkj234234lkjslkfh2342412&cityCode=(null)&lat=23.11&lng=113.30&cityName=");
                var ret = wc.DownloadString(url);
                return ret;
            }
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["data"]["list"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["id"].ToString(), stations[i]["title"].ToString(), stations[i]["address"].ToString(), stations[i]["charges_quantity"].ToString() });
            }

            return list;
        }
    }
}

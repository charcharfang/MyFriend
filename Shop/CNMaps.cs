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
    public class CNMaps:ICharge
    {
        public string GetStationDetail(string staid)
        {
            return "未破解";
        }

        public string GetStations()
        {
            string url = "https://yuntuapi.amap.com/datasearch/local";
            string data = "filter=&city=%E5%85%A8%E5%9B%BD&output=json&keywords=&tableid=59031b107bbf197dd181cfe5&limit=20&language=zh&key=8b3fe04edd67f0524c77adc0eb1c3b7b&page=1&sortrule=&scode=290bcfd633ffdd6e6e6731df101d1134&ts=1530490941016";

            var ret = Utils.PostData(url, data);
            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["datas"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["_id"].ToString(), stations[i]["province"].ToString(), stations[i]["cityName"].ToString(), stations[i]["_name"].ToString(), stations[i]["_address"].ToString(), stations[i]["quantity"].ToString() });
            }

            return list;
        }
    }
}

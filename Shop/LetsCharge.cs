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
    public class LetsCharge:ICharge
    {
        public string GetStationDetail(string staid)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var ret = wc.DownloadString("http://admin.letscharge.cn/api/parking_areas/"+staid+".json?current_lat=90&current_lng=90&current_version=3.6.9");
                ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);
                return ret;
            }
        }

        public string GetStations()
        {
            using(WebClient wc = new WebClient())
            {                
                return wc.DownloadString("http://admin.letscharge.cn/api/parking_areas/search_map_only_point_free.json?current_version=3.6.9");
            }
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["items"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["id"].ToString(), stations[i]["free_charing_space"].ToString() });
            }

            return list;
        }
    }
}

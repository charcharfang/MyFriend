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
    public class iEV:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://share.i-ev.com/open/map/sitedetails/c_id/" + staid;
            var ret = String.Empty;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                ret = wc.DownloadString(url);

                //ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            }

            return ret;
        }

        public string GetStations()
        {
            //涵盖地球，最后那个level=12，是实验出来的。默认是5。
            string url = "http://share.i-ev.com/open/location/get_index?lat_l=180&lng_l=1&lat_r=1&lng_r=180&my_lat=90&my_lng=90&lat=&lng=&type=%20&owner=1&operator=%20&terminal=%20&zoom_level=12";
            var ret = String.Empty;

            using (WebClient wc = new WebClient())
            {                
                ret = wc.DownloadString(url);
                //ret = Utils.Decode(ret).Replace("\\\"", "\"").Replace(@"\\\/", @"/");
            }

            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["data"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["c_id"].ToString(), stations[i]["name"].ToString(), stations[i]["pay_name"].ToString(), stations[i]["fast_count"].ToString(), stations[i]["slow_count"].ToString() });
            }

            return list;
        }
    }
}

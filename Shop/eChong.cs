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
    public class eChong : ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://www.e-chong.com/getChargerByGroup?groupID=" + staid;
            string data = "_INVOKE_TYPE_=ajax";

            var ret = Utils.PostData(url, data,
                headers: new Dictionary<string, string>() { { "X-Requested-With", "XMLHttpRequest" }},
                accept: "application/json, text/javascript, */*; q=0.01",
                cookies: new List<Cookie>() {
                    new Cookie("JSESSIONID", "b788f3633c4da39c9a81a96506a2", "/", "www.e-chong.com"),
                    new Cookie("phoneNum", "", "/", "www.e-chong.com")
                }
            );

            ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            return Utils.Decode(ret);
        }

        public string GetStations()
        {
            //string cityurl = "http://www.e-chong.com/getCityInfo?_INVOKE_TYPE_=ajax";
            //using(WebClient wc = new WebClient())
            //{
            //    wc.Encoding = Encoding.UTF8;
            //    var allcities = wc.DownloadString(cityurl);
            //}
            string url = "http://www.e-chong.com/getMap";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var ret = wc.DownloadString(url);
                return ret;
            }
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken) as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["groupID"].ToString(), stations[i]["region"].ToString(), stations[i]["district"].ToString(), stations[i]["address"].ToString(), stations[i]["installDate"].ToString() });
            }

            return list;
        }
    }
}

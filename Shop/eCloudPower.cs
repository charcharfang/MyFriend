using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyFriend.Shop
{
    public class eCloudPower : ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://api.ecloudpower.com:8080/api/v1/station/"+staid;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var ret = wc.DownloadString(url);

                return JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);
            }
        }

        public string GetStations()
        {
            var url = "http://api.ecloudpower.com:8080/api/v1/station/all";
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

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["resultObject"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                var s = stations[i];
                list.Add(new List<string>() { Convert.ToString(s["id"]), Convert.ToString(s["name"]), Convert.ToString(s["institution_name"]), Convert.ToString(s["fastCount"]), Convert.ToString(s["slowCount"])});
                
            }

            return list;
        }
    }
}

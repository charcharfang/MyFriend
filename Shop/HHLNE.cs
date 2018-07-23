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
    public class HHLNE:ICharge
    {
        public string GetStationDetail(string staid)
        {
            //https://api.hhlne.com/charging/station_pole_list?offset=0&&count=1000&stationId=877209e2809611e7b79400ac84eb10f1
            string url = "https://api.hhlne.com/charging/new_station_detail?stationId=" + staid;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                                                 
                wc.Headers.Add("Auzhorization", "eyJhbGciOiJIUzUxMiJ9.eyJjbGllbnRJZCI6IkFCQyIsImltZWkiOiI2MzFEMENCMzIzQjI0OUM3QjUxQzg2OUNDNjYwQzQ1NSIsImV4cGlyYXRpb24iOjE1MzEzOTI3MzIxNzAsImlkIjoiNTZhNzllM2Y5OWYzNDg0MmFlOTEzMjE0MTJiZGY4NTYiLCJ1c2VyTmFtZSI6IjE1ODAwMzM2MjIzIiwicGxhdGZvcm0iOiJpb3MifQ.SbYGG5aojiS38P52K3Arx0IO3X_5HhiXUrW3Ux6NytXOFtJSrUPGQ0XdfLm7PTCHw9rR1-UzmRsMYpDfWYHzsw");
                wc.Headers.Add("_version", "3.5.1");
                wc.Headers.Add("_platform", "ios");
                wc.Headers.Add("Accept-Encoding", "br,gzip,deflate");
                wc.Headers.Add("Accept", "*/*");
                wc.Headers.Add("User-Agent", "chong dian zhuang/3.5.1(iPhone;iOS 11.4;Scale/3.00)");
                wc.Headers.Add("Accept-Language", "zh-Hans-CN;q=1");
                var ret = wc.DownloadString(url);

                ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

                return ret;
            }
        }

        public string GetStations()
        {
            string url = "https://api.hhlne.com/charging/all_station_list?cityId=&count=999999&latitude=0.000000&longitude=0.000000&offset=0&operatingParty=&provinceId=&rateType=0&stationType=";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Auzhorization", "eyJhbGciOiJIUzUxMiJ9.eyJjbGllbnRJZCI6IkFCQyIsImltZWkiOiI2MzFEMENCMzIzQjI0OUM3QjUxQzg2OUNDNjYwQzQ1NSIsImV4cGlyYXRpb24iOjE1MzEzOTI3MzIxNzAsImlkIjoiNTZhNzllM2Y5OWYzNDg0MmFlOTEzMjE0MTJiZGY4NTYiLCJ1c2VyTmFtZSI6IjE1ODAwMzM2MjIzIiwicGxhdGZvcm0iOiJpb3MifQ.SbYGG5aojiS38P52K3Arx0IO3X_5HhiXUrW3Ux6NytXOFtJSrUPGQ0XdfLm7PTCHw9rR1-UzmRsMYpDfWYHzsw");
                wc.Headers.Add("_version", "3.5.1");
                wc.Headers.Add("_platform", "ios");
                var ret = wc.DownloadString(url);
                return ret;
            }
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["chargingStationTableList"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                var s = stations[i];
                list.Add(new List<string>() { Convert.ToString(s["id"]), Convert.ToString(s["stationName"]), Convert.ToString(s["operatingParty"]), Convert.ToString(s["quickCharge"]), Convert.ToString(s["slowCharge"]) });
            }

            return list;
        }
    }
}

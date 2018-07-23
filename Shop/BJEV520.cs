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
    public class BJEV520:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://bq.bjev520.com/chargingStation/xxindex?chargingStation="+staid;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var ret = wc.DownloadString(url);
                return ret;
            }
        }

        public string GetStations()
        {
            string url = "http://bq.bjev520.com/chargingStation/api/getChargingStationList";
            string data = "scope=-1&owner=-1&city=%E9%9D%92%E5%B2%9B%E5%B8%82&latitude=39.98174&longitude=116.30631&brandStatus=&operator=&cpbrand=";

            var ret = Utils.PostData(url, data,
                headers:new Dictionary<string, string>() { { "X-Requested-With", "XMLHttpRequest" } },
                cookies:new List<System.Net.Cookie>() { new System.Net.Cookie("JSESSIONID","9BA18449F8476A316B264FC8D890460A","/","bq.bjev520.com")}
                );

            return ret;
        }
        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["chargingStationList"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["id"].ToString(), stations[i]["name"].ToString(), stations[i]["operater"].ToString(), stations[i]["fastChargingTypeCount"].ToString(), stations[i]["slowChargingTypeCount"].ToString() });
            }

            return list;
        }
    }
}

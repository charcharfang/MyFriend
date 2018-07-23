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
    public class _1Charge:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://mapi1.1charge.cn/station?t=get_detail";
            string data = "{\"staId\":"+staid+",\"lat\":33.123456,\"lng\":119.123456}";

            var ret = Utils.PostData(url, data);
            string json = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            url = "http://mapi1.1charge.cn/pile?t=get";
            data = "{\"staId\":" + staid + ",\"pageSize\":999,\"pageIndex\":1}";

            var ret2 = Utils.PostData(url, data);
            string json2 = JToken.Parse(ret2).ToString(Newtonsoft.Json.Formatting.Indented);
            return Utils.Decode(json+"\r\n"+json2);
        }

        public string GetStations()
        {
            string cityurl = "http://mapi1.1charge.cn/area?t=get_city_all";
            StringBuilder sb = new StringBuilder();

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var city = wc.DownloadString(cityurl);
                var jsondata = (JsonConvert.DeserializeObject(city) as JToken)["city"] as JArray;
                foreach(var json in jsondata)
                {
                    var citycode = Convert.ToInt32(json["Farea_id"]);
                    var cityname = Convert.ToString(json["Farea_name"]);

                    string url = "http://mapi1.1charge.cn/station?t=get4list";
                    string data = "{\"cityId\":"+citycode.ToString()+",\"sort\":1,\"dist\":0,\"pageSize\":9999,\"cityName\":\""+cityname+ "\",\"lng\" : 119.123456,\"key\":\"\",\"lat\" : 33.123456,\"staType\":\"\",\"pageIndex\":1,\"cityName_en\":\"\"}";

                    var ret = Utils.PostData(url, data);
                    sb.AppendLine(cityname);
                    sb.AppendLine(ret);
                }
            }

            return sb.ToString();
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var alllines = bigtext.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < alllines.Length; i += 2) {
                var cityname = alllines[i];
                var stations = (JsonConvert.DeserializeObject(alllines[i + 1]) as JToken)["data"]["station"] as JArray;
                foreach(var s in stations)
                {
                    list.Add(new List<string>() {cityname,Convert.ToString(s["Fsta_id"]), Convert.ToString(s["Fsta_name"]) });                    
                }
            }

            return list;
        }
    }
}

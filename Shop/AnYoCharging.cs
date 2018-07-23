using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Utility;

namespace MyFriend.Shop
{
    public class AnYoCharging:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://wx.anyocharging.com/api/v1/station/station/get";
            string data = "{\"id\":\""+staid+ "\"}";

            var ret = Utils.PostData(url, data, headers: new Dictionary<string, string>() {
                { "Authorization", "d2VpeGluLmFwcC5hbnlvY2hhcmdpbmcuY29tOk1qVXlOamhqTjJVeVlUTTBabU00T0dVME5tTTFNbVl3" },
                { "SESSIONID", "MzUzYjQzNjg4YzQ0YThlNmVjYTk5YmJj" },
                {"r","1530785863892" },
                },
                contentType:"applicatoin/json"
            );

            return JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public string GetStations()
        {
            string url = "http://wx.anyocharging.com/api/v1/station/station/search";
            string data = "{\"data\":\"{\"query\":\"\",\"city\":\"\",\"longitude\":120.51093230164267,\"latitude\":36.17809907769056,\"location\":\"附近\",\"order_by\":\"distance\",\"show_public\":true,\"show_specific\":true,\"show_ctrlable\":false,\"offset\":0,\"limit\":999999}";

            var ret = Utils.PostData(url, data,headers:new Dictionary<string, string>() {
                { "Authorization", "d2VpeGluLmFwcC5hbnlvY2hhcmdpbmcuY29tOk1qVXlOamhqTjJVeVlUTTBabU00T0dVME5tTTFNbVl3" },
                { "SESSIONID", "MzUzYjQzNjg4YzQ0YThlNmVjYTk5YmJj" },
                {"r","1530785863892" }
            });

            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["data"]["stations"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["id"].ToString(), stations[i]["name"].ToString(), stations[i]["open_type"].ToString(), stations[i]["ac_cnt"].ToString(), stations[i]["dc_cnt"].ToString() });
            }

            return list;
        }
    }
}

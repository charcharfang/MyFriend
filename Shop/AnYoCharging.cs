using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Utility;

namespace MyFriend.Shop
{
    public class AnYoCharging : ICharge
    {
        private string sessionid = "MjE1YTkyODVkMmVmNDEzZGE0YWIwNzBl";
        public string GetStationDetail(string staid)
        {
            string url = "http://wx.anyocharging.com/api/v1/station/station/get";
            string data = "{\"id\":\"" + staid + "\"}";

            var ret = Utils.PostData(url, data, headers: new Dictionary<string, string>() {
                { "Authorization", "d2VpeGluLmFwcC5hbnlvY2hhcmdpbmcuY29tOk1qVXlOamhqTjJVeVlUTTBabU00T0dVME5tTTFNbVl3" },
                { "SESSIONID", sessionid },
                {"r","1530785863892" },
                },
                contentType: "applicatoin/json"
            );

            return JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public string GetStations()
        {
            string url = "http://wx.anyocharging.com/action?code=061MbCta0KU0xv1m6Yra0XvUta0MbCty&state=search-stations";
            
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var html = wc.DownloadString(url);
                if (false == string.IsNullOrEmpty(html))
                {
                    sessionid = Utils.GetPartFromString(html, "?token=", "&bindtype");
                }
            }

            url = "http://wx.anyocharging.com/api/v1/station/station/search";
            string data = "{\"data\":\"{\"query\":\"\",\"city\":\"\",\"longitude\":120.51093230164267,\"latitude\":36.17809907769056,\"location\":\"附近\",\"order_by\":\"distance\",\"show_public\":true,\"show_specific\":true,\"show_ctrlable\":false,\"offset\":0,\"limit\":999999}";

            var ret = Utils.PostData(url, data, headers: new Dictionary<string, string>() {
                { "Authorization", "d2VpeGluLmFwcC5hbnlvY2hhcmdpbmcuY29tOk1qVXlOamhqTjJVeVlUTTBabU00T0dVME5tTTFNbVl3" },
                { "SESSIONID", sessionid },
                {"r","1532676532209" }
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

        public string Transform2Station(string bigtext)
        {
            //var header = Utils.GetUnifiedDataStructureFormatter(UnifiedDataStructure.Station);

            //var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["data"]["stations"] as JArray;
            //StringBuilder sb = new StringBuilder();
            ////"ID,AppID,Operator,ID2,Name,Address,Lng,Lat,FastCount,SlowCount,ElectricPrice,ServicePrice,ParkDesc,
            ////SiteGuide,BdLng,BdLat,Star,Label,Payment,Location,Province,City,District,StubGroupType,OperationType,
            ////OperationTime,Tel"
            //for (int i = 0; i < stations.Count; i++)
            //{
            //    var s = stations[i];
            //    sb.AppendFormat(header, 
            //        "AnYoCharging_" + Convert.ToString(s["id"]), 
            //        "安悦充电", 
            //        Convert.ToString(s["provider"]), 
            //        Convert.ToString(s["id"]),
            //        Convert.ToString(s["name"]),
            //        Convert.ToString(s["address"]),
            //        Convert.ToString(s["longitude"]), 
            //        Convert.ToString(s["latitude"]),
            //        Convert.ToString(s["fast_cnt"]), 
            //        Convert.ToString(s["slow_cnt"]), 
            //        Convert.ToString(s["dc_costfee"]) + "_" + Convert.ToString(s["ac_costfee"]),
            //        Convert.ToString(s["serve_fee"]), 
            //        Convert.ToString(s["parking_desc"]),
            //        Convert.ToString(s["office_description"]),
            //        Convert.ToString(s["longitude"]), 
            //        Convert.ToString(s["latitude"]),
            //        Convert.ToString(s["score"]), 
            //        "", 
            //        Convert.ToString(s["fee_type"]), 
            //        "",
            //        Convert.ToString(s["province"]), 
            //        Convert.ToString(s["city"]), 
            //        Convert.ToString(s["district"]),
            //        "", 
            //        "", 
            //        Convert.ToString(s["open_forbusiness_date"]), 
            //        ""
            //    );
            //}

            //return sb.ToString();
            return "";
        }

        public string Transform2Order(string bigtext)
        {
            return "";
        }
    }
}

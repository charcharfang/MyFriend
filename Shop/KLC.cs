using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace MyFriend.Shop
{
    public class KLC:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://47.107.27.65:8070/ChargeSation/selectChargeSationById";
            string data = "latitude=36&longitude=117&sid="+staid;

            var ret = Utils.PostData(url, data);
            string json = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            url = "http://47.107.27.65:8070/ChargePile/selectChargePilePart";
            data = "sid=" + staid;

            ret = Utils.PostData(url, data);
            string json2 = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);


            return json + "\r\n" + json2;
        }

        public string GetStations()
        {
            StringBuilder sb = new StringBuilder();

            string url = "http://47.107.27.65:8070/ChargeSation/chargeListView";

            for (int page = 1; page < 999999; page++)
            {
                string data = "address=&latitude=36&longitude=117&order=0&page="+page.ToString();

                var ret = Utils.PostData(url, data
                    //,headers:new Dictionary<string, string>() { {"token","" } },
                    );

                var array = (JsonConvert.DeserializeObject(ret) as JToken)["dataContent"] as JArray;
                if (array.Count < 1) break;

                sb.AppendLine(ret);
            }

            return sb.ToString();
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var lines = bigtext.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var stations = (JsonConvert.DeserializeObject(line) as JToken)["dataContent"] as JArray;

                for (int i = 0; i < stations.Count; i++)
                {
                    list.Add(new List<string>() { stations[i]["sid"].ToString(), stations[i]["sname"].ToString(), stations[i]["address"].ToString(), stations[i]["emptyFast"].ToString(), stations[i]["emptyLower"].ToString() });
                }
            }
            return list;
        }
    }
}

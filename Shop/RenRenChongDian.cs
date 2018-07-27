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
    public class RenRenChongDian:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://yyc.renrenchongdian.com/charge/pile";
            string data = "{\"pileId\":\"" + staid + "\"}";

            var ret = Utils.PostData(url, data, contentType:"application/json");
            ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            return ret;
        }

        public string GetStations()
        {
            string url = "http://yyc.renrenchongdian.com/charge/pile/newlist";
            string data = "{\"pileType\":\"1,2,3\",\"start\":0,\"operatorIds\":\"-1\",\"count\":999999,\"longitude\":90,\"latitude\":90,\"isReservable\":false,\"hasOtherOperator\":false,\"hasPersonalPile\":false,\"distance\":10007180,\"cityId\":\"\",\"onlyOpen\":false}";

            var ret = Utils.PostData(url, data, contentType:"application/json");

            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["result"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                var s = stations[i]["pile"];
                list.Add(new List<string>() { s["pileId"].ToString(), s["pileName"].ToString(), s["directNum"].ToString(), Convert.ToString(s["alterNum"]) });
            }

            return list;
        }
    }
}

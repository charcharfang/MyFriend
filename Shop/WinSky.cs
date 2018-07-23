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
    public class WinSky:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://weixin.win-sky.com.cn/winsky-wx/charge/station/"+staid;
            string data = "type=0&calLongitude=117&calLatitude=36";

            var ret = Utils.PostData(url, data);
            var json = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            url = "http://weixin.win-sky.com.cn/winsky-wx/charge/station/" + staid+"/stakes";
            data = "type=0&calLongitude=117&calLatitude=36";

            var moredetail = Utils.PostData(url, data);
            var json2 = JToken.Parse(moredetail).ToString(Newtonsoft.Json.Formatting.Indented);

            return json + "\r\n" + json2;
        }

        public string GetStations()
        {
            string url = "http://weixin.win-sky.com.cn/winsky-wx/charge/stations";
            //关键字：站
            string data = "calLongitude=117&calLatitude=36&groupName=%E7%AB%99";

            var ret = Utils.PostData(url, data);
            ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["data"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["uuid"].ToString(), stations[i]["groupName"].ToString(), stations[i]["stakeDcFree"].ToString(), Convert.ToString(stations[i]["stakeAcFree"]) });
            }

            return list;
        }
    }
}

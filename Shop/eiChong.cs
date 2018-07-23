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
    public class eiChong:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "https://api.eichong.com/api/app/powerStation.do?ext=t&userId=1&powerStationId=" + staid + "&longitude=117&latitude=36";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var ret = wc.DownloadString(url);

                ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

                return Utils.Decode(ret);
            }
        }

        //订单
        //https://api.eichong.com/api/app/chargeShow/chargeOrderList.do?ext=t&userId=8888&pageNumber=1&pageNum=99999
        //userId此时不大于62000，所以用户人数应该不超过62000人。

        public string GetStations()
        {
            string url = "https://api.eichong.com/api/app/powerStation/search.do?ext=t&cityCode=&keyWord=&longitude=117&latitude=36&range=&isJustAc=&isJustDc=&isNotBusy=0&minPower=1kw&maxPower=99999kw&parkingFeeType=&pageNumber=1&sortType=distance&distanceLongitude=117&distanceLatitude=36&pageNum=999999";
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

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["data"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                int ac = Convert.ToInt32(stations[i]["epAcTotalNum"]);
                int dc = Convert.ToInt32(stations[i]["epDcTotalNum"]);

                list.Add(new List<string>() { stations[i]["id"].ToString(), stations[i]["provinceName"].ToString(), stations[i]["cityName"].ToString(), stations[i]["name"].ToString(), stations[i]["address"].ToString(),Convert.ToString(ac+dc) });
            }

            return list;
        }
    }
}

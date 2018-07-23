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
    public class WeiZiWo:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://mobile.weiziwo.net/api/station/pile!details.do";
            string data = String.Format("model.pointId="+staid+"&token=acooluselesstoken");

            var ret = Utils.PostData(url, data);
            ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            return ret;
        }

        public string GetStations()
        {
            string url = "http://mobile.weiziwo.net/api/login.do";
            string data = "model.cID=YourMagicCID&model.password=YourRealPassword&model.phone=IOS&model.username=YourMobileNumber";

            //var token = Utils.PostData(url, data);
            var token = "a cool useless token";

            url = "http://mobile.weiziwo.net/api/station/viewnearby!get.do";
            data = "model.lat=36.00&model.lon=119.00&model.range=100000&token=" + token;
            var ret = Utils.PostData(url, data);

            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["points"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["id"].ToString(), stations[i]["name"].ToString(), stations[i]["operatorId"].ToString(), Convert.ToString(stations[i]["pointType"]) });
            }

            return list;
        }
    }
}

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
    public class UEEE : ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://www.ueee.cn/wx/map/ajaxdetail";
            string data = String.Format("id={0}&longitude=&latitude=&source=2",staid);

            var ret = Utils.PostData(url, data);
            ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            return ret;
        }

        public string GetStations()
        {
            string url = "http://www.ueee.cn/wx/map/ajaxlist";
            string data = "page=1&pagesize=10000&longitude=&latitude=&source=2&usestate=&content=&chargerType=&openStatus=";

            var ret = Utils.PostData(url, data);

            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken) as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["id"].ToString(), stations[i]["name"].ToString(), stations[i]["address"].ToString(), Convert.ToString(stations[i]["number"]) });
            }

            return list;
        }
    }
}

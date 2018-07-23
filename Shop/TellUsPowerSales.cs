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
    public class TellUsPowerSales : ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "https://xiaochengxu.telluspowersales.com/index.php?r=fd/chargdetial&siteId="+ staid;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var ret = wc.DownloadString(url);
                ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

                return Utils.Decode(ret);
            }
        }

        public string GetStations()
        {
            List<string> urllist = new List<string>() {
                "https://xiaochengxu.telluspowersales.com/index.php?r=fd/chargelsital&longitude=116.39564439654349&latitude=39.92998633415992",
                "https://xiaochengxu.telluspowersales.com/index.php?r=fd/chargelsital&longitude=121.48789867758752&latitude=31.24916282916863",
                "https://xiaochengxu.telluspowersales.com/index.php?r=fd/chargelsital&longitude=108.95443752408026&latitude=34.26523679184847",
                "https://xiaochengxu.telluspowersales.com/index.php?r=fd/chargelsital&longitude=113.49741965532301&latitude=23.039290342968467"
            };

            string ret = String.Empty;

            foreach (var url in urllist)
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    ret += wc.DownloadString(url);
                    ret += "\r\n";
                }
            }

            return Utils.Decode(ret);
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var lines = bigtext.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var stations = (JsonConvert.DeserializeObject(line) as JToken)["result"] as JArray;

                for (int i = 0; i < stations.Count; i++)
                {
                    list.Add(new List<string>() { stations[i]["id"].ToString()});
                }
            }
            return list;
        }
    }
}

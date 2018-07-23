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
    public class EnergeDog:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://121.42.53.24/charge/APIes/StationPile/detailup";
            string data = "--Boundary+7EA7AB7BF62ACBA8\r\nContent-Disposition: form-data; name=\"Electricities_id\"\r\n\r\n"+staid+"\r\n--Boundary+7EA7AB7BF62ACBA8\r\nContent-Disposition: form-data; name=\"lat\"\r\n\r\n36\r\n--Boundary+7EA7AB7BF62ACBA8\r\nContent-Disposition: form-data; name=\"lng\"\r\n\r\n119\r\n--Boundary+7EA7AB7BF62ACBA8\r\nContent-Disposition: form-data; name=\"page\"\r\n\r\n0\r\n--Boundary+7EA7AB7BF62ACBA8\r\nContent-Disposition: form-data; name=\"pageSize\"\r\n\r\n0\r\n--Boundary+7EA7AB7BF62ACBA8\r\nContent-Disposition: form-data; name=\"token\"\r\n\r\n9d5672af6e0e2479decdacfebe7f3591\r\n--Boundary+7EA7AB7BF62ACBA8\r\nContent-Disposition: form-data; name=\"userid\"\r\n\r\n\r\n--Boundary+7EA7AB7BF62ACBA8--";

            var ret = Utils.PostData(url, data,
                contentType: "multipart/form-data;boundary=Boundary+7EA7AB7BF62ACBA8"
                );

            ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);
            return ret;
        }

        public string GetStations()
        {
            string url = "http://121.42.53.24/charge/APIes/ChargeInterface/stationList";
            string data = "--Boundary+C7F2C04AE0224663\r\nContent-Disposition: form-data; name=\"app\"\r\n\r\n0\r\n--Boundary+C7F2C04AE0224663\r\nContent-Disposition: form-data; name=\"card\"\r\n\r\n0\r\n--Boundary+C7F2C04AE0224663\r\nContent-Disposition: form-data; name=\"cash\"\r\n\r\n0\r\n--Boundary+C7F2C04AE0224663\r\nContent-Disposition: form-data; name=\"chargeMethod\"\r\n\r\n0\r\n--Boundary+C7F2C04AE0224663\r\nContent-Disposition: form-data; name=\"free\"\r\n\r\n0\r\n--Boundary+C7F2C04AE0224663\r\nContent-Disposition: form-data; name=\"open\"\r\n\r\n0\r\n--Boundary+C7F2C04AE0224663\r\nContent-Disposition: form-data; name=\"parkingMethod\"\r\n0\r\n--Boundary+C7F2C04AE0224663\r\nContent-Disposition: form-data; name=\"token\"\r\n\r\n--Boundary+C7F2C04AE0224663--\r\n";

            var ret = Utils.PostData(url, data,
                contentType: "multipart/form-data; boundary=Boundary+C7F2C04AE0224663"
                );

            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["info"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                var s = stations[i];
                list.Add(new List<string>() { Convert.ToString(s["id"]), Convert.ToString(s["name"]), Convert.ToString(s["lng"]), Convert.ToString(s["lat"])});

            }

            return list;
        }
    }
}

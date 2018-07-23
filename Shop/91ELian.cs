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
    public class _91ELian : ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "https://app02.91elian.com/evcharge-as/api/appointment/getStationAppointmentInfo";
            string data = "{\"stationId\":\"" + staid + "\"}";

            var ret = Utils.PostData(url, data, contentType: "application/json");
            string json = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            url = "https://app02.91elian.com/evcharge-as/api/chargeStation/getStationInfo";
            data = "{\"stationId\":\"" + staid + "\"}";

            var ret2 = Utils.PostData(url, data, contentType: "application/json");
            string json2 = JToken.Parse(ret2).ToString(Newtonsoft.Json.Formatting.Indented);

            return json + "\r\n" + json2;
        }

        public string GetStations()
        {
            string url = "https://app02.91elian.com/evcharge-as/api/chargeStation/getStations";
            string data = "{\"lon\":\"117.13\",\"sortType\":3,\"chargeTypes\":\"\",\"chargeVelocitys\":\"\",\"isPublicBuf\":\"\",\"lat\":\"36.67\",\"isOperateBuf\":\"\",\"maxVoltages\":\"\"}";

            var ret = Utils.PostData(url, data,
                contentType: "application/json"
                //headers:new Dictionary<string, string>() { { "secretKey", "01e06898b58b0aebc8b4db25f5acc7d298b01d3b" }, { "token", "" }, { "imei", "" }, { "userId", "" }, { "Accept-Encoding", "br,gzip,deflate" }, {"Accept-Language","zh-Hans-CN;q=1" } },
                //userAgent: "TITANS/2.6.7 (iPhone; iOS 11.4; Scale/3.00)"
                );

            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken)["data"]["stationList"] as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["stationId"].ToString(), stations[i]["stationName"].ToString(), stations[i]["pileCount"].ToString(), stations[i]["pileInfoStatus"].ToString()});
            }

            return list;
        }
    }
}

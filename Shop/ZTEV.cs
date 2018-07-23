using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace MyFriend.Shop
{
    public class ZTEV : ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://szompr.ztev.com.cn:8827/chargeApp/ztev/App_QueryStationDetail";
            string data = "{\"stationID\":\"" + staid + "\"}";

            var ret = Utils.PostData(url, data, contentType: "application/json");

            return ret;
        }

        public string GetStations()
        {
            string url = "http://szompr.ztev.com.cn:8827/chargeApp/ztev/App_QueryStationByMap";
            string data = "{\"stationType\":\"\",\"sortType\":1,\"posX\" : 119.123456,\"posY\" : 33.123456,\"chargeType\":\"\"}";

            var ret = Utils.PostData(url, data,contentType:"application/json");

            return ret;
        }
    }
}

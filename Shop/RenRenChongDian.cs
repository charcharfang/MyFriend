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
            string url = "http://yyc.renrenchongdian.com/charge/station";
            string data = "{\"stationId\":\"" + staid + "\"}";

            var ret = Utils.PostData(url, data, contentType:"application/json");

            return ret;
        }

        public string GetStations()
        {
            string url = "http://yyc.renrenchongdian.com/charge/pile/newlist";
            string data = "{\"pileType\":\"1,2,3\",\"start\":0,\"operatorIds\":\"-1\",\"count\":999999,\"longitude\":90,\"latitude\":90,\"isReservable\":false,\"hasOtherOperator\":false,\"hasPersonalPile\":false,\"distance\":10007180,\"cityId\":\"\",\"onlyOpen\":false}";

            var ret = Utils.PostData(url, data, contentType:"application/json");

            return ret;
        }
    }
}

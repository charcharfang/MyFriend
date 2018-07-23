using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace MyFriend.Shop
{
    public class Parwa:ICharge
    {
        public string GetStationDetail(string staid)
        {
            return "貌似没有站";
        }

        public string GetStations()
        {
            string url = "http://120.26.110.49/electric/app/public/getstationlist";
            string data = "distance=50000&latitude=36.17&longitde=120.50";

            var ret = Utils.PostData(url, data);

            return ret;
        }
    }
}

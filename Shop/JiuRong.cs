using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace MyFriend.Shop
{
    public class JiuRong : ICharge
    {
        public string GetStationDetail(string staid)
        {
            throw new NotImplementedException();
        }

        public string GetStations()
        {
            string url = "http://weixin.jiurongkg.com/public/station/getStationList.action";
            string data = "longitude=117&latitude=36";

            return Utils.PostData(url, data);
        }
    }
}

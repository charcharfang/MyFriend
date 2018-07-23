using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyFriend.Shop
{
    public class PowerLife:ICharge
    {
        //https://www.powerlife.com.cn/api/d/operator/list，供应商
        public string GetStationDetail(string staid)
        {
            string url = "http://www.powerlife.com.cn/api/d/pile/detail2?code="+staid+"&key=KEY&loc=118,36&operator=op052";
            string data = String.Format("id={0}&longitude=&latitude=&source=2", staid);

            //var ret = Utils.PostData(url, data);

            return "";
        }

        public string GetStations()
        {
            string url = "https://www.powerlife.com.cn/api/d/pile/marker?key=KEY&loc=118,36&count=99999&interface=0&operator=&ver=2.0.0";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var ret = wc.DownloadString(url);
                return ret;
            }
            
        }
    }
}

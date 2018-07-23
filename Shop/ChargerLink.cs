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
    public class ChargerLink:ICharge
    {
        public string GetStationDetail(string staid)
        {
            string url = "http://wechat.bustil.com/api/plug/details";
            string data = "id="+staid+"&lat=90&lng=90";

            var ret = Utils.PostData(url, data,
                headers: new Dictionary<string, string>() { { "X-Requested-With", "XMLHttpRequest" }, { "CL-TOKEN", "53353334-d868-428e-bbc4-afbc9280e1b4" } },
                cookies: new List<Cookie>() { new Cookie("laravel_session", "eyJpdiI6Ijk3NFhOK1prS0pCaUtFcWY0djRVcXc9PSIsInZhbHVlIjoiQUhzNDNxXC9mdVprMXBtQWF2U1FLVnF6RGxSVVFsUU1YcGlkWnNKSmFkeDJ5UFZ1T2NtdTZvMTR0UDB0UDVFeENocEFJZCtnUkFoTGZNdkhZUnhLUkNnPT0iLCJtYWMiOiJjNjdhOWNkZjNkNTlkNTdiNjhjZDI1NTRhNWYxZGUxOGU2MzY0OWQ2MTRmOGM1MTI1NTQxOWNmYzZmMTYxYmM1In0%3D", "/", "wechat.bustil.com") },
                accept: "application/json, text/javascript, */*; q=0.01"
                );
            return Utils.Decode(ret);
        }

        public string GetStationsEncrypted()
        {
            string url = "https://app-bustil-api.chargerlink.com/spot/searchSpot";
            string data = "latitude=90&longitude=90&page=1&sort=1&pagesize=999999";

            var ret = Utils.PostData(url, data);
            return ret;
        }

        public string GetStations()
        {
            var ctext = SetCookie();
            string url = "http://wxcloud.saas.chargerlink.com/api/user/login";
            string data = "code=061leZWz0Zi2dg1OvOXz0jAWWz0leZWz&servicerId=0a53b5de234543978e418e152aa5b3f2";

            string[] cs = ctext.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            var ckey = cs[0];
            var cvalue = cs[1].Replace("; path", "");

            var ret = Utils.PostData(url, data,
                headers: new Dictionary<string, string>() { { "X-Requested-With", "XMLHttpRequest" } },
                //{ "Accept", "application/json,text/javascript,*/*;q=0.01" }},
                cookies: new List<Cookie>() { new Cookie(ckey, cvalue, "/", "wxcloud.saas.chargerlink.com") },
                accept: "application/json, text/javascript, */*; q=0.01"
                );
            var others = Utils.Decode(ret);

            var token = (JsonConvert.DeserializeObject(ret) as JToken)["data"]["token"].ToString();
            

            url = "http://wxcloud.saas.chargerlink.com/api/plug/list";
            data = "keywords=&lat=90&lng=90&page=1&limit=9999";

            ret = Utils.PostData(url, data, 
                headers: new Dictionary<string, string>() { { "X-Requested-With", "XMLHttpRequest" },
                    { "CL-TOKEN", token }},
                cookies:new List<Cookie>() {new Cookie(ckey, cvalue, "/", "wechat.bustil.com") },
                accept: "application/json, text/javascript, */*; q=0.01"
                );
            return Utils.Decode(ret);
        }

        private string SetCookie()
        {
            string ret = String.Empty;

            string url = "http://wxcloud.saas.chargerlink.com/s/api?&code=061leZWz0Zi2dg1OvOXz0jAWWz0leZWZ&state=STATE&appid=wx984674c96bd465d0";
            using (WebClient wc = new WebClient())
            {
                wc.DownloadString(url);
                ret = wc.ResponseHeaders["Set-Cookie"];
            }

            return ret;
        }
    }
}

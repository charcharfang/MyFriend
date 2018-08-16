using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace MyFriend.Shop
{
    public class StarCharge : ICharge
    {
        public string GetStationDetail(string staid)
        {
            var ret = GetStationDetailInternal(staid);
            if (ret == "") return "";

            ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            return ret;
        }

        private string GetStationDetailInternal(string staid)
        {
            //Utils.WriteLog("Processing "+staid);
            //string url = "https://wx.starcharge.com/api/xcx.getStubGroup?FRAMEparams=%7B%22stubGroupId%22:%22" + staid + "%22,%22lat%22:36,%22lng%22:117%7D";
            //string data = "{}";
            //var ret = Utils.PostData(url, data);

            //return ret;
            string url = "https://app.starcharge.com/webApi/stubGroup/stubGroupDetailNew";
            //staid = "13c5bd63-af61-48ea-be99-566728d7d06d";
            string data = "FRAMEparams=%7B%22id%22%3A%22" + staid + "%22%2C%22gisType%22%3A%221%22%2C%22lat%22%3A%220%22%2C%22lng%22%3A%220%22%2C%22userId%22%3A%22586df5d4-1a05-4f4f-b94a-1dbe95d6c9e9%22%2C%22tabType%22%3A0%7D";

            try
            {
                var ret = Utils.PostData(url, data
                    //headers:new Dictionary<string, string>() { { "X-Requested-With", "XMLHttpRequest" } },
                    //cookies:new List<Cookie>(){
                    //    new Cookie("SERVERID","b47c9cbf6504664a1b28cd9324d36d5e|1533258390|1533258360","/","app.starcharge.com"),
                    //    new Cookie("JSESSIONID","8D5F2E3B34951EACD2EBA6013FD1F2C8","/","app.starcharge.com")
                    //},
                    //userAgent: "Mozilla/5.0 (iPhone; CPU iPhone OS 11_4_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15G77"
                    );
                return ret;
            }
            catch (Exception ex)
            {
                Utils.WriteLog(staid);
                Utils.WriteLog(ex.Message);
                return "";
            }
        }

        private string GetStationsByCity(string city)
        {
            string url = "https://wx.starcharge.com/api/xcx.findStubGroup?FRAMEparams=%7B%22screenItems%22:%22%22,%22lng%22:36,%22lat%22:117,%22city%22:%22" + city + "%22%7D";
            string data = "{}";
            var ret = Utils.PostData(url, data);

            return ret;
        }

        public string GetStationsFromWx()
        {
            //这个方式，没有城市信息
            //string url = "https://wx.starcharge.com/api/xcx.findStubGroup?FRAMEparams=%7B%22city%22:%22%22,%22screenItems%22:%22%22,%22orderClause%22:0,%22lng%22:117,%22lat%22:36,%22page%22:1,%22pagecount%22:999999%7D";

            string url = "https://wx.starcharge.com/api/weChat.cityList?FRAMEparams=%7B%7D";
            string data = "{}";
            StringBuilder sb = new StringBuilder();

            var ret = Utils.PostData(url, data);
            var citylist = (JsonConvert.DeserializeObject(ret) as JToken)["data"] as JArray;
            foreach (var city in citylist)
            {
                var id = Convert.ToString(city["id"]);
                var name = Convert.ToString(city["name"]);
                var cityinfo = GetStationsByCity(id);

                sb.AppendLine(id);
                sb.AppendLine(name);
                sb.AppendLine(cityinfo);
            }

            return sb.ToString();
        }

        public string[] GetStationsID()
        {
            var lines = File.ReadAllLines("starcharge-stations.txt", Encoding.UTF8);
            //Array.Copy(lines, 1,
            string patch = "starcharge-stations-patch.txt";
            FileInfo fi = new FileInfo(patch);

            DateTime time1 = DateTime.Parse(lines[0]);
            DateTime time2 = fi.LastWriteTime;

            if ((time2 - time1).TotalMinutes > 1)
            {
                var patchtext = File.ReadAllText(patch, Encoding.GetEncoding("gb2312"));
                var array = (JsonConvert.DeserializeObject(patchtext) as JToken)["data"] as JArray;
                List<string> patches = new List<string>();
                foreach (var s in array)
                {
                    var sid = Convert.ToString(s["id"]);
                    if (lines.Contains(sid)) continue;

                    patches.Add(sid);
                }

                if (patches.Count > 0)
                {
                    string[] newlines = new string[lines.Length + patches.Count];
                    newlines[0] = time2.ToString("yyyy-MM-dd HH:mm:ss");

                    Array.Copy(lines, 1, newlines, 1, lines.Length - 1);
                    Array.Copy(patches.ToArray(), 0, newlines, lines.Length, patches.Count);

                    File.WriteAllLines("starcharge-stations.txt", newlines, Encoding.UTF8);

                    return newlines;
                }
            }

            return lines;
        }

        public string GetStations()
        {
            StringBuilder sb = new StringBuilder();
            var stations = GetStationsID();
            foreach (var staid in stations)
            {
                var s = GetStationDetailInternal(staid);
                if (s == "") continue;
                sb.AppendLine(s);
            }

            return sb.ToString();
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            string[] all = bigtext.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int c = 0; c < all.Length; c += 3)
            {
                if (all[c + 0] == "500300")
                {
                    c = c - 1;
                    continue;
                }
                var stations = (JsonConvert.DeserializeObject(all[c + 2]) as JToken)["data"] as JArray;

                for (int i = 0; i < stations.Count; i++)
                {
                    var s = stations[i];
                    list.Add(new List<string>() { all[c + 1], Convert.ToString(s["id"]), Convert.ToString(s["name"]), Convert.ToString(s["address"]), Convert.ToString(s["stubDcCnt"]), Convert.ToString(s["stubAcCnt"]) });
                }
            }
            return list;
        }

        public string Transform2Station(string bigtext, string ts)
        {
            var tuple = Utils.GetUnifiedDataStructureFormatter(UnifiedDataStructure.Station);

            List<List<string>> list = new List<List<string>>();
            StringBuilder sb = new StringBuilder();

            sb.Append(tuple.Item1);
            string[] all = bigtext.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < all.Length; i++)
            {
                var s = (JsonConvert.DeserializeObject(all[i]) as JToken)["data"];
                if (Convert.ToString(s) == "") continue;
                var key = Convert.ToString(s["city"]);

                key = ConvertFuckedCitycode(key);

                var province = Utils.CityMappings[key].Item1;
                var city = Utils.CityMappings[key].Item2;
                var district = Utils.CityMappings[key].Item3;

                var array = s["stubList"] as JArray;
                //int fastkw = 0, slowkw = 0;
                //int nfast = 0, nslow = 0;

                //foreach (var pile in array)
                //{
                //    string name = Convert.ToString(pile["name"]);
                //    int kw = Convert.ToInt32(pile["kw"]);

                //    if (name.Length == 13) kw = kw / 2;

                //    if (kw > 7)
                //    {
                //        fastkw += kw;
                //        nfast++;
                //    }
                //    else
                //    {
                //        slowkw += kw;
                //        nslow++;
                //    }
                //}

                foreach (var pile in array)
                {
                    string name = Convert.ToString(pile["name"]);
                    int kw = Convert.ToInt32(pile["kw"]);

                    if (name.Length == 13) kw = kw / 2;

                    sb.AppendFormat(tuple.Item2,
                        "星星充电",//APP名称
                        Convert.ToString(s["operatorId"]) == "" ? "星星充电" : Convert.ToString(s["operatorId"]),//运营商
                        Convert.ToString(s["id"]),//电站编号
                        Convert.ToString(s["name"]),//电站名称
                        province,//省
                        city,//市
                        district,//区
                        Convert.ToString(s["address"]),//电站地址
                        GetStubGroupType(Convert.ToString(s["stubGroupType"])),//电站类型
                        GetOperationType(Convert.ToString(s["type"])),//运营类型
                        Convert.ToString(s["serviceTime"]),//运营时间
                        "",//电话
                        Convert.ToString(s["gisGcj02Lng"]),//经度
                        Convert.ToString(s["gisGcj02Lat"]),//纬度
                        0,//Convert.ToString(fastkw + slowkw),//总功率
                        (kw>7)?kw:0,//快充总功率
                        (kw<=7)?kw:0,//慢充总功率
                        (kw>7)?1:0,
                        (kw<=7)?1:0,
                        "",//电费
                        "",//服务费
                        Convert.ToString(s["totalFeeInfo"]),//#总费用
                        Convert.ToString(s["parkingFeeInfo"]),//停车
                        Convert.ToString(s["parkingInfo"]),//指引
                        Convert.ToString(s["gisGcj02Lng"]),//BD经度
                        Convert.ToString(s["gisGcj02Lat"]),//BD纬度
                        Convert.ToString(s["scoreAverage"]),//电站评分
                        "",//标签
                        Convert.ToString(s["paymentType"]),//支付方式,

                        Convert.ToString(pile["id"]),//桩编号
                        Convert.ToString(pile["name"]),//桩名称
                        Convert.ToString(pile["type"]),//桩类型
                        Convert.ToString(pile["status"]),//桩状态
                        Convert.ToString(pile["modelNo"]),//桩型号
                        Convert.ToString(pile["ratedCurrent"]),//额定电流
                        Convert.ToString(pile["kw"]),//功率
                        Convert.ToString(pile["voltageUpperLimit"]),//电压上限
                        Convert.ToString(pile["voltageLowerLimit"]),//电压下限
                        Convert.ToString(pile["voltageAuxiliary"])//辅源
                    );
                }
            }

            return sb.ToString();
        }

        private object GetOperationType(string type)
        {
            if (type == "") return "自营";
            else return type;
        }

        private string GetStubGroupType(string type)
        {
            if (type == "0") return "公共站";
            else if (type == "2") return "专用站";
            else return type;
        }

        private string ConvertFuckedCitycode(string key)
        {
            //傻逼星星，这么多不规范的编码！
            //法克鱿！你娘的用的哪年的编码表？
            if (key == "110200") key = "110100";
            else if (key == "120200") key = "120100";
            else if (key == "120107") key = "120116";
            else if (key == "310200") key = "310151";
            else if (key == "500300") key = "500200";
            else if (key == "341400") key = "340181";
            else if (key == "522200") key = "520600";
            else if (key == "522400") key = "520500";
            else if (key == "542100") key = "540300";
            else if (key == "542200") key = "540500";
            else if (key == "542300") key = "540200";
            else if (key == "542600") key = "540400";
            else if (key == "632100") key = "630200";
            else if (key == "652100") key = "650400";
            else if (key == "652200") key = "650500";
            else if (key == "810100") key = "810000";
            else if (key == "820100") key = "820000";
            else if (key == "441901") key = "441900";
            else if (key == "320482") key = "320413";
            else if (key == "310230") key = "310115";
            else if (key == "120109") key = "120116";
            else if (key == "320504") key = "320508";
            else if (key == "320502") key = "320508";
            else if (key == "320125") key = "320118";
            else if (key == "320124") key = "320117";
            else if (key == "320721") key = "320707";
            else if (key == "320584") key = "320509";
            else if (key == "110228") key = "110108";
            else if (key == "110229") key = "110119";
            else if (key == "320705") key = "320706";
            else if (key == "360122") key = "360112";
            else if (key == "321190") key = "321102";
            else if (key == "321088") key = "321012";
            else if (key == "320390") key = "320303";
            else if (key == "320323") key = "320312";
            else if (key == "532621") key = "532601";
            else if (key == "469035") key = "469029";
            else if (key == "320503") key = "320508";
            else if (key == "511821") key = "511803";
            else if (key == "320405") key = "320412";
            else if (key == "522401") key = "520502";

            else if (key == "130603") key = "130606";
            else if (key == "120108") key = "120116";
            else if (key == "330682") key = "330604";
            else if (key == "469033") key = "469027";
            else if (key == "360782") key = "360703";
            else if (key == "510122") key = "510116";
            else if (key == "532526") key = "532504";
            else if (key == "512081") key = "510185";
            else if (key == "610126") key = "610117";
            else if (key == "469034") key = "469028";
            else if (key == "410307") key = "410311";
            else if (key == "442001") key = "442000";
            else if (key == "330183") key = "330111";
            else if (key == "450122") key = "450110";
            else if (key == "441421") key = "441403";
            else if (key == "430122") key = "430112";
            else if (key == "810107") key = "810007";
            else if (key == "370802") key = "370801";
            else if (key == "320811") key = "320812";
            else if (key == "330621") key = "330601";

            return key;
        }
        public string Transform2Order(string ts, string filter)
        {
            //Filter这里不用
            var tuple = Utils.GetUnifiedDataStructureFormatter(UnifiedDataStructure.Order);

            List<List<string>> list = new List<List<string>>();
            StringBuilder sb = new StringBuilder();

            sb.Append(tuple.Item1);
            var bigtext = GetStations();

            string[] all = bigtext.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < all.Length; i++)
            {
                var s = (JsonConvert.DeserializeObject(all[i]) as JToken)["data"];
                if (Convert.ToString(s) == "") continue;
                var key = Convert.ToString(s["city"]);

                key = ConvertFuckedCitycode(key);

                var province = Utils.CityMappings[key].Item1;
                var city = Utils.CityMappings[key].Item2;
                var district = Utils.CityMappings[key].Item3;

                var array = s["stubList"] as JArray;

                foreach (var pile in array)
                {
                    var stub = pile["stubInfo"];
                    if (Convert.ToString(stub["orderId"]) == "") continue;

                    sb.AppendFormat(tuple.Item2,
                        ts,//时间戳`
                        "星星充电",//APP名称
                        Convert.ToString(s["operatorId"]) == "" ? "星星充电" : Convert.ToString(s["operatorId"]),//运营商
                        Convert.ToString(s["id"]),//电站编号
                        Convert.ToString(pile["id"]),//桩编号
                        Convert.ToString(stub["currentO"]),//实际电流
                        Convert.ToString(stub["voltageO"]),//实际电压
                        Convert.ToString(stub["chargeSoc"]),//SOC
                        Convert.ToString(stub["tempCar"]),//电池温度
                        Convert.ToString(stub["tempStub"]),//桩温度
                        Convert.ToString(stub["tempGun"]),//枪温度
                        Convert.ToString(stub["chargePower"]),//电量
                        Convert.ToString(stub["orderId"]),//订单编号
                        Convert.ToString(stub["orderId2"]),//订单编号2
                        Convert.ToString(stub["electric"]),//电表值
                        Convert.ToString(stub["electricTime"]),//电表时间
                        Convert.ToString(stub["chargeStartTime"])//开始充电时间
                    );
                }
            }

            return sb.ToString();
        }
    }
}

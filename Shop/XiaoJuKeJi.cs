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
    public class XiaoJuKeJi : ICharge
    {
        public string GetStationDetail(string staid)
        {
            //GetAvailableCityList();
            //貌似只返回前20个订单
            string url = "https://epower.xiaojukeji.com/station-api/station/getoneinfo?fullstationid=" + staid + "&token=hfa8PUlXINatfs78erJtpS_l1FevoIAKcH-1t1vHyackzDmKw0AQQNG7_LgQtaik7konnzvMIi9JG2wcCd_dCMf_83aGUsSkkyIMo0wYTrmqqjCCsjW7Z3TVltGEMVNHSoqvb4QfCoRfKpvm7L6Ypa2tCf9UFzZq53F73v82Sl_CibIMj97MZuFMYdlUIxb3QLh8yOuxvwMAAP__&lat=32.020966&lng=118.743457&userlat=31.26565088106128&userlng=121.52128222412895";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var ret = wc.DownloadString(url);
                return JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);
            }
        }

        public string GetStations()
        {
            //List<double> gpsdata = new List<double>(){116.41667, 39.91667,121.43333, 34.50000,117.20000, 39.13333,114.10000, 22.20000,113.23333, 23.16667,120.20000, 30.26667,106.45000, 29.56667,
            //    119.30000, 26.08333,103.73333, 36.03333,106.71667, 26.56667,113.00000, 28.21667,118.78333, 32.05000,115.90000, 28.68333,123.38333, 41.80000,112.53333, 37.86667,104.06667, 30.66667,
            //    91.00000, 29.60000,87.68333, 43.76667,102.73333, 25.05000,108.95000, 34.26667,101.75000, 36.56667,106.26667, 38.46667,126.63333, 45.75000,125.35000, 43.88333,114.31667, 30.51667,
            //    113.65000, 34.76667,114.48333, 38.03333,110.35000, 20.01667,113.50000, 22.20000,117.17,31.52,108.19,22.48,114.17,30.35,111.41,40.48,117.00,36.40
            //};
            var gpsdata = GetAvailableCityList();

            StringBuilder sb = new StringBuilder();
            JArray alldata = new JArray();
            for (int i = 0; i < gpsdata.Count; i += 2)
            {
                string url = "https://epower.xiaojukeji.com/station-api/station/querylist";
                string data = "{\"lat\":" + gpsdata[i + 1].ToString() + ",\"lng\":" + gpsdata[i].ToString() + ",\"type\":2,\"distance\":999,\"chargeTypeFast\":0,\"chargeTypeSlow\":0,\"undergroundTop\":0,\"undergroundBottom\":0,\"park\":0,\"shop\":0,\"bathroom\":0,\"canopy\":0,\"keeper\":0,\"recommend\":0,\"lounge\":0,\"owner\":\"map\",\"chargeType\":null,\"underground\":null,\"ownerChange\":false,\"token\":\"hfa8PUlXINatfs78erJtpS_l1FevoIAKcH-1t1vHyackzDmKw0AQQNG7_LgQtaik7konnzvMIi9JG2wcCd_dCMf_83aGUsSkkyIMo0wYTrmqqjCCsjW7Z3TVltGEMVNHSoqvb4QfCoRfKpvm7L6Ypa2tCf9UFzZq53F73v82Sl_CibIMj97MZuFMYdlUIxb3QLh8yOuxvwMAAP__\",\"userlat\":39.90449873823792,\"userlng\":116.4069765424048}";

                var ret = Utils.PostData(url, data,
                    cookies: new List<Cookie>() {
                    new Cookie("omgh5sid","276033450616-1531739609058","/", "epower.xiaojukeji.com"),
                    new Cookie("omgtrc","7854580928785817033","/", "epower.xiaojukeji.com"),
                    new Cookie("ticket","hfa8PUlXINatfs78erJtpS_l1FevoIAKcH-1t1vHyackzDmKw0AQQNG7_LgQtaik7konnzvMIi9JG2wcCd_dCMf_83aGUsSkkyIMo0wYTrmqqjCCsjW7Z3TVltGEMVNHSoqvb4QfCoRfKpvm7L6Ypa2tCf9UFzZq53F73v82Sl_CibIMj97MZuFMYdlUIxb3QLh8yOuxvwMAAP__","/", "epower.xiaojukeji.com"),
                    },
                    headers: new Dictionary<string, string>() { { "X-Requested-With", "com.sdu.didi.gsui" } },
                    userAgent: "Mozilla/5.0 (Linux; Android 4.4.4; MuMu Build/V417IR) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/33.0.0.0 Safari/537.36 FusionKit/2.0.0_didigsui_900_1600_MuMu-cancro_19_4.4.4_5.1.12_303",
                    accept: "application/json, text/plain, */*",
                    contentType: "application/json;charset=UTF-8"

                    );

                var tmpdata = (JsonConvert.DeserializeObject(ret) as JToken)["data"] as JArray;
                if (tmpdata != null)
                {
                    foreach (var tmp in tmpdata) alldata.Add(tmp);
                    sb.AppendLine(ret);
                }
            }

            var list = alldata.GroupBy(j => j["id"].ToString()).Select(j => j.First()).ToList();
            var bigtext = JsonConvert.SerializeObject(list);
            GetSomething(bigtext);

            return bigtext;
        }



        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken) as JArray;
            var opelist = GetOperators();

            for (int i = 0; i < stations.Count; i++)
            {
                var staid = stations[i]["fullStationId"].ToString();
                var staname = stations[i]["stationName"].ToString();
                var star = String.Format("{0:N2}", Convert.ToDouble(stations[i]["star"].ToString()));
                var fastnum = stations[i]["fastChargeNum"].ToString();
                var slownum = stations[i]["slowChargeNum"].ToString();
                var salesprice = String.Format("{0:N2}", Convert.ToDouble(stations[i]["totalSalePrice"].ToString()));
                var labelname = stations[i]["labelNames"].ToString().Replace("[]", "").Replace("\r\n", "").Replace("[", "").Replace("]", "").Replace("\"", "");

                string ope = String.Empty;
                var f = opelist.Where(o => staid.StartsWith(o.Key)).ToList();
                if (f.Count < 1) ope = "【未知】"; else ope = f[0].Value;

                list.Add(new List<string>() { staid, staname, ope, star, fastnum, slownum, salesprice, labelname });
            }

            return list;
        }

        private Dictionary<string, string> GetOperators()
        {
            string url = "https://epower.xiaojukeji.com/station-api/station/getoperators";
            string data = "{}";
            Dictionary<string, string> opelist = new Dictionary<string, string>();

            var ret = Utils.PostData(url, data);
            var operators = (JsonConvert.DeserializeObject(ret) as JToken)["data"] as JArray;
            foreach(var ope in operators)
            {
                opelist.Add(Convert.ToString(ope["operatorId"]), Convert.ToString(ope["operatorName"]));
            }

            return opelist;
            #region 运营商字典
            

            opelist.Add("395815801", "特来电");
            opelist.Add("101437000", "小桔充电");
            opelist.Add("MA1MY0GF9", "云快充");
            opelist.Add("321895837", "万马快充");
            opelist.Add("MA1GBBAR8", "顺充");
            opelist.Add("320513112", "依威能源");
            opelist.Add("339767534", "智充");
            opelist.Add("101437375", "富电");
            opelist.Add("074842972", "充电队长");
            opelist.Add("MA0019FW4", "云杉");
            opelist.Add("MA1FP0228", "安悦");
            opelist.Add("MA003WA12", "高陆通");
            opelist.Add("MA4KLMAK4", "哪儿充");
            opelist.Add("914401153", "劲桩");
            opelist.Add("MA59J8YL8", "万城万充");
            opelist.Add("565843400", "普天");
            opelist.Add("726811355", "鹏辉");
            opelist.Add("A5DM667X3", "致联");
            opelist.Add("554443284", "智网");
            opelist.Add("MA5AKE4G9", "绿盈新能源");
            opelist.Add("MA59KMRQ8", "绿盈");
            #endregion 运营商字典

            return opelist;
        }
        private void GetSomething(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken) as JArray;

            var opelist = GetOperators();

            StringBuilder sbStation = new StringBuilder();
            StringBuilder sbOrder = new StringBuilder();
            sbStation.AppendLine("经度,纬度,编号,电站,运营商,评分,快充,慢充,收费,标签,便利店,卫生间,有人值守,简餐,雨棚,休息室,停车费,开放时间");
            sbOrder.AppendLine("城市,经度,纬度,编号,电站,运营商,日期,品牌,车型");

            for (int i = 0; i < stations.Count; i++)
            {
                var staid = stations[i]["fullStationId"].ToString();
                string ope = String.Empty;


                var f = opelist.Where(o => staid.StartsWith(o.Key+"_")).ToList();
                if (f.Count < 1) ope = "【未知】"; else ope = f[0].Value;

                var staname = stations[i]["stationName"].ToString();
                var star = String.Format("{0:N2}", Convert.ToDouble(stations[i]["star"].ToString()));
                var fastnum = stations[i]["fastChargeNum"].ToString();
                var slownum = stations[i]["slowChargeNum"].ToString();
                var salesprice = String.Format("{0:N2}", Convert.ToDouble(stations[i]["totalSalePrice"].ToString()));
                var labelname = stations[i]["labelNames"].ToString().Replace("[]", "").Replace("\r\n", "").Replace("[", "").Replace("]", "").Replace("\"", "");

                list.Add(new List<string>() { staid, staname, star, fastnum, slownum, ope, salesprice, labelname });

                var lat = Convert.ToDouble(stations[i]["lat"].ToString());
                var lng = Convert.ToDouble(stations[i]["lng"].ToString());
                sbStation.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}\r\n",
                    lat, lng, staid, staname, ope, star, fastnum, slownum, salesprice, labelname
                    );
                //var detail = GetStationDetail(staid);
                //var detailJson = (JsonConvert.DeserializeObject(detail) as JToken)["data"] as JToken;

                //var ccode = detailJson["cityAreacode"].ToString();
                //var shop = Convert.ToString(detailJson["shop"]);//便利店
                //var bathroom = Convert.ToString(detailJson["bathroom"]);//卫生间
                //var keeper = Convert.ToString(detailJson["keeper"]);//有人值守
                //var restaurant = Convert.ToString(detailJson["restaurant"]);//简餐
                //var rainshed = Convert.ToString(detailJson["rainshed"]);//雨棚
                //var lounge = Convert.ToString(detailJson["lounge"]);//休息室
                //var park = detailJson["parkDesc"].ToString().Replace("\n", ",");
                ////var opentime = detailJson["openTime"].ToString();

                //sbStation.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}\r\n",
                //    ccode, lat, lng,staid, staname, ope, star, fastnum, slownum, salesprice, labelname, shop, bathroom,
                //    keeper, restaurant, rainshed, lounge, park
                //    );

                //var rec = detailJson["chargeRecords"] as JArray;
                //foreach (var cr in rec)
                //{
                //    sbOrder.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8}\r\n",
                //        ccode, lat, lng,staid, staname, ope, cr["latestChargeTime"].ToString(), cr["brandName"].ToString(), cr["modelName"].ToString()
                //        );
                //}

                System.Diagnostics.Debug.WriteLine("Processing " + staname);
            }

            File.WriteAllText("c:\\temp\\stations.csv", sbStation.ToString(), Encoding.UTF8);
            //File.WriteAllText("c:\\temp\\orders.csv", sbOrder.ToString(), Encoding.UTF8);

        }

        private List<double> GetAvailableCityList()
        {
            List<double> ret = new List<double>();
            var alllines = File.ReadAllLines("gpsinfo.csv", Encoding.UTF8);
            Dictionary<string, Tuple<double, double>> mappings = new Dictionary<string, Tuple<double, double>>();
            for(int i = 1; i < alllines.Length; i++)
            {
                var tmp = alllines[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (mappings.ContainsKey(tmp[2])) continue;

                mappings.Add(tmp[2], Tuple.Create<double, double>(Convert.ToDouble(tmp[3]), Convert.ToDouble(tmp[4])));
            }

            //https://common.diditaxi.com.cn/general/webEntry/citylist?productid=257
            //https://as.xiaojukeji.com/ep/as/feature?name=entrance_admin&key=hfa8PUlXINatfs78erJtpS_l1FevoIAKcH-1t1vHyackzDmKw0AQQNG7_LgQtaik7konnzvMIi9JG2wcCd_dCMf_83aGUsSkkyIMo0wYTrmqqjCCsjW7Z3TVltGEMVNHSoqvb4QfCoRfKpvm7L6Ypa2tCf9UFzZq53F73v82Sl_CibIMj97MZuFMYdlUIxb3QLh8yOuxvwMAAP__&phone=13911112222&city=1&ns=service&_t=1532403307795&_source=jssdk
            string url = "https://common.diditaxi.com.cn/general/webEntry/citylist?productid=257";
            Dictionary<int, string> citilist = new Dictionary<int, string>();
            //List<string> availables = new List<string>();

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var json = wc.DownloadString(url);
                var list = (JsonConvert.DeserializeObject(json) as JToken)["groups"] as JArray;
                foreach (var clist in list)
                {
                    foreach (var city in clist["cities"] as JArray)
                    {
                        int cid = Convert.ToInt32(city["cityid"]);
                        if (false == citilist.ContainsKey(cid)) citilist.Add(cid, Convert.ToString(city["name"]));
                    }
                }

            }
            foreach (var cid in citilist.Keys)
            {
                url = "https://as.xiaojukeji.com/ep/as/feature?name=entrance_admin&key=hfa8PUlXINatfs78erJtpS_l1FevoIAKcH-1t1vHyackzDmKw0AQQNG7_LgQtaik7konnzvMIi9JG2wcCd_dCMf_83aGUsSkkyIMo0wYTrmqqjCCsjW7Z3TVltGEMVNHSoqvb4QfCoRfKpvm7L6Ypa2tCf9UFzZq53F73v82Sl_CibIMj97MZuFMYdlUIxb3QLh8yOuxvwMAAP__&phone=13911112222&city=" + cid.ToString() + "&ns=service&_t=1532403307795&_source=jssdk";
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    var json = wc.DownloadString(url);
                    var allow = Convert.ToString((JsonConvert.DeserializeObject(json) as JToken)["data"][0]["allow"]).ToLower().Trim();
                    if (allow == "true")
                    {
                        var cinfo = mappings.Where(city => city.Key == citilist[cid]).ToList();
                        if (cinfo.Count > 0)
                        {
                            ret.Add(cinfo[0].Value.Item1);
                            ret.Add(cinfo[0].Value.Item2);
                        }
                        //availables.Add(citilist[cid]);
                    }
                }
            }

            return ret;
        }
    }
}

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
            //貌似只返回前20个订单
            string url = "https://epower.xiaojukeji.com/station-api/station/getoneinfo?fullstationid=" + staid + "&token=wP-B1I7diV3rll6CFnh0k0eQjLY8zdcWCS6J-8bwQGkkzDmKw0AQQNG7_LgQtaharUonnzvMIi9JG2wcCd_dCMf_83aGUsSkkyIMo0wYTrmqqjCCsiVXz1hVe0YXxkwdKSm-vhF-KBB-qeyas3szS1t6F_6pJmzUzuP2vP9tlL6EE2UZ1nw2NeFMYdlVI5p7IFw-5PXY3wEAAP__&lat=32.020966&lng=118.743457&userlat=31.26565088106128&userlng=121.52128222412895";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var ret = wc.DownloadString(url);
                return JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);
            }
        }

        public string GetStations()
        {
            List<double> gpsdata = new List<double>(){116.41667, 39.91667,121.43333, 34.50000,117.20000, 39.13333,114.10000, 22.20000,113.23333, 23.16667,120.20000, 30.26667,106.45000, 29.56667,
                119.30000, 26.08333,103.73333, 36.03333,106.71667, 26.56667,113.00000, 28.21667,118.78333, 32.05000,115.90000, 28.68333,123.38333, 41.80000,112.53333, 37.86667,104.06667, 30.66667,
                91.00000, 29.60000,87.68333, 43.76667,102.73333, 25.05000,108.95000, 34.26667,101.75000, 36.56667,106.26667, 38.46667,126.63333, 45.75000,125.35000, 43.88333,114.31667, 30.51667,
                113.65000, 34.76667,114.48333, 38.03333,110.35000, 20.01667,113.50000, 22.20000,117.17,31.52,108.19,22.48,114.17,30.35,111.41,40.48,117.00,36.40
            };

            StringBuilder sb = new StringBuilder();
            JArray alldata = new JArray();
            for (int i = 0; i < gpsdata.Count; i += 2)
            {
                string url = "https://epower.xiaojukeji.com/station-api/station/querylist";
                string data = "{\"lat\":" + gpsdata[i + 1].ToString() + ",\"lng\":" + gpsdata[i].ToString() + ",\"type\":2,\"distance\":999,\"chargeTypeFast\":0,\"chargeTypeSlow\":0,\"undergroundTop\":0,\"undergroundBottom\":0,\"park\":0,\"shop\":0,\"bathroom\":0,\"canopy\":0,\"keeper\":0,\"recommend\":0,\"lounge\":0,\"owner\":\"map\",\"chargeType\":null,\"underground\":null,\"ownerChange\":false,\"token\":\"VEh0V6m5Jpw40u_Kz7kfyvid9vx2TCo78mJKY1eAFqJMxzkOglAQgOGrmL-eYhYGXuY2KLhUGF6MBeHutpbfwUyBcKWyaQ7uo1na1JqwUIOwUseFvn3220qp8J57_277QlmG5Wgx-Snc_y08KCybasToHghPCkV4UXr-AgAA__8=\",\"userlat\":39.90449873823792,\"userlng\":116.4069765424048}";

                var ret = Utils.PostData(url, data,
                    cookies: new List<Cookie>() {
                    new Cookie("omgh5sid","970523391850-1531561550832","/", "epower.xiaojukeji.com"),
                    new Cookie("omgtrc","-1910293759674151314","/", "epower.xiaojukeji.com"),
                    new Cookie("ticket","VEh0V6m5Jpw40u_Kz7kfyvid9vx2TCo78mJKY1eAFqJMxzkOglAQgOGrmL-eYhYGXuY2KLhUGF6MBeHutpbfwUyBcKWyaQ7uo1na1JqwUIOwUseFvn3220qp8J57_277QlmG5Wgx-Snc_y08KCybasToHghPCkV4UXr-AgAA__8=","/", "epower.xiaojukeji.com"),
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
            return JsonConvert.SerializeObject(list);
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

                list.Add(new List<string>() { staid, staname, ope,star, fastnum, slownum, salesprice, labelname });
            }

            return list;
        }

        private Dictionary<string,string> GetOperators()
        {
            #region 运营商字典
            Dictionary<string, string> opelist = new Dictionary<string, string>();

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
            sbStation.AppendLine("城市,经度,纬度,电站,运营商,评分,快充,慢充,收费,标签,便利店,卫生间,有人值守,简餐,雨棚,休息室,停车费,开放时间");
            sbOrder.AppendLine("城市,经度,纬度,电站,运营商,日期,品牌,车型");

            for (int i = 0; i < stations.Count; i++)
            {
                var staid = stations[i]["fullStationId"].ToString();
                string ope = String.Empty;


                var f = opelist.Where(o => staid.StartsWith(o.Key)).ToList();
                if (f.Count < 1) ope = "【未知】"; else ope = f[0].Value;

                var staname = stations[i]["stationName"].ToString();
                var star = String.Format("{0:N2}", Convert.ToDouble(stations[i]["star"].ToString()));
                var fastnum = stations[i]["fastChargeNum"].ToString();
                var slownum = stations[i]["slowChargeNum"].ToString();
                var salesprice = String.Format("{0:N2}", Convert.ToDouble(stations[i]["totalSalePrice"].ToString()));
                var labelname = stations[i]["labelNames"].ToString().Replace("[]", "").Replace("\r\n", "").Replace("[", "").Replace("]", "").Replace("\"", "");

                list.Add(new List<string>() { staid, staname, star, fastnum, slownum, ope, salesprice, labelname });

                var detail = GetStationDetail(staid);
                var detailJson = (JsonConvert.DeserializeObject(detail) as JToken)["data"] as JToken;
                var lat = Convert.ToDouble(detailJson["lat"].ToString());
                var lng = Convert.ToDouble(detailJson["lng"].ToString());
                var ccode = detailJson["cityAreacode"].ToString();
                var shop = Convert.ToString(detailJson["shop"]);//便利店
                var bathroom = Convert.ToString(detailJson["bathroom"]);//卫生间
                var keeper = Convert.ToString(detailJson["keeper"]);//有人值守
                var restaurant = Convert.ToString(detailJson["restaurant"]);//简餐
                var rainshed = Convert.ToString(detailJson["rainshed"]);//雨棚
                var lounge = Convert.ToString(detailJson["lounge"]);//休息室
                var park = detailJson["parkDesc"].ToString();
                //var opentime = detailJson["openTime"].ToString();

                sbStation.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}\r\n",
                    ccode, lat, lng, staname, ope, star, fastnum, slownum, salesprice, labelname, shop, bathroom,
                    keeper, restaurant, rainshed, lounge, park
                    );

                var rec = detailJson["chargeRecords"] as JArray;
                foreach (var cr in rec)
                {
                    sbOrder.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}\r\n",
                        ccode, lat, lng, staname, ope, cr["latestChargeTime"].ToString(), cr["brandName"].ToString(), cr["modelName"].ToString()
                        );
                }

                System.Diagnostics.Debug.WriteLine("Processing " + staname);
            }

            File.WriteAllText("c:\\temp\\stations.csv", sbStation.ToString(), Encoding.UTF8);
            File.WriteAllText("c:\\temp\\orders.csv", sbOrder.ToString(), Encoding.UTF8);
            
        }
    }
}

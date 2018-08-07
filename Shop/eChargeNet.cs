using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace MyFriend.Shop
{
    public class eChargeNet : ICharge
    {
        private string GetStationDetailInternal(string staid)
        {
            var ret = String.Empty;

            using (WebClient wc = new WebClient())
            {
                byte[] buf = wc.DownloadData("http://www.echargenet.com:9003/portal/charge-total/rest/plug/getChargerList?plugId=" + staid);
                ret = Encoding.UTF8.GetString(buf);
            }
            return ret;
        }

        public string GetStationDetail(string staid)
        {
            var ret = GetStationDetailInternal(staid);
            ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);

            return Utils.Decode(ret);
        }

        public string GetCorrespondingStations(string name)
        {
            string fname = Utils.PATH + Guid.NewGuid().ToString().Replace("-", "") + ".zip";
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile("http://ecmyappfs.echargenet.com/chargelink-file/file-server/zip/" + name + ".zip", fname);
            }

            if (File.Exists(Utils.PATH + name + ".txt"))
            {
                File.Delete(Utils.PATH + name + ".txt");
            }
            ZipFile.ExtractToDirectory(fname, "data");
            File.Delete(fname);

            string ret = File.ReadAllText(Utils.PATH + name + ".txt");
            return ret;
        }

        public string GetStations()
        {
            var ret1 = GetCorrespondingStations("echargenet");
            var ret2 = GetCorrespondingStations("echargenet_third");

            var stations1 = (JsonConvert.DeserializeObject(ret1) as JToken) as JArray;
            var stations2 = (JsonConvert.DeserializeObject(ret2) as JToken) as JArray;

            JArray array = new JArray();

            foreach (var s in stations1)
            {
                array.Add(s);
            }
            foreach (var s in stations2)
            {
                array.Add(s);
            }

            var ret = JsonConvert.SerializeObject(array);
            return ret;
        }

        private string GetOperatorName(int type, string comname)
        {
            var ope2 = Enum.GetName(typeof(OperatorTypes), type);
            var name = "";
            if (ope2 == null)
            {
                name = type.ToString();
                System.Diagnostics.Debug.WriteLine(String.Format("{0}-{1}", type, comname));
            }
            else
            {
                name = ope2;
            }

            return name;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken) as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                var name = GetOperatorName(Convert.ToInt32(stations[i]["operatorTypes"]), stations[i]["company"].ToString());

                list.Add(new List<string>() { stations[i]["id"].ToString(), stations[i]["province"].ToString(), stations[i]["city"].ToString(), stations[i]["company"].ToString(), name, stations[i]["quantity"].ToString() });
            }

            return list;
        }

        public string Transform2Station(string bigtext, string ts)
        {
            var tuple = Utils.GetUnifiedDataStructureFormatter(UnifiedDataStructure.Station);

            List<List<string>> list = new List<List<string>>();
            StringBuilder sb = new StringBuilder();

            sb.Append(tuple.Item1);

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken) as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                var s = stations[i];
                int fastkw = 0, slowkw = 0;
                int nfast = 0, nslow = 0;

                var detail = GetStationDetailInternal(Convert.ToString(s["id"]));
                var array = (JsonConvert.DeserializeObject(detail) as JToken)["data"] as JArray;
                if (array == null)
                {
                    Utils.WriteLog(JsonConvert.SerializeObject(s));
                    continue;
                }
                string chargedesc = "", payinfo = "", area = "";
                foreach (var pilegroup in array)
                {
                    foreach (var pile in pilegroup["equipments"])
                    {
                        int p = Convert.ToInt32(Convert.ToDouble(pile["outPower"]));
                        if (p > 7)
                        {
                            fastkw += p;
                            nfast++;
                        }
                        else
                        {
                            slowkw += p;
                            nslow++;
                        }

                        chargedesc = Convert.ToString(pile["chargeDesc"]);
                        payinfo = Convert.ToString(pile["payInfo"]);
                        area = Convert.ToString(pile["area"]);
                    }
                }
                int opetype = Convert.ToInt32(s["operatorTypes"]);

                sb.AppendFormat(tuple.Item2,
                        ts,//时间戳
                        "e充电",//APP名称
                        GetOperatorName(opetype, s["company"].ToString()),//运营商
                        Convert.ToString(s["id"]),//电站编号
                        Convert.ToString(s["company"]),//电站名称
                        s["province"],//省
                        s["city"],//市
                        "",//区
                        Convert.ToString(s["address"]),//电站地址
                        "",//电站类型
                        (opetype==1)?"自营":"非自营",//运营类型
                        "",//运营时间
                        "",//电话
                        Convert.ToString(s["lng"]),//经度
                        Convert.ToString(s["lat"]),//纬度
                        Convert.ToString(fastkw + slowkw),//总功率
                        Convert.ToString(fastkw),//快充总功率
                        Convert.ToString(slowkw),//慢充总功率
                        Convert.ToString(nfast),//快充个数
                        Convert.ToString(nslow),//慢充个数
                        "",//电费
                        "",//服务费
                        chargedesc,//#总费用
                        "",//停车
                        area,//指引
                        "",//BD经度
                        "",//BD纬度
                        "",//电站评分
                        "",//标签
                        payinfo//支付方式
                    );
            }

            return sb.ToString();
        }

        private enum OperatorTypes
        {
            未知 = 0,
            国家电网 = 1,
            特来电 = 4,
            星星充电 = 5,
            山东鲁能智能 = 9,
            南京运监 = 11,
            富电 = 13,
            聚电 = 17,
        }

        private enum PayType
        {
            国网充电卡 = 1,
            二维码 = 2,
            账户余额 = 4,
        }

        //OperatorType:1国网4特来电5星星充电9山东鲁能智能11南京运监
    }
}

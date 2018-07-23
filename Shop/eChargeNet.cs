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
        public string GetStationDetail(string staid)
        {
            var ret = String.Empty;

            using (WebClient wc = new WebClient())
            {
                byte[] buf = wc.DownloadData("http://www.echargenet.com:9003/portal/charge-total/rest/plug/getChargerList?plugId=" + staid);
                ret = Encoding.UTF8.GetString(buf);

                ret = JToken.Parse(ret).ToString(Newtonsoft.Json.Formatting.Indented);
            }

            return Utils.Decode(ret);
        }

        public string GetStations()
        {
            string fname = Utils.PATH + Guid.NewGuid().ToString().Replace("-", "") + ".zip";
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile("http://ecmyappfs.echargenet.com/chargelink-file/file-server/zip/echargenet.zip", fname);
            }

            if (File.Exists(Utils.PATH + "echargenet.txt"))
            {
                File.Delete(Utils.PATH + "echargenet.txt");
            }
            ZipFile.ExtractToDirectory(fname, "data");
            File.Delete(fname);

            string ret = File.ReadAllText(Utils.PATH + "echargenet.txt");
            return ret;
        }

        public List<List<string>> GetWellFormattedStations(string bigtext)
        {
            List<List<string>> list = new List<List<string>>();

            var stations = (JsonConvert.DeserializeObject(bigtext) as JToken) as JArray;

            for (int i = 0; i < stations.Count; i++)
            {
                list.Add(new List<string>() { stations[i]["id"].ToString(), stations[i]["province"].ToString(), stations[i]["city"].ToString(), stations[i]["company"].ToString(),stations[i]["address"].ToString(), stations[i]["quantity"].ToString() });
            }

            return list;
        }
    }
}

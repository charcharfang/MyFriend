using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;

namespace StarChargeTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            tbPatchedStations.Text = "";

            string cookie = Utils.GetCookie("https://app.starcharge.com/stubGroup/stubGroupDetailNew.do?appLat=0&accountId=1890f5d4-1c05-4f4f-b64a-1dba3dd789e9&distance=0&id=be795a70-ecd7-4025-b43a-212b22704116&appLng=0&stubType=0&tabType=0&gisType=1");
            var clist = cookie.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var acwtc = clist[0].Split(new char[] { '=' })[1];//有效期1年

            string url = "https://app.starcharge.com/api/stubGroup/stubGroupList";
            string data = "deviceid=DAA4B1F2-135C-012-A92B-A642B407DD7E&lastQueryTime=2018.08.01%2000%3A00%3A00";
            string acwsc = "";

            for (int i = 0; i < 3; i++)
            {
                string ret = Utils.PostData(url, data,
                    cookies: new List<Cookie>()
                    {
                    new Cookie("SERVERID","63594c5f9b4d6ad9d7acb6d5e4d99558|1535971303|1535971303","/","app.starcharge.com"),
                    new Cookie("acw_tc",acwtc,"/","app.starcharge.com"),
                    },
                    headers: new Dictionary<string, string>()
                    {
                    {"X-Ca-Signature",tbSignature.Text }
                    }

                    );

                if (ret.StartsWith("<html>"))
                {
                    string arg1 = Utils.GetPartFromString(ret, "arg1='", "'");
                    string tmp = acwsc;
                    acwsc = Utils.GetACWSC(arg1);
                    Utils.WriteLog(String.Format("ACWSC变更成功，从{0}->{1}", tmp, acwsc));

                    continue;
                }
            }
        }        
    }
}

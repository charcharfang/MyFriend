using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Net;
using Newtonsoft.Json;

namespace MyFriend.Friend
{
    class Program
    {
        static Dictionary<int, ShopItem> shoplist;
        static int curMainPage = 0;
        static int curSubPage = 0;
        static int rowsPerPage = 20;
        static string cmd = "";
        static string app = "";
        static string type = "";


        static void Main(string[] args)
        {

            if (!IsValidArgument(args, out cmd, out app, out type)) return;

            InitAll();
            switch (cmd)
            {
                case "list":
                    EnterListMode();
                    break;
                case "dump":
                    DoDump(app, type);
                    break;
                default:
                    EnterListMode();
                    break;
            }

            return;
        }

        private static void DoDump(string app, string type)
        {
            Utils.WriteLog("开始dump数据");
            var clslist = app.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in shoplist)
            {
                if (app.ToLower() == "all")
                {
                    Utils.WriteLog("\tDump " + item.Value.Name);                    
                    DoDumpFriend(item.Value, type);
                }
                else
                {
                    var cls = item.Value.Class;
                    foreach (var friend in clslist)
                    {
                        if (cls.ToLower() == friend.ToLower())
                        {
                            Utils.WriteLog("\tDump " + item.Value.Name);
                            DoDumpFriend(item.Value, type);
                        }
                    }
                }
            }
            Utils.WriteLog("完成Dump数据");
        }
        private static void EnterListMode()
        {
            Console.SetWindowSize(140, 28);

            while (true)
            {
                ShowMainMenu();
                ShowShop();
                Console.CursorLeft = 0;
                string input = Console.ReadLine().Trim().ToLower();
                switch (input)
                {
                    case "q":
                        return;
                    //case "s":
                    //    DoDump(app,type);
                    //    continue;
                    case "u":
                        curMainPage = (curMainPage <= 0 ? 0 : curMainPage - 1);
                        continue;
                    case "d":
                        curMainPage = (curMainPage >= shoplist.Count / rowsPerPage ? shoplist.Count / rowsPerPage : curMainPage + 1);
                        continue;
                    default:
                        int index = -1;
                        if (true == int.TryParse(input, out index))
                        {
                            if (shoplist.ContainsKey(index))
                            {
                                ShowMainMenu(shoplist[index].Name);
                                ShowSubMenu(shoplist[index]);
                            }
                        }
                        continue;
                }
            }
        }
        private static bool IsValidArgument(string[] args, out string cmd, out string app, out string type)
        {
            cmd = "list";
            app = "all";
            type = "station";

            if (args.Length != 0 && args.Length != 2 && args.Length != 4 && args.Length != 6)
            {
                goto INVALID;
            }

            for (int i = 0; i < args.Length; i += 2)
            {
                switch (args[i].ToLower().Trim())
                {
                    case "-cmd":
                        cmd = args[i + 1].ToLower().Trim();
                        if (cmd != "dump" && cmd != "list") goto INVALID;
                        continue;
                    case "-app":
                        app = args[i + 1].ToLower().Trim();
                        continue;
                    case "-type":
                        type = args[i + 1].ToLower().Trim();
                        if (type != "station" && type != "order") goto INVALID;
                        continue;
                    default:
                        goto INVALID;
                }
            }

            goto SUCCESS;

            INVALID:
            ShowHelp();
            return false;
            SUCCESS:
            return true;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("用法 : MF -cmd <LIST|DUMP> -app <[ALL]|[APP1+APP2+APPN]> -type <Station|Order>");
            Console.WriteLine("");
            Console.WriteLine("       -cmd");
            Console.WriteLine("       |-不输入，则进入list模式");
            Console.WriteLine("       |-list，其他参数无效，显示所有插件中的station信息");
            Console.WriteLine("       |-dump，保存指定app数据到CSV文件中");
            Console.WriteLine("");
            Console.WriteLine("       -app");
            Console.WriteLine("       |-不输入或输入all，则处理所有插件中的app");
            Console.WriteLine("       |-加号分隔的APP名称组合，对应shopitems.xml的Class名字");
            Console.WriteLine("       |-当command是list时，此参数无效");
            Console.WriteLine("");
            Console.WriteLine("       -type");
            Console.WriteLine("       |-默认为station，即只有电站信息，不包含桩的信息");
            Console.WriteLine("       |-order，取桩明细信息，有的可以取到实时输出电流、电压、SOC");
            Console.WriteLine("       |-当command是list时，此参数无效");
            Console.WriteLine("");

            Console.WriteLine("举例：");
            Console.WriteLine("     mf，进入list模式，可以交互");
            Console.WriteLine("     mf -cmd dump，处理所有插件中的APP并保存数据到data目录下");
            Console.WriteLine("     mf -cmd dump -app eChargeNet|StarCharge|AnYoCharging|eiChong，把充电网、星星、安悦、万马的数据保存到data目录下");
            Console.WriteLine("");
        }
        private static void DoDumpFriend(ShopItem item, string type)
        {
            var bigtext = GetBigtext(item);

            var t = item.Type;
            var obj = Activator.CreateInstance(t);
            string dump = String.Empty;
            string ts = String.Empty;

            if (type == "station")
            {
                ts = Utils.TimeStampDate;
                dump = Convert.ToString(t.InvokeMember("Transform2Station",
                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                null, obj, new object[] { bigtext, ts }
                            ));
            }
            else if (type == "order")
            {
                ts = Utils.TimeStampTime;
                dump = Convert.ToString(t.InvokeMember("Transform2Order",
                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                null, obj, new object[] { bigtext, ts }
                            ));
            }

            File.WriteAllText(String.Format("data\\{0}_{1}.dmp", item.Class, ts), dump);

        }

        private static void Test()
        {
            //string url = "https://www.kgechargenet.cn:8443/electricitya/services/ClientService";
            //string data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><InterfaceServiceEntrance xmlns=\"http://service.clientService.car.service.kge.com\"><whatEverYouNamed>{\"updatetime\":\"1970-01-01\",\"account\":\"13811112222\",\"sessionid\":\"FBC357B8CBDD5EE65DC344BEC617E4DF-n1\",\"enter\":\"cpm_gcsliss\",\"reqtime\":\"1532584330\"}</whatEverYouNamed></InterfaceServiceEntrance></soap:Body></soap:Envelope>";

            //var ret = Utils.PostData(url, data);
            string url = "http://wechat.xcharger.net/arranging/nearbyajax?lat=39.97243&lng=116.48978&start=0&dealer=&oasourceid=gh_cf544c25844a&onlyFree=false&onlyShared=false&siteType=&siteVehicleType=0";
            using (WebClient wc = new WebClient())
            {
                //pageType=list; SESSION=99aa4a91-d1a4-4bcf-96b9-1d80c011922d                                                                                                         
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_4 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15F79 MicroMessenger/6.7.0 NetType/4G Language/zh_CN");
                wc.Headers.Add(HttpRequestHeader.Cookie, "pageType=list; SESSION=99aa4a91-d1a4-4bcf-96b9-1d80c011922d");
                wc.Encoding = Encoding.UTF8;

                var ret = wc.DownloadString(url);
            }
        }

        private static void ShowShop()
        {
            List<List<string>> list = new List<List<string>>();
            foreach (var key in shoplist.Keys)
            {
                var newlist = shoplist[key].GetStringList();
                newlist.Insert(0, key.ToString());
                list.Add(newlist);
            }
            DrawTable(
                new List<string> { "序号", "模块", "公司", "网址", "备注" },
                new List<int> { 8, 14, 28, 35 },
                new List<TextAlignment> { TextAlignment.Left, TextAlignment.Left, TextAlignment.Left, TextAlignment.Left, TextAlignment.Left },
                list,
                curMainPage
            );
            return;
            //┌──────────┐
            //│          │
            //└──────────┘            


        }

        private static void SetTitle(string friendName = "")
        {
            string s = "My Friend";
            if (false == String.IsNullOrEmpty(friendName))
            {
                s += " - " + friendName;
            }
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Yellow;

            int width = Console.WindowWidth;
            int width2 = (width - s.Length) / 2;

            string title = String.Empty;
            for (int i = 0; i < width2; i++)
            {
                title += " ";
            }
            title += s;
            for (int i = 0; i < width2 + 1; i++)
            {
                title += " ";
            }
            Console.Write(title);

            ResetColor();
        }

        private static void ResetColor()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void FillLineByColor(int left, int top, int filllen, ConsoleColor cc)
        {
            Console.CursorLeft = left;
            Console.CursorTop = top;
            Console.BackgroundColor = cc;
            string fill = String.Empty;
            Console.Write(fill.PadLeft(filllen));

        }
        private static void ShowMainMenu(string friendName = "")
        {
            Console.Clear();

            SetTitle(friendName: friendName);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;

            FillLineByColor(0, 1, Console.WindowWidth, ConsoleColor.DarkGray);
            Console.CursorTop = 1;
            string menu = "【Q】退出 【S】保存电站信息 【其他数字】调用模块 【U】上一页 【D】下一页";

            Console.Write(menu);//.PadRight(Console.WindowWidth-Encoding.Default.GetByteCount(menu)));
            Console.CursorTop = 2;
        }

        private static void ShowSubMenu(ShopItem item)
        {
            while (true)
            {
                ShowStationMenu(item);

                Console.CursorLeft = 0;
                string input = Console.ReadLine().Trim().ToLower();
                switch (input)
                {
                    case "q":
                        return;
                    case "c":
                        var fname = String.Format("{0}\\cache-{1}.txt", Utils.PATH, item.Type.FullName);
                        if (File.Exists(fname)) File.Delete(fname);
                        continue;
                    default:

                        if (input.StartsWith("s"))
                        {
                            ShowStation(item);
                        }
                        continue;
                }
            }
        }

        private static void ShowStationMenu(ShopItem item)
        {
            ResetColor();
            Console.Clear();

            SetTitle(friendName: item.Name);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;

            FillLineByColor(0, 1, Console.WindowWidth, ConsoleColor.DarkGray);
            Console.CursorTop = 1;
            string menu = "【S】电站列表 【F电站名称或编号】搜索电站 【C】清除该模块数据缓存 【V电站编号】电站详情 ";

            Console.Write(menu);

            Console.CursorTop = 2;
            FillLineByColor(0, 2, Console.WindowWidth, ConsoleColor.DarkGray);
            Console.CursorTop = 2;
            menu = "【Q】退到模块列表 【U】上一页 【D】下一页 【数字】进入指定页";
            Console.Write(menu);

            Console.CursorTop = 3;
            ResetColor();
        }

        private static void ShowStationDetail(Type t, string staid)
        {
            try
            {
                var obj = Activator.CreateInstance(t);
                string dret = t.InvokeMember("GetStationDetail",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                    null, obj, new object[] { staid }
                ).ToString();


                //string json = JToken.Parse(dret).ToString(Newtonsoft.Json.Formatting.Indented);
                //json = Utils.Decode(json);
                Console.CursorTop = 2;
                Console.WriteLine(dret);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private static void ShowStation(ShopItem item)
        {
            var stations = GetCachedStations(item);
            int totalPages = (stations.Count - 1) / rowsPerPage + 1;

            while (true)
            {
                ShowStationMenu(item);
                DrawTable(item.Headers, item.HeadersWidth, item.HeadersAlignment, stations, curSubPage);
                Console.CursorLeft = 0;
                string originalInput = Console.ReadLine().Trim().Replace("\0", "");
                string input = originalInput.ToLower();
                switch (input)
                {
                    case "q":
                        return;
                    case "u":
                        curSubPage = (curSubPage <= 0 ? 0 : curSubPage - 1);
                        continue;
                    case "d":
                        curSubPage = (curSubPage >= totalPages - 1 ? totalPages - 1 : curSubPage + 1);
                        continue;
                    default:
                        if (input.StartsWith("v"))
                        {
                            ShowStationDetailMenu(item);
                            ShowStationDetail(item.Type, originalInput.Substring(1).Trim());
                            Console.CursorTop = 2;
                            Console.CursorLeft = 0;
                            Console.ReadLine();
                        }
                        else
                        {
                            int page = 0;
                            if (true == int.TryParse(originalInput, out page))
                            {
                                if (page >= totalPages) page = totalPages;
                                if (page < 1) page = 1;

                                curSubPage = page - 1;
                            }
                        }
                        continue;
                }
            }


        }

        private static void ShowStationDetailMenu(ShopItem item)
        {
            Console.Clear();
            SetTitle(friendName: item.Name);

            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;

            FillLineByColor(0, 1, Console.WindowWidth, ConsoleColor.DarkGray);
            Console.CursorTop = 1;
            string menu = "【按任意键】退到电站列表";

            Console.Write(menu);

            Console.CursorTop = 2;

            ResetColor();
        }

        static void ProcessBadFile()
        {
            var lines = File.ReadAllLines("citylist.txt", Encoding.GetEncoding("gb2312"));
            Dictionary<string, string> plist = new Dictionary<string, string>();
            Dictionary<string, string> clist = new Dictionary<string, string>();
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i < lines.Length; i++)
            {
                var tmp = lines[i].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                var key = tmp[0];
                if (key.Length == 2) plist.Add(key, tmp[1]);
                if (key.Length == 4)
                {
                    clist.Add(key, tmp[1]);
                    if (key == "1101" || key == "1201" || key == "3101" || key == "5001")
                    {
                        sb.AppendLine(key + "00," + plist[key.Substring(0, 2)] + "," + plist[key.Substring(0, 2)] + "," + tmp[1]);
                    }
                    else
                    {
                        sb.AppendLine(key + "00," + plist[key.Substring(0, 2)] + "," + clist[key.Substring(0, 4)] + "," + tmp[1]);
                    }
                }
                if (key.Length == 6)
                {

                    sb.AppendLine(key + "," + plist[key.Substring(0, 2)] + "," + clist[key.Substring(0, 4)] + "," + tmp[1]);
                }
            }

            File.WriteAllText("citymapping.txt", sb.ToString());
        }
        static void InitAll()
        {
            var shop = AppDomain.CurrentDomain.BaseDirectory + "MyFriend.Shop.dll";
            var shopcfg = AppDomain.CurrentDomain.BaseDirectory + "ShopItems.xml";

            #region read items from xml configuration
            shoplist = new Dictionary<int, ShopItem>();
            var xml = new XmlDocument();
            xml.Load(shopcfg);
            var rootnode = xml.SelectSingleNode("/Friends");
            var asm = Assembly.LoadFile(shop);
            int i = 1;

            var baseclass = rootnode.Attributes["BaseClass"].Value;
            foreach (XmlNode childNode in rootnode.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Comment) continue;
                string clsname = childNode.Attributes["Class"].Value;

                ShopItem si = new ShopItem();
                si.Class = clsname;
                si.Name = childNode.Attributes["Name"].Value;
                si.Company = childNode.Attributes["Company"].Value;
                si.URL = childNode.Attributes["URL"].Value;
                si.Description = childNode.Attributes["Description"].Value;

                Type t = asm.GetType(baseclass + "." + clsname);
                if (null == t) throw new Exception("配置文件的类名没找到：" + clsname);

                si.Type = t;

                if (childNode.Attributes["Headers"] != null)
                {
                    si.Headers = childNode.Attributes["Headers"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                if (childNode.Attributes["HeadersWidth"] != null)
                {
                    List<int> width = new List<int>();
                    var tmp = childNode.Attributes["HeadersWidth"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var w in tmp)
                    {
                        width.Add(Convert.ToInt32(w));
                    }
                    si.HeadersWidth = width;
                }
                if (childNode.Attributes["HeadersAlignment"] != null)
                {
                    List<TextAlignment> align = new List<TextAlignment>();
                    var tmp = childNode.Attributes["HeadersAlignment"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var w in tmp)
                    {
                        switch (w.ToLower())
                        {
                            case "l": align.Add(TextAlignment.Left); break;
                            case "m": align.Add(TextAlignment.Middle); break;
                            case "r": align.Add(TextAlignment.Right); break;
                            default: align.Add(TextAlignment.Left); break;
                        }
                    }
                    si.HeadersAlignment = align;
                }

                shoplist.Add(i++, si);
            }

            #endregion read items from xml configuration


        }

        static string GetBigtext(ShopItem item)
        {
            var t = item.Type;
            string fname = String.Format("{0}\\Cache-{1}.txt", Utils.PATH, t.FullName);
            string bigtext = String.Empty;

            var obj = Activator.CreateInstance(t);

            if (false == File.Exists(fname))
            {
                bigtext = t.InvokeMember("GetStations",
                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                null, obj, new object[] { }
                            ).ToString();
                File.WriteAllText(fname, bigtext);
            }
            else
            {
                bigtext = File.ReadAllText(fname);
            }

            return bigtext;
        }
        static List<List<string>> GetCachedStations(ShopItem item)
        {
            var bigtext = GetBigtext(item);
            var t = item.Type;
            var obj = Activator.CreateInstance(t);

            var ret = t.InvokeMember("GetWellFormattedStations",
                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                null, obj, new object[] { bigtext }
                            ) as List<List<string>>;
            return ret;
        }

        private static void DrawTable(List<string> headers, List<int> headersWidth, List<TextAlignment> alignments, List<List<string>> rows, int curPage)
        {
            ResetColor();
            Console.CursorLeft = 0;

            int width = Console.WindowWidth - 2;
            int top = Console.CursorTop;

            #region Header Top Border
            Console.Write("┌");
            for (int i = 0; i < width; i++)
            {
                Console.Write("─");
            }
            Console.CursorTop = top;
            Console.CursorLeft = width;
            Console.Write("┐");
            #endregion Header Top Border

            top++;

            #region Headers
            int left = 0;
            for (int i = 0; i < headers.Count; i++)
            {
                Console.CursorLeft = left;
                Console.CursorTop = top;
                Console.Write("│");

                if (i < headers.Count - 1)
                {
                    Console.CursorLeft = left + (headersWidth[i] - headers[i].Length) / 2;
                    left += headersWidth[i];
                }
                else
                {
                    Console.CursorLeft = left + 2;
                }
                Console.Write(headers[i]);
            }

            Console.CursorLeft = width;
            Console.Write("│");
            #endregion Headers

            #region Header Bottom Border
            top++;
            Console.CursorLeft = 0;
            Console.CursorTop = top;
            Console.Write("└");
            for (int i = 0; i < width; i++)
            {
                Console.Write("─");
            }
            Console.CursorTop = top;
            Console.CursorLeft = width;
            Console.Write("┘");
            #endregion Header Bottom Border

            top++;
            left = 0;

            #region Rows
            int lines = rowsPerPage * curPage;
            int remained = rows.Count - lines;
            int start = lines + 1;
            int end = (remained >= rowsPerPage) ? lines + rowsPerPage : rows.Count;

            for (int i = start; i <= end; i++)
            {
                List<string> list = rows[i - 1];
                Console.CursorLeft = 0;
                Console.CursorTop = top;
                left = 0;

                for (int j = 0; j < headers.Count; j++)
                {
                    Console.CursorLeft = left;
                    Console.CursorTop = top;

                    Console.Write("│");
                    if (j < headers.Count)
                    {
                        switch (alignments[j])
                        {
                            case TextAlignment.Left:
                                Console.CursorLeft = left + 2;
                                Console.Write(list[j]);
                                break;
                            case TextAlignment.Middle:
                                if (j == headers.Count - 1)
                                {
                                    Console.CursorLeft = left + (width - left) / 2;
                                }
                                else
                                {
                                    Console.CursorLeft = left + (headersWidth[j] - headers[j].Length) / 2;
                                }
                                Console.Write(list[j]);
                                break;
                            case TextAlignment.Right:
                                if (j == headers.Count - 1)
                                {
                                    Console.CursorLeft = width - list[j].Length;
                                }
                                else
                                {
                                    Console.CursorLeft = left + headersWidth[j] - list[j].Length;//这里要补齐
                                }
                                Console.Write(list[j]);
                                break;
                            default:
                                Console.CursorLeft = left + 2;
                                Console.Write(list[j]);
                                break;
                        }
                        if (j < headers.Count - 1)
                        {
                            left += headersWidth[j];
                        }
                        else
                        {
                            Console.CursorLeft = width;
                            Console.Write("│");
                        }
                    }
                }

                top++;
            }
            #endregion Rows

            #region Footer
            Console.CursorLeft = 0;
            Console.CursorTop = top;
            Console.Write("└");
            for (int i = 0; i < width; i++)
            {
                Console.Write("─");
            }
            Console.CursorTop = top;
            Console.CursorLeft = width;
            Console.Write("┘");
            #endregion Footer
            top++;
            Console.CursorLeft = 0;
            Console.CursorTop = top;
        }

        private static void SaveData()
        {
            var text = File.ReadAllText(@"data\Cache-MyFriend.Shop.eiChong.txt", Encoding.UTF8);
            var array = (JsonConvert.DeserializeObject(text) as JToken)["data"] as JArray;
            StringBuilder sb = new StringBuilder();
            sb.Append("ID,电站,省代码,省,城市代码,城市,地址,交流桩个数,直流桩个数,纬度,经度,功率,交流个数,交流枪头个数,直流个数,直流枪头个数,桩总数\r\n");

            foreach (var sta in array)
            {
                string staid = Convert.ToString(sta["id"]);
                System.Diagnostics.Debug.WriteLine(staid);
                string url = "https://api.eichong.com/api/app/powerStation.do?ext=t&userId=1&powerStationId=" + staid + "&longitude=117&latitude=36";
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    var ret = wc.DownloadString(url);
                    var json = (JsonConvert.DeserializeObject(ret) as JToken)["data"]["everyPowerEpAndEpHeadNumMap"] as JToken;
                    foreach (var kw in json.Children())
                    {
                        string key = kw.Path.Substring(kw.Path.LastIndexOf(".") + 1).Replace("kw", "");
                        int dcEpHeadNum = Convert.ToInt32(kw.First["dcEpHeadNum"]);
                        int acEpHeadNum = Convert.ToInt32(kw.First["acEpHeadNum"]);
                        int dcEpNum = Convert.ToInt32(kw.First["dcEpNum"]);
                        int acEpNum = Convert.ToInt32(kw.First["acEpNum"]);

                        sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}\r\n",
                        Convert.ToString(sta["id"]), Convert.ToString(sta["name"]), Convert.ToString(sta["provinceCode"]), Convert.ToString(sta["provinceName"]), Convert.ToString(sta["cityCode"]), Convert.ToString(sta["cityName"]),
                        Convert.ToString(sta["address"]), Convert.ToInt32(sta["epAcTotalNum"]), Convert.ToInt32(sta["epDcTotalNum"]),
                        Convert.ToDouble(sta["latitude"]), Convert.ToDouble(sta["longitude"]),
                        Convert.ToInt32(key), Convert.ToInt32(acEpNum), Convert.ToInt32(acEpHeadNum), Convert.ToInt32(dcEpNum), Convert.ToInt32(dcEpHeadNum), Convert.ToInt32(dcEpNum) + Convert.ToInt32(acEpNum)
                        );
                    }
                }
            }

            File.WriteAllText(@"data\eichong.csv", sb.ToString(), Encoding.UTF8);
        }

        private static void SaveData2()
        {
            var text = File.ReadAllText(@"data\Cache-MyFriend.Shop.eCloudPower.txt", Encoding.UTF8);
            var array = (JsonConvert.DeserializeObject(text) as JToken)["resultObject"] as JArray;
            //list.Add(new List<string>() { Convert.ToString(s["id"]), Convert.ToString(s["name"]), Convert.ToString(s["institution_name"]), Convert.ToString(s["fastCount"]), Convert.ToString(s["slowCount"])});

            StringBuilder sb = new StringBuilder();
            sb.Append("ID,电站,地址,供应商,快慢充,portname,国标,描述\r\n");

            foreach (var sta in array)
            {
                foreach (var fast in sta["fastList"])
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}\r\n",
                    Convert.ToString("'" + sta["id"]) + "'", Convert.ToString(sta["name"]), Convert.ToString(sta["address"]), Convert.ToString(sta["institution_name"]),
                    "快充",
                    Convert.ToString(fast["stakePortName"]), Convert.ToString(fast["chargeInterfaceName"]), Convert.ToString(fast["description"])
                    );
                }
                foreach (var slow in sta["slowList"])
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}\r\n",
                    Convert.ToString("'" + sta["id"] + "'"), Convert.ToString(sta["name"]), Convert.ToString(sta["address"]), Convert.ToString(sta["institution_name"]),
                    "慢充",
                    Convert.ToString(slow["stakePortName"]), Convert.ToString(slow["chargeInterfaceName"]), Convert.ToString(slow["description"])
                    );
                }
            }

            File.WriteAllText(@"data\eCloudPower.csv", sb.ToString(), Encoding.UTF8);
        }

        private static void SaveData3()
        {
            var text = File.ReadAllText(@"data\Cache-MyFriend.Shop.starcharge.txt", Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            sb.Append("城市,ID,电站,地址,#快充,#慢充,经度,纬度,类型,建设完成\r\n");

            List<List<string>> list = new List<List<string>>();

            string[] all = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

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
                    sb.Append(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}\r\n",
                        all[c + 1], Convert.ToString(s["id"]), Convert.ToString(s["name"]), Convert.ToString(s["address"]), Convert.ToString(s["stubDcCnt"]), Convert.ToString(s["stubAcCnt"]),
                        Convert.ToString(s["gisGcj02Lng"]), Convert.ToString(s["gisGcj02Lat"]),
                        Convert.ToString(s["stubGroupType"]) == "0" ? "公共" : "专用",
                        Convert.ToString(s["isBuilded"]) == "0" ? "否" : "是"
                        ));
                }
            }

            File.WriteAllText(@"data\starcharge.csv", sb.ToString(), Encoding.UTF8);
        }
    }

    internal class ShopItem
    {
        public string Class { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }

        public List<string> Headers { get; set; }
        public List<int> HeadersWidth { get; set; }
        public List<TextAlignment> HeadersAlignment { get; set; }
        public List<string> GetStringList()
        {
            return new List<string>() { Name, Company, URL, Description };
        }
    }

    internal enum TextAlignment
    {
        Left = 1,
        Middle = 2,
        Right = 4
    }

}


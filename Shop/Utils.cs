using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Utility
{
    public class Utils
    {
        static Utils()
        {
            PATH = AppDomain.CurrentDomain.BaseDirectory + "data\\";
            if (false == Directory.Exists(PATH))
            {
                Directory.CreateDirectory(PATH);
            }
        }
        public static string PATH = String.Empty;
        public static void WriteLog(string message)
        {
            string log = String.Format("[ {0} ] - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message);
            Console.WriteLine(log);
            using (StreamWriter sw = new StreamWriter(Utils.PATH + "log.txt", true))
            {
                sw.WriteLine(log);
            }
        }

        public static string PostData(string url, string data,
            string contentType = "application/x-www-form-urlencoded",
            string accept = "*/*",
            Dictionary<string, string> headers = null,
            List<Cookie> cookies = null,
            string userAgent = ""
            )
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //https://stackoverflow.com/questions/42757481/how-to-sent-http-2-0-request-in-visual-c
            //HTTP 2.0 support in Framework 4.6.2 version only.
            //https://msdn.microsoft.com/en-us/library/ms171868(v=vs.110).aspx

            request.Method = "POST";
            request.ContentType = contentType;
            request.Accept = accept;

            if (headers != null)
            {
                foreach (var key in headers.Keys)
                {
                    request.Headers.Add(key, headers[key]);
                }
            }

            if (cookies != null)
            {
                CookieContainer cc = new CookieContainer();
                foreach (var c in cookies)
                {
                    cc.Add(c);
                }

                request.CookieContainer = cc;
            }
            //request.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate(AppDomain.CurrentDomain.BaseDirectory + "fqq.cer"));
            request.ServicePoint.Expect100Continue = true;
            if (false == String.IsNullOrEmpty(userAgent)) request.UserAgent = userAgent;

            //https://stackoverflow.com/questions/26389899/how-do-i-disable-ssl-fallback-and-use-only-tls-for-outbound-connections-in-net/26392698#26392698
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;

            byte[] buf = Encoding.Default.GetBytes(data);

            request.ContentLength = buf.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(buf, 0, buf.Length);
            }

            string ret = String.Empty;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if (response.ContentEncoding.ToLower().IndexOf("gzip") > -1)
                    {
                        using (var responseStream2 = new GZipStream(responseStream, CompressionMode.Decompress))
                        {
                            using (StreamReader streamReader = new StreamReader(responseStream2))
                            {
                                ret = streamReader.ReadToEnd();
                                System.Diagnostics.Debug.WriteLine(ret);
                            }
                        }
                    }

                    else
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            ret = reader.ReadToEnd();
                        }

                    }
                }
            }

            return ret;

        }

        static Regex reUnicode = new Regex(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);
        public static string Decode(string s)
        {
            var ret = reUnicode.Replace(s, m =>
            {
                short c;
                if (short.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out c))
                {
                    return "" + (char)c;
                }
                return m.Value;
            });

            ret = ret.Replace("\\/", "//");
            return ret;
        }

        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    HashSet<TKey> seenKeys = new HashSet<TKey>();
        //    foreach (TSource element in source)
        //    {
        //        if (seenKeys.Add(keySelector(element)))
        //        {
        //            yield return element;
        //        }
        //    }
        //}

        public static string GetPartFromString(string content, string s1, string s2)
        {
            string ret = String.Empty;
            int startIndex = 0;

            int index = content.IndexOf(s1, startIndex);
            if (index > -1)
            {
                int index2 = content.IndexOf(s2, s1.Length + index);
                if (index2 > -1)
                {
                    ret = content.Substring(s1.Length + index, index2 - index - s1.Length);
                    startIndex = index2 + s2.Length;
                }
            }

            return ret;
        }

        public static string GetUnifiedDataStructureFormatter()
        {
            var shopcfg = AppDomain.CurrentDomain.BaseDirectory + "ShopItems.xml";

            var xml = new XmlDocument();
            xml.Load(shopcfg);
            var rootnode = xml.SelectSingleNode("/Friends");
            var baseclass = rootnode.Attributes["UnifiedDataStructure"].Value;

            var formatter = Convert.ToString(baseclass);
            var tmp = formatter.Split(new char[] { ',' });
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < tmp.Length; i++)
            {
                sb.Append("{").Append(i).Append("}_CCF_");
            }
            if (sb.Length > 0) sb.Remove(sb.Length-5, 5);

            return sb.ToString() + "\r\n";
        }
    }


}

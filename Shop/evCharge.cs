using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyFriend.Shop
{
    public class evCharge:ICharge
    {
        public string GetStationDetail(string staid)
        {
            using (WebClient wc = new WebClient())
            {
                var ret = wc.DownloadString("https://cdz.evcharge.cc/zhannew/basic/web/index.php/zhanlist/zhan-info2?version=3.1&zhan_id="+staid);
                return ret;
                //{"info":"hwXxFXx1hH0LDW0kKdnHlhvvS\/hnwS3wy9zDIhXoKrwKgGQ2MyXnZiM8GiXnw0zsCm6VdJANhGlQi+kdGWmc\/yjal4XRC138EcbDP85oB0z2j0Fq2xv7PzBFV5zg9CSE0icB8KLFMJwMxs92jaDoR77spO4p4C+UcUyYNXxHRjBkvujtl0HbM6h0\/ex1oMKd+v4qVga24eI53EcrWwvtG2IdcEdozMbMMSqVZMaDmHS9K\/md5KdIbzgZKVCFQNbw5nKHxTGrOMoBJ7Z\/oRhratBlKNvzGF\/kutfHsTZ0gsjz9yAc+YwgSlkLXM+R09bfJWACLO4+H\/aQoYs1jk3mhbYJgcnp1kpy\/bzL+nbsy2WhmpynBpTW3CqqhEIDtd1oSwE9VuKFlVvKj3xOMXcyoar9lAg56m5I3yvEYsIjp04DUIn20KowIwHdZS\/So4uS6Kk1T1qQ6oLDV9LZ0iq+dcJVoZ80o3ZclIPB9upQItRF\/POCZIX921OZSvoslklI8yTdOKGi08RHWE5mkpI6a5EPRd40zXL0SCVpxjBemSNIuSiEM\/kITBTqBU3Knv+NPZ0sKdSoDg\/0SwOoUeFrQhlACJuMp1dd8F+89oNGne260xq+KL3zqCbPFRcVEBDq93J1HdHyisazjmav+zmFRJyna8zH4e7OoIoPqCcE5qJo1SmzgEU7mRSPj\/Xg1xXuCWiFf1a3j1tL69bfr0CUXcJZUTrzMPdmkD4L3khbUiNuX6YE4MY0pYZSkr2Fyq6enEutQAX7lGVX0iseAyMqlKMPM1S0nwZNfG9Gr05G638kKzAaqtjt+OjMWpcj2A+OocAWwsbe7WwgG8P\/1JcRLCn8gby+1+fd95un71UDeLXrnyp8Kzj4Xwh2+Xp+I1Zoqkoy73u27PX7K4MpYNApzhv0oxo43n4PXdL7kWGgQR2SuCECge0h1kYO59CIpoZigYGjHqjK1HKoap0+XB\/vGscnoV2SV9nJyyTjSSzoR2wIvAP9ptActbOV2if7fuJiZ73+utLsvwAJo4j\/8DjGPb073q3DsD8V9noGknU0a9\/2T0\/pD4+KsRvyn+O6o32VQsYCnCCZGWqsd11cFrLF\/L2YIH72UGS5Mki+16fyEyQa6XMv06uWMRS8pVKG6pNr+jWlcAvnZ4KbGTpCEUBCDQiPfEKbOQRlYVU1kKcsfAEiVYKtavqKQXUidqWksNL\/GcyNS\/kMA0udTxx2HcJNUH6zvjB8KEpW+DxvgEKPlzfz7GXNIFFy9KJhPjaMMu0DAILRndfKYn2yVtMpSrFsNv0gQ8K3deQeoRZLp5MD8tVTScpKgtFPxkjSo3v86sXqYf24OkNmkgFiT5J+U2L3heWkk1a+7780vSiz5SGzFJcayEikbhrE1V9nDkWFiDv8kRiixu\/BCrg463Rt2+99zW\/wDJLRRU2e2t98uVJupktUprQsL7VINPeMCd3eCojTeP24G22\/vVYmECTTnPmRiY7dx1TV\/YZancNR51SKB3bK5xSv9FZsNzGFdBROP0ChcUxqUqX297wNopoChFELeX1eArv\/IPvC\/Eqa6mVTgY1fJcVqnGhVODcBpmBrc6xXYr6LRC4crNd16NPDa97FkC6dwE13NaTU95ZVap8qV8CXBUXxWP8JisdmiZO9W\/WLnMzSfrVj9of6wiv9109GamqLKcqjIew2RR1r02w8QOgXKVDuCjJ87Wzbg4kkLg8fQ3R9oaS+UaJmY+DClPM9Ou8R4l1P+R48Qguh+iOGu24w8cVlIw0zn81hyZHuIodd30fN1RRm+rX9Cum1+ISDnAGi07ZCMu2ReNt1Tiq0qPT9jQ\/5VChtcbMH2fT+X1X0KKRnVKRrQegdST3i7\/4gzl9gTB1F7kfWPefGzZl7NSjKa2ty4YecrYUTT1By79yuEBppS\/9C0PHIjOkZod0pMfheYkssewx4T2OLzkwKt8lkNrrqbcsqR1Vw4I\/d4S0iHrozYU9MEeBV\/pfrZ3dXXjT+iVCvRfd25ymDisTS7RFD2ICea5GWm+1bNashaDEpvbhARy\/kh7xwtX59yIH9BwloFM378ELx86b5ea4z3jiwZNgkJoSAxEozgcI4CTF84THx46dpLT38d+Rlfh+u1mRCWohUkuDI","pic":[{"id":"95446","url":"teld\/2017-12-22\/aa9133d661a64c8ab6984d13a75b9d30.jpg","zhan_id":"10005111145328","timer":"2017-12-22 03:07:43","user":"1","ischeck":"1"},{"id":"95447","url":"teld\/2017-12-22\/24d3277ca86c412bb3b921b94980903c.jpg","zhan_id":"10005111145328","timer":"2017-12-22 03:07:43","user":"1","ischeck":"1"},{"id":"95448","url":"teld\/2017-12-22\/aa9133d661a64c8ab6984d13a75b9d30.jpg","zhan_id":"10005111145328","timer":"2017-12-22 03:07:44","user":"1","ischeck":"1"},{"id":"95449","url":"teld\/2017-12-22\/bc4a4cf286294ccdb07f9c4ded6ce529.jpg","zhan_id":"10005111145328","timer":"2017-12-22 03:07:44","user":"1","ischeck":"1"},{"id":"95450","url":"teld\/2017-12-22\/9fafde5195754facb9f92a07647beecf.jpg","zhan_id":"10005111145328","timer":"2017-12-22 03:07:45","user":"1","ischeck":"1"}]}
                //info不可解
            }
        }

        public string GetStations()
        {
            return "未破解";
        }
    }
}

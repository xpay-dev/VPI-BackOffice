using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SDGUtil.Utility
{
    public class Toolkit
    {
        public static String signWithMD5(String keyPara, String srcPara, String pubkey)
        {
            if (String.IsNullOrEmpty(keyPara))
            {
                keyPara = Toolkit.random(24);
            }

            //RSA 
            String keyEncrypt = BouncyCastleRSA.EncryptByPublicKey(keyPara, pubkey);

            //Des3 
            System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
            byte[] srcEnc = TripleDES.EncodeECB(utf8.GetBytes(keyPara), null, utf8.GetBytes(srcPara));
            String srcEncrypt = Convert.ToBase64String(srcEnc);

            //MD5
            String srcSign = ComputeMD5Hash(srcPara);
            byte[] b = Encoding.Default.GetBytes(srcSign);
            srcSign = Convert.ToBase64String(b);

            String version = "MD5";
            String tData = Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes(version));
            tData += "|" + "";
            tData += "|" + keyEncrypt;
            tData += "|" + srcEncrypt;
            tData += "|" + srcSign;
            return tData;
        }

        public static String random(int len)
        {
            String str = "";
            Random rander = new Random();
            for (int i = 0; i < len; i++)
            {
                str += HEXCHAR[rander.Next(16)];
            }

            return str;
        }

        public static String ComputeMD5Hash(string data)
        {
            StringBuilder hashOutput = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytesHash = md5provider.ComputeHash(new UTF8Encoding().GetBytes(data));

            for (int i = 0; i < bytesHash.Length; i++)
            {
                hashOutput.Append(bytesHash[i].ToString("x2"));
            }

            return hashOutput.ToString().ToUpper();
        }

        private static char[] HEXCHAR = {'0', '1', '2', '3', '4', '5', '6', '7',
                '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'};

        public static String bytePadLeft(String input, char c, int length)
        {
            String output = input;
            while (Encoding.ASCII.GetBytes(output).Length < length)
            {
                output = c + output;
            }
            return output;
        }

        public static String getXmlValue(String xml, String name)
        {
            if (String.IsNullOrEmpty(xml) || String.IsNullOrEmpty(name))
            {
                return "";
            }
            String tag = "<" + name + ">";
            String endTag = "</" + name + ">";
            if (!xml.Contains(tag) || !xml.Contains(endTag))
            {
                return "";
            }
            String value = xml.Substring(xml.IndexOf(tag) + tag.Length, xml.IndexOf(endTag));
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            return "";
        }

        public static String Verify(String keyPara, String sign)
        {

            String[] values = sign.Split('|');
            String version = Convert.ToBase64String(Encoding.Default.GetBytes(values[0]));

            String srcEncrypt = values[3];
            String srcSign = values[4];

            byte[] srcEnc = StringToBase64Byte(srcEncrypt);
            byte[] srcSigned = StringToBase64Byte(srcSign);
            byte[] keyBt = null;

            keyBt = Encoding.UTF8.GetBytes(keyPara);
            byte[] srcBt = TripleDES.DecodeECB(keyBt, null, srcEnc);
            //String src = new String(srcBt, "UTF-8");
            String src = Encoding.UTF8.GetString(srcBt);

            //MD5 md5 = new MD5();
            //String strSign = md5.getMD5ofByte(srcBt);
            String strSign = ComputeMD5Hash(src);
            if (!strSign.Equals(Encoding.UTF8.GetString(srcSigned)))
            {
                throw new Exception("fail to verifySignedData MD5");
            }

            return src;
        }

        public static String Base64ToString(string base64)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static String StringToBase64(String src)
        {
            if (String.IsNullOrEmpty(src))
                return "";
            System.Text.Encoding encode = System.Text.Encoding.ASCII;
            byte[] bytedata = encode.GetBytes(src);
            return Convert.ToBase64String(bytedata, 0, bytedata.Length);
        }

        public static byte[] StringToBase64Byte(String src)
        {
            if (String.IsNullOrEmpty(src))
                return null;
            System.Text.Encoding encode = System.Text.Encoding.ASCII;
            return encode.GetBytes(src);
        }

        public static String getCurrency(String currencyType)
        {
            //人民币 港币 澳门币 台币 马元 朝鲜 泰铢 美元 日元 韩币 新元 欧元 英镑
            //CNY HKD MOP TWD MYR KPW THB USD JPY KRW SGD EUR GBP
            if (currencyType.Equals("01"))
            {
                return "CNY";
            }
            if (currencyType.Equals("02"))
            {
                return "HKD";
            }
            if (currencyType.Equals("03"))
            {
                return "MOP";
            }
            if (currencyType.Equals("04"))
            {
                return "TWD";
            }
            if (currencyType.Equals("05"))
            {
                return "MYR";
            }
            if (currencyType.Equals("06"))
            {
                return "KPW";
            }
            if (currencyType.Equals("07"))
            {
                return "THB";
            }
            if (currencyType.Equals("08"))
            {
                return "USD";
            }
            if (currencyType.Equals("09"))
            {
                return "JPY";
            }
            if (currencyType.Equals("10"))
            {
                return "KRW";
            }
            if (currencyType.Equals("11"))
            {
                return "SGD";
            }
            if (currencyType.Equals("12"))
            {
                return "EUR";
            }
            if (currencyType.Equals("13"))
            {
                return "GBP";
            }
            return "CNY";
        }

        public string getString(string src)
        {
            return (string.IsNullOrWhiteSpace(src) ? "" : (" " + src.Trim()));
        }
    }
}

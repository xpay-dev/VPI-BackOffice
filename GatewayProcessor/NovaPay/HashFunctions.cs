using SDGUtil.RSA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thx.Research.Security;

namespace GatewayProcessor.NovaPay
{
    public sealed class HashFunctions
    {
        public static IEnumerable<KeyValuePair<string, string>> ToKVP(Hashtable kvp)
        {
            var k = new List<KeyValuePair<string, string>>(0);
            foreach (var key in kvp.Keys)
            {
                k.Add(new KeyValuePair<string, string>(key.ToString(), kvp[key].ToString()));
            }
            return k.ToArray();
        }

        public static Hashtable buildParamRequest(TransactionData transData)
        {
            Hashtable sParaTemp = new Hashtable();

            string privateKey = transData.PrivateKey;

            sParaTemp.Add("gateway", "");
            sParaTemp.Add("merchantId", transData.MerchantId);
            sParaTemp.Add("privateKey", privateKey);
            sParaTemp.Add("paymentType", null);
            sParaTemp.Add("payBank", null);
            sParaTemp.Add("inputCharset", "UTF-8");
            sParaTemp.Add("returnUrl", null);
            sParaTemp.Add("notifyUrl", transData.NotifyUrl);
            sParaTemp.Add("merchantTradeId", transData.MerchantTradeId);
            sParaTemp.Add("goodsTitle", transData.GoodsTitle);
            sParaTemp.Add("currency", transData.Currency);
            sParaTemp.Add("amountFee", transData.AmountFee);
            sParaTemp.Add("version", "1.0");
            sParaTemp.Add("payIp", SDGUtil.Functions.GetIPAddress());
            sParaTemp.Add("signType", transData.SignType);
            sParaTemp.Add("identityType", null);
            sParaTemp.Add("identityId", null);

            sParaTemp.Add("firstName", transData.FirstName);
            sParaTemp.Add("lastName", transData.LastName);
            sParaTemp.Add("issuingBank", transData.IssuingBank);
            sParaTemp.Add("phone", transData.Phone);
            sParaTemp.Add("email", transData.Email);
            sParaTemp.Add("country", transData.Country);
            sParaTemp.Add("state", transData.State);
            sParaTemp.Add("city", transData.City);
            sParaTemp.Add("address", transData.Address);
            sParaTemp.Add("zip", transData.Zip);

            String paymentCode = null;
            if (transData.CardNumber != null && transData.ExpiryDate != null && transData.Cvv != null)
            {
                paymentCode = transData.CardNumber + "|" + transData.ExpiryDate + "|" + transData.Cvv;
            }

            sParaTemp.Add("paymentCode", paymentCode);

            return sParaTemp;
        }

        public static Hashtable buildSignRequest(Hashtable sParaTemp)
        {
            Hashtable sPara = paraFilter(sParaTemp, new String[] { "sign", "signType", "privateKey", "gateway", "paymentCode" });

            String prestr = createLinkString(sPara); //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            String mysign = "";
            if (sParaTemp["signType"].ToString().Equals("RSA"))
            {
                mysign = RSAFromPkcs8.sign(prestr, sParaTemp["privateKey"].ToString(), sPara["inputCharset"].ToString());
            }

            if (sParaTemp["signType"].ToString().Equals("MD5"))
            {
                //mysign = DigestUtils.md5Hex((prestr + sParaTemp.get("privateKey")).getBytes(sPara.get("inputCharset")) );
            }

            sPara.Add("sign", mysign);
            sPara.Add("signType", sParaTemp["signType"].ToString());

            if (sParaTemp["paymentCode"] != null)
            {
                String paymentCode = Rsa.EncryptByPrivateKey(sParaTemp["paymentCode"].ToString(), RSAFromPkcs8.DecodePemPrivateKey(sParaTemp["privateKey"].ToString()).ExportParameters(true));
                sPara.Add("paymentCode", paymentCode);
            }
            return sPara;
        }

        public static Hashtable paraFilter(Hashtable sArray, string[] param)
        {
            Hashtable result = new Hashtable();

            if (sArray == null || sArray.Count <= 0)
            {
                return result;
            }

            foreach (string key in sArray.Keys)
            {
                string value = (string)sArray[key];
                if (value == null || value.Equals("") || existParams(key, param))
                {
                    continue;
                }
                result.Add(key, value);
            }

            return result;
        }

        private static bool existParams(string key, string[] param)
        {
            foreach (string paramTemp in param)
            {
                if (key.Equals(paramTemp, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static String createLinkString(Hashtable param)
        {
            List<string> keys = new List<string>();

            foreach (string paramTemp in param.Keys)
            {
                keys.Add(paramTemp);
            }
            keys.Sort();

            string prestr = "";

            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                string value = ((string)param[key]);

                if (i == keys.Count - 1)
                {
                    prestr = prestr + key + "=" + value;
                }
                else
                {
                    prestr = prestr + key + "=" + value + "&";
                }
            }

            return (prestr);
        }

        public static Hashtable buildParamRefund(TransactionData transData)
        {
            Hashtable sParaTemp = new Hashtable();
            string privateKey = transData.PrivateKey;

            sParaTemp.Add("merchantId", transData.MerchantId);
            sParaTemp.Add("privateKey", privateKey);
            sParaTemp.Add("inputCharset", "UTF-8");
            sParaTemp.Add("notifyUrl", transData.NotifyUrl);
            sParaTemp.Add("merchantTradeId", transData.MerchantTradeId);
            sParaTemp.Add("version", "1.0");
            sParaTemp.Add("signType", "RSA");

            return sParaTemp;
        }

        public static Hashtable buildRequestRefund(Hashtable sParaTemp)
        {
            Hashtable sPara = paraFilter(sParaTemp, new String[] { "sign", "signType", "privateKey", "gateway" });

            String prestr = createLinkString(sPara); //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            String mysign = "";
            if (sParaTemp["signType"].ToString().Equals("RSA"))
            {
                mysign = RSAFromPkcs8.sign(prestr, sParaTemp["privateKey"].ToString(), sPara["inputCharset"].ToString());
            }
            if (sParaTemp["signType"].ToString().Equals("MD5"))
            {
                //mysign = DigestUtils.md5Hex((prestr + sParaTemp.get("privateKey")).getBytes(sPara.get("inputCharset")) );
            }

            sPara.Add("sign", mysign);
            sPara.Add("signType", sParaTemp["signType"].ToString());

            return sPara;
        }
    }
}

using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SDGUtil
{
    public static class Functions
    {
        #region Get Card Type

        public static int GetCardType(string cardnum)
        {
            //bool isCardNumValid = CardValidator.Luhn.IsValid(cardnum);

            //if (!isCardNumValid)
            //{
            //    return 0;
            //}

            int cardtypeid;

            if (cardnum.Length == 13)
            {
                if (cardnum.StartsWith("4"))
                {
                    //VISA
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.Visa);
                }
                else
                {
                    //Invalid card
                    cardtypeid = 0;
                }
            }
            else if (cardnum.Length == 14)
            {
                if (cardnum.StartsWith("300") || cardnum.StartsWith("301") || cardnum.StartsWith("302") || cardnum.StartsWith("303") || cardnum.StartsWith("304") || cardnum.StartsWith("305"))
                {
                    //Diner club carte blanche
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.Diners);
                }
                else if (cardnum.StartsWith("36") || cardnum.StartsWith("38"))
                {
                    //Diner club international
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.Diners);
                }
                else
                {
                    //Invalid card
                    cardtypeid = 0;
                }
            }
            else if (cardnum.Length == 15)
            {
                if (cardnum.StartsWith("34") || cardnum.StartsWith("37"))
                {
                    //AMEX
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.AmericanExpress);
                }
                else if (cardnum.StartsWith("2014") || cardnum.StartsWith("2149"))
                {
                    //Diner club enRoute
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.Diners);
                }
                else
                {
                    //Invalid card
                    cardtypeid = 0;
                }
            }
            else if (cardnum.Length == 16)
            {
                if (cardnum.StartsWith("62"))
                {
                    //China UnionPay
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.ChinaUnionPay);
                }
                else if (cardnum.StartsWith("6011") || cardnum.StartsWith("6221") || cardnum.StartsWith("6222") || cardnum.StartsWith("6223") || cardnum.StartsWith("6224") || cardnum.StartsWith("6225") || cardnum.StartsWith("6226") || cardnum.StartsWith("6227") || cardnum.StartsWith("6228") || cardnum.StartsWith("6229") || cardnum.StartsWith("65"))
                {
                    //Discover Card
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.Discover);
                }
                else if (cardnum.StartsWith("3528") || cardnum.StartsWith("3529") || cardnum.StartsWith("353") || cardnum.StartsWith("354") || cardnum.StartsWith("355") || cardnum.StartsWith("356") || cardnum.StartsWith("357") || cardnum.StartsWith("358"))
                {
                    //JCB
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.JCB);
                }
                else if (cardnum.StartsWith("51") || cardnum.StartsWith("52") || cardnum.StartsWith("53") || cardnum.StartsWith("54") || cardnum.StartsWith("55") || cardnum.StartsWith("67")
                    || cardnum.StartsWith("22") || cardnum.StartsWith("23") || cardnum.StartsWith("24") || cardnum.StartsWith("25") || cardnum.StartsWith("26") || cardnum.StartsWith("27"))
                {
                    //Master Card
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.MasterCard);
                }
                else if (cardnum.StartsWith("4"))
                {
                    //VISA
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.Visa);
                }
                else
                {
                    //Invalid card
                    cardtypeid = 0;
                }
            }
            else if (cardnum.Length == 17 || cardnum.Length == 18 || cardnum.Length == 19)
            {
                if (cardnum.StartsWith("62"))
                {
                    //China UnionPay
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.ChinaUnionPay);
                }
                else if(cardnum.StartsWith("67"))
                {
                    //MasterCard Maestro
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.MasterCard);
                }
                else
                {
                    //Invalid card
                    cardtypeid = 0;
                }
            }
            else
            {
                //Invalid card
                cardtypeid = 0;
            }

            return cardtypeid;
        }

        public static int GetCTCardType(string cardTypeCode)
        {
            int cardtypeid = 0;
            switch (cardTypeCode)
            {
                case "V":
                    //VISA
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.Visa);
                    break;

                case "M":
                    //Mastercard
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.MasterCard);
                    break;

                case "A":
                    //American Express
                    cardtypeid = Convert.ToInt32(SDGDAL.Enums.CardTypes.AmericanExpress);
                    break;

                default:
                    cardtypeid = 0;
                    break;
            }

            return cardtypeid;
        }

        #endregion Get Card Type

        #region Hash Card Number

        public static string HashCardNumber(string cardNum)
        {
            try
            {
                //int cardlength = cardNum.Length - 16;
                //get 16 char
                string lastfourCC = cardNum.Substring(cardNum.Length - 4, 4);
                string hash = "XXXX-XXXX-XXXX-";
                return hash + lastfourCC;
            }
            catch
            {
                return "XXXXXXXXXXXX";
            }
        }

        public static string HashCheckAccountNum(string accountNum)
        {
            try
            {
                int length = accountNum.Length;
                int x = length / 3;
                int y = length - x;

                string newNum = accountNum.Substring(y);
                string hash = "";
                for (int i = 0; i < y; i++)
                {
                    hash += "*";
                }

                return hash + newNum;
            }
            catch
            {
                return "**********";
            }
        }

        #endregion Hash Card Number

        public static string Format_Datetime(DateTime dateTime)
        {
            try
            {
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                return Convert.ToString(dateTime);
            }
        }

        // All input strings must be POSITIVE numbers (in string format)
        public static decimal ConvertNumeric(string input)
        {
            try
            {
                return Convert.ToDecimal(input);
            }
            catch
            {
                return -1;
            }
        }

        /**********************************************************
         *   Can Convert String To Decimal
         * -------------------------------------------------------
         *  in  : string sInput
         *  out : boolean canConvert
         *
         * ********************************************************/

        public static Boolean CanConvertStringToDecimal(string sInput)
        {
            Boolean canConvert = false;

            try
            {
                decimal dDeciaml = Convert.ToDecimal(sInput);
                canConvert = true;
            }
            catch (Exception ex)
            {
                // for DEBUG
                string sDebug = ex.Message;

                canConvert = false;
            }

            return canConvert;
        }

        public static string ConvertCardTypeName(decimal Ref_CardType_ID)
        {
            string cardType;

            try
            {
                if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.AmericanExpress))
                {
                    cardType = "AMEX";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.Barclaycard))
                {
                    cardType = "Barclays";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.Cash))
                {
                    cardType = "Cash";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.Cheque))
                {
                    cardType = "ACH";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.ChinaUnionPay))
                {
                    cardType = "UnionPay";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.Citibank))
                {
                    cardType = "Citybank";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.Debit))
                {
                    cardType = "Debit";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.Diners))
                {
                    cardType = "Diners";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.Discover))
                {
                    cardType = "Discover";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.JCB))
                {
                    cardType = "JCB";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.MasterCard))
                {
                    cardType = "Master Card";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.Visa))
                {
                    cardType = "VISA";
                }
                else if (Ref_CardType_ID == Convert.ToDecimal(SDGDAL.Enums.CardTypes.EBT))
                {
                    cardType = "EBT";
                }
                else
                {
                    cardType = Convert.ToString(Ref_CardType_ID);
                }
            }
            catch
            {
                return Convert.ToString(Ref_CardType_ID);
            }

            return cardType;
        }

        public static string ConvertTransTypeName(decimal Ref_TransType_ID)
        {
            string transType;
            try
            {
                if (Ref_TransType_ID == Convert.ToDecimal(SDGDAL.Enums.TransactionType.BalanceInquiry))
                {
                    transType = "Balance Inquiry";
                }
                else if (Ref_TransType_ID == Convert.ToDecimal(SDGDAL.Enums.TransactionType.Capture))
                {
                    transType = "SALE";
                }
                else if (Ref_TransType_ID == Convert.ToDecimal(SDGDAL.Enums.TransactionType.CashBack))
                {
                    transType = "CASH BACK";
                }
                else if (Ref_TransType_ID == Convert.ToDecimal(SDGDAL.Enums.TransactionType.Sale))
                {
                    transType = "SALE";
                }
                else if (Ref_TransType_ID == Convert.ToDecimal(SDGDAL.Enums.TransactionType.Refund))
                {
                    transType = "REFUND";
                }
                else if (Ref_TransType_ID == Convert.ToDecimal(SDGDAL.Enums.TransactionType.Void))
                {
                    transType = "VOID";
                }
                else
                {
                    transType = Convert.ToString(Ref_TransType_ID);
                }
            }
            catch
            {
                return Convert.ToString(Ref_TransType_ID);
            }

            return transType;
        }

        public static string ConvertDecimalAmountWithFormat(decimal amount)
        {
            try
            {
                return String.Format("{0:0.00}", amount);
            }
            catch
            {
                return Convert.ToString(amount);
            }
        }

        public static string ConvertCurrencyISO(decimal Currency_ID)
        {
            try
            {
                return ((SDGDAL.Enums.CurrencyISOCode)Currency_ID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public static string ConvertDatetime(DateTime inputdate)
        {
            try
            {
                string format = "yyyy/MM/dd HH:mm:ss";
                return inputdate.ToString(format);
            }
            catch
            {
                return inputdate.ToString();
            }
        }

        public static string CalaculateBatchTotal(string origal, string newamount, string currency)
        {
            string newinput2 = newamount.Replace(currency, "");

            decimal dInput1 = Convert.ToDecimal(origal);
            decimal dInput2 = Convert.ToDecimal(newinput2);
            decimal total = dInput1 + dInput2;

            return ConvertDecimalAmountWithFormat(total);
        }

        public static string Base64Encode(string input)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(input);
            return System.Convert.ToBase64String(result);
        }

        public static string Base64Decode(string input)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static byte[] ConvertHexToByteArray(string hexString)
        {
            int bytesCount = (hexString.Length) / 2;
            byte[] bytes = new byte[bytesCount];
            for (int x = 0; x < bytesCount; ++x)
            {
                bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
            }

            return bytes;
        }

        public static string ConvertImageToHex(Image img)
        {
            byte[] byteArray = null;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, 0);
                byteArray = ms.ToArray();
            }
            string hexString = "";
            foreach (byte b in byteArray)
            {
                hexString += b.ToString("x2");
            }

            return hexString.Replace("-", "");
        }

        public static string GetIPAddress()
        {
            string strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            return ipAddress.ToString();
        }

        public static string GenerateSystemTraceAudit()
        {
            Random rnd = new Random();
            int value = rnd.Next(100000, 999999);

            return Convert.ToString(value);
        }

        public static string GenerateRefNumUsingDate()
        {
            return DateTime.Now.ToString("yyyy").Substring(3) + DateTime.Now.Date.DayOfYear.ToString() + DateTime.Now.ToString("hh");
        }

        public static string GenerateRetrievalNumber(int len)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < len; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }

        public static string ParseString(string data)
        {
            return "&" + data;
        }

        public static string ComputeMD5Hash(string data)
        {
            StringBuilder hashOutput = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytesHash = md5provider.ComputeHash(new UTF8Encoding().GetBytes(data));

            for (int i = 0; i < bytesHash.Length; i++)
            {
                hashOutput.Append(bytesHash[i].ToString("x2"));
            }

            return hashOutput.ToString();
        }

        public static string sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public static String getValue(String xml, String name)
        {
            if (string.IsNullOrWhiteSpace(xml))
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
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
            return "";
        }

        public static String getString(string src)
        {
            return (string.IsNullOrWhiteSpace(src) ? "" : (" " + src.Trim()));
        }

        public static int GetNumericCountryCode(string country)
        {
            int code = 0;

            switch(country)
            {
                case "AL": case "ALB":
                    code = 8;
                    break;

                case "DZ": case "DZA":
                    code = 12;
                    break;

                case "AS": case "ASM":
                    code = 16;
                    break;

                case "AD": case "AND":
                    code = 20;
                    break;

                case "AO": case "AGO":
                    code = 24;
                    break;

                case "AI": case "AIA":
                    code = 660;
                    break;

                case "AQ": case "ATA":
                    code = 10;
                    break;

                case "AG": case "ATG":
                    code = 28;
                    break;

                case "AR": case "ARG":
                    code = 32;
                    break;

                case "AM": case "ARM":
                    code = 51;
                    break;

                case "AW": case "ABW":
                    code = 533;
                    break;

                case "AU": case "AUS":
                    code = 36;
                    break;

                case "AT": case "AUT":
                    code = 40;
                    break;

                case "AZ": case "AZE":
                    code = 31;
                    break;

                case "BS": case "BHS":
                    code = 44;
                    break;

                case "BH": case "BHR":
                    code = 48;
                    break;

                case "BD": case "BGD":
                    code = 50;
                    break;

                case "BB": case "BRB":
                    code = 52;
                    break;

                case "BY": case "BLR":
                    code = 112;
                    break;

                case "BE": case "BEL":
                    code = 56;
                    break;

                case "BZ": case "BLZ":
                    code = 84;
                    break;

                case "BJ": case "BEN":
                    code = 204;
                    break;

                case "BM": case "BMU":
                    code = 60;
                    break;

                case "BT": case "BTN":
                    code = 64;
                    break;

                case "BO": case "BOL":
                    code = 68;
                    break;

                case "BA": case "BIH":
                    code = 70;
                    break;

                case "BW": case "BWA":
                    code = 76;
                    break;

                case "IO": case "IOT":
                    code = 86;
                    break;

                case "VG": case "VGB":
                    code = 92;
                    break;

                case "BN": case "BRN":
                    code = 96;
                    break;

                case "BG": case "BGR":
                    code = 100;
                    break;

                case "CA": case "CAN":
                    code = 124;
                    break;

                case "CN":case "CHN":
                    code = 124;
                    break;

                case "HK":case "HKG":
                    code = 344;
                    break;

                case "IN": case "IND":
                    code = 356;
                    break;

                case "ID": case "IDN":
                    code = 360;
                    break;

                case "PH": case "PHL":
                    code = 608;
                    break;
            }

            return code;
        }
    }
}
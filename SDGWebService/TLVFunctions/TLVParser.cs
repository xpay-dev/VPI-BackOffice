using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDGDAL.Repositories;

namespace SDGWebService.TLVFunctions
{
    public class TLVParser
    {
        public static ClassTLV DecodeTLV(string tlv)
        {
            DebitSystemTraceNumRepository _traceNumberRepo = new DebitSystemTraceNumRepository();

            #region EMV Request Data

            string amount;
            string appCryptogram;
            string appProfile;
            string atc;
            string cryptogramInfoData;
            string issuerData;
            string terminalCountryCode;
            string amountOther;
            string tvr;
            string currencyCode;
            string transactionDate;
            string transactionType;
            string unpredNumber;

            #endregion

            #region Emv Additional Request Data

            string panSeqNumber;
            string appVersionNumber;
            string cvm;
            string deviceSerialNumber;
            string posEntryMode;
            string terminalType;
            string transSeqCounter;
            string dedicatedFileName;

            #endregion

            #region Track Data
            string nameOnCard;
            string track2;
            string cardNumber = string.Empty;
            string expiryDate = string.Empty;
            string serviceCode = string.Empty;
            string discretionaryData = string.Empty;
            #endregion

            ClassTLV response = new ClassTLV();
            List<TrackData> listTrackData = new List<TrackData>();
            List<SubField1Data> listSubField1Data = new List<SubField1Data>();
            List<SubField2Data> listSubField2Data = new List<SubField2Data>();

            TLVTags tag = new TLVTags();
            List<TLV> tlvList = TLVParser.parse(tlv);

            if(tlvList.Count() == 1)
            {
                tlvList = tlvList[0].tlvList;
            }
            ///EMV Request Data
            atc = TLVParser.searchTLV(tlvList, tag.AppTransactionCounter);
            appCryptogram = TLVParser.searchTLV(tlvList, tag.ApplicationCryptogram);
            cryptogramInfoData = TLVParser.searchTLV(tlvList, tag.CryptogramInfoData);
            issuerData = TLVParser.searchTLV(tlvList, tag.IssuerAppData);
            unpredNumber = TLVParser.searchTLV(tlvList, tag.UnpredictableNumber);
            tvr = TLVParser.searchTLV(tlvList, tag.TerminalVerificationResult);
            transactionDate = TLVParser.searchTLV(tlvList, tag.TransactionDate);
            amount = TLVParser.searchTLV(tlvList, tag.Amount);
            transactionType = TLVParser.searchTLV(tlvList, tag.TransactionType);
            currencyCode = TLVParser.searchTLV(tlvList, tag.CurrencyCode);
            appProfile = TLVParser.searchTLV(tlvList, tag.ApplicationProfile);
            terminalCountryCode = TLVParser.searchTLV(tlvList, tag.TerminalCountryCode);
            amountOther = TLVParser.searchTLV(tlvList, tag.AmountCashback);

            ///Additional EMV Request Data
            panSeqNumber = TLVParser.searchTLV(tlvList, tag.PanSeqNumber);
            appVersionNumber = TLVParser.searchTLV(tlvList, tag.AppVersion);
            cvm = TLVParser.searchTLV(tlvList, tag.CardVerificationMethod);
            deviceSerialNumber = TLVParser.searchTLV(tlvList, tag.SerialNumber);
            posEntryMode = TLVParser.searchTLV(tlvList, tag.PosEntryMode);
            terminalType = TLVParser.searchTLV(tlvList, tag.TerminalType);
            transSeqCounter = TLVParser.searchTLV(tlvList, tag.TransactionSeqCounter);
            dedicatedFileName = TLVParser.searchTLV(tlvList, tag.DedicatedFileName);

            ///Track Data
            track2 = TLVParser.searchTLV(tlvList, tag.Track2);

            #region Tap Data if Null
            if (cryptogramInfoData == null)
                cryptogramInfoData = "80";
            if (appVersionNumber == null)
                appVersionNumber = "0000";
            if (cvm == null)
                cvm = "000000";
            if (deviceSerialNumber == null)
                deviceSerialNumber = "0".PadRight(8, '0');
            if (posEntryMode == null)
                posEntryMode = "05";
            if (terminalType == null)
                terminalType = "00";
            if (transSeqCounter == null)
                transSeqCounter = _traceNumberRepo.GenerateSystemTraceNumber().PadRight(8, '0');
            if (dedicatedFileName == null)
                dedicatedFileName = "".PadRight(32, ' ');

            #endregion

            if(track2 != null)
            {
                int end = track2.IndexOf("F");
                int start = track2.IndexOf("D") + 8;
                cardNumber = track2.Substring(0, track2.IndexOf("D"));
                expiryDate = track2.Substring(track2.IndexOf("D") + 1, 4);
                serviceCode = track2.Substring(track2.IndexOf("D") + 5, 3);
                discretionaryData = track2.Substring(start).Replace("F", "");

                nameOnCard = TLVParser.searchTLV(tlvList, tag.NameOnCard);

                listTrackData.Add(new TrackData()
                {
                    CardNumber = cardNumber,
                    ExpiryDate = expiryDate,
                    ServiceCode = serviceCode,
                    DiscretionaryData = discretionaryData
                });

                listSubField1Data.Add(new SubField1Data()
                {
                    Amount = amount,
                    AmountOther = amountOther,
                    AppInterchangeProfile = appProfile,
                    ApplicationCryptogram = appCryptogram,
                    Atc = atc,
                    CryptogramInfoData = cryptogramInfoData,
                    IssuerData = issuerData.PadRight(64, ((char)0x20)),
                    TerminalCountryCode = terminalCountryCode.Substring(1, 3),
                    TerminalVerificationResults = tvr,
                    TransCurrencyCode = currencyCode.Substring(1, 3),
                    TransDate = transactionDate,
                    TransType = transactionType,
                    UnpredictableNumber = unpredNumber
                });

                listSubField2Data.Add(new SubField2Data()
                {
                    AppVersionNumber = appVersionNumber,
                    Cvm = cvm,
                    DedicatedFileName = dedicatedFileName.PadRight(32, ((char)0x20)),
                    InterfaceSerialNumber = ParseTagData(deviceSerialNumber),
                    PanSeqNumber = panSeqNumber,
                    PosEntryMode = posEntryMode,
                    TerminalType = terminalType,
                    TransSequenceCounter = transSeqCounter
                });

                response.TrackData = listTrackData;
                response.SubField1Data = listSubField1Data;
                response.SubField2Data = listSubField2Data;

                response.Track2 = track2;
                response.CardNumber = cardNumber;
                response.NameOnCard = nameOnCard;
                response.EmvIccData = appCryptogram + cryptogramInfoData + issuerData + unpredNumber + tvr + transactionDate + amount + transactionType + currencyCode + appProfile + terminalCountryCode;
                response.ExpiryYear = expiryDate.Substring(0, 2);
                response.ExpiryMonth = expiryDate.Substring(2);
            }
            else
            {
                response.Track2 = null;
                response.CardNumber = null;
                response.ExpiryMonth = null;
                response.ExpiryYear = null;
            }

            return response;
        }

        private static string ParseTagData(string deviceSerialNumber)
        {
            string sn = string.Empty;
            for (int i = 0; i < deviceSerialNumber.Length; i++)
            {
                if (i % 2 != 0)
                {
                    string r = deviceSerialNumber[i].ToString();
                    sn = sn + r;
                }
            }

            return sn;
        }

        public static List<TLV> parse(String tlv)
        {
            try
            {
                return getTLVList(hexToByteArray(tlv));
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static List<TLV> getTLVList(byte[] data)
        {
            int index = 0;

            List<TLV> tlvList = new List<TLV>();

            try
            {
                while (index < data.Length)
                {

                    byte[] tag;
                    byte[] length;
                    byte[] value;

                    bool isNested;
                    if ((data[index] & (byte)0x20) == (byte)(0x20))
                    {
                        isNested = true;
                    }
                    else
                    {
                        isNested = false;
                    }

                    if ((data[index] & (byte)0x1F) == (byte)(0x1F))
                    {
                        int lastByte = index + 1;
                        while ((data[lastByte] & (byte)0x80) == (byte)0x80)
                        {
                            ++lastByte;
                        }
                        tag = new byte[lastByte - index + 1];
                        Array.Copy(data, index, tag, 0, tag.Length);
                        index += tag.Length;
                    }
                    else
                    {
                        tag = new byte[1];
                        tag[0] = data[index];
                        ++index;

                        if (tag[0] == 0x00)
                        {
                            break;
                        }
                    }

                    if ((data[index] & (byte)0x80) == (byte)(0x80))
                    {
                        int n = (data[index] & (byte)0x7F) + 1;
                        length = new byte[n];
                        Array.Copy(data, index, length, 0, length.Length);
                        index += length.Length;
                    }
                    else
                    {
                        length = new byte[1];
                        length[0] = data[index];
                        ++index;
                    }

                    int num = getLengthInt(length);
                    num = Math.Min(num, data.Length - index);
                    value = new byte[num];
                    Array.Copy(data, index, value, 0, value.Length);
                    index += value.Length;

                    TLV tlv = new TLV();
                    tlv.tag = toHexString(tag);
                    tlv.length = toHexString(length);
                    tlv.value = toHexString(value);
                    tlv.isNested = isNested;

                    if (isNested)
                    {
                        tlv.tlvList = getTLVList(value);
                    }

                    tlvList.Add(tlv);
                }
            }
            catch (Exception e)
            {
            }

            return tlvList;
        }

        private static int getLengthInt(byte[] data)
        {
            if ((data[0] & (byte)0x80) == (byte)(0x80))
            {
                int n = data[0] & (byte)0x7F;
                int length = 0;
                for (int i = 1; i < n + 1; ++i)
                {
                    length <<= 8;
                    length |= (data[i] & 0xFF);
                }
                return length;
            }
            else
            {
                return data[0] & 0xFF;
            }
        }

        public static byte[] hexToByteArray(String data)
        {
            byte[] result;

            result = new byte[data.Length / 2];

            for (int i = 0; i < data.Length; i += 2)
            {
                String t1 = data.Substring(i, 2);

                result[i / 2] = Convert.ToByte(data.Substring(i, 2), 16);
            }

            return result;
        }

        protected static String toHexString(byte[] b)
        {
            String result = "";
            for (int i = 0; i < b.Length; i++)
            {
                result += Convert.ToString((b[i] & 0xFF) + 0x100, 16).Substring(1);
            }
            return result;
        }

        public static string searchTLV(List<TLV> tlvList, String targetTag)
        {
            for (int i = 0; i < tlvList.Count(); ++i)
            {
                TLV tlv = tlvList.ElementAt(i);
                if (tlv.tag.Equals(targetTag.ToLower()))
                {
                    return tlv.value.ToUpper();
                }
            }
            return null;
        }

        public class TLV
        {
            public String tag;
            public String length;
            public String value;

            public bool isNested;
            public List<TLV> tlvList;
        }

        public static ClassTLV ParseWisePadTrack2(string decryptedTrack2)
        {
            ClassTLV response = new ClassTLV();
            List<TrackData> listTrackData = new List<TrackData>();

            try
            {
                ///Parse TLV Track2
                decryptedTrack2 = decryptedTrack2.TrimStart('B');
                int end = decryptedTrack2.IndexOf("F");
                int start = decryptedTrack2.IndexOf("D") + 8;
                string cardNumber = decryptedTrack2.Substring(0, decryptedTrack2.IndexOf("D"));
                string expiryDate = decryptedTrack2.Substring(decryptedTrack2.IndexOf("D") + 1, 4);
                string serviceCode;
                string discretionaryData;

                try
                {
                    serviceCode = decryptedTrack2.Substring(decryptedTrack2.IndexOf("D") + 5, 3).Replace("F", "0");
                }
                catch (Exception ex)
                {
                    serviceCode = "000";
                }

                try
                {
                    discretionaryData = decryptedTrack2.Substring(start, end - start);
                }
                catch (Exception ex)
                {
                    discretionaryData = "000000";
                }

                listTrackData.Add(new TrackData()
                {
                    CardNumber = cardNumber,
                    ExpiryDate = expiryDate,
                    ServiceCode = serviceCode,
                    DiscretionaryData = discretionaryData
                });

                response.TrackData = listTrackData;
                response.CardNumber = cardNumber;
                response.Track2 = ";" + cardNumber + "=" + expiryDate + serviceCode + discretionaryData + "?";

                return response;
            }
            catch(Exception ex)
            {
                response.Track2 = null;

                return response;
            }
        }

        public static ClassTLV ParseQPOSTrack2(string decryptedTrack2)
        {
            ClassTLV response = new ClassTLV();
            List<TrackData> listTrackData = new List<TrackData>();

            try
            {
                decryptedTrack2 = decryptedTrack2.Replace("F", "");
                int start = decryptedTrack2.IndexOf("D");
                string cardNumber = decryptedTrack2.Substring(0, decryptedTrack2.IndexOf("D"));
                string expiryDate = decryptedTrack2.Substring(decryptedTrack2.IndexOf("D") + 1, 4);
                string serviceCode;
                string discretionaryData;

                try
                {
                    serviceCode = decryptedTrack2.Substring(decryptedTrack2.IndexOf("D") + 5, 3).Replace("F", "0");
                }
                catch (Exception ex)
                {
                    serviceCode = "000";
                }

                try
                {
                    discretionaryData = decryptedTrack2.Substring(start + 8, decryptedTrack2.Length - (start + 8));
                }
                catch (Exception ex)
                {
                    discretionaryData = "000000";
                }

                listTrackData.Add(new TrackData()
                {
                    CardNumber = cardNumber,
                    ExpiryDate = expiryDate,
                    ServiceCode = serviceCode,
                    DiscretionaryData = discretionaryData
                });

                response.TrackData = listTrackData;
                response.CardNumber = cardNumber;
                response.Track2 = ";" + decryptedTrack2.Replace('D', '=') + "?";

                return response;
            }
            catch (Exception ex)
            {
                response.Track2 = null;

                return response;
            }
        }
    }
}

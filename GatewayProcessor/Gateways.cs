using CT_EMV_CLASSES;
using CT_EMV_CLASSES.DOWNLOAD;
using CT_EMV_CLASSES.FINANCIAL;
using CTPaymentUtilities;
using HPA_ISO8583;
using SDGDAL;
using SDGUtil;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using GatewayProcessor.NovaPay;
using GatewayProcessor.TrustPay;
using GatewayProcessor.PayOnline;
using GatewayProcessor.Payeco;
using System.Collections.Generic;
using Newtonsoft.Json;
using SDGUtil.RSA;
using System.Collections;
using System.Collections.Specialized;
using SDGUtil.Utility;
using System.Security.Cryptography.X509Certificates;
using GatewayProcessor.VeritasPayment;

namespace GatewayProcessor
{
    public class Gateways
    {
        #region CTPayment Gateway

        public async Task<HOST_TERMINAL_DOWNLOAD> DownloadEMVHostAndTerminals(HEADER_REQUEST input)
        {
            HOST_TERMINAL_DOWNLOAD output = new HOST_TERMINAL_DOWNLOAD();

            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 3, 00);
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["CTPaymentTransaction"].ToString());

                HttpResponseMessage httpResponse = client.PostAsJsonAsync("emv/VeritasPay/ctpayment/hostparamdownload/", input.GetRequestHeader()).Result;

                output = await httpResponse.Content.ReadAsAsync<HOST_TERMINAL_DOWNLOAD>();

                return output;
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "DownloadEMVHostAndTerminals", ex.StackTrace);
            }

            return null;
        }

        public async Task<CTPaymentGateway.CTServiceResponse> ProcessCTPaymentCreditTransaction(string transData, string process)
        {
            CTPaymentGateway.CTServiceResponse response = new CTPaymentGateway.CTServiceResponse();
            CTPaymentGateway.ApiResponse apiResponse = new CTPaymentGateway.ApiResponse();

            try
            {
                #region CTPayment Purchase/Void

                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 3, 00);
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["CTPaymentTransaction"].ToString());

                HttpResponseMessage httpResponse = client.PostAsJsonAsync("emv/VeritasPay/ctpayment/transaction/", transData).Result;

                apiResponse = await httpResponse.Content.ReadAsAsync<CTPaymentGateway.ApiResponse>();

                EMV_RESPONSE emvResponse = new EMV_RESPONSE(apiResponse.ResponseMessage);

                if (apiResponse.ErrorNumber == "000")
                {
                    response.Status = "Approved";
                    response.Message = apiResponse.ErrorMessage;
                    response.AcqResponseCode = apiResponse.ErrorNumber;
                    response.Amount = Convert.ToDecimal(emvResponse.TransAmount1.TA);
                    response.AuthorizeId = emvResponse.ApprovalCode.ApprovalCodeField;
                    response.BatchNumber = emvResponse.Settlement.Settlement;
                    response.ReceiptNumber = emvResponse.HeaderResponse.SeqNo;
                    response.TransactionNumber = emvResponse.ApprovalCode.ApprovalCodeField;
                    response.PosEntryMode = Convert.ToInt32(emvResponse.HeaderResponse.POSEntryMode);
                    response.TerminalId = emvResponse.HeaderResponse.TID;
                }
                else
                {
                    response.Status = "Declined";
                    response.Message = apiResponse.ErrorMessage;
                    response.AcqResponseCode = apiResponse.ErrorNumber;
                    response.Amount = Convert.ToDecimal(emvResponse.TransAmount1.TA);

                    response.AuthorizeId = string.Empty;
                    response.BatchNumber = string.Empty;
                    response.ReceiptNumber = string.Empty;
                    response.PosEntryMode = 02;
                    response.TerminalId = string.Empty;

                    ApplicationLog.LogCTPaymentError("Gateway-CTPayment", process, transData, apiResponse.ErrorNumber + "-" + apiResponse.ErrorMessage);
                }

                #endregion CTPayment Purchase/Void
            }catch (Exception ex)
            {
                response.Message = "Transaction Failed while trying to connect to CTPayment API. " + ex.Message;
                ApplicationLog.LogCTPaymentError("Gateway-CTPayment", process, transData, response.Message + "-" + ex.Message);
            }

            return response;
        }

        #region CTPayment Card Not Present Transaction

        public async Task<AckOutput> AcceptCTAcknowledge(string transNumber)
        {
            AckOutput response = new AckOutput();

            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 3, 00);
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["CTPaymentURL"].ToString());

                HttpResponseMessage httpResponse = client.PostAsJsonAsync("VeritasPay/ctpayment/Acknowledge/", transNumber).Result;

                response = await httpResponse.Content.ReadAsAsync<AckOutput>();
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "AcceptCTAcknowledge", ex.StackTrace);
            }

            return response;
        }

        public async Task<TransactionOutput> ProcessCTPaymentTransaction(String secureId)
        {
            TransactionOutput output = new TransactionOutput();

            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 3, 00);
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["CTPaymentURL"].ToString());

                HttpResponseMessage httpResponse = client.PostAsJsonAsync("VeritasPay/ctpayment/Transaction/", secureId).Result;

                output = await httpResponse.Content.ReadAsAsync<TransactionOutput>();
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessCTPaymentTransaction", ex.StackTrace);
            }

            return output;
        }

        public async Task<TransactionOutput> ProcessCTPaymentPurchase(RedirectInput input)
        {
            TransactionOutput output = new TransactionOutput();

            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 3, 00);
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["CTPaymentURL"].ToString());

                HttpResponseMessage httpResponse = client.PostAsJsonAsync("VeritasPay/ctpayment/Purchase/", input).Result;

                output = await httpResponse.Content.ReadAsAsync<TransactionOutput>();
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessCTPaymentPurchase", ex.StackTrace);
            }

            return output;
        }

        #endregion CTPayment Card Not Present Transaction

        public async Task<CTPaymentGateway.EmvServiceResponse> ProcessCTPaymentCreditEMVTransactionMIGS(MasterCard.TransactionData transactionData, string transData, string process)
        {
            CTPaymentGateway.EmvServiceResponse response = new CTPaymentGateway.EmvServiceResponse();

            try
            {
                #region CTPayment EMV Purchase

                if (process.ToLower().Trim().Equals("purchase"))
                {
                    #region MarterCard Purchase DEMO

                    if (process.ToLower().Trim().Equals("purchase"))
                    {
                        string entryMode = "05";
                        string version = "&vpc_Version=1";
                        string command = "&vpc_Command=pay";
                        string accesscode = "&vpc_AccessCode=" + transactionData.AccessCode;
                        string merchtxnref = "&vpc_MerchTxnRef=" + transactionData.MerchTxnRef;
                        string merchant = "&vpc_Merchant=" + transactionData.MerchantId;
                        string orderinfo = "&vpc_OrderInfo=" + transactionData.OrderInfo;
                        string amount = "&vpc_Amount=" + transactionData.Amount;
                        string cardnum = "&vpc_CardNum=" + transactionData.CardNumber;
                        string cardexp = "&vpc_CardExp=" + transactionData.CardExpirationDate;
                        string currency = "&vpc_Currency=" + transactionData.Currency;
                        string track2 = "&vpc_CardTrack2=" + HttpUtility.UrlEncode(transactionData.Track2) + "X";
                        string posentrymode = "&vpc_POSEntryMode=" + entryMode;
                        string cardSeq = "&vpc_CardSeqNum=004";
                        string terminalCap = "&vpc_TerminalInputCapability=CM";
                        string terminalId = "&vpc_TerminalID=" + transactionData.TerminalId;
                        string iccData = "%vpc_EMVICCData=" + Functions.Base64Encode(transData);

                        UnicodeEncoding UE = new UnicodeEncoding();
                        byte[] hashvalue;
                        byte[] msg = UE.GetBytes(transactionData.SecureHash);
                        SHA256Managed hs = new SHA256Managed();

                        string hex = "";

                        hashvalue = hs.ComputeHash(msg);
                        foreach (byte x in hashvalue)
                        {
                            hex += String.Format("{0:x2}", x);
                        }

                        //plugged in the SHA256 encrypted hex value in the securehash-zo
                        string securehash = "&vpc_SecureHash=" + hex;
                        string securehashtype = "&vpc_SecureHashType=SHA256";

                        string url = System.Configuration.ConfigurationManager.AppSettings["MasterCardDemoURL"].ToString();
                        string postData;

                        postData = version + command + accesscode + merchtxnref + merchant + orderinfo + amount + cardnum + cardexp + currency + securehash + securehashtype + posentrymode + track2 + terminalId + cardSeq + iccData;

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                        request.Proxy = null;
                        request.Method = "POST";
                        request.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                        byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = byteArray.Length;
                        Stream dataStream = request.GetRequestStream();
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();

                        WebResponse webResponse = request.GetResponse();

                        string res_status = ((HttpWebResponse)webResponse).StatusDescription;

                        dataStream = webResponse.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        reader.Close();
                        dataStream.Close();
                        webResponse.Close();

                        //read response
                        try
                        {
                            //CONVERSION of server response to XML
                            string responseXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<MC_RESPONSE " + responseFromServer.Replace("\u003d", "\u003d\"").Replace("\u0026", "\"\n ") + "\"/>\n";

                            StringBuilder output = new StringBuilder();
                            using (XmlReader xmlreader = XmlReader.Create(new StringReader(responseXML)))
                            {
                                xmlreader.ReadToFollowing("MC_RESPONSE");
                                //reading from the xml-ified version of server response..
                                //then filling in the data in the response data class for Mastercard
                                response.Command = xmlreader.GetAttribute("vpc_Command");
                                response.MerchTxnRef = xmlreader.GetAttribute("vpc_MerchTxnRef");
                                response.MerchantId = xmlreader.GetAttribute("vpc_Merchant");
                                response.OrderInfo = xmlreader.GetAttribute("vpc_OrderInfo");
                                response.Amount = Convert.ToDecimal(xmlreader.GetAttribute("vpc_Amount"));
                                response.Locale = xmlreader.GetAttribute("vpc_Locale");
                                response.Message = xmlreader.GetAttribute("vpc_Message");
                                response.TransactionNumber = xmlreader.GetAttribute("vpc_TransactionNo");
                                response.TransactionResponseCode = xmlreader.GetAttribute("vpc_TxnResponseCode");
                                response.ReceiptNumber = xmlreader.GetAttribute("vpc_ReceiptNo");
                                response.AcqResponseCode = xmlreader.GetAttribute("vpc_AcqResponseCode");
                                response.BatchNumber = xmlreader.GetAttribute("vpc_BatchNo");
                                response.AuthorizeId = xmlreader.GetAttribute("vpc_AuthorizeId");
                                response.Card = xmlreader.GetAttribute("vpc_Card");

                                //fields that were not covered in the Basic output fields:
                                response.AVSResultCode = xmlreader.GetAttribute("vpc_AVSResultCode");
                                response.AcqAVSResponseCode = xmlreader.GetAttribute("vpc_AcqAVSRespCode");
                                response.AcqCSCResponseCode = xmlreader.GetAttribute("vpc_AcqCSCRespCode");
                                response.CSCResultCode = xmlreader.GetAttribute("vpc_CSCResultCode");
                                //Save EntryMode
                                response.PosEntryMode = Convert.ToInt32(entryMode);

                                if (response.TransactionResponseCode.Equals("0"))
                                {
                                    response.Status = "Approved";
                                }
                                else
                                {
                                    response.Status = "Declined";
                                }

                                response.Message = GetResponseMessage(response.AcqResponseCode, response.Message);

                                response.Message = response.Message.Replace("+", " ").Replace("%2F", "/") + " " + "Response Code: " + response.TransactionResponseCode;
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Message = res_status + " " + responseFromServer;
                            response.Status = "Declined";

                            ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessMasterCard Demo Purchase", ex.StackTrace);
                        }

                        return response;
                    }

                    #endregion MarterCard Purchase DEMO
                }

                #endregion CTPayment EMV Purchase
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessCTPaymentEMVTransaction", ex.StackTrace);
            }

            return response;
        }

        #endregion

        #region MasterCard Gateway

        #region MasterCard Live

        public MasterCard.ServiceResponse ProcessMasterCard(MasterCard.TransactionData transactionData, MasterCard.APILogin apiLogin, string process, int cardTypeId)
        {
            MasterCard.ServiceResponse response = new MasterCard.ServiceResponse();

            try
            {
                #region MasterCard Void LIVE

                if (process.ToLower().Trim().Equals("void"))
                {
                    string version = "&vpc_Version=1";
                    string command = "&vpc_Command=voidPurchase";
                    string merchtxnref = "&vpc_MerchTxnRef=" + transactionData.MerchTxnRef;
                    string accesscode = "&vpc_AccessCode=" + transactionData.AccessCode;
                    string merchant = "&vpc_Merchant=" + transactionData.MerchantId;
                    string orderinfo = "&vpc_TransNo=" + transactionData.TransNumber;
                    string username = "&vpc_User=" + apiLogin.Username;
                    string password = "&vpc_Password=" + apiLogin.Password;
                    string authorisationrequest = "&vpc_ReturnAuthResponseData=Y";

                    UnicodeEncoding UE = new UnicodeEncoding();
                    byte[] hashvalue;
                    byte[] msg = UE.GetBytes(transactionData.SecureHash);
                    SHA256Managed hs = new SHA256Managed();

                    string hex = "";

                    hashvalue = hs.ComputeHash(msg);
                    foreach (byte x in hashvalue)
                    {
                        hex += String.Format("{0:x2}", x);
                    }

                    //plugged in the SHA256 encrypted hex value in the securehash-zo
                    string securehash = "&vpc_SecureHash=" + hex;
                    string securehashtype = "&vpc_SecureHashType=SHA256";

                    string url = System.Configuration.ConfigurationManager.AppSettings["MasterCardURL"].ToString();

                    string postData = version + command + merchtxnref + accesscode + merchant + orderinfo + username + password + authorisationrequest;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                    request.Proxy = null;
                    request.Method = "POST";
                    request.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse webResponse = request.GetResponse();

                    string res_status = ((HttpWebResponse)webResponse).StatusDescription;

                    dataStream = webResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                    webResponse.Close();

                    //read response
                    try
                    {
                        //CONVERSION of server response to XML
                        string responseXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<MC_RESPONSE " + responseFromServer.Replace("\u003d", "\u003d\"").Replace("\u0026", "\"\n ") + "\"/>\n";

                        StringBuilder output = new StringBuilder();
                        using (XmlReader xmlreader = XmlReader.Create(new StringReader(responseXML)))
                        {
                            xmlreader.ReadToFollowing("MC_RESPONSE");
                            //reading from the xml-ified version of server response..
                            //then filling in the data in the response data class for Mastercard
                            response.Command = xmlreader.GetAttribute("vpc_Command");
                            response.MerchTxnRef = xmlreader.GetAttribute("vpc_MerchTxnRef");
                            response.MerchantId = xmlreader.GetAttribute("vpc_Merchant");
                            response.OrderInfo = xmlreader.GetAttribute("vpc_CapturedAmount"); //amount voided
                            response.Amount = Convert.ToDecimal(xmlreader.GetAttribute("vpc_Amount"));
                            response.Locale = xmlreader.GetAttribute("vpc_Locale");
                            response.TransactionNumber = xmlreader.GetAttribute("vpc_TransactionNo");

                            response.Message = xmlreader.GetAttribute("vpc_Message");

                            response.TransactionResponseCode = xmlreader.GetAttribute("vpc_TxnResponseCode");
                            response.ReceiptNumber = xmlreader.GetAttribute("vpc_ReceiptNo");
                            response.AcqResponseCode = xmlreader.GetAttribute("vpc_AcqResponseCode");
                            response.BatchNumber = xmlreader.GetAttribute("vpc_BatchNo");
                            response.AuthorizeId = xmlreader.GetAttribute("vpc_TransactionNo"); //ticketing
                            response.Card = xmlreader.GetAttribute("vpc_Card");

                            //fields that were not covered in the Basic output fields:
                            response.AVSResultCode = xmlreader.GetAttribute("vpc_AVSResultCode");
                            response.AcqAVSResponseCode = xmlreader.GetAttribute("vpc_AcqAVSRespCode");
                            response.AcqCSCResponseCode = xmlreader.GetAttribute("vpc_AcqCSCRespCode");
                            response.CSCResultCode = xmlreader.GetAttribute("vpc_CSCResultCode");

                            if (response.TransactionResponseCode.Equals("0"))
                            {
                                response.Status = "Approved";
                            }
                            else
                            {
                                response.Status = "Declined";
                            }

                            response.Message = GetResponseMessage(response.AcqResponseCode, response.Message);

                            response.Message = response.Message.Replace("+", " ").Replace("%2F", "/") + " " + "Response Code: " + response.TransactionResponseCode;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = res_status + " " + responseFromServer;
                        response.Status = "Declined";

                        ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessMasterCard Void", ex.StackTrace);
                    }

                    return response;
                }

                #endregion MasterCard Void LIVE

                #region MasterCard Purchase LIVE

                else if (process.ToLower().Trim().Equals("purchase"))
                {
                    if (cardTypeId == 1 || string.IsNullOrEmpty(transactionData.LRC)) //Mastercard and Empty LRC
                    {
                        transactionData.LRC = "X";
                    }

                    string entryMode = "900";
                    string version = "&vpc_Version=1";
                    string command = "&vpc_Command=pay";
                    string accesscode = "&vpc_AccessCode=" + transactionData.AccessCode;
                    string merchtxnref = "&vpc_MerchTxnRef=" + transactionData.MerchTxnRef;
                    string merchant = "&vpc_Merchant=" + transactionData.MerchantId;
                    string orderinfo = "&vpc_OrderInfo=" + transactionData.OrderInfo;
                    string amount = "&vpc_Amount=" + transactionData.Amount;
                    string cardnum = "&vpc_CardNum=" + transactionData.CardNumber;
                    string cardexp = "&vpc_CardExp=" + transactionData.CardExpirationDate;
                    string currency = "&vpc_Currency=" + transactionData.Currency;
                    string csc = "&vpc_CardSecurityCode=" + transactionData.CSC;
                    string authorisationrequest = "&vpc_ReturnAuthResponseData=Y";
                    string posentrymode = "&vpc_POSEntryMode=" + entryMode;
                    string terminalId = "&vpc_TerminalID=" + transactionData.TerminalId;
                    string cardtrack2 = "&vpc_CardTrack2=" + HttpUtility.UrlEncode(transactionData.Track2) + transactionData.LRC;
                    string addendumData = "&vpc_AddendumData=" + HttpUtility.UrlEncode("/VER/1"
                                        + "//" + "PFI/" + transactionData.PFI
                                        + "//" + "ISO/" + transactionData.ISO
                                        + "//" + "SMI/" + transactionData.SMI
                                        + "//" + "PFN/" + transactionData.PFN
                                        + "//" + "SMN/" + transactionData.SMN
                                        + "//" + "MSA/" + transactionData.MSA
                                        + "//" + "MCI/" + transactionData.MCI
                                        + "//" + "MST/" + transactionData.MST
                                        + "//" + "MCO/" + transactionData.MCO
                                        + "//" + "MPC/" + transactionData.MPC
                                        + "//" + "MPP/" + transactionData.MPP
                                        + "//" + "MCC/" + transactionData.MCC + "/");

                    if (transactionData.Track2.Length == 40)
                    {
                        cardtrack2 = "&vpc_CardTrack2=" + HttpUtility.UrlEncode(transactionData.Track2);
                    }

                    UnicodeEncoding UE = new UnicodeEncoding();
                    byte[] hashvalue;
                    byte[] msg = UE.GetBytes(transactionData.SecureHash);
                    SHA256Managed hs = new SHA256Managed();

                    string hex = "";

                    hashvalue = hs.ComputeHash(msg);
                    foreach (byte x in hashvalue)
                    {
                        hex += String.Format("{0:x2}", x);
                    }

                    //plugged in the SHA256 encrypted hex value in the securehash-zo
                    string securehash = "&vpc_SecureHash=" + hex;
                    string securehashtype = "&vpc_SecureHashType=SHA256";

                    string url = System.Configuration.ConfigurationManager.AppSettings["MasterCardURL"].ToString();

                    if (transactionData.TerminalId == null || transactionData.TerminalId == string.Empty)
                    {
                        terminalId = string.Empty;
                    }

                    if (transactionData.PFI == null || transactionData.TerminalId == string.Empty)
                    {
                        addendumData = string.Empty;
                    }

                    string postData = version + command + accesscode + merchtxnref + merchant + orderinfo + amount + cardnum + cardexp + currency + securehash + securehashtype + authorisationrequest + posentrymode + cardtrack2 + addendumData + terminalId;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                    request.Proxy = null;
                    request.Method = "POST";
                    request.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse webResponse = request.GetResponse();

                    string res_status = ((HttpWebResponse)webResponse).StatusDescription;

                    dataStream = webResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                    webResponse.Close();

                    //read response
                    try
                    {
                        //CONVERSION of server response to XML
                        string responseXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<MC_RESPONSE " + responseFromServer.Replace("\u003d", "\u003d\"").Replace("\u0026", "\"\n ") + "\"/>\n";

                        StringBuilder output = new StringBuilder();
                        using (XmlReader xmlreader = XmlReader.Create(new StringReader(responseXML)))
                        {
                            xmlreader.ReadToFollowing("MC_RESPONSE");
                            //reading from the xml-ified version of server response..
                            //then filling in the data in the response data class for Mastercard
                            response.Command = xmlreader.GetAttribute("vpc_Command");
                            response.MerchTxnRef = xmlreader.GetAttribute("vpc_MerchTxnRef");
                            response.MerchantId = xmlreader.GetAttribute("vpc_Merchant");
                            response.OrderInfo = xmlreader.GetAttribute("vpc_OrderInfo");
                            response.Amount = Convert.ToDecimal(xmlreader.GetAttribute("vpc_Amount"));
                            response.Locale = xmlreader.GetAttribute("vpc_Locale");
                            response.Message = xmlreader.GetAttribute("vpc_Message");
                            response.TransactionNumber = xmlreader.GetAttribute("vpc_TransactionNo");
                            response.TransactionResponseCode = xmlreader.GetAttribute("vpc_TxnResponseCode");
                            response.ReceiptNumber = xmlreader.GetAttribute("vpc_ReceiptNo");
                            response.AcqResponseCode = xmlreader.GetAttribute("vpc_AcqResponseCode");
                            response.BatchNumber = xmlreader.GetAttribute("vpc_BatchNo");
                            response.AuthorizeId = xmlreader.GetAttribute("vpc_AuthorizeId");
                            response.Card = xmlreader.GetAttribute("vpc_Card");

                            //fields that were not covered in the Basic output fields:
                            response.AVSResultCode = xmlreader.GetAttribute("vpc_AVSResultCode");
                            response.AcqAVSResponseCode = xmlreader.GetAttribute("vpc_AcqAVSRespCode");
                            response.AcqCSCResponseCode = xmlreader.GetAttribute("vpc_AcqCSCRespCode");
                            response.CSCResultCode = xmlreader.GetAttribute("vpc_CSCResultCode");

                            //Save EntryMode
                            response.PosEntryMode = Convert.ToInt32(entryMode);

                            if (response.TransactionResponseCode.Equals("0"))
                            {
                                response.Status = "Approved";
                            }
                            else
                            {
                                response.Status = "Declined";
                            }

                            response.Message = GetResponseMessage(response.AcqResponseCode, response.Message);

                            response.Message = response.Message.Replace("+", " ").Replace("%2F", "/") + " " + "Response Code: " + response.TransactionResponseCode;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = res_status + " " + responseFromServer;
                        response.Status = "Declined";

                        ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessMasterCard Purchase", ex.StackTrace);
                    }

                    return response;
                }

                #endregion MasterCard Purchase LIVE
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessMasterCard", ex.StackTrace);
            }

            return response;
        }

        public MasterCard.ServiceResponse ProcessMasterCardDemo(MasterCard.TransactionData transactionData, MasterCard.APILogin apiLogin, string process, int cardTypeId)
        {
            MasterCard.ServiceResponse response = new MasterCard.ServiceResponse();

            try
            {
                #region MarterCard Purchase DEMO

                if (process.ToLower().Trim().Equals("purchase"))
                {
                    if (cardTypeId == 1 || string.IsNullOrEmpty(transactionData.LRC)) //Mastercard and Empty LRC
                    {
                        transactionData.LRC = "X";
                    }

                    string entryMode = "900";
                    string version = "&vpc_Version=1";
                    string command = "&vpc_Command=pay";
                    string accesscode = "&vpc_AccessCode=" + transactionData.AccessCode;
                    string merchtxnref = "&vpc_MerchTxnRef=" + transactionData.MerchTxnRef;
                    string merchant = "&vpc_Merchant=" + transactionData.MerchantId;
                    string orderinfo = "&vpc_OrderInfo=" + transactionData.OrderInfo;
                    string amount = "&vpc_Amount=" + transactionData.Amount;
                    string cardnum = "&vpc_CardNum=" + transactionData.CardNumber;
                    string cardexp = "&vpc_CardExp=" + transactionData.CardExpirationDate;
                    string currency = "&vpc_Currency=" + transactionData.Currency;
                    string track2 = "&vpc_CardTrack2=" + HttpUtility.UrlEncode(transactionData.Track2) + transactionData.LRC;
                    string posentrymode = "&vpc_POSEntryMode=" + entryMode;
                    string cardSeq = "&vpc_CardSeqNum=001";
                    string terminalCap = "&vpc_TerminalInputCapability=CM";
                    string terminalId = "&vpc_TerminalID=" + transactionData.TerminalId;
                    string addendumData = "&vpc_AddendumData=" + HttpUtility.UrlEncode("/VER/1"
                                        + "//" + "PFI/" + transactionData.PFI
                                        + "//" + "ISO/" + transactionData.ISO
                                        + "//" + "SMI/" + transactionData.SMI
                                        + "//" + "PFN/" + transactionData.PFN
                                        + "//" + "SMN/" + transactionData.SMN
                                        + "//" + "MSA/" + transactionData.MSA
                                        + "//" + "MCI/" + transactionData.MCI
                                        + "//" + "MST/" + transactionData.MST
                                        + "//" + "MCO/" + transactionData.MCO
                                        + "//" + "MPC/" + transactionData.MPC
                                        + "//" + "MPP/" + transactionData.MPP
                                        + "//" + "MCC/" + transactionData.MCC + "/");

                    UnicodeEncoding UE = new UnicodeEncoding();
                    byte[] hashvalue;
                    byte[] msg = UE.GetBytes(transactionData.SecureHash);
                    SHA256Managed hs = new SHA256Managed();

                    string hex = "";

                    hashvalue = hs.ComputeHash(msg);
                    foreach (byte x in hashvalue)
                    {
                        hex += String.Format("{0:x2}", x);
                    }

                    //plugged in the SHA256 encrypted hex value in the securehash-zo
                    string securehash = "&vpc_SecureHash=" + hex;
                    string securehashtype = "&vpc_SecureHashType=SHA256";

                    string url = System.Configuration.ConfigurationManager.AppSettings["MasterCardDemoURL"].ToString();

                    if (transactionData.TerminalId == null || transactionData.TerminalId == string.Empty)
                    {
                        terminalId = string.Empty;
                    }

                    if (transactionData.PFI == null || transactionData.PFI == string.Empty)
                    {
                        addendumData = string.Empty;
                    }

                    string postData = version + command + accesscode + merchtxnref + merchant + orderinfo + amount + cardnum + cardexp + currency + securehash + securehashtype + posentrymode + track2 + addendumData + terminalId;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                    request.Proxy = null;
                    request.Method = "POST";
                    request.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse webResponse = request.GetResponse();

                    string res_status = ((HttpWebResponse)webResponse).StatusDescription;

                    dataStream = webResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                    webResponse.Close();

                    //read response
                    try
                    {
                        //CONVERSION of server response to XML
                        string responseXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<MC_RESPONSE " + responseFromServer.Replace("\u003d", "\u003d\"").Replace("\u0026", "\"\n ") + "\"/>\n";

                        StringBuilder output = new StringBuilder();
                        using (XmlReader xmlreader = XmlReader.Create(new StringReader(responseXML)))
                        {
                            xmlreader.ReadToFollowing("MC_RESPONSE");
                            //reading from the xml-ified version of server response..
                            //then filling in the data in the response data class for Mastercard
                            response.Command = xmlreader.GetAttribute("vpc_Command");
                            response.MerchTxnRef = xmlreader.GetAttribute("vpc_MerchTxnRef");
                            response.MerchantId = xmlreader.GetAttribute("vpc_Merchant");
                            response.OrderInfo = xmlreader.GetAttribute("vpc_OrderInfo");
                            response.Amount = Convert.ToDecimal(xmlreader.GetAttribute("vpc_Amount"));
                            response.Locale = xmlreader.GetAttribute("vpc_Locale");
                            response.Message = xmlreader.GetAttribute("vpc_Message");
                            response.TransactionNumber = xmlreader.GetAttribute("vpc_TransactionNo");
                            response.TransactionResponseCode = xmlreader.GetAttribute("vpc_TxnResponseCode");
                            response.ReceiptNumber = xmlreader.GetAttribute("vpc_ReceiptNo");
                            response.AcqResponseCode = xmlreader.GetAttribute("vpc_AcqResponseCode");
                            response.BatchNumber = xmlreader.GetAttribute("vpc_BatchNo");
                            response.AuthorizeId = xmlreader.GetAttribute("vpc_AuthorizeId");
                            response.Card = xmlreader.GetAttribute("vpc_Card");

                            //fields that were not covered in the Basic output fields:
                            response.AVSResultCode = xmlreader.GetAttribute("vpc_AVSResultCode");
                            response.AcqAVSResponseCode = xmlreader.GetAttribute("vpc_AcqAVSRespCode");
                            response.AcqCSCResponseCode = xmlreader.GetAttribute("vpc_AcqCSCRespCode");
                            response.CSCResultCode = xmlreader.GetAttribute("vpc_CSCResultCode");
                            //Save EntryMode
                            response.PosEntryMode = Convert.ToInt32(entryMode);

                            if (response.TransactionResponseCode.Equals("0"))
                            {
                                response.Status = "Approved";
                            }
                            else
                            {
                                response.Status = "Declined";
                            }

                            response.Message = GetResponseMessage(response.AcqResponseCode, response.Message);

                            response.Message = response.Message.Replace("+", " ").Replace("%2F", "/") + " " + "Response Code: " + response.TransactionResponseCode;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = res_status + " " + responseFromServer;
                        response.Status = "Declined";

                        ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessMasterCard Demo Purchase", ex.StackTrace);
                    }

                    return response;
                }

                #endregion MarterCard Purchase DEMO

                #region MasterCard VOID Demo

                else if (process.ToLower().Trim().Equals("void"))
                {
                    string version = "&vpc_Version=1";
                    string command = "&vpc_Command=voidPurchase";
                    string merchtxnref = "&vpc_MerchTxnRef=" + transactionData.MerchTxnRef;
                    string accesscode = "&vpc_AccessCode=" + transactionData.AccessCode;
                    string merchant = "&vpc_Merchant=" + transactionData.MerchantId;
                    string orderinfo = "&vpc_TransNo=" + transactionData.TransNumber;
                    string username = "&vpc_User=" + apiLogin.Username;
                    string password = "&vpc_Password=" + apiLogin.Password;
                    string authorisationrequest = "&vpc_ReturnAuthResponseData=Y";

                    UnicodeEncoding UE = new UnicodeEncoding();
                    byte[] hashvalue;
                    byte[] msg = UE.GetBytes(transactionData.SecureHash);
                    SHA256Managed hs = new SHA256Managed();

                    string hex = "";

                    hashvalue = hs.ComputeHash(msg);
                    foreach (byte x in hashvalue)
                    {
                        hex += String.Format("{0:x2}", x);
                    }

                    //plugged in the SHA256 encrypted hex value in the securehash-zo
                    string securehash = "&vpc_SecureHash=" + hex;
                    string securehashtype = "&vpc_SecureHashType=SHA256";

                    string url = System.Configuration.ConfigurationManager.AppSettings["MasterCardDemoURL"].ToString();

                    string postData = version + command + merchtxnref + accesscode + merchant + orderinfo + username + password + authorisationrequest;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                    request.Proxy = null;
                    request.Method = "POST";
                    request.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse webResponse = request.GetResponse();

                    string res_status = ((HttpWebResponse)webResponse).StatusDescription;

                    dataStream = webResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                    webResponse.Close();

                    //read response
                    try
                    {
                        //CONVERSION of server response to XML
                        string responseXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<MC_RESPONSE " + responseFromServer.Replace("\u003d", "\u003d\"").Replace("\u0026", "\"\n ") + "\"/>\n";

                        StringBuilder output = new StringBuilder();
                        using (XmlReader xmlreader = XmlReader.Create(new StringReader(responseXML)))
                        {
                            xmlreader.ReadToFollowing("MC_RESPONSE");
                            //reading from the xml-ified version of server response..
                            //then filling in the data in the response data class for Mastercard
                            response.Command = xmlreader.GetAttribute("vpc_Command");
                            response.MerchTxnRef = xmlreader.GetAttribute("vpc_MerchTxnRef");
                            response.MerchantId = xmlreader.GetAttribute("vpc_Merchant");
                            response.OrderInfo = xmlreader.GetAttribute("vpc_CapturedAmount"); //amount voided
                            response.Amount = Convert.ToDecimal(xmlreader.GetAttribute("vpc_Amount"));
                            response.Locale = xmlreader.GetAttribute("vpc_Locale");
                            response.TransactionNumber = xmlreader.GetAttribute("vpc_TransactionNo");

                            response.Message = xmlreader.GetAttribute("vpc_Message");

                            response.TransactionResponseCode = xmlreader.GetAttribute("vpc_TxnResponseCode");
                            response.ReceiptNumber = xmlreader.GetAttribute("vpc_ReceiptNo");
                            response.AcqResponseCode = xmlreader.GetAttribute("vpc_AcqResponseCode");
                            response.BatchNumber = xmlreader.GetAttribute("vpc_BatchNo");
                            response.AuthorizeId = xmlreader.GetAttribute("vpc_TransactionNo"); //ticketing
                            response.Card = xmlreader.GetAttribute("vpc_Card");

                            //fields that were not covered in the Basic output fields:
                            response.AVSResultCode = xmlreader.GetAttribute("vpc_AVSResultCode");
                            response.AcqAVSResponseCode = xmlreader.GetAttribute("vpc_AcqAVSRespCode");
                            response.AcqCSCResponseCode = xmlreader.GetAttribute("vpc_AcqCSCRespCode");
                            response.CSCResultCode = xmlreader.GetAttribute("vpc_CSCResultCode");

                            if (response.TransactionResponseCode.Equals("0"))
                            {
                                response.Status = "Approved";
                            }
                            else
                            {
                                response.Status = "Declined";
                            }

                            response.Message = GetResponseMessage(response.AcqResponseCode, response.Message);

                            response.Message = response.Message.Replace("+", " ").Replace("%2F", "/") + " " + "Response Code: " + response.TransactionResponseCode;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = res_status + " " + responseFromServer;
                        response.Status = "Declined";

                        ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessMasterCard Demo Void", ex.StackTrace);
                    }

                    return response;
                }

                #endregion MasterCard VOID Demo
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessMasterCardDemo", ex.StackTrace);
            }

            return response;
        }

        #endregion

        #region MasterCard API Card Not Present

        public MasterCard.ServiceResponse ProcessMasterCardDemoEMVCardNotPresent(MasterCard.TransactionData transactionData, MasterCard.APILogin apiLogin, string process, int cardTypeId)
        {
            MasterCard.ServiceResponse response = new MasterCard.ServiceResponse();

            try
            {
                #region MarterCard Purchase DEMO

                if (process.ToLower().Trim().Equals("purchase"))
                {
                    if (cardTypeId == 1 || string.IsNullOrEmpty(transactionData.LRC)) //Mastercard and Empty LRC
                    {
                        transactionData.LRC = "X";
                    }

                    string entryMode = "900";
                    string version = "&vpc_Version=1";
                    string command = "&vpc_Command=pay";
                    string accesscode = "&vpc_AccessCode=" + transactionData.AccessCode;
                    string merchtxnref = "&vpc_MerchTxnRef=" + transactionData.MerchTxnRef;
                    string merchant = "&vpc_Merchant=" + transactionData.MerchantId;
                    string orderinfo = "&vpc_OrderInfo=" + transactionData.OrderInfo;
                    string amount = "&vpc_Amount=" + transactionData.Amount;
                    string cardnum = "&vpc_CardNum=" + transactionData.CardNumber;
                    string cardexp = "&vpc_CardExp=" + transactionData.CardExpirationDate;
                    string currency = "&vpc_Currency=" + transactionData.Currency;
                    string track2 = "&vpc_CardTrack2=" + HttpUtility.UrlEncode(transactionData.Track2) + transactionData.LRC;
                    string posentrymode = "&vpc_POSEntryMode=" + entryMode;
                    string cardSeq = "&vpc_CardSeqNum=001";
                    string terminalCap = "&vpc_TerminalInputCapability=CM";
                    string terminalId = "&vpc_TerminalID=" + transactionData.TerminalId;
                    string addendumData = "&vpc_AddendumData=" + HttpUtility.UrlEncode("/VER/1"
                                        + "//" + "PFI/" + transactionData.PFI
                                        + "//" + "ISO/" + transactionData.ISO
                                        + "//" + "SMI/" + transactionData.SMI
                                        + "//" + "PFN/" + transactionData.PFN
                                        + "//" + "SMN/" + transactionData.SMN
                                        + "//" + "MSA/" + transactionData.MSA
                                        + "//" + "MCI/" + transactionData.MCI
                                        + "//" + "MST/" + transactionData.MST
                                        + "//" + "MCO/" + transactionData.MCO
                                        + "//" + "MPC/" + transactionData.MPC
                                        + "//" + "MPP/" + transactionData.MPP
                                        + "//" + "MCC/" + transactionData.MCC + "/");

                    UnicodeEncoding UE = new UnicodeEncoding();
                    byte[] hashvalue;
                    byte[] msg = UE.GetBytes(transactionData.SecureHash);
                    SHA256Managed hs = new SHA256Managed();

                    string hex = "";

                    hashvalue = hs.ComputeHash(msg);
                    foreach (byte x in hashvalue)
                    {
                        hex += String.Format("{0:x2}", x);
                    }

                    //plugged in the SHA256 encrypted hex value in the securehash-zo
                    string securehash = "&vpc_SecureHash=" + hex;
                    string securehashtype = "&vpc_SecureHashType=SHA256";

                    string url = System.Configuration.ConfigurationManager.AppSettings["MasterCardDemoURL"].ToString();

                    if (transactionData.TerminalId == null || transactionData.TerminalId == string.Empty)
                    {
                        terminalId = string.Empty;
                    }

                    if (transactionData.PFI == null || transactionData.PFI == string.Empty)
                    {
                        addendumData = string.Empty;
                    }

                    string postData = version + command + accesscode + merchtxnref + merchant + orderinfo + amount + cardnum + cardexp + currency + securehash + securehashtype;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                    request.Proxy = null;
                    request.Method = "POST";
                    request.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse webResponse = request.GetResponse();

                    string res_status = ((HttpWebResponse)webResponse).StatusDescription;

                    dataStream = webResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                    webResponse.Close();

                    //read response
                    try
                    {
                        //CONVERSION of server response to XML
                        string responseXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<MC_RESPONSE " + responseFromServer.Replace("\u003d", "\u003d\"").Replace("\u0026", "\"\n ") + "\"/>\n";

                        StringBuilder output = new StringBuilder();
                        using (XmlReader xmlreader = XmlReader.Create(new StringReader(responseXML)))
                        {
                            xmlreader.ReadToFollowing("MC_RESPONSE");
                            //reading from the xml-ified version of server response..
                            //then filling in the data in the response data class for Mastercard
                            response.Command = xmlreader.GetAttribute("vpc_Command");
                            response.MerchTxnRef = xmlreader.GetAttribute("vpc_MerchTxnRef");
                            response.MerchantId = xmlreader.GetAttribute("vpc_Merchant");
                            response.OrderInfo = xmlreader.GetAttribute("vpc_OrderInfo");
                            response.Amount = Convert.ToDecimal(xmlreader.GetAttribute("vpc_Amount"));
                            response.Locale = xmlreader.GetAttribute("vpc_Locale");
                            response.Message = xmlreader.GetAttribute("vpc_Message");
                            response.TransactionNumber = xmlreader.GetAttribute("vpc_TransactionNo");
                            response.TransactionResponseCode = xmlreader.GetAttribute("vpc_TxnResponseCode");
                            response.ReceiptNumber = xmlreader.GetAttribute("vpc_ReceiptNo");
                            response.AcqResponseCode = xmlreader.GetAttribute("vpc_AcqResponseCode");
                            response.BatchNumber = xmlreader.GetAttribute("vpc_BatchNo");
                            response.AuthorizeId = xmlreader.GetAttribute("vpc_AuthorizeId");
                            response.Card = xmlreader.GetAttribute("vpc_Card");

                            //fields that were not covered in the Basic output fields:
                            response.AVSResultCode = xmlreader.GetAttribute("vpc_AVSResultCode");
                            response.AcqAVSResponseCode = xmlreader.GetAttribute("vpc_AcqAVSRespCode");
                            response.AcqCSCResponseCode = xmlreader.GetAttribute("vpc_AcqCSCRespCode");
                            response.CSCResultCode = xmlreader.GetAttribute("vpc_CSCResultCode");
                            //Save EntryMode
                            response.PosEntryMode = Convert.ToInt32(entryMode);

                            if (response.TransactionResponseCode.Equals("0"))
                            {
                                response.Status = "Approved";
                            }
                            else
                            {
                                response.Status = "Declined";
                            }

                            response.Message = GetResponseMessage(response.AcqResponseCode, response.Message);

                            response.Message = response.Message.Replace("+", " ").Replace("%2F", "/") + " " + "Response Code: " + response.TransactionResponseCode;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = res_status + " " + responseFromServer;
                        response.Status = "Declined";

                        ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessMasterCard Demo Purchase", ex.StackTrace);
                    }

                    return response;
                }

                #endregion MarterCard Purchase DEMO

                #region MasterCard VOID Demo

                else if (process.ToLower().Trim().Equals("void"))
                {
                    string version = "&vpc_Version=1";
                    string command = "&vpc_Command=voidPurchase";
                    string merchtxnref = "&vpc_MerchTxnRef=" + transactionData.MerchTxnRef;
                    string accesscode = "&vpc_AccessCode=" + transactionData.AccessCode;
                    string merchant = "&vpc_Merchant=" + transactionData.MerchantId;
                    string orderinfo = "&vpc_TransNo=" + transactionData.TransNumber;
                    string username = "&vpc_User=" + apiLogin.Username;
                    string password = "&vpc_Password=" + apiLogin.Password;
                    string authorisationrequest = "&vpc_ReturnAuthResponseData=Y";

                    UnicodeEncoding UE = new UnicodeEncoding();
                    byte[] hashvalue;
                    byte[] msg = UE.GetBytes(transactionData.SecureHash);
                    SHA256Managed hs = new SHA256Managed();

                    string hex = "";

                    hashvalue = hs.ComputeHash(msg);
                    foreach (byte x in hashvalue)
                    {
                        hex += String.Format("{0:x2}", x);
                    }

                    //plugged in the SHA256 encrypted hex value in the securehash-zo
                    string securehash = "&vpc_SecureHash=" + hex;
                    string securehashtype = "&vpc_SecureHashType=SHA256";

                    string url = System.Configuration.ConfigurationManager.AppSettings["MasterCardDemoURL"].ToString();

                    string postData = version + command + merchtxnref + accesscode + merchant + orderinfo + username + password + authorisationrequest;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                    request.Proxy = null;
                    request.Method = "POST";
                    request.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse webResponse = request.GetResponse();

                    string res_status = ((HttpWebResponse)webResponse).StatusDescription;

                    dataStream = webResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                    webResponse.Close();

                    //read response
                    try
                    {
                        //CONVERSION of server response to XML
                        string responseXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<MC_RESPONSE " + responseFromServer.Replace("\u003d", "\u003d\"").Replace("\u0026", "\"\n ") + "\"/>\n";

                        StringBuilder output = new StringBuilder();
                        using (XmlReader xmlreader = XmlReader.Create(new StringReader(responseXML)))
                        {
                            xmlreader.ReadToFollowing("MC_RESPONSE");
                            //reading from the xml-ified version of server response..
                            //then filling in the data in the response data class for Mastercard
                            response.Command = xmlreader.GetAttribute("vpc_Command");
                            response.MerchTxnRef = xmlreader.GetAttribute("vpc_MerchTxnRef");
                            response.MerchantId = xmlreader.GetAttribute("vpc_Merchant");
                            response.OrderInfo = xmlreader.GetAttribute("vpc_CapturedAmount"); //amount voided
                            response.Amount = Convert.ToDecimal(xmlreader.GetAttribute("vpc_Amount"));
                            response.Locale = xmlreader.GetAttribute("vpc_Locale");
                            response.TransactionNumber = xmlreader.GetAttribute("vpc_TransactionNo");

                            response.Message = xmlreader.GetAttribute("vpc_Message");

                            response.TransactionResponseCode = xmlreader.GetAttribute("vpc_TxnResponseCode");
                            response.ReceiptNumber = xmlreader.GetAttribute("vpc_ReceiptNo");
                            response.AcqResponseCode = xmlreader.GetAttribute("vpc_AcqResponseCode");
                            response.BatchNumber = xmlreader.GetAttribute("vpc_BatchNo");
                            response.AuthorizeId = xmlreader.GetAttribute("vpc_TransactionNo"); //ticketing
                            response.Card = xmlreader.GetAttribute("vpc_Card");

                            //fields that were not covered in the Basic output fields:
                            response.AVSResultCode = xmlreader.GetAttribute("vpc_AVSResultCode");
                            response.AcqAVSResponseCode = xmlreader.GetAttribute("vpc_AcqAVSRespCode");
                            response.AcqCSCResponseCode = xmlreader.GetAttribute("vpc_AcqCSCRespCode");
                            response.CSCResultCode = xmlreader.GetAttribute("vpc_CSCResultCode");

                            if (response.TransactionResponseCode.Equals("0"))
                            {
                                response.Status = "Approved";
                            }
                            else
                            {
                                response.Status = "Declined";
                            }

                            response.Message = GetResponseMessage(response.AcqResponseCode, response.Message);

                            response.Message = response.Message.Replace("+", " ").Replace("%2F", "/") + " " + "Response Code: " + response.TransactionResponseCode;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = res_status + " " + responseFromServer;
                        response.Status = "Declined";

                        ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessMasterCard Demo Void", ex.StackTrace);
                    }

                    return response;
                }

                #endregion MasterCard VOID Demo
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessMasterCardDemo", ex.StackTrace);
            }

            return response;
        }

        #endregion

        public static string GetResponseMessage(string acqResponseCode, string defaultMsg)
        {
            string msg;
            try
            {
                switch (acqResponseCode)
                {
                    case "00":
                        msg = "Approved";
                        break;

                    case "05":
                        msg = "Do not honor.";
                        break;

                    case "12":
                        msg = "Invalid Transaction.";
                        break;

                    case "13":
                        msg = "Invalid Amount.";
                        break;

                    case "14":
                        msg = "Invalid card number.";
                        break;

                    case "30":
                        msg = "Format Error.";
                        break;

                    case "58":
                        msg = "Transaction not permitted to acquirer.";
                        break;

                    case "25":
                        msg = "Unable to locate record on file.";
                        break;

                    default:
                        msg = defaultMsg.Replace("%3A", ":");
                        break;
                }

                return msg;
            }
            catch
            {
                return "Declined";
            }
        }
        #endregion

        #region Maxbank Credit/Debit Purchase

        public async Task<SwitchResponse> ProcessPurchaseMaxbankGateway(CardDetails debitData, string process)
        {
            SwitchResponse response = new SwitchResponse();
            string socketResult = string.Empty;
            string parsedata = string.Empty;
            string mti = string.Empty;

            string[] de = new string[130];

            switch (process.ToUpper())
            {
                case "PURCHASE":
                    mti = "0200";

                    de[2] = debitData.CardNumber;
                    de[3] = ConfigurationManager.AppSettings["PCP"].ToString();
                    de[4] = debitData.Amount.ToString().Replace(".", "").PadLeft(12, '0');
                    de[7] = DateTime.Today.ToString("MMddhhmmss");
                    de[11] = debitData.SystemTraceAudit;
                    de[12] = DateTime.Now.ToString("hhmmss");
                    de[13] = DateTime.Now.ToString("MMdd");
                    //de[14] = debitData.ExpirationDate;
                    de[15] = DateTime.Now.ToString("MMdd");
                    de[18] = ConfigurationManager.AppSettings["MERCHANTTYPE"].ToString();
                    de[22] = ConfigurationManager.AppSettings["ENTRYMODE"].ToString();
                    de[24] = ConfigurationManager.AppSettings["FUNCTIONCODE"].ToString();
                    de[25] = ConfigurationManager.AppSettings["POSCC"].ToString();
                    de[32] = ConfigurationManager.AppSettings["AIIC"].ToString().PadLeft(10, '0');
                    de[35] = debitData.Track2Data;
                    de[37] = debitData.SystemTraceAudit.PadLeft(12, '0');
                    de[41] = debitData.TerminalID;
                    de[42] = debitData.MerchantID;
                    de[49] = debitData.CurrencyCode;
                    de[52] = debitData.PinBlock; 

                    de[45] = debitData.Track1Data;
                    de[43] = debitData.NameOnCard;
                    de[28] = debitData.AmountTransactionFee;
                    de[30] = debitData.AmountTransactionProcessingFee;
                    de[29] = debitData.AmountSettlementFee;
                    de[31] = debitData.AmountSettlementProcessingFee;
                    de[48] = debitData.PrivateAdditionalData; 
                    de[55] = debitData.ChipCardData;
                    de[61] = debitData.Invoice;
                    de[63] = debitData.AdditionalData;
                    de[64] = debitData.MessageAuthorizationCode;
                    
                    break;

                case "REVERSAL":
                    mti = "0420";

                    de[2] = debitData.CardNumber;
                    de[3] = ConfigurationManager.AppSettings["PCP"].ToString();
                    de[4] = debitData.Amount.ToString().Replace(".", "").PadLeft(12, '0');
                    de[7] = DateTime.Today.ToString("MMddhhmmss");
                    de[11] = debitData.SystemTraceAudit; //same as original request
                    de[12] = DateTime.Now.ToString("hhmmss");
                    de[13] = DateTime.Now.ToString("MMdd");
                    //de[14] = debitData.ExpirationDate;
                    de[15] = DateTime.Now.ToString("MMdd");
                    de[18] = ConfigurationManager.AppSettings["MERCHANTTYPE"].ToString();
                    de[22] = ConfigurationManager.AppSettings["ENTRYMODE"].ToString();
                    de[24] = ConfigurationManager.AppSettings["FUNCTIONCODE"].ToString();
                    de[25] = ConfigurationManager.AppSettings["POSCC"].ToString();
                    de[32] = ConfigurationManager.AppSettings["AIIC"].ToString().PadLeft(10, '0');
                    de[35] = debitData.Track2Data;
                    de[37] = debitData.SystemTraceAudit.PadLeft(12, '0');
                    de[41] = debitData.TerminalID;
                    de[42] = debitData.MerchantID;
                    de[49] = debitData.CurrencyCode;
                    de[52] = debitData.PinBlock;

                    de[45] = debitData.Track1Data;
                    de[43] = debitData.NameOnCard;
                    de[28] = debitData.AmountTransactionFee;
                    de[30] = debitData.AmountTransactionProcessingFee;
                    de[29] = debitData.AmountSettlementFee;
                    de[31] = debitData.AmountSettlementProcessingFee;
                    de[48] = debitData.PrivateAdditionalData;
                    de[55] = debitData.ChipCardData;
                    de[61] = debitData.Invoice;
                    de[63] = debitData.AdditionalData;
                    de[64] = debitData.MessageAuthorizationCode;

                    break;

                case "VOID":
                    mti = "0200";

                    de[2] = debitData.CardNumber;
                    de[3] = ConfigurationManager.AppSettings["PCV"].ToString();
                    de[4] = debitData.Amount.ToString().Replace(".", "").PadLeft(12, '0');
                    de[11] = debitData.SystemTraceAudit;
                    de[12] = DateTime.Now.ToString("hhmmss");
                    de[13] = DateTime.Now.ToString("MMdd");
                    //de[14] = debitData.ExpirationDate;
                    de[18] = ConfigurationManager.AppSettings["MERCHANTTYPE"].ToString();
                    de[22] = ConfigurationManager.AppSettings["ENTRYMODE"].ToString();
                    de[24] = ConfigurationManager.AppSettings["FUNCTIONCODE"].ToString();
                    de[25] = ConfigurationManager.AppSettings["POSCC"].ToString();
                    de[28] = debitData.AmountTransactionFee;
                    de[29] = debitData.AmountSettlementFee;
                    de[30] = debitData.AmountTransactionProcessingFee;
                    de[31] = debitData.AmountSettlementProcessingFee;
                    de[35] = debitData.Track2Data;
                    de[37] = debitData.RetrievalReferenceNumber;
                    de[38] = debitData.AuthorizationIDResponse;
                    de[39] = debitData.ResponseCode;
                    de[41] = debitData.TerminalID;
                    de[42] = debitData.MerchantID;
                    de[43] = debitData.NameOnCard;
                    de[45] = debitData.Track1Data;
                    de[48] = debitData.PrivateAdditionalData;
                    de[52] = debitData.PinBlock;
                    de[55] = debitData.ChipCardData;
                    de[61] = debitData.Invoice;
                    de[63] = debitData.AdditionalData;
                    de[64] = debitData.MessageAuthorizationCode;

                    break;

                case "INQUIRY":
                    mti = "0200";

                    de[11] = debitData.SystemTraceAudit;
                    de[37] = debitData.SystemTraceAudit.PadLeft(12, '0');
                    de[4] = debitData.Amount.ToString().Replace(".", "").PadLeft(12, '0');
                    de[7] = DateTime.Today.ToString("MMddhhmmss");
                    de[12] = DateTime.Now.ToString("hhmmss");
                    de[13] = DateTime.Now.ToString("MMdd");
                    de[35] = debitData.Track2Data;
                    de[45] = debitData.Track1Data;

                    if (debitData.AccountType == "2")
                        de[3] = ConfigurationManager.AppSettings["PCBSA"].ToString();
                    else if (debitData.AccountType == "3")
                        de[3] = ConfigurationManager.AppSettings["PCBCA"].ToString();

                    de[18] = ConfigurationManager.AppSettings["MERCHANTTYPE"].ToString();
                    de[24] = ConfigurationManager.AppSettings["FUNCTIONCODE"].ToString();
                    de[22] = ConfigurationManager.AppSettings["ENTRYMODE"].ToString();
                    de[32] = ConfigurationManager.AppSettings["AIIC"].ToString();
                    de[25] = ConfigurationManager.AppSettings["POSCC"].ToString();

                    de[28] = debitData.AmountTransactionFee;
                    de[30] = debitData.AmountTransactionProcessingFee;
                    de[29] = debitData.AmountSettlementProcessingFee;
                    de[33] = debitData.ForwardingInstitutionCode;

                    de[2] = debitData.CardNumber;
                    de[43] = debitData.NameOnCard;
                    de[42] = debitData.MerchantID;
                    de[41] = debitData.TerminalID;

                    de[49] = debitData.CurrencyCode;
                    de[48] = debitData.PrivateAdditionalData;
                    de[52] = debitData.PinBlock;
                    de[55] = debitData.ChipCardData;
                    de[64] = debitData.MessageAuthorizationCode;
                    break;
            }

            try
            {
                string data = new ISO8583().Build(de, mti);
                SocketClient socket = new SocketClient();
                response = socket.CallSynchronousSocketClient("MB" + data);
            }
            catch (Exception ex)
            {
                response.ErrNumber = "-1";
                response.Status = "Declined";
                response.Message = ex.Message;
            }
                        
            return response;
        }

        #endregion Maxbank Debit Purchase

        #region IsignThis ECOM
        public IsignThis.pub_downstream_dto ProcessEcomTransactionKYC(IsignThis.auth_request authRequest)
        {
            IsignThis.pub_downstream_dto response = new IsignThis.pub_downstream_dto();
            string url = ConfigurationManager.AppSettings["IsignURL"].ToString();

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 3, 00);
                    client.BaseAddress = new Uri("");

                    client.DefaultRequestHeaders.TryAddWithoutValidation("From", ConfigurationManager.AppSettings["From"].ToString());
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + ConfigurationManager.AppSettings["Authorization"].ToString());

                    HttpResponseMessage httpResponse = client.PostAsJsonAsync("authorization/", authRequest).Result;

                    response = httpResponse.Content.ReadAsAsync<IsignThis.pub_downstream_dto>().Result;


                }
            }
            catch(Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessEcomTransactionKYC", ex.StackTrace);
            }

            return response;
        }
        #endregion

        #region GlobalOnePay
        public GlobalOnePay.PaymentResponse ProcessGlobalOnePayTransaction(GlobalOnePay.TransactionData transactionData, string process)
        {
            GlobalOnePay.PaymentResponse response = new GlobalOnePay.PaymentResponse();

            try
            {
                string dt = DateTime.UtcNow.ToString("dd-MM-yyyy:hh:mm:ss:fff");

                #region Purchase GlobalOnePay
                if ((process.ToLower().Trim().Equals("purchase")))
                {
                    string hashInput = transactionData.TerminalId + transactionData.OrderId + transactionData.Amount + dt + transactionData.Hash;

                    var xmlRequest = new XDocument(new XDeclaration("1.0", "UTF-8", ""),
                    new XElement("PAYMENT",
                    new XElement("ORDERID", transactionData.OrderId),
                    new XElement("TERMINALID", transactionData.TerminalId),
                    new XElement("AMOUNT", transactionData.Amount),
                    new XElement("DATETIME", dt),
                    new XElement("CARDNUMBER", transactionData.CardNumber),
                    new XElement("CARDTYPE", transactionData.CardType),
                    new XElement("CARDEXPIRY", transactionData.CardExpiry),
                    new XElement("CARDHOLDERNAME", transactionData.CardHolderName),
                    new XElement("HASH", Functions.ComputeMD5Hash(hashInput)),
                    new XElement("CURRENCY", transactionData.Currency),
                    new XElement("TERMINALTYPE", "1"),
                    new XElement("TRANSACTIONTYPE", "7"),
                    new XElement("CVV", transactionData.Cvv)));

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    string url = System.Configuration.ConfigurationManager.AppSettings["GlobalOnePayURL"].ToString();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    byte[] bytes;
                    bytes = System.Text.Encoding.ASCII.GetBytes(xmlRequest.ToString());
                    request.ContentType = "text/xml; encoding='utf-8'";
                    request.ContentLength = bytes.Length;
                    request.Method = "POST";
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                    HttpWebResponse httpResponse;
                    httpResponse = (HttpWebResponse)request.GetResponse();
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = httpResponse.GetResponseStream();
                        string responseStr = new StreamReader(responseStream).ReadToEnd();

                        XDocument doc = XDocument.Parse(responseStr);

                        if (CheckForError(responseStr) == false)
                        {
                            var item = doc.Descendants("PAYMENTRESPONSE").Select(i => new
                            {
                                UniqueRef = i.Element("UNIQUEREF").Value,
                                ResponseCode = i.Element("RESPONSECODE").Value,
                                ResponseText = i.Element("RESPONSETEXT").Value,
                                ApprovalCode = i.Element("APPROVALCODE").Value,
                                DateTime = i.Element("DATETIME").Value,
                                CvvResponse = i.Element("CVVRESPONSE").Value,
                                Hash = i.Element("HASH").Value
                            });

                            foreach (var pay in item)
                            {
                                response.UniqueReference = pay.UniqueRef;
                                response.ResponseCode = "00";
                                response.Message = pay.ResponseText;
                                response.ApprovalCode = pay.ApprovalCode;
                                response.CvvResponse = pay.CvvResponse;
                                response.Hash = pay.Hash;
                                response.Status = "Approved";
                            }
                        }
                        else
                        {
                            var item = doc.Descendants("ERROR").Select(e => new
                            {
                                ErrorMessage = e.Element("ERRORSTRING").Value,
                            });

                            foreach (var err in item)
                            {
                                response.Message = err.ErrorMessage;
                                response.Status = "Declined";
                            }
                        }
                    }
                }
                #endregion

                #region Void GlobalOnePay
                else if ((process.ToLower().Trim().Equals("void")))
                {
                    string hashInput = transactionData.TerminalId + transactionData.TransNumber + transactionData.Amount + dt + transactionData.Hash;

                    var xmlRequest = new XDocument(new XDeclaration("1.0", "UTF-8", ""),
                    new XElement("REFUND",
                    new XElement("UNIQUEREF", transactionData.TransNumber),
                    new XElement("TERMINALID", transactionData.TerminalId),
                    new XElement("AMOUNT", transactionData.Amount),
                    new XElement("DATETIME", dt),
                    new XElement("HASH", Functions.ComputeMD5Hash(hashInput)),
                    new XElement("OPERATOR", transactionData.Operator),
                    new XElement("REASON", transactionData.ReasonToRefund)));

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    string url = System.Configuration.ConfigurationManager.AppSettings["GlobalOnePayURL"].ToString();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    byte[] bytes;
                    bytes = System.Text.Encoding.ASCII.GetBytes(xmlRequest.ToString());
                    request.ContentType = "text/xml; encoding='utf-8'";
                    request.ContentLength = bytes.Length;
                    request.Method = "POST";
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                    HttpWebResponse httpResponse;
                    httpResponse = (HttpWebResponse)request.GetResponse();
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = httpResponse.GetResponseStream();
                        string responseStr = new StreamReader(responseStream).ReadToEnd();

                        XDocument doc = XDocument.Parse(responseStr);

                        if (CheckForError(responseStr) == false)
                        {
                            var item = doc.Descendants("REFUNDRESPONSE").Select(i => new
                            {
                                UniqueRef = i.Element("UNIQUEREF").Value,
                                ResponseCode = i.Element("RESPONSECODE").Value,
                                ResponseText = i.Element("RESPONSETEXT").Value,
                                DateTime = i.Element("DATETIME").Value,
                                Hash = i.Element("HASH").Value
                            });

                            foreach (var pay in item)
                            {
                                response.UniqueReference = pay.UniqueRef;
                                response.ResponseCode = "00";
                                response.Message = pay.ResponseText;
                                response.Hash = pay.Hash;
                                response.Status = "Approved";
                            }
                        }
                        else
                        {
                            var item = doc.Descendants("ERROR").Select(e => new
                            {
                                ErrorMessage = e.Element("ERRORSTRING").Value,
                            });

                            foreach (var err in item)
                            {
                                response.Message = err.ErrorMessage;
                                response.Status = "Declined";
                            }
                        }
                    }
                }
                #endregion
            }
            catch(Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessGlobalOnePayTransaction", ex.StackTrace);
            }

            return response;
        }

        public static bool CheckForError(string response)
        {
            XDocument doc = XDocument.Parse(response);

            var item = doc.Descendants("ERROR").Select(i => new
            {
                ErrorString = i.Element("ERRORSTRING").Value
            });

            foreach (var pay in item)
            {
                if (!string.IsNullOrEmpty(pay.ErrorString))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Worldnet
        public Worldnet.PaymentResponse ProcessWorldnetTransaction(Worldnet.TransactionData transactionData, string process)
        {
            Worldnet.PaymentResponse response = new Worldnet.PaymentResponse();

            try
            {
                string dt = DateTime.UtcNow.ToString("dd-MM-yyyy:hh:mm:ss:fff");

                #region Purchase Worldnet
                if ((process.ToLower().Trim().Equals("purchase")))
                {
                    string hashInput = transactionData.TerminalId + transactionData.OrderId + transactionData.Amount + dt + transactionData.Hash;

                    var xmlRequest = new XDocument(new XDeclaration("1.0", "UTF-8", ""),
                    new XElement("PAYMENT",
                    new XElement("ORDERID", transactionData.OrderId),
                    new XElement("TERMINALID", transactionData.TerminalId),
                    new XElement("AMOUNT", transactionData.Amount),
                    new XElement("DATETIME", dt),
                    new XElement("CARDNUMBER", transactionData.CardNumber),
                    new XElement("CARDTYPE", transactionData.CardType),
                    new XElement("CARDEXPIRY", transactionData.CardExpiry),
                    new XElement("CARDHOLDERNAME", transactionData.CardHolderName),
                    new XElement("HASH", Functions.ComputeMD5Hash(hashInput)),
                    new XElement("CURRENCY", transactionData.Currency),
                    new XElement("TERMINALTYPE", "2"),
                    new XElement("TRANSACTIONTYPE", "7"),
                    new XElement("CVV", transactionData.Cvv)));

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    string url = System.Configuration.ConfigurationManager.AppSettings["WorldnetPayURL"].ToString();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    byte[] bytes;
                    bytes = System.Text.Encoding.ASCII.GetBytes(xmlRequest.ToString());
                    request.ContentType = "text/xml; encoding='utf-8'";
                    request.ContentLength = bytes.Length;
                    request.Method = "POST";
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                    HttpWebResponse httpResponse;
                    httpResponse = (HttpWebResponse)request.GetResponse();
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = httpResponse.GetResponseStream();
                        string responseStr = new StreamReader(responseStream).ReadToEnd();

                        XDocument doc = XDocument.Parse(responseStr);

                        if (CheckForError(responseStr) == false)
                        {
                            var item = doc.Descendants("PAYMENTRESPONSE").Select(i => new
                            {
                                UniqueRef = i.Element("UNIQUEREF").Value,
                                ResponseCode = i.Element("RESPONSECODE").Value,
                                ResponseText = i.Element("RESPONSETEXT").Value,
                                ApprovalCode = i.Element("APPROVALCODE").Value,
                                DateTime = i.Element("DATETIME").Value,
                                CvvResponse = i.Element("CVVRESPONSE").Value,
                                Hash = i.Element("HASH").Value
                            });

                            foreach (var pay in item)
                            {
                                response.UniqueReference = pay.UniqueRef;
                                response.ResponseCode = "00";
                                response.Message = pay.ResponseText;
                                response.ApprovalCode = pay.ApprovalCode;
                                response.Datetime = Convert.ToDateTime(pay.DateTime);
                                response.CvvResponse = pay.CvvResponse;
                                response.Hash = pay.Hash;
                                response.Status = "Approved";
                            }
                        }
                        else
                        {
                            var item = doc.Descendants("ERROR").Select(e => new
                            {
                                ErrorMessage = e.Element("ERRORSTRING").Value,
                            });

                            foreach (var err in item)
                            {
                                response.Message = err.ErrorMessage;
                                response.Status = "Declined";
                            }
                        }
                    }
                }
                #endregion

                #region Void Worldnet
                else if ((process.ToLower().Trim().Equals("void")))
                {
                    string hashInput = transactionData.TerminalId + transactionData.TransNumber + transactionData.Amount + dt + transactionData.Hash;

                    var xmlRequest = new XDocument(new XDeclaration("1.0", "UTF-8", ""),
                    new XElement("REFUND",
                    new XElement("UNIQUEREF", transactionData.TransNumber),
                    new XElement("TERMINALID", transactionData.TerminalId),
                    new XElement("AMOUNT", transactionData.Amount),
                    new XElement("DATETIME", dt),
                    new XElement("HASH", Functions.ComputeMD5Hash(hashInput)),
                    new XElement("OPERATOR", transactionData.Operator),
                    new XElement("REASON", transactionData.ReasonToRefund)));

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    string url = System.Configuration.ConfigurationManager.AppSettings["WorldnetPayURL"].ToString();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    byte[] bytes;
                    bytes = System.Text.Encoding.ASCII.GetBytes(xmlRequest.ToString());
                    request.ContentType = "text/xml; encoding='utf-8'";
                    request.ContentLength = bytes.Length;
                    request.Method = "POST";
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                    HttpWebResponse httpResponse;
                    httpResponse = (HttpWebResponse)request.GetResponse();
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = httpResponse.GetResponseStream();
                        string responseStr = new StreamReader(responseStream).ReadToEnd();

                        XDocument doc = XDocument.Parse(responseStr);

                        if (CheckForError(responseStr) == false)
                        {
                            var item = doc.Descendants("REFUNDRESPONSE").Select(i => new
                            {
                                UniqueRef = i.Element("UNIQUEREF").Value,
                                ResponseCode = i.Element("RESPONSECODE").Value,
                                ResponseText = i.Element("RESPONSETEXT").Value,
                                DateTime = i.Element("DATETIME").Value,
                                Hash = i.Element("HASH").Value
                            });

                            foreach (var pay in item)
                            {
                                response.UniqueReference = pay.UniqueRef;
                                response.ResponseCode = "00";
                                response.Message = pay.ResponseText;
                                response.Hash = pay.Hash;
                                response.Status = "Approved";
                            }
                        }
                        else
                        {
                            var item = doc.Descendants("ERROR").Select(e => new
                            {
                                ErrorMessage = e.Element("ERRORSTRING").Value,
                            });

                            foreach (var err in item)
                            {
                                response.Message = err.ErrorMessage;
                                response.Status = "Declined";
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessWorldnetTransaction", ex.StackTrace);
            }

            return response;
        }
        #endregion

        #region NovaPay
        public NovaPay.ServiceResponse ProcessNovaToPay(Hashtable hashParam, string process)
        {
            ServiceResponse response = new ServiceResponse();
            string url = ConfigurationManager.AppSettings["NovaPayURL"].ToString();

            try
            {
                switch (process.ToUpper())
                {
                    #region NovaToPay PURCHASE
                    case "PURCHASE":
                        
                        using (HttpClient client = new HttpClient())
                        {
                            client.Timeout = new TimeSpan(0, 3, 00);
                            client.BaseAddress = new Uri(url);

                            var req = new List<KeyValuePair<string, string>>(0);
                            foreach (var key in hashParam.Keys)
                            {
                                req.Add(new KeyValuePair<string, string>(key.ToString(), hashParam[key].ToString()));
                            }

                            var content = new FormUrlEncodedContent(req);
                            var result = client.PostAsync("/icp/pay.html", content);

                            string resultContent = result.Result.Content.ReadAsStringAsync().Result;

                            response = JsonConvert.DeserializeObject<NovaPay.ServiceResponse>(resultContent);

                            if (response.errorCode.Replace(" ","") == "" || response.errorCode == "00" || response.errorCode == "0000")
                            {
                                response.errorCode = "00";
                                response.Status = "Approved";
                            }
                            else
                            {
                                response.Status = "Declined";
                            }
                        }

                        break;

                    #endregion

                    #region NovaToPay REFUND

                    case "VOID":

                        using (HttpClient client = new HttpClient())
                        {
                            client.Timeout = new TimeSpan(0, 3, 00);
                            client.BaseAddress = new Uri(url);

                            var req = new List<KeyValuePair<string, string>>(0);
                            foreach (var key in hashParam.Keys)
                            {
                                req.Add(new KeyValuePair<string, string>(key.ToString(), hashParam[key].ToString()));
                            }

                            var content = new FormUrlEncodedContent(req);
                            var result = client.PostAsync("/refund.html", content);

                            string resultContent = result.Result.Content.ReadAsStringAsync().Result;

                            response = JsonConvert.DeserializeObject<NovaPay.ServiceResponse>(resultContent);

                            if (response.flag == "1")
                            {
                                response.Status = "Approved";
                                response.Message = "Successful";
                            }
                            else
                            {
                                response.errorCode = response.flag;
                                response.Status = "Declined";
                                response.Message = response.message;
                            }
                        }

                        break;
                    #endregion
                }
            }
            catch(Exception ex)
            {
                ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessNovaToPay", ex.StackTrace);
            }

            return response;
        }
        #endregion

        #region TrustPay
        public TrustPay.TransactionResponse ProcessTrustPay(Transaction trans, string process)
        {
            TrustPay.TransactionResponse response = new TrustPay.TransactionResponse();

            #region Purchase
            if (process.ToLower().Trim().Equals("purchase"))
            {
                string url = ConfigurationManager.AppSettings["URL"].ToString();
                try
                {
                    using (var client = new WebClient())
                    {
                        var data = new NameValueCollection();
                        string signinfo = SDGUtil.Functions.sha256_hash(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}",
                                                        trans.MerNo,
                                                        trans.Gatewayno,
                                                        trans.orderNo,
                                                        trans.orderCurrency,
                                                        trans.orderAmount,
                                                        trans.firstName,
                                                        trans.lastName,
                                                        trans.cardNo,
                                                        trans.cardExpireYear,
                                                        trans.cardExpireMonth,
                                                        trans.cardSecurityCode,
                                                        trans.email,
                                                        ConfigurationManager.AppSettings["signkey"].ToString()));

                        data.Add("merNo", trans.MerNo);
                        data.Add("gatewayNo", trans.Gatewayno);
                        data.Add("csid", trans.csid);
                        data.Add("orderNo", trans.orderNo);
                        data.Add("orderCurrency", trans.orderCurrency);
                        data.Add("orderAmount", trans.orderAmount);
                        data.Add("cardNo", trans.cardNo);
                        data.Add("cardExpireMonth", trans.cardExpireMonth);
                        data.Add("cardExpireYear", trans.cardExpireYear);
                        data.Add("cardSecurityCode", trans.cardSecurityCode);
                        data.Add("issuingBank", trans.issuingBank);
                        data.Add("firstName", trans.firstName);
                        data.Add("lastName", trans.lastName);
                        data.Add("email", trans.email);
                        data.Add("ip", trans.ip);
                        data.Add("phone", trans.phone);
                        data.Add("country", trans.country);
                        data.Add("state", trans.state);
                        data.Add("city", trans.city);
                        data.Add("address", trans.address);
                        data.Add("remark", trans.remark);
                        data.Add("returnUrl", trans.returnUrl);
                        data.Add("signInfo", signinfo);
                        data.Add("zip", trans.zip);
                        var result = client.UploadValues(ConfigurationManager.AppSettings["URL"].ToString(), data);
                        string sample = Encoding.ASCII.GetString(result);

                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(sample);

                        response.merNo = xml.SelectSingleNode("/respon")["merNo"].InnerText;
                        response.gatewayNo = xml.SelectSingleNode("/respon")["gatewayNo"].InnerText;
                        response.orderAmount = xml.SelectSingleNode("/respon")["orderAmount"].InnerText;
                        response.orderCurrency = xml.SelectSingleNode("/respon")["orderCurrency"].InnerText;
                        response.orderInfo = xml.SelectSingleNode("/respon")["orderInfo"].InnerText;
                        response.orderNo = xml.SelectSingleNode("/respon")["orderNo"].InnerText;
                        response.orderStatus = xml.SelectSingleNode("/respon")["orderStatus"].InnerText;
                        response.remark = xml.SelectSingleNode("/respon")["remark"].InnerText;
                        response.signInfo = xml.SelectSingleNode("/respon")["signInfo"].InnerText;
                        response.tradeNo = xml.SelectSingleNode("/respon")["tradeNo"].InnerText;
                        response.responseCode = xml.SelectSingleNode("/respon")["responseCode"].InnerText;

                        if (response.orderStatus == "1")
                        {
                            response.Status = "Approved";
                        }
                        else if (response.orderStatus == "-1")
                        {
                            response.Status = "Pending";
                        }
                        else if (response.orderStatus == "-2")
                        {
                            response.Status = "To be confirmed";
                        }
                        else
                        {
                            response.Status = "Declined";
                            response.responseCode = "-1";
                        }
                    }
                }
                catch (Exception ex)
                {
                    trans.Status = "Failed";
                    ApplicationLog.LogError("GatewayProcessor", ex.Message, "ProcessTrustPay", ex.StackTrace);
                }
            }
            #endregion

            #region Void
            else if (process.ToLower().Trim().Equals("void"))
            {
                string url = ConfigurationManager.AppSettings["RefundURL"].ToString();

                try
                {
                    using (var client = new WebClient())
                    {
                        var data = new NameValueCollection();
                        string signinfo = SDGUtil.Functions.sha256_hash(string.Format("{0}{1}{2}{3}{4}",
                                                        trans.MerNo,
                                                        trans.Gatewayno,
                                                        trans.tradeNo,
                                                        trans.refundType,
                                                        ConfigurationManager.AppSettings["signkey"].ToString()));

                        data.Add("merNo", trans.MerNo);
                        data.Add("gatewayNo", trans.Gatewayno);
                        data.Add("tradeNo", trans.tradeNo);
                        data.Add("refundType", trans.refundType);
                        data.Add("tradeAmount", trans.orderAmount);
                        data.Add("refundAmount", trans.orderAmount);
                        data.Add("currency", trans.orderCurrency);
                        data.Add("refundReason", trans.refundReason);
                        data.Add("remark", trans.remark);
                        data.Add("signInfo", signinfo);
                        var result = client.UploadValues(url, data);
                        string sample = Encoding.ASCII.GetString(result);

                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(sample);

                        trans.Status = "DONE";

                        response.merNo = xml.DocumentElement.FirstChild["merNo"].InnerText;
                        response.gatewayNo = xml.DocumentElement.FirstChild["gatewayNo"].InnerText;
                        response.batchNo = xml.DocumentElement.FirstChild["batchNo"].InnerText;
                        response.signInfo = xml.DocumentElement.FirstChild["signInfo"].InnerText;
                        response.responseCode = xml.DocumentElement.FirstChild["code"].InnerText;
                        response.description = xml.DocumentElement.FirstChild["description"].InnerText;
                        response.tradeNo = xml.DocumentElement.FirstChild["tradeNo"].InnerText;
                        response.remark = xml.DocumentElement.FirstChild["remark"].InnerText;

                        if(response.responseCode == "00")
                        {
                            response.Status = "Approved";
                            response.Message = response.description;
                        }
                        else
                        {
                            response.Status = "Declined";
                            response.Message = response.description;
                        }
                    }
                }
                catch (Exception ex)
                {
                    trans.Status = "FAILED";
                    response.Message = ex.Message;
                }
            }
            #endregion

            return response;
        }
        #endregion

        #region PayOnline
        public PayOnline.TransactionResponse ProcessPayOnline(TransactionRequest trans, string process)
        {
            PayOnline.TransactionResponse response = new PayOnline.TransactionResponse();

            if(process.ToLower().Trim().Equals("purchase"))
            {
                string url = ConfigurationManager.AppSettings["PayOnlinePurchaseURL"].ToString();

                try
                {
                    using (var client = new WebClient())
                    {
                        var param = new NameValueCollection();
                        string securityKey = SDGUtil.Functions.ComputeMD5Hash("MerchantId=" + trans.MerchantId + "&OrderId=" + trans.OrderId + "&Amount=" + trans.Amount + "&Currency=" + trans.Amount + "&PrivateSecurityKey=" + trans.SecurityKey);
                        param.Add("MerchantId", trans.MerchantId);
                        param.Add("OrderId", trans.OrderId);
                        param.Add("Amount", trans.Amount);
                        param.Add("Currency", trans.Currency);
                        param.Add("SecurityKey", securityKey);
                        param.Add("Ip", trans.Ip);
                        param.Add("Email", trans.Email);
                        param.Add("CardHolderName", trans.CardHolderName);
                        param.Add("CardNumber", trans.CardNumber);
                        param.Add("CardExpDate", trans.CardExpiry);
                        param.Add("CardCvv", trans.Cvv);
                        param.Add("Country", trans.Country);
                        param.Add("City", trans.City);
                        param.Add("Address", trans.Address);
                        param.Add("Zip", trans.Zip);
                        param.Add("State", trans.State);
                        param.Add("Phone", trans.Phone);
                        param.Add("Issuer", trans.Issuer);
                        param.Add("ContentType", "xml");

                        var result = client.UploadValues(url, param);
                        string apiResult = Encoding.ASCII.GetString(result);

                        XDocument doc = XDocument.Parse(apiResult);

                        if (CheckErrorForPayOnline(apiResult) == false)
                        {
                            var item = doc.Descendants("transaction").Select(i => new
                            {
                                Id = i.Element("id").Value,
                                Operation = i.Element("operation").Value,
                                Result = i.Element("result").Value,
                                Code = i.Element("code").Value,
                                Status = i.Element("status").Value
                            });

                            foreach (var pay in item)
                            {
                                response.Result = pay.Result;
                                response.ErrorCode = pay.Code;
                                response.Status = "Approved";
                            }
                        }
                        else
                        {
                            var item = doc.Descendants("error").Select(e => new
                            {
                                ErrorCode = e.Element("code").Value,
                                Result = e.Element("message")
                            });

                            foreach (var err in item)
                            {
                                response.ErrorCode = err.ErrorCode.ToString();
                                response.Result = err.Result.ToString();
                                response.Status = "Declined";
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    response.Status = "Failed";
                    ApplicationLog.LogError("GatewayProcessor", ex.Message, "PayOnline", ex.StackTrace);
                }
            }
            else if(process.ToLower().Trim().Equals("void"))
            {
                string url = ConfigurationManager.AppSettings["PayOnlineVoidURL"].ToString();

                try
                {
                    using (var client = new WebClient())
                    {
                        var param = new NameValueCollection();
                        string securityKey = SDGUtil.Functions.ComputeMD5Hash("MerchantId=" + trans.MerchantId + "&OrderId=" + trans.OrderId + "&Amount=" + trans.Amount + "&Currency=" + trans.Amount + "&PrivateSecurityKey=" + trans.SecurityKey);
                        param.Add("MerchantId", trans.MerchantId);
                        param.Add("TransactionId", trans.OrderId);
                        param.Add("SecurityKey", securityKey);
                        param.Add("ContentType", "xml");

                        var result = client.UploadValues(url, param);
                        string apiResult = Encoding.ASCII.GetString(result);

                        XDocument doc = XDocument.Parse(apiResult);

                        if (CheckErrorForPayOnline(apiResult) == false)
                        {
                            var item = doc.Descendants("transaction").Select(i => new
                            {
                                Id = i.Element("id").Value,
                                Operation = i.Element("operation").Value,
                                Result = i.Element("result").Value,
                                Code = i.Element("code").Value,
                                Status = i.Element("status").Value
                            });

                            foreach (var pay in item)
                            {
                                response.Result = pay.Result;
                                response.ErrorCode = pay.Code;
                                response.Status = "Approved";
                            }
                        }
                        else
                        {
                            var item = doc.Descendants("error").Select(e => new
                            {
                                ErrorCode = e.Element("code").Value,
                                Result = e.Element("message")
                            });

                            foreach (var err in item)
                            {
                                response.ErrorCode = err.ErrorCode.ToString();
                                response.Result = err.Result.ToString();
                                response.Status = "Declined";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Status = "Failed";
                    ApplicationLog.LogError("GatewayProcessor", ex.Message, "PayOnline", ex.StackTrace);
                }
            }
            return response;
        }

        public static bool CheckErrorForPayOnline(string response)
        {
            XDocument doc = XDocument.Parse(response);

            var item = doc.Descendants("error").Select(i => new
            {
                ErrorCode = i.Element("code").Value,
                ErrorMsg = i.Element("message").Value
            });

            foreach (var pay in item)
            {
                if (!string.IsNullOrEmpty(pay.ErrorMsg))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region PayEco
        public String ProcessPayEco(string srcXml)
        {
            string response = string.Empty;

            String encryptKey = Toolkit.random(24);
            String publicKey = ConfigurationManager.AppSettings["PublicKey"].ToString();
            String url = ConfigurationManager.AppSettings["PayEcoURL"].ToString();

            String tmp = Toolkit.signWithMD5(encryptKey, srcXml, publicKey);
            response = Toolkit.bytePadLeft(tmp.Length + "", '0', 6) + tmp;

            return response;
        }

        public static String generateAutoSubmitForm(String actionUrl,
           String request_text)
        {
            String method = "POST";
            StringBuilder html = new StringBuilder();
            html.Append("<html><head></head><body>")
            .Append("<form id=\"pay_form\" name=\"pay_form\" action=\"")
            .Append(actionUrl).Append("\" method=\"" + method + "\">\n")
            .Append("<input type=\"hidden\" name=\"" + request_text + "\">\n")
            .Append("</form>\n")
            .Append("<script language=\"javascript\">window.onload=function(){document.pay_form.submit();}</script>\n")
            .Append("</body></html>");
            return html.ToString();
        }
        #endregion

        #region MVISA Direct
        public MVisa.Response ProcessVisaDirect(string path, string requestBodyString)
        {
            MVisa.Response response = new MVisa.Response();

            string requestURL = ConfigurationManager.AppSettings["visaUrl"] + path;
            string userId = ConfigurationManager.AppSettings["userId"];
            string password = ConfigurationManager.AppSettings["password"];
            string certificatePath = ConfigurationManager.AppSettings["cert"];
            string certificatePassword = ConfigurationManager.AppSettings["certPassword"];

            HttpWebRequest request = WebRequest.Create(requestURL) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            var requestStringBytes = Encoding.UTF8.GetBytes(requestBodyString);
            request.GetRequestStream().Write(requestStringBytes, 0, requestStringBytes.Length);

            /// Add headers
            string authString = userId + ":" + password;
            var authStringBytes = System.Text.Encoding.UTF8.GetBytes(authString);
            string authHeaderString = Convert.ToBase64String(authStringBytes);
            request.Headers["Authorization"] = "Basic " + authHeaderString;

            /// Add certificate
            var certificate = new X509Certificate2(certificatePath, certificatePassword);
            request.ClientCertificates.Add(certificate);

            try
            {
                /// Make the call
                using (HttpWebResponse httpResponse = request.GetResponse() as HttpWebResponse)
                {
                    var encoding = ASCIIEncoding.ASCII;
                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(String.Format(
                         "Server error.\n\nStatusCode:{0}\n\nStatusDescription:{1}\n\nResponseHeaders:{2}",
                         httpResponse.StatusCode,
                         httpResponse.StatusDescription,
                         httpResponse.Headers.ToString()));
                    }

                    using (var reader = new StreamReader(httpResponse.GetResponseStream(), encoding))
                    {
                        string resultContent = reader.ReadToEnd();
                        response = JsonConvert.DeserializeObject<MVisa.Response>(resultContent);

                        if(response.actionCode == "00")
                        {
                            response.Status = "Approved";
                            response.Message = "Success Transaction";
                        }
                        else
                        {
                            response.Status = "Declined";
                        }
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Response is HttpWebResponse)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)e.Response;
                }

                response.Message = e.Message;
            }

            return response;
        }
        #endregion

        #region PayMaya Purchase

        public async Task<SwitchResponse> ProcessPayMayaGateway(CardDetails cardData, string process)
        {
            SwitchResponse response = new SwitchResponse();
            string socketResult = string.Empty;
            string parsedata = string.Empty;
            string mti = string.Empty;

            string[] de = new string[130];

            switch (process.ToUpper())
            {
                case "PURCHASE":
                    mti = "0100";

                    de[2] = cardData.CardNumber;
                    de[3] = ConfigurationManager.AppSettings["PC"].ToString();
                    de[4] = cardData.Amount.ToString().Replace(".", "").PadLeft(12, '0');
                    de[7] = DateTime.Today.ToString("MMddhhmmss");
                    de[11] = cardData.SystemTraceAudit;
                    de[14] = cardData.ExpirationDate;
                    de[22] = "052";
                    de[35] = cardData.Track2Data;
                    de[37] = cardData.RetrievalReferenceNumber;
                    de[41] = cardData.TerminalID;
                    de[42] = cardData.MerchantID;
                    de[49] = cardData.CurrencyCode;
                    de[52] = cardData.PinBlock;
                    de[55] = cardData.ChipCardData;
                    de[61] = "0000000000901608";

                    break;

                case "REVERSAL":
                    mti = "0400";

                    de[2] = cardData.CardNumber;
                    de[3] = ConfigurationManager.AppSettings["PC"].ToString();
                    de[4] = cardData.Amount.ToString().Replace(".", "").PadLeft(12, '0');
                    de[7] = DateTime.Today.ToString("MMddhhmmss");
                    de[11] = cardData.SystemTraceAudit;
                    de[14] = cardData.ExpirationDate;
                    de[37] = cardData.RetrievalReferenceNumber;
                    de[41] = cardData.TerminalID;
                    de[42] = cardData.MerchantID;
                    de[49] = cardData.CurrencyCode;

                    break;
            }

            try
            {
                string data = new PAYMAYA_ISO8583.PAYISO8583().Build(de, mti);
                SocketClient socket = new SocketClient();
                response = socket.CallSynchronousSocketClient("PM" + data);
            }
            catch (Exception ex)
            {
                response.ErrNumber = "-1";
                response.Status = "Declined";
                response.Message = ex.Message;
            }

            return response;
        }

        #endregion PayMaya Purchase
    }
}
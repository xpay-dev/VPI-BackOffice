using CT_EMV_CLASSES;
using CT_EMV_CLASSES.FINANCIAL;
using GatewayProcessor.MasterCard;
using SDGDAL.Repositories;
using SDGUtil;
using SDGWebService.Classes;
using SDGWebService.TLVFunctions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SDGWebService.WebserviceFunctions
{
    public class WebserviceFunctionsOffline
    {
        private EMVCreditDebitRepository _emvCreditDebitRepo = new EMVCreditDebitRepository();
        private DebitSystemTraceNumRepository _traceNumRepo = new DebitSystemTraceNumRepository();

        /// <summary>
        /// CTPayment EMV Credit Purchase
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PurchaseResponse TestCTEmvCreditPurchase(string iccData, string amount)
        {
            PurchaseResponse response = new PurchaseResponse();
            response.POSWSResponse = new POSWSResponse();

            TransactionData transdata = new TransactionData();

            WisepadLayer.WisePad wisepad = new WisepadLayer.WisePad();
            ClassTLV emvDataResult = new ClassTLV();

            string wisepadKey = System.Configuration.ConfigurationManager.AppSettings["DKWisepad1"].ToString();
            string wisepadKeyTlvEmv = System.Configuration.ConfigurationManager.AppSettings["DKWisepad1Emv"].ToString();

            string decryptEmvIcc = wisepad.DecryptC5EmvData(iccData, wisepadKeyTlvEmv);

            var resultTlv = TLVParser.DecodeTLV(decryptEmvIcc);
            emvDataResult.EmvIccData = resultTlv.EmvIccData.ToUpper();

            emvDataResult.SubField1Data = resultTlv.SubField1Data;
            emvDataResult.SubField2Data = resultTlv.SubField2Data;
            emvDataResult.TrackData = resultTlv.TrackData;

            #region Old CTPayment Codes

            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
            GatewayProcessor.MasterCard.TransactionData transData = new GatewayProcessor.MasterCard.TransactionData();

            #region Header Request

            string msgClass = "FO";
            string operatorId = "   ";
            string posEntryMode = "05";
            string terminalId = "BBPOS002"; //mid.Param_6; //SMART018
            string transType = "000";
            string msgVersion = "05";
            string posStatIndicator = "000";
            string seqNumber = _traceNumRepo.GenerateSystemTraceNumber();
            string seqByte = "0";

            HEADER_REQUEST header = new HEADER_REQUEST(msgClass, terminalId, operatorId, seqByte + seqNumber, transType, DateTime.Now, posEntryMode, msgVersion, posStatIndicator);

            header.GetRequestHeader();

            #endregion Header Request

            #region Track2 Equivalent Data

            TRACK2_EQUIVALENT_DATA track2EquivalentData = new TRACK2_EQUIVALENT_DATA(emvDataResult.TrackData[0].CardNumber, resultTlv.TrackData[0].ExpiryDate, emvDataResult.TrackData[0].ServiceCode, emvDataResult.TrackData[0].DiscretionaryData);
            EMV emv = new EMV(track2EquivalentData);
            TRACK2_REQUEST tr = new TRACK2_REQUEST(emv);
            emv.TED = track2EquivalentData;
            tr.EMV = emv;

            #endregion Track2 Equivalent Data

            //TRANSAMOUNT
            decimal orgAmount = Convert.ToDecimal(amount);
            decimal finalAmount = orgAmount * 100;
            string parseAmount;
            try
            {
                parseAmount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                //transAmount.TA = emvDataResult.SubField1Data[0].Amount;
            }
            catch
            {
                parseAmount = finalAmount.ToString();
            }

            #region TRANS AMOUNT

            TRANS_AMOUNT_1 transAmount = new TRANS_AMOUNT_1(parseAmount, "");

            #endregion TRANS AMOUNT

            #region CASHBACK AMOUNT

            CASH_BACK_AMOUNT cashbackAmount = new CASH_BACK_AMOUNT();

            #endregion CASHBACK AMOUNT

            #region APP ACCOUNT TYPE

            APP_ACCOUNT_TYPE appAccountType = new APP_ACCOUNT_TYPE();

            #endregion APP ACCOUNT TYPE

            #region SubField1

            SUBFIDE1 subfield1 = new SUBFIDE1(emvDataResult.SubField1Data[0].Amount,
                                              emvDataResult.SubField1Data[0].ApplicationCryptogram,
                                              emvDataResult.SubField1Data[0].AppInterchangeProfile,
                                              emvDataResult.SubField1Data[0].Atc,
                                              emvDataResult.SubField1Data[0].CryptogramInfoData,
                                              emvDataResult.SubField1Data[0].IssuerData,
                                              emvDataResult.SubField1Data[0].TerminalCountryCode,
                                              emvDataResult.SubField1Data[0].AmountOther,
                                              emvDataResult.SubField1Data[0].TerminalVerificationResults,
                                              emvDataResult.SubField1Data[0].TransCurrencyCode,
                                              emvDataResult.SubField1Data[0].TransDate,
                                              emvDataResult.SubField1Data[0].TransType,
                                              emvDataResult.SubField1Data[0].UnpredictableNumber);

            #endregion SubField1

            #region SubField2

            SUBFIDE2 subfield2 = new SUBFIDE2(emvDataResult.SubField2Data[0].PanSeqNumber,
                                              emvDataResult.SubField2Data[0].AppVersionNumber,
                                              emvDataResult.SubField2Data[0].Cvm,
                                              emvDataResult.SubField2Data[0].InterfaceSerialNumber,
                                              emvDataResult.SubField2Data[0].PosEntryMode,
                                              emvDataResult.SubField2Data[0].TerminalType,
                                              emvDataResult.SubField2Data[0].TransSequenceCounter,
                                              emvDataResult.SubField2Data[0].DedicatedFileName);

            #endregion SubField2

            #region Invoice Number Field

            string invoiceNumberField = "0" + (String.Format("{0:d6}", (DateTime.Now.Ticks / 10) % 1000000));

            INVOICE_NUMBER invoiceNumber = new INVOICE_NUMBER(invoiceNumberField, "");

            #endregion Invoice Number Field

            #region POS CONDITIONS FIELD

            POS_CONDITIONS posConditions = new POS_CONDITIONS(0, 0, CT_EMV_CLASSES.METHODS.FIDP_CARDHOLDER_AUTHENTICATION_METHOD.NOT_AUTHENTICATED, 0);

            #endregion POS CONDITIONS FIELD

            EMV_REQUEST emvRequest = new EMV_REQUEST(header, tr, transAmount, null, subfield1, subfield2, null, posConditions, invoiceNumber);

            #region Smart MID Credential

            transdata.AccessCode = "2FE9791F";
            transdata.Amount = parseAmount;
            transdata.CardExpirationDate = resultTlv.TrackData[0].ExpiryDate;
            transdata.CardNumber = resultTlv.TrackData[0].CardNumber;
            transdata.CSC = "";
            transdata.Currency = "PHP";
            transdata.MerchantId = "SHI100001033";
            transdata.MerchTxnRef = seqNumber;
            transdata.OrderInfo = seqNumber;
            transdata.SecureHash = "556DEEBFB12123CB6BA102D10472E809";
            transdata.Track1 = "";
            transdata.Track2 = ";" + resultTlv.TrackData[0].CardNumber + "=" + resultTlv.TrackData[0].ExpiryDate + resultTlv.TrackData[0].ServiceCode + resultTlv.TrackData[0].DiscretionaryData + "?";
            transdata.TerminalId = "SMART018";

            #endregion Smart MID Credential

            var apiResponse = gateway.ProcessCTPaymentCreditTransaction(emvRequest.EMVRequest(), "purchase");

            if (apiResponse.Result.Status == "Approved")
            {
                response.POSWSResponse.ErrNumber = "0";
                response.POSWSResponse.Message = apiResponse.Result.Message;
                response.POSWSResponse.Status = apiResponse.Result.Status;

                response.AuthNumber = apiResponse.Result.AuthorizeId;
                response.SequenceNumber = apiResponse.Result.BatchNumber;
            }
            else if (apiResponse.Result.Status == "Declined")
            {
                response.POSWSResponse.ErrNumber = apiResponse.Result.AcqResponseCode;
                response.POSWSResponse.Message = apiResponse.Result.Message;
                response.POSWSResponse.Status = apiResponse.Result.Status;

                response.AuthNumber = apiResponse.Result.AuthorizeId;
                response.SequenceNumber = apiResponse.Result.BatchNumber;
            }
            else
            {
                response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Result.Message;
                response.POSWSResponse.ErrNumber = "2101.7";
                response.POSWSResponse.Status = "Declined";
                return response;
            }

            #endregion Old CTPayment Codes

            return response;
        }

        public string TestData(string name)
        {
            return "Your Name is " + name;
        }

        private byte[] HexStringToBytes(string hexa)
        {
            byte[] data = new byte[hexa.Length / 2];
            int j = 0;
            for (int i = 0; i < hexa.Length; i += 2)
            {
                data[j] = Convert.ToByte(hexa.Substring(i, 2), 16);
                ++j;
            }

            return data;
        }

        public PurchaseResponse TestCTSwipePurchase(string track1, string track2, string amount)
        {
            PurchaseResponse response = new PurchaseResponse();
            response.POSWSResponse = new POSWSResponse();

            try
            {
                TransactionData transdata = new TransactionData();

                MobileAppMethods mobileAppMethods = new MobileAppMethods();
                WisepadLayer.WisePad wisepad = new WisepadLayer.WisePad();
                string cardNumber = string.Empty;
                string cardExpiry = string.Empty;
                string serviceCode = string.Empty;
                string ddata = string.Empty;

                string wisepadKey = System.Configuration.ConfigurationManager.AppSettings["DKWisepad1"].ToString();

                track1 = wisepad.DecryptTrackData1(track1, wisepadKey);
                track2 = wisepad.DecryptTrackData2(track2, wisepadKey);

                if (!string.IsNullOrEmpty(track2))
                {
                    int init = track2.IndexOf("=");
                    int end = track2.IndexOf("?");

                    cardExpiry = track2.Substring(init + 1, 4);
                    serviceCode = track2.Substring(init + 5, 3);
                    ddata = track2.Substring(init + 8, (track2.Length - 1) - (init + 8));

                    cardNumber = mobileAppMethods.MobileApp_WisePad_CardNum_Track2(track2);
                }
                else
                {
                    cardNumber = mobileAppMethods.MobileApp_Parse_CardNum_Track1(track1);

                    string cdata = track1.Substring(track1.Length - 28);
                    serviceCode = cdata.Substring(0, 3);
                    ddata = cdata.Substring(cdata.Length - 10, 3);
                    //track2 = ";" + cardNumber + "=" + ex + sc + "0000000000" + ddata + "?";
                }

                #region Old CTPayment Codes

                GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                GatewayProcessor.MasterCard.TransactionData transData = new GatewayProcessor.MasterCard.TransactionData();

                #region Header Request

                string msgClass = "FO";
                string operatorId = "   ";
                string posEntryMode = "02";
                string terminalId = "BBPOS002"; //mid.Param_6; //SMART018
                string transType = "000";
                string msgVersion = "04";
                string posStatIndicator = "000";
                string seqNumber = _traceNumRepo.GenerateSystemTraceNumber();
                string seqByte = "0";

                HEADER_REQUEST header = new HEADER_REQUEST(msgClass, terminalId, operatorId, seqByte + seqNumber, transType, DateTime.Now, posEntryMode, msgVersion, posStatIndicator);

                header.GetRequestHeader();

                #endregion Header Request

                #region Track2 Equivalent Data

                TRACK2_EQUIVALENT_DATA track2EquivalentData = new TRACK2_EQUIVALENT_DATA(cardNumber, cardExpiry, serviceCode, ddata);
                EMV emv = new EMV(track2EquivalentData);
                TRACK2_REQUEST tr = new TRACK2_REQUEST(emv);
                emv.TED = track2EquivalentData;
                tr.EMV = emv;

                #endregion Track2 Equivalent Data

                //TRANSAMOUNT
                decimal orgAmount = Convert.ToDecimal(amount);
                decimal finalAmount = orgAmount * 100;
                string parseAmount;
                try
                {
                    parseAmount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                    //transAmount.TA = emvDataResult.SubField1Data[0].Amount;
                }
                catch
                {
                    parseAmount = finalAmount.ToString();
                }

                #region TRANS AMOUNT

                TRANS_AMOUNT_1 transAmount = new TRANS_AMOUNT_1(parseAmount, "");

                #endregion TRANS AMOUNT

                #region CASHBACK AMOUNT

                CASH_BACK_AMOUNT cashbackAmount = new CASH_BACK_AMOUNT();

                #endregion CASHBACK AMOUNT

                #region APP ACCOUNT TYPE

                APP_ACCOUNT_TYPE appAccountType = new APP_ACCOUNT_TYPE();

                #endregion APP ACCOUNT TYPE

                #region Invoice Number Field

                string invoiceNumberField = "0" + (String.Format("{0:d6}", (DateTime.Now.Ticks / 10) % 1000000));

                INVOICE_NUMBER invoiceNumber = new INVOICE_NUMBER(invoiceNumberField, "");

                #endregion Invoice Number Field

                #region POS CONDITIONS FIELD

                POS_CONDITIONS posConditions = new POS_CONDITIONS(0, 0, 0, 0);

                #endregion POS CONDITIONS FIELD

                EMV_REQUEST emvRequest = new EMV_REQUEST(header, tr, transAmount, null, null, null, null, posConditions, invoiceNumber);

                var apiResponse = gateway.ProcessCTPaymentCreditTransaction(emvRequest.EMVRequest(), "purchase");

                if (apiResponse.Result.Status == "Approved")
                {
                    response.POSWSResponse.ErrNumber = "0";
                    response.POSWSResponse.Message = apiResponse.Result.Message;
                    response.POSWSResponse.Status = apiResponse.Result.Status;

                    response.AuthNumber = apiResponse.Result.AuthorizeId;
                    response.SequenceNumber = apiResponse.Result.BatchNumber;
                }
                else if (apiResponse.Result.Status == "Declined")
                {
                    response.POSWSResponse.ErrNumber = apiResponse.Result.AcqResponseCode;
                    response.POSWSResponse.Message = apiResponse.Result.Message;
                    response.POSWSResponse.Status = apiResponse.Result.Status;

                    response.AuthNumber = apiResponse.Result.AuthorizeId;
                    response.SequenceNumber = apiResponse.Result.BatchNumber;
                }
                else
                {
                    response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Result.Message;
                    response.POSWSResponse.ErrNumber = "2101.7";
                    response.POSWSResponse.Status = "Declined";
                    return response;
                }

                #endregion Old CTPayment Codes
            }
            catch (Exception ex)
            {
                string result = ex.Message;
            }

            return response;
        }
  
    }
}
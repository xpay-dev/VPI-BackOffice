using CT_EMV_CLASSES;
using CT_EMV_CLASSES.FINANCIAL;
using HPA_ISO8583;
using PAYMAYA_ISO8583;
using Newtonsoft.Json.Linq;
using SDGDAL;
using SDGDAL.Repositories;
using SDGUtil;
using SDGWebService.Classes;
using SDGWebService.TLVFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using GatewayProcessor.PayMaya;
using System.Web.Script.Serialization;

namespace SDGWebService.WebserviceFunctions
{
    public class WebserviceFunctionCreditSwipe
    {
        private MobileAppRepository _mAppRepo = new MobileAppRepository();
        private MerchantRepository _merchantRepo = new MerchantRepository();
        private TransactionRepository _transRepo = new TransactionRepository();
        private MidsRepository _midsRepo = new MidsRepository();
        private EmailServerRepository _emailServerRepo = new EmailServerRepository();
        private MobileAppFeaturesRepository _posFeatures = new MobileAppFeaturesRepository();
        private MerchantBranchPOSRepository _posRepo = new MerchantBranchPOSRepository();
        private UserRepository _userRepo = new UserRepository();
        private DebitSystemTraceNumRepository _traceNumRepo = new DebitSystemTraceNumRepository();
        private DeviceRepository _deviceRepo = new DeviceRepository();
        private MerchantBranchRepository _branchRepo = new MerchantBranchRepository();
        private EMVCreditDebitRepository _emvCreditDebitRepo = new EMVCreditDebitRepository();
        private Functions.MobileAppFunctions mobileAppFunctions = new Functions.MobileAppFunctions();
        private AndroidAppVersionRepo _androidAppVersionRepo = new AndroidAppVersionRepo();
        private BatchReportRepository _batchRepo = new BatchReportRepository();

        private POSWSResponse GetCardDetails(CardDetails cd, SDGDAL.Enums.SwipeDevice swipeDevice, SDGDAL.Enums.CardAction cardAction,
            out string track1, out string track2, out string cardNumber, out string nameOnCard,
            out string expDate, out string expYear, out string expMonth, out ClassTLV emvDataResult, out string LRC)
        {
            string bdk = System.Configuration.ConfigurationManager.AppSettings["BDK"].ToString();
            POSWSResponse response = new POSWSResponse();
            WisepadLayer.WisePad wisepad = new WisepadLayer.WisePad();
            RamblerLayer1.Class1 rambler1 = new RamblerLayer1.Class1();
            MobileAppMethods mobileAppMethods = new MobileAppMethods();
            Track1Data track1Data = new Track1Data();
            Track2Data track2Data = new Track2Data();
            TLVParser tlvParser = new TLVParser();
            emvDataResult = new ClassTLV();
            track1 = "";
            track2 = "";
            cardNumber = "";
            nameOnCard = "";
            expDate = "";
            expYear = "";
            expMonth = "";
            LRC = "";

            switch (swipeDevice)
            {
                #region Rambler DeviceId = 1
                case SDGDAL.Enums.SwipeDevice.Rambler:
                    string dk2 = ConfigurationManager.AppSettings["DK2"].ToString();

                    var resultTrack2 = rambler1.IDC_Decrypt2(cd.Track1, cd.Ksn, dk2);
                    var resultTrack1 = rambler1.IDC_Decrypt_Track1(cd.Track1, cd.Ksn, dk2);

                    track2 = resultTrack2.Track2;
                    track1 = resultTrack1.Track1;

                    if (resultTrack1.Track1 != "")
                    {
                        cardNumber = mobileAppMethods.MobileApp_Parse_CardNum_Track1(resultTrack1.Track1);
                    }
                    else
                    {
                        cardNumber = mobileAppMethods.MobileApp_Parse_CardNum_Track2(resultTrack2.Track2);
                    }

                    if (track2 == "")
                    {
                        int start = track1.LastIndexOf("^");
                        int end = track1.LastIndexOf("?");

                        string cdata = track1.Substring(start + 1, (track1.Length - (start + 1) - 1));

                        if (cdata.Length == 20)
                        {
                            track2 = ";" + cardNumber + "=" + cdata + "?";
                        }
                        else
                        {
                            string sc = cdata.Substring(4, 3);
                            string ddata1 = cdata.Substring(7, 10);
                            string ddata = cdata.Substring(cdata.Length - 9, 3);
                            track2 = ";" + cardNumber + "=" + cd.ExpYear + cd.ExpMonth + sc + ddata1 + ddata + "?";
                        }
                    }

                    if (!string.IsNullOrEmpty(resultTrack2.LRCTrack2))
                    {
                        LRC = resultTrack2.LRCTrack2;
                    }
                    else
                    {
                        LRC = resultTrack1.LRCTrack1;
                    }

                    break;
                #endregion

                #region Rambler Demo DeviceId = 2
                case SDGDAL.Enums.SwipeDevice.RamblerDemo:
                    RamblerLayer1.Class1 rambler = new RamblerLayer1.Class1();
                    var resTrack2 = rambler1.IDC_Decrypt2(cd.Track1, cd.Ksn, bdk);
                    var resTrack1 = rambler1.IDC_Decrypt_Track1(cd.Track1, cd.Ksn, bdk);

                    track2 = resTrack2.Track2;
                    track1 = resTrack1.Track1;

                    if (resTrack1.Track1 != "")
                    {
                        cardNumber = mobileAppMethods.MobileApp_Parse_CardNum_Track1(resTrack1.Track1);
                    }
                    else
                    {
                        cardNumber = mobileAppMethods.MobileApp_Parse_CardNum_Track2(resTrack2.Track2);
                    }

                    if (track2 == "")
                    {
                        int start = track1.LastIndexOf("^");
                        int end = track1.LastIndexOf("?");

                        string cdata = track1.Substring(start + 1, (track1.Length - (start + 1) - 1));

                        if (cdata.Length == 20)
                        {
                            track2 = ";" + cardNumber + "=" + cdata + "?";
                        }
                        else
                        {
                            string sc = cdata.Substring(4, 3);
                            string ddata1 = cdata.Substring(7, 10);
                            string ddata = cdata.Substring(cdata.Length - 9, 3);
                            track2 = ";" + cardNumber + "=" + cd.ExpYear + cd.ExpMonth + sc + ddata1 + ddata + "?";
                        }
                    }

                    if (!string.IsNullOrEmpty(resTrack2.LRCTrack2))
                    {
                        LRC = resTrack2.LRCTrack2;
                    }
                    else
                    {
                        LRC = resTrack1.LRCTrack1;
                    }

                    break;
                #endregion

                #region WisePad1 DeviceId = 3
                case SDGDAL.Enums.SwipeDevice.WisePad1:
                    string wisepadKey = System.Configuration.ConfigurationManager.AppSettings["DKWisepad1"].ToString();
                    string wisepadKeyTlvEmv = System.Configuration.ConfigurationManager.AppSettings["DKWisepad1Emv"].ToString();

                    if (cardAction == Enums.CardAction.EMV)
                    {
                        string decryptEmvIcc = wisepad.DecryptC5EmvData(cd.EmvICCData, wisepadKeyTlvEmv);

                        var resultTlv = TLVParser.DecodeTLV(decryptEmvIcc);
                        emvDataResult.EmvIccData = resultTlv.EmvIccData.ToUpper();

                        emvDataResult.SubField1Data = resultTlv.SubField1Data;
                        emvDataResult.SubField2Data = resultTlv.SubField2Data;
                        emvDataResult.TrackData = resultTlv.TrackData;
                        track2 = resultTlv.Track2;
                        cardNumber = resultTlv.CardNumber;

                        break;
                    }
                    else
                    {
                        track1 = wisepad.DecryptTrackData1(cd.Track1, wisepadKey);
                        track2 = wisepad.DecryptTrackData2(cd.Track2, wisepadKey);
                    }

                    if (!string.IsNullOrEmpty(track2))
                    {
                        cardNumber = mobileAppMethods.MobileApp_WisePad_CardNum_Track2(track2);
                    }
                    else
                    {
                        cardNumber = mobileAppMethods.MobileApp_Parse_CardNum_Track1(track1);

                        string cdata = track1.Substring(track1.Length - 28);
                        string sc = cdata.Substring(0, 3);
                        string ddata = cdata.Substring(cdata.Length - 10, 3);
                        track2 = ";" + cardNumber + "=" + cd.ExpYear + cd.ExpMonth + sc + "0000000000" + ddata + "?";
                    }

                    break;
                #endregion

                #region WisePad2 DeviceId = 4
                case SDGDAL.Enums.SwipeDevice.WisePad2:
                    string key = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);
                    string decryptedTrack2 = WisepadLayer.TripleDES.decrypt_CBC(cd.Track2, key);

                    if (cardAction == Enums.CardAction.SwipeCard)
                    {
                        var resWisepadTrack2 = TLVParser.ParseWisePadTrack2(decryptedTrack2);

                        if (resWisepadTrack2.Track2 != null)
                        {
                            cardNumber = resWisepadTrack2.CardNumber;
                            track2 = resWisepadTrack2.Track2;
                        }
                        else
                        {
                            response.ErrNumber = "2100.1";
                            response.Message = "Cannot read Track Data.";
                            response.Status = "Declined";
                            return response;
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2100.2";
                        response.Message = "Invalid Swiped Card";
                        response.Status = "Declined";
                        return response;
                    }

                    break;
                #endregion

                #region ChipperAudio 1.0 DeviceId = 5
                case SDGDAL.Enums.SwipeDevice.ChipperAudioI:
                    string keyData = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);
                    string decTrack2 = WisepadLayer.TripleDES.decrypt_CBC(cd.Track2, keyData);

                    if (cardAction == Enums.CardAction.SwipeCard)
                    {
                        var resWisepadTrack2 = TLVParser.ParseWisePadTrack2(decTrack2);

                        if (resWisepadTrack2.Track2 != null)
                        {
                            cardNumber = resWisepadTrack2.CardNumber;
                            track2 = resWisepadTrack2.Track2;
                        }
                        else
                        {
                            response.ErrNumber = "2100.1";
                            response.Message = "Cannot read Track Data.";
                            response.Status = "Declined";
                            return response;
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2100.2";
                        response.Message = "Invalid Swiped Card";
                        response.Status = "Declined";
                        return response;
                    }

                    break;
                #endregion

                #region Chipper Audio 2.0 DeviceId = 6
                case SDGDAL.Enums.SwipeDevice.ChipperAudioII:
                    string chipperIkey = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);
                    string chipperIdecTrack2 = WisepadLayer.TripleDES.decrypt_CBC(cd.Track2, chipperIkey);

                    if (cardAction == Enums.CardAction.SwipeCard)
                    {
                        var resWisepadTrack2 = TLVParser.ParseWisePadTrack2(chipperIdecTrack2);

                        if (resWisepadTrack2.Track2 != null)
                        {
                            cardNumber = resWisepadTrack2.CardNumber;
                            track2 = resWisepadTrack2.Track2;
                        }
                        else
                        {
                            response.ErrNumber = "2100.1";
                            response.Message = "Cannot read Track Data.";
                            response.Status = "Declined";
                            return response;
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2100.2";
                        response.Message = "Invalid Swiped Card";
                        response.Status = "Declined";
                        return response;
                    }

                    break;
                #endregion

                #region WisePos DeviceId = 7
                case SDGDAL.Enums.SwipeDevice.WisePos:
                    string keyWisePos = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);
                    string decTrack2WisePos = WisepadLayer.TripleDES.decrypt_CBC(cd.Track2, keyWisePos);

                    if (cardAction == Enums.CardAction.SwipeCard)
                    {
                        var resWisepadTrack2 = TLVParser.ParseWisePadTrack2(decTrack2WisePos);

                        if (resWisepadTrack2.Track2 != null)
                        {
                            cardNumber = resWisepadTrack2.CardNumber;
                            track2 = resWisepadTrack2.Track2;
                        }
                        else
                        {
                            response.ErrNumber = "2100.1";
                            response.Message = "Cannot read Track Data.";
                            response.Status = "Declined";
                            return response;
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2100.2";
                        response.Message = "Invalid Swiped Card";
                        response.Status = "Declined";
                        return response;
                    }

                    break;
                #endregion

                #region WisePad Plus DeviceId = 8
                case SDGDAL.Enums.SwipeDevice.WisePadPlus:
                    string keyBT1 = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);
                    string decTrack2BT = WisepadLayer.TripleDES.decrypt_CBC(cd.Track2, keyBT1);

                    if (cardAction == Enums.CardAction.SwipeCard)
                    {
                        var resWisepadTrack2 = TLVParser.ParseWisePadTrack2(decTrack2BT);

                        if (resWisepadTrack2.Track2 != null)
                        {
                            cardNumber = resWisepadTrack2.CardNumber;
                            track2 = resWisepadTrack2.Track2;
                        }
                        else
                        {
                            response.ErrNumber = "2100.1";
                            response.Message = "Cannot read Track Data.";
                            response.Status = "Declined";
                            return response;
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2100.2";
                        response.Message = "Invalid Swiped Card";
                        response.Status = "Declined";
                        return response;
                    }

                    break;
                #endregion

                #region Chipper BT 1.0 DeviceId = 9
                case SDGDAL.Enums.SwipeDevice.ChipperBT1:
                    string keyChipperBT1 = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);
                    string decTrack2ChipperBT = WisepadLayer.TripleDES.decrypt_CBC(cd.Track2, keyChipperBT1);

                    if (cardAction == Enums.CardAction.SwipeCard)
                    {
                        var resWisepadTrack2 = TLVParser.ParseWisePadTrack2(decTrack2ChipperBT);

                        if (resWisepadTrack2.Track2 != null)
                        {
                            cardNumber = resWisepadTrack2.CardNumber;
                            track2 = resWisepadTrack2.Track2;
                        }
                        else
                        {
                            response.ErrNumber = "2100.1";
                            response.Message = "Cannot read Track Data.";
                            response.Status = "Declined";
                            return response;
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2100.2";
                        response.Message = "Invalid Swiped Card";
                        response.Status = "Declined";
                        return response;
                    }

                    break;
                #endregion

                #region Chipper BT 2.0 Deviceid = 10
                case SDGDAL.Enums.SwipeDevice.ChipperBT2:
                    string keyBT2 = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);
                    string decTrack2BT2 = WisepadLayer.TripleDES.decrypt_CBC(cd.Track2, keyBT2);

                    if (cardAction == Enums.CardAction.SwipeCard)
                    {
                        var resWisepadTrack2 = TLVParser.ParseWisePadTrack2(decTrack2BT2);

                        if (resWisepadTrack2.Track2 != null)
                        {
                            cardNumber = resWisepadTrack2.CardNumber;
                            track2 = resWisepadTrack2.Track2;
                        }
                        else
                        {
                            response.ErrNumber = "2100.1";
                            response.Message = "Cannot read Track Data.";
                            response.Status = "Declined";
                            return response;
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2100.2";
                        response.Message = "Invalid Swiped Card";
                        response.Status = "Declined";
                        return response;
                    }

                    break;
                #endregion

                #region QPosMini DeviceId = 11
                case SDGDAL.Enums.SwipeDevice.QPosMini:
                    string iPek = ConfigurationManager.AppSettings["IPEK"].ToString();
                    string keyIpek = DSPREADQPosLayer.EMVSwipeTLV.DUKPTServer.GetDataKeyFromIPEK(cd.Ksn, iPek);
                    string trackData = DSPREADQPosLayer.EMVSwipeTLV.TripleDES.decrypt_CBC(cd.Track2, keyIpek);

                    if (cardAction == Enums.CardAction.SwipeCard)
                    {
                        var qposTrack2 = TLVParser.ParseQPOSTrack2(trackData);

                        if (qposTrack2.Track2 != null)
                        {
                            cardNumber = qposTrack2.CardNumber;
                            track2 = qposTrack2.Track2;
                        }
                        else
                        {
                            response.ErrNumber = "2100.1";
                            response.Message = "Cannot read Track Data.";
                            response.Status = "Declined";
                            return response;
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2100.2";
                        response.Message = "Invalid Swiped Card";
                        response.Status = "Declined";
                        return response;
                    }

                    break;
                #endregion

                default:
                    response.ErrNumber = "2100.3";
                    response.Message = "Invalid Swipe Device";
                    response.Status = "Declined";
                    return response;
            }

            return response;
        }

        public PurchaseResponse TransactionPurchaseCreditSwipe(TransactionRequest request) 
        {
            string action = string.Empty;

            PurchaseResponse response = new PurchaseResponse();
            response.POSWSResponse = new POSWSResponse();

            object obj = response.POSWSResponse;
            if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.TRANSACTION_PURCHASE_CREDIT_SWIPE, (object)request, out obj))
            {
                response.POSWSResponse = (POSWSResponse)obj;
                return response;
            }

            var transactionAttemptId = 0;

            response.POSWSResponse.ErrNumber = "0";

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    var mobileApp = new SDGDAL.Entities.MobileApp();
                    var account = new SDGDAL.Entities.Account();

                    action = "checking mobile app availability.";

                    response.POSWSResponse = mobileAppFunctions.CheckStatus(request.POSWSRequest.RToken, out mobileApp, out account);

                    if (response.POSWSResponse.Status == "Declined")
                    {
                        return response;
                    }

                    MobileAppMethods mobileAppMethods = new MobileAppMethods();

                    string track1 = "";
                    string track2 = "";
                    string cardNumber = "";
                    string nameOnCard = "";
                    string expDate = "";
                    string expYear = "";
                    string expMonth = "";
                    string LRC = "";

                    SDGDAL.Enums.SwipeDevice sd = (SDGDAL.Enums.SwipeDevice)request.Device;
                    SDGDAL.Enums.CardAction cardAction = (SDGDAL.Enums.CardAction)request.Action;

                    ClassTLV emvDataResult = new ClassTLV();

                    response.POSWSResponse = GetCardDetails(request.CardDetails, sd, cardAction, out track1, out track2,
                        out cardNumber, out nameOnCard, out expDate, out expYear, out expMonth, out emvDataResult, out LRC);

                    if (response.POSWSResponse.Status == "Declined")
                    {
                        return response;
                    }

                    #region Read Tracks

                    try
                    {
                        action = "reading track data.";

                        decimal tempNum = Convert.ToDecimal(cardNumber);

                        decimal dMonth, dYear;

                        dMonth = SDGUtil.Functions.ConvertNumeric(request.CardDetails.ExpMonth);
                        dYear = SDGUtil.Functions.ConvertNumeric(request.CardDetails.ExpYear);

                        if (dMonth == -1 || dYear == -1)
                        {
                            response.POSWSResponse.ErrNumber = "2100.3";
                            response.POSWSResponse.Message = "Card decode error";
                            response.POSWSResponse.Status = "Declined";
                            response.POSWSResponse.UpdatePending = false;
                            return response;
                        }
                    }
                    catch
                    {
                        response.POSWSResponse.ErrNumber = "2100.3";
                        response.POSWSResponse.Message = "Card decode error";
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.UpdatePending = false;
                        return response;
                    }

                    #endregion Read Tracks

                    action = "logging mobile app action.";
                    mobileAppFunctions.LogMobileAppAction("Purchase Credit Transaction Swipe", mobileApp.MobileAppId, account.AccountId, request.POSWSRequest.GPSLat, request.POSWSRequest.GPSLong);

                    if (!mobileApp.MobileAppFeatures.CreditTransaction)
                    {
                        response.POSWSResponse.ErrNumber = "2101.1";
                        response.POSWSResponse.Message = "Credit transactions are currently disabled on this device";
                        response.POSWSResponse.Status = "Declined";
                        return response;
                    }

                    #region Transaction

                    bool updatePending = mobileApp.UpdatePending;

                    var transaction = new SDGDAL.Entities.Transaction();
                    var transactionAttempt = new SDGDAL.Entities.TransactionAttempt();

                    if (!_deviceRepo.HasDeviceByMasterDeviceId(request.Device))
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "No Device Available.";
                        response.POSWSResponse.ErrNumber = "2101.3";
                        response.POSWSResponse.UpdatePending = updatePending;
                        return response;
                    }

                    #region validate device

                    //try
                    //{
                    //    LogicLayer.classDevice classDev = new classDevice();
                    //    LogicLayer.classDevice.EntityDevice sendDev = new classDevice.EntityDevice();
                    //    LogicLayer.classDevice.DeviceResponse retDev = new classDevice.DeviceResponse();

                    //    sendDev.DeviceLink.Device_ID = dDeviceID;
                    //    sendDev.DeviceLink.MobileApp_Id = retMob.MobileApp[0].MobileAppRow.ID;
                    //    sendDev.AssignedDevice.Merchant_ID = retMob.MobileApp[0].MerchantRow.ID;

                    //    retDev = classDev.DeviceValidation(sendDev);

                    //    if (retDev.ErrNum != DataLayer1.ENums.DeviceError.NoError)
                    //    {
                    //        Ret.Status = "DECLINED";
                    //        Ret.Message = "Unauthorized Payment Device";
                    //        Ret.ErrNumber = "2045.13";
                    //        Ret.UpdatePending = false;
                    //        return Ret;
                    //    }
                    //}
                    //catch
                    //{
                    //    Ret.Status = "DECLINED";
                    //    Ret.Message = "Invalid Input Type";
                    //    Ret.ErrNumber = "2045.14";
                    //    Ret.UpdatePending = false;
                    //    return Ret;
                    //}

                    #endregion validate device

                    action = "setting up transaction and transactionattempt details.";
                    int posId = mobileApp.MerchantBranchPOSId;

                    //get cardtype and do simple card validate
                    int cardTypeId = SDGUtil.Functions.GetCardType(cardNumber);
                    if (cardTypeId != 0)
                    {
                        transaction.CardTypeId = cardTypeId;
                    }
                    else
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "Invalid card type";
                        response.POSWSResponse.ErrNumber = "2101.2";
                        response.POSWSResponse.UpdatePending = updatePending;
                        response.TransactionNumber = "0";
                        return response;
                    }

                    transaction.CardNumber = cardNumber;
                    transaction.NameOnCard = request.CardDetails.NameOnCard;
                    transaction.ExpMonth = request.CardDetails.ExpMonth;
                    transaction.ExpYear = request.CardDetails.ExpYear;
                    transaction.CSC = "";
                    transaction.OriginalAmount = request.CardDetails.Amount;

                    transaction.TaxAmount = 0;

                    #region //TODO: get TaxId

                    //Tax resTax = new Tax();
                    //resTax = getTax(retMob.MobileApp[0].MobileAppRow.MerchantLocationPOSID);
                    //decimal Tax_ID = resTax.TaxID;
                    //decimal TaxCal_ID = resTax.TaxCalID;

                    //if (Tax_ID != 0)
                    //{
                    //    sendTrans.TransDetail.Tax_ID = Tax_ID;
                    //}
                    //else
                    //{
                    //    Ret.Status = "DECLINED";
                    //    Ret.Message = "Invalid Tax";
                    //    Ret.ErrNumber = "2045.16";
                    //    Ret.UpdatePending = isUpdate;
                    //    Ret.TransNumber = "0";
                    //    return Ret;
                    //}

                    #endregion //TODO: get TaxId

                    //TODO: Should compute?
                    transaction.FinalAmount = request.CardDetails.Amount;

                    try
                    {
                        transaction.CurrencyId = _transRepo.GetCurrencyIdByCurrencyName(request.CardDetails.Currency);
                    }
                    catch
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "Invalid currency.";
                        response.POSWSResponse.ErrNumber = "2101.3";
                        response.POSWSResponse.UpdatePending = updatePending;
                        response.TransactionNumber = "0";
                        return response;
                    }

                    transaction.Track1 = track1;
                    transaction.Track2 = track2;
                    transaction.Track3 = request.CardDetails.Track3;

                    transaction.DateCreated = DateTime.Now;
                    transaction.RefNumApp = request.CardDetails.RefNumberApp;
                    transaction.RefNumSales = request.CardDetails.RefNumberSale;
                    transaction.Notes = request.Device.ToString();
                    transaction.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.CreditSwipe);
                    transaction.MerchantPOSId = mobileApp.MerchantBranchPOSId;
                    transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale);
                    transactionAttempt.AccountId = account.AccountId;
                    transactionAttempt.MobileAppId = mobileApp.MobileAppId;
                    transactionAttempt.DeviceId = request.Device;
                    transactionAttempt.GPSLat = request.POSWSRequest.GPSLat;
                    transactionAttempt.GPSLong = request.POSWSRequest.GPSLong;
                    transactionAttempt.Amount = transaction.OriginalAmount;
                    transactionAttempt.Notes = request.Device.ToString();

                    #region TODO: Compute tax

                    //TODO: Compute tax? Do Tax?
                    //calculate tax
                    //TaxDetail retTax = new TaxDetail();
                    //decimal TaxAmount = dAmount - dTips;
                    //retTax = calculateTax(TaxAmount, Tax_ID, TaxCal_ID);
                    //if (retTax.printTax)
                    //{
                    //    sendTrans.Tax1 = retTax.Tax1;
                    //    sendTrans.Tax2 = retTax.Tax2;
                    //}
                    //else
                    //{
                    //    sendTrans.Tax1 = "";
                    //    sendTrans.Tax2 = "";
                    //}

                    //transaction.TransactionAttempts.Add(transactionAttempt); //cause of error

                    #endregion TODO: Compute tax

                    #region Handle Transaction response

                    action = "setting up transaction entry for database.";

                    try
                    {
                        action = "checking mid status before setting up transaction entry.";
                        var mid = new SDGDAL.Entities.Mid();

                        mid = _midsRepo.GetMidByPosIdAndCardTypeId(transaction.MerchantPOSId, transaction.CardTypeId);

                        if (mid == null)
                        {
                            response.POSWSResponse.ErrNumber = "2101.4";
                            response.POSWSResponse.Message = "Mid not found.";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }
                        else
                        {
                            if (!mid.IsActive || mid.IsDeleted)
                            {
                                response.POSWSResponse.ErrNumber = "2101.5";
                                response.POSWSResponse.Message = "Mid is Inactive.";
                                response.POSWSResponse.Status = "Declined";

                                return response;
                            }

                            if (!mid.Switch.IsActive)
                            {
                                response.POSWSResponse.ErrNumber = "2101.6";
                                response.POSWSResponse.Message = "Switch is Inactive.";
                                response.POSWSResponse.Status = "Declined";

                                return response;
                            }
                        }

                        #region Encrypt Card Data

                        action = "trying to encrypt card data.";
                        //ENCRYPT CARD DATA
                        string NE_CARD = transaction.CardNumber;
                        string NE_EMONTH = transaction.ExpMonth;
                        string NE_EYEAR = transaction.ExpYear;
                        string NE_CSC = transaction.CSC;
                        string E_CARD;
                        string E_EMONTH, E_EYEAR;
                        string E_CSC;
                        byte[] desKey;
                        byte[] desIV;

                        //card number masking
                        string s = NE_CARD.Substring(NE_CARD.Length - 4);
                        string r = new string('*', NE_CARD.Length);
                        string MASK_CARD = r + s;
                        //month masking
                        string MASK_EMONTH = new string('*', NE_EMONTH.Length);
                        //year masking
                        string MASK_EYEAR = new string('*', NE_EYEAR.Length);
                        //CSC masking
                        string MASK_CSC = new string('*', NE_CSC.Length);

                        E_CARD = Utility.GenerateSymmetricKeyAndEcryptData(MASK_CARD, out desKey, out desIV);

                        transaction.Key = new SDGDAL.Entities._Key();
                        transaction.Key.Key = Convert.ToBase64String(desKey);
                        transaction.Key.IV = Convert.ToBase64String(desIV);

                        E_EMONTH = Utility.EncryptDataWithExistingKey(NE_EMONTH, desKey, desIV);
                        E_EYEAR = Utility.EncryptDataWithExistingKey(NE_EYEAR, desKey, desIV);
                        E_CSC = Utility.EncryptDataWithExistingKey(NE_CSC, desKey, desIV);

                        #endregion Encrypt Card Data

                        action = "checking if address is required. ";
                        if (mid.Switch.IsAddressRequired)
                        {
                            action = "saving temp transaction because switch requires address.";
                            var tempTransaction = new SDGDAL.Entities.TempTransaction();

                            tempTransaction.CopyProperties(transaction);

                            var nTempTransaction = _transRepo.CreateTempTransaction(tempTransaction);

                            if (nTempTransaction.TransactionId > 0)
                            {
                                response.POSWSResponse.Status = "Declined";
                                response.POSWSResponse.Message = "This transaction require customer to enter their billing address.";
                                response.POSWSResponse.ErrNumber = "2101.7";
                                return response;
                            }
                            else
                            {
                                throw new Exception(action);
                            }
                        }

                        transactionAttempt.TransactionChargesId = mid.TransactionChargesId;

                        action = "checking if switch is active. ";
                        if (!mid.Switch.IsActive)
                        {
                            transactionAttempt.DateSent = DateTime.Now;
                            transactionAttempt.DateReceived = DateTime.Now;
                            transactionAttempt.DepositDate = DateTime.Now;
                            transactionAttempt.Notes = ((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId).ToString() + " Declined. Switch inactive.";
                            transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                        }
                        else
                        {
                            transactionAttempt.DateSent = DateTime.Now;
                            transactionAttempt.DateReceived = Convert.ToDateTime("1900/01/01");
                            transactionAttempt.DepositDate = Convert.ToDateTime("1900/01/01");
                            transactionAttempt.TransNumber = SDGUtil.Functions.GenerateSystemTraceAudit();

                            if (!mid.IsActive || mid.IsDeleted)
                            {
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                            }
                        }

                        action = "saving transaction details to database.";
                        var nTransaction = new SDGDAL.Entities.Transaction();
                        nTransaction.CopyProperties(transaction);

                        nTransaction.CardNumber = E_CARD;
                        nTransaction.ExpMonth = E_EMONTH;
                        nTransaction.ExpYear = E_EYEAR;
                        nTransaction.CSC = E_CSC;
                        nTransaction.CurrencyId = mid.CurrencyId;
                        nTransaction.MidId = mid.MidId;
                        nTransaction.Track1 = "";
                        nTransaction.Track2 = "";
                        nTransaction.Track3 = "";
                        
                        //Save transactions to db
                        var rTransaction = _transRepo.CreateTransaction(nTransaction, transactionAttempt);

                        if (rTransaction.TransactionId > 0)
                        {
                            action = "processing transaction for api integration. Transaction was successfully saved.";
                            transaction.TransactionId = rTransaction.TransactionId;

                            if (((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId) == Enums.TransactionType.Declined)
                            {
                                response.POSWSResponse.Message = "Transaction failed. Please contact Support.";
                                response.POSWSResponse.ErrNumber = "2101.8";
                                response.POSWSResponse.Status = "Declined";
                                return response;
                            }
                            else
                            {
                                transactionAttemptId = transactionAttempt.TransactionAttemptId;

                                #region Master Card Demo

                                if (mid.Switch.SwitchCode == "MasterCardDemo")
                                {
                                    if (transactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale))
                                    {
                                        action = "processing master card.";
                                        GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                        GatewayProcessor.MasterCard.TransactionData transData = new GatewayProcessor.MasterCard.TransactionData();
                                        GatewayProcessor.MasterCard.APILogin apiLogin = new GatewayProcessor.MasterCard.APILogin();

                                        transData.AccessCode = mid.Param_1;
                                        transData.MerchantId = mid.Param_2;
                                        transData.TerminalId = mid.Param_6;
                                        transData.SecureHash = mid.Param_3;
                                        apiLogin.Username = mid.Param_4;
                                        apiLogin.Password = mid.Param_5;

                                        ///for mid AddendumData param
                                        transData.PFI = mid.Param_13;
                                        transData.ISO = mid.Param_14;
                                        transData.SMI = mid.Param_15;
                                        transData.PFN = mid.Param_16;
                                        transData.SMN = mid.Param_17;
                                        transData.MSA = mid.Param_18;
                                        transData.MCI = mid.Param_19;
                                        transData.MST = mid.Param_20;
                                        transData.MCO = mid.Param_21;
                                        transData.MPC = mid.Param_22;
                                        transData.MPP = mid.Param_23;
                                        transData.MCC = mid.Param_24;

                                        decimal orgAmount = transactionAttempt.Amount;
                                        decimal finalAmount = orgAmount * 100;

                                        try
                                        {
                                            transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                        }
                                        catch
                                        {
                                            transData.Amount = finalAmount.ToString();
                                        }

                                        transData.CardNumber = transaction.CardNumber;
                                        transData.CardExpirationDate = transaction.ExpYear.ToString() + transaction.ExpMonth.ToString();
                                        transData.CSC = transaction.CSC;
                                        transData.Currency = mid.Currency.CurrencyCode;
                                        transData.TransNumber = rTransaction.TransactionId + "-" + transactionAttempt.TransactionAttemptId;
                                        transData.OrderInfo = transData.TransNumber;
                                        transData.SecureHash = transData.SecureHash;
                                        transData.MerchTxnRef = transaction.TransactionId + "-" + transactionAttempt.TransactionAttemptId;
                                        transData.Track1 = transaction.Track1;
                                        transData.Track2 = transaction.Track2;
                                        transData.LRC = LRC;
                                        transData.EmvIccData = emvDataResult.EmvIccData;

                                        var apiResponse = gateway.ProcessMasterCardDemo(transData, apiLogin, "purchase", cardTypeId);

                                        if (apiResponse.Status == "Approved")
                                        {
                                            transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                            transactionAttempt.AuthNumber = apiResponse.AuthorizeId;
                                            transactionAttempt.ReturnCode = apiResponse.AcqResponseCode;
                                            transactionAttempt.SeqNumber = apiResponse.ReceiptNumber;
                                            transactionAttempt.TransNumber = apiResponse.TransactionNumber;
                                            transactionAttempt.PosEntryMode = apiResponse.PosEntryMode;

                                            transactionAttempt.BatchNumber = apiResponse.BatchNumber;
                                            transactionAttempt.DisplayReceipt = "";
                                            transactionAttempt.DisplayTerminal = "";
                                            transactionAttempt.DateReceived = DateTime.Now;
                                            transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                            transactionAttempt.Notes = " API Mastercard Demo Purchase Approved";

                                            response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)rTransaction.CardTypeId);
                                            response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                            response.POSWSResponse.ErrNumber = "0";
                                            response.POSWSResponse.Message = "Transaction Successful.";
                                            response.POSWSResponse.Status = "Approved";
                                        }
                                        else if (apiResponse.Status == "Declined")
                                        {
                                            transactionAttempt.PosEntryMode = apiResponse.PosEntryMode;
                                            transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                            transactionAttempt.AuthNumber = "";
                                            transactionAttempt.ReturnCode = apiResponse.AcqResponseCode == null ? apiResponse.TransactionResponseCode : apiResponse.AcqResponseCode;
                                            transactionAttempt.SeqNumber = apiResponse.ReceiptNumber;
                                            transactionAttempt.TransNumber = apiResponse.TransactionNumber;
                                            transactionAttempt.DisplayReceipt = "";
                                            transactionAttempt.DisplayTerminal = "";
                                            transactionAttempt.DateReceived = DateTime.Now;
                                            transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                            transactionAttempt.Notes = " API Mastercard Demo Purchase Declined." + apiResponse.Message;
                                            transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                            response.POSWSResponse.ErrNumber = apiResponse.AcqResponseCode == null ? apiResponse.TransactionResponseCode : apiResponse.AcqResponseCode;
                                            response.POSWSResponse.Message = apiResponse.Message;
                                            response.POSWSResponse.Status = "Declined";
                                        }
                                        else
                                        {
                                            response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Message;
                                            response.POSWSResponse.ErrNumber = "2101.7";
                                            response.POSWSResponse.Status = "Declined";
                                            return response;
                                        }

                                        #region TO DO Update Balance Mid

                                        // TODO: Should update mid balance (if already implemented)
                                        /*
                                         try
                                        {
                                            //update balance
                                            classMids.EntityMids sendBalance = new classMids.EntityMids();
                                            classMids.MidsResponse retBalance = new classMids.MidsResponse();
                                            classMids._Mids dataMids = new classMids._Mids();
                                            classMids classMids = new classMids();

                                            decimal newBalance = MidsDetail.MidsPrice.CurrentBalance + transAttData.TransAttemptDetail.Amount;

                                            dataMids.MidsPrice.ID = MidsDetail.MidsPrice.ID;
                                            dataMids.MidsPrice.Batch = MidsDetail.MidsPrice.Batch;
                                            dataMids.MidsPrice.BatchCloseTypeID = MidsDetail.MidsPrice.BatchCloseTypeID;
                                            dataMids.MidsPrice.BatchTime = MidsDetail.MidsPrice.BatchTime;
                                            dataMids.MidsPrice.Capture = MidsDetail.MidsPrice.Capture;
                                            dataMids.MidsPrice.CardNotPresent = MidsDetail.MidsPrice.CardNotPresent;
                                          //  dataMids.MidsPrice.ChargeBack = MidsDetail.MidsPrice.ChargeBack;
                                            dataMids.MidsPrice.CurrentBalance = newBalance;
                                            dataMids.MidsPrice.DailyLimit = MidsDetail.MidsPrice.DailyLimit;
                                            dataMids.MidsPrice.Purchased = MidsDetail.MidsPrice.Purchased;
                                            dataMids.MidsPrice.DiscountRate = MidsDetail.MidsPrice.DiscountRate;
                                            dataMids.MidsPrice.eCommerce = MidsDetail.MidsPrice.eCommerce;
                                            dataMids.MidsPrice.PreAuth = MidsDetail.MidsPrice.PreAuth;
                                            dataMids.MidsPrice.Refund = MidsDetail.MidsPrice.Refund;
                                            dataMids.MidsPrice.Settlement = MidsDetail.MidsPrice.Settlement;
                                            dataMids.MidsPrice.Void = MidsDetail.MidsPrice.Void;

                                            sendBalance.MidsDetail.Add(dataMids);

                                            retBalance = classMids.MidsPrice_Update(sendBalance);
                                        }
                                        catch
                                        {
                                        }
                                         */

                                        #endregion TO DO Update Balance Mid
                                    }
                                    else
                                    {
                                        string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                        response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }

                                #endregion Master Card Demo

                                #region Master Card

                                else if (mid.Switch.SwitchCode == "MasterCard")
                                {
                                    if (transactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale))
                                    {
                                        action = "processing master card.";
                                        GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                        GatewayProcessor.MasterCard.TransactionData transData = new GatewayProcessor.MasterCard.TransactionData();
                                        GatewayProcessor.MasterCard.APILogin apiLogin = new GatewayProcessor.MasterCard.APILogin();

                                        transData.AccessCode = mid.Param_1;
                                        transData.MerchantId = mid.Param_2;
                                        transData.TerminalId = mid.Param_6;
                                        transData.SecureHash = mid.Param_3;
                                        apiLogin.Username = mid.Param_4;
                                        apiLogin.Password = mid.Param_5;

                                        ///for mid AddendumData param
                                        transData.PFI = mid.Param_13;
                                        transData.ISO = mid.Param_14;
                                        transData.SMI = mid.Param_15;
                                        transData.PFN = mid.Param_16;
                                        transData.SMN = mid.Param_17;
                                        transData.MSA = mid.Param_18;
                                        transData.MCI = mid.Param_19;
                                        transData.MST = mid.Param_20;
                                        transData.MCO = mid.Param_21;
                                        transData.MPC = mid.Param_22;
                                        transData.MPP = mid.Param_23;
                                        transData.MCC = mid.Param_24;

                                        decimal orgAmount = transactionAttempt.Amount;
                                        decimal finalAmount = orgAmount * 100;

                                        try
                                        {
                                            transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                        }
                                        catch
                                        {
                                            transData.Amount = finalAmount.ToString();
                                        }

                                        transData.CardNumber = transaction.CardNumber;
                                        transData.CardExpirationDate = transaction.ExpYear.ToString() + transaction.ExpMonth.ToString();
                                        transData.CSC = transaction.CSC;
                                        transData.Currency = request.CardDetails.Currency;
                                        transData.TransNumber = transaction.TransactionId + "-" + transactionAttempt.TransactionAttemptId;
                                        transData.OrderInfo = transData.TransNumber;
                                        transData.MerchTxnRef = transaction.TransactionId + "-" + transactionAttempt.TransactionAttemptId;
                                        transData.Track1 = transaction.Track1;
                                        transData.Track2 = transaction.Track2;
                                        transData.LRC = LRC;
                                        transData.EmvIccData = emvDataResult.EmvIccData;

                                        var apiResponse = gateway.ProcessMasterCard(transData, apiLogin, "purchase", cardTypeId);

                                        action = "updating transaction attempt id:" + transactionAttempt.TransactionAttemptId + ". Api Response Status: " + apiResponse.Status + ". Api Response Message: " + apiResponse.Message + ". ";
                                        if (apiResponse.Status == "Approved")
                                        {
                                            transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                            transactionAttempt.AuthNumber = apiResponse.AuthorizeId;
                                            transactionAttempt.ReturnCode = apiResponse.AcqResponseCode;
                                            transactionAttempt.SeqNumber = apiResponse.ReceiptNumber;
                                            transactionAttempt.TransNumber = apiResponse.TransactionNumber;
                                            transactionAttempt.BatchNumber = apiResponse.BatchNumber;
                                            transactionAttempt.DisplayReceipt = "";
                                            transactionAttempt.DisplayTerminal = "";
                                            transactionAttempt.DateReceived = DateTime.Now;
                                            transactionAttempt.PosEntryMode = apiResponse.PosEntryMode;
                                            transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                            transactionAttempt.Notes = "API Mastercard Purchase Approved";

                                            response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)rTransaction.CardTypeId);
                                            response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                            response.POSWSResponse.ErrNumber = "0";
                                            response.POSWSResponse.Message = "Transaction Successful.";
                                            response.POSWSResponse.Status = "Approved";
                                        }
                                        else if (apiResponse.Status == "Declined")
                                        {
                                            transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                            transactionAttempt.AuthNumber = "";
                                            transactionAttempt.ReturnCode = apiResponse.AcqResponseCode == null ? apiResponse.TransactionResponseCode : apiResponse.AcqResponseCode;
                                            transactionAttempt.SeqNumber = apiResponse.ReceiptNumber;
                                            transactionAttempt.TransNumber = apiResponse.TransactionNumber;
                                            transactionAttempt.DisplayReceipt = "";
                                            transactionAttempt.DisplayTerminal = "";
                                            transactionAttempt.PosEntryMode = apiResponse.PosEntryMode;
                                            transactionAttempt.DateReceived = DateTime.Now;
                                            transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                            transactionAttempt.Notes = "API Mastercard Purchase Declined." + apiResponse.Message;
                                            transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                            response.POSWSResponse.ErrNumber = apiResponse.AcqResponseCode == null ? apiResponse.TransactionResponseCode : apiResponse.AcqResponseCode;
                                            response.POSWSResponse.Message = apiResponse.Message;
                                            response.POSWSResponse.Status = "Declined";
                                        }
                                        else
                                        {
                                            response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Message;
                                            response.POSWSResponse.ErrNumber = "2111.3";
                                            response.POSWSResponse.Status = "Declined";
                                            return response;
                                        }

                                        #region Decrypt Card Number

                                        string Key = transactionAttempt.Transaction.Key.Key;
                                        string KeyIV = transactionAttempt.Transaction.Key.IV;
                                        string E_Card = transactionAttempt.Transaction.CardNumber;

                                        String hashCardNumber = Utility.DecryptEncDataWithKeyAndIV(E_Card, Key, KeyIV);

                                        transaction.CardNumber = SDGUtil.Functions.HashCardNumber(hashCardNumber);

                                        #endregion Decrypt Card Number

                                        #region // TODO: Should update mid balance (if already implemented)

                                        /*
                                         try
                                        {
                                            //update balance
                                            classMids.EntityMids sendBalance = new classMids.EntityMids();
                                            classMids.MidsResponse retBalance = new classMids.MidsResponse();
                                            classMids._Mids dataMids = new classMids._Mids();
                                            classMids classMids = new classMids();

                                            decimal newBalance = MidsDetail.MidsPrice.CurrentBalance + transAttData.TransAttemptDetail.Amount;

                                            dataMids.MidsPrice.ID = MidsDetail.MidsPrice.ID;
                                            dataMids.MidsPrice.Batch = MidsDetail.MidsPrice.Batch;
                                            dataMids.MidsPrice.BatchCloseTypeID = MidsDetail.MidsPrice.BatchCloseTypeID;
                                            dataMids.MidsPrice.BatchTime = MidsDetail.MidsPrice.BatchTime;
                                            dataMids.MidsPrice.Capture = MidsDetail.MidsPrice.Capture;
                                            dataMids.MidsPrice.CardNotPresent = MidsDetail.MidsPrice.CardNotPresent;
                                          //  dataMids.MidsPrice.ChargeBack = MidsDetail.MidsPrice.ChargeBack;
                                            dataMids.MidsPrice.CurrentBalance = newBalance;
                                            dataMids.MidsPrice.DailyLimit = MidsDetail.MidsPrice.DailyLimit;
                                            dataMids.MidsPrice.Purchased = MidsDetail.MidsPrice.Purchased;
                                            dataMids.MidsPrice.DiscountRate = MidsDetail.MidsPrice.DiscountRate;
                                            dataMids.MidsPrice.eCommerce = MidsDetail.MidsPrice.eCommerce;
                                            dataMids.MidsPrice.PreAuth = MidsDetail.MidsPrice.PreAuth;
                                            dataMids.MidsPrice.Refund = MidsDetail.MidsPrice.Refund;
                                            dataMids.MidsPrice.Settlement = MidsDetail.MidsPrice.Settlement;
                                            dataMids.MidsPrice.Void = MidsDetail.MidsPrice.Void;

                                            sendBalance.MidsDetail.Add(dataMids);

                                            retBalance = classMids.MidsPrice_Update(sendBalance);
                                        }
                                        catch
                                        {
                                        }
                                         */

                                        #endregion // TODO: Should update mid balance (if already implemented)
                                    }
                                    else
                                    {
                                        string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                        response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }

                                #endregion Master Card

                                #region CTPayment Swipe Purchase

                                else if (mid.Switch.SwitchCode == "CTPayment")
                                {
                                    GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                    GatewayProcessor.MasterCard.TransactionData transData = new GatewayProcessor.MasterCard.TransactionData();

                                    #region CTPayment Request

                                    #region Header Request

                                    string msgClass = "FO";
                                    string operatorId = "   ";
                                    string posEntryMode = "02";
                                    string terminalId = mid.Param_6; // "BBPOS002"; //mid.Param_6; //SMART018
                                    string transType = "000";
                                    string msgVersion = "04";
                                    string posStatIndicator = "000";
                                    string seqNumber = SDGUtil.Functions.GenerateSystemTraceAudit();
                                    string seqByte = "0";

                                    HEADER_REQUEST header = new HEADER_REQUEST(msgClass, terminalId, operatorId, seqByte + seqNumber, transType, DateTime.Now, posEntryMode, msgVersion, posStatIndicator);

                                    header.GetRequestHeader();

                                    #endregion Header Request

                                    #region Track2 Equivalent Data

                                    int init = track2.IndexOf("=");
                                    int end = track2.IndexOf("?");

                                    string serviceCode = track2.Substring(init + 5, 3);
                                    string ddata = track2.Substring(init + 8, (track2.Length - 1) - (init + 8));

                                    TRACK2_EQUIVALENT_DATA track2EquivalentData = new TRACK2_EQUIVALENT_DATA(cardNumber, request.CardDetails.ExpYear + request.CardDetails.ExpMonth, serviceCode, ddata);
                                    EMV emv = new EMV(track2EquivalentData);
                                    TRACK2_REQUEST tr = new TRACK2_REQUEST(emv);
                                    emv.TED = track2EquivalentData;
                                    tr.EMV = emv;

                                    #endregion Track2 Equivalent Data

                                    #region TRANS AMOUNT

                                    decimal orgAmount = Convert.ToDecimal(request.CardDetails.Amount);
                                    decimal finalAmount = orgAmount * 100;
                                    string parseAmount;

                                    try
                                    {
                                        parseAmount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                    }
                                    catch
                                    {
                                        parseAmount = finalAmount.ToString();
                                    }

                                    if (parseAmount.Length < 2)
                                    {
                                        parseAmount = parseAmount.ToString().PadLeft(2, '0');
                                    }

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

                                    #endregion CTPayment Request

                                    EMV_REQUEST emvRequest = new EMV_REQUEST(header, tr, transAmount, null, null, null, null, posConditions, invoiceNumber);

                                    var apiResponse = gateway.ProcessCTPaymentCreditTransaction(emvRequest.EMVRequest(), "purchase");

                                    action = "updating transaction attempt id:" + transactionAttempt.TransactionAttemptId + ". Api Response Status: " + apiResponse.Status + ". Api Response Message: " + apiResponse.Result.Message + ". ";
                                    if (apiResponse.Result.Status == "Approved")
                                    {
                                        transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                        transactionAttempt.AuthNumber = apiResponse.Result.AuthorizeId;
                                        transactionAttempt.ReturnCode = "00";
                                        transactionAttempt.SeqNumber = apiResponse.Result.ReceiptNumber;
                                        transactionAttempt.TransNumber = apiResponse.Result.TransactionNumber;
                                        transactionAttempt.BatchNumber = apiResponse.Result.BatchNumber;
                                        transactionAttempt.DisplayReceipt = "";
                                        transactionAttempt.DisplayTerminal = apiResponse.Result.TerminalId;
                                        transactionAttempt.DateReceived = DateTime.Now;
                                        transactionAttempt.PosEntryMode = apiResponse.Result.PosEntryMode;
                                        transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        transactionAttempt.Notes = "API Swiped CTPayment Purchase Approved";

                                        response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)rTransaction.CardTypeId);
                                        response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                        response.POSWSResponse.ErrNumber = "0";
                                        response.POSWSResponse.Message = "Transaction Successful.";
                                        response.POSWSResponse.Status = "Approved";
                                    }
                                    else if (apiResponse.Result.Status == "Declined")
                                    {
                                        transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                        transactionAttempt.AuthNumber = "";
                                        transactionAttempt.ReturnCode = apiResponse.Result.AcqResponseCode;
                                        //transactionAttempt.RefNumber = apiResponse.ReceiptNumber;
                                        transactionAttempt.SeqNumber = apiResponse.Result.ReceiptNumber;
                                        transactionAttempt.TransNumber = apiResponse.Result.TransactionNumber;
                                        transactionAttempt.DisplayReceipt = "";
                                        transactionAttempt.DisplayTerminal = "";
                                        transactionAttempt.PosEntryMode = apiResponse.Result.PosEntryMode;
                                        transactionAttempt.DateReceived = DateTime.Now;
                                        transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        transactionAttempt.Notes = "API Swiped CTPayment Purchase Declined." + apiResponse.Result.Message;
                                        transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                        response.POSWSResponse.ErrNumber = apiResponse.Result.AcqResponseCode;
                                        response.POSWSResponse.Message = apiResponse.Result.Message;
                                        response.POSWSResponse.Status = "Declined";
                                    }
                                    else
                                    {
                                        response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Result.Message;
                                        response.POSWSResponse.ErrNumber = "2111.3";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }

                                #endregion CTPayment Swipe Purchase

                                #region Offline Switch

                                else if (mid.Switch.SwitchCode == "Offline")
                                {
                                    transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                    transactionAttempt.AuthNumber = "VP-001";
                                    transactionAttempt.ReturnCode = "00";
                                    transactionAttempt.SeqNumber = "621814389248";
                                    transactionAttempt.TransNumber = "1100000648";
                                    transactionAttempt.BatchNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
                                    transactionAttempt.DisplayReceipt = mid.Param_2;
                                    transactionAttempt.DisplayTerminal = mid.Param_6;
                                    transactionAttempt.DateReceived = DateTime.Now;
                                    transactionAttempt.PosEntryMode = 05;
                                    transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                    transactionAttempt.Notes = "API Offline Purchase Approved";

                                    response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)rTransaction.CardTypeId);
                                    response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                    response.POSWSResponse.ErrNumber = "0";
                                    response.POSWSResponse.Message = "Transaction Successful.";
                                    response.POSWSResponse.Status = "Approved";
                                }

                                #endregion Offline Switch

                                #region MaxBank Swipe

                                else if (mid.Switch.SwitchCode == "Maxbank")
                                {
                                    if (transactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale))
                                    {
                                        action = "processing swipe maxbank api";

                                        GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                        GatewayProcessor.VeritasPayment.CardDetails transData = new GatewayProcessor.VeritasPayment.CardDetails();

                                        transData.MerchantID = mid.Param_2;
                                        transData.TerminalID = mid.Param_6;

                                        decimal orgAmount = transactionAttempt.Amount;
                                        decimal finalAmount = orgAmount * 100;

                                        try
                                        {
                                            transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                        }
                                        catch
                                        {
                                            transData.Amount = finalAmount.ToString();
                                        }

                                        transData.SystemTraceAudit = transactionAttempt.TransNumber;
                                        transData.CardNumber = cardNumber;
                                        transData.NameOnCard = request.CardDetails.NameOnCard;
                                        transData.Track2Data = track2.Replace('D', '=').TrimStart(';').TrimEnd('F').TrimEnd('?');
                                        transData.Track1Data = null;
                                        transData.ExpirationDate = (transaction.ExpMonth == null || transaction.ExpYear == null) ? null : transaction.ExpYear + transaction.ExpMonth;
                                        transData.CurrencyCode = mid.Currency.IsoCode;
                                        transData.PinBlock = null;

                                        //Fees
                                        transData.AmountTransactionFee = "00000000";

                                        action = "processing transaction for credit maxbank api integration. Transaction was successfully saved.";

                                        try
                                        {
                                            var apiResponse = gateway.ProcessPurchaseMaxbankGateway(transData, "PURCHASE");

                                            if (apiResponse.Result.Status == "Approved")
                                            {
                                                transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                                transactionAttempt.AuthNumber = apiResponse.Result.AuthorizationID;
                                                transactionAttempt.ReturnCode = apiResponse.Result.ReturnCode;
                                                transactionAttempt.TransNumber = apiResponse.Result.SytemTraceAudit;
                                                transactionAttempt.Reference = apiResponse.Result.Reference;
                                                transactionAttempt.BatchNumber = _batchRepo.GenerateBatchNumber(mobileApp.MobileAppId, Convert.ToInt32(SDGDAL.Enums.Ref_PaymentType.Credit));
                                                transactionAttempt.DisplayReceipt = transData.MerchantID;
                                                transactionAttempt.DisplayTerminal = transData.TerminalID;
                                                transactionAttempt.DateReceived = DateTime.Now;
                                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                                transactionAttempt.Notes = "API Maxbank Swipe Purchase Approved";
                                                response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                                response.POSWSResponse.ErrNumber = "0";
                                                response.POSWSResponse.Message = "Transaction Successful.";
                                                response.POSWSResponse.Status = "Approved";
                                            }
                                            else if(apiResponse.Result.Status == "Declined")
                                            {
                                                transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                                transactionAttempt.AuthNumber = apiResponse.Result.AuthorizationID;
                                                transactionAttempt.ReturnCode = apiResponse.Result.ReturnCode;
                                                transactionAttempt.TransNumber = transData.SystemTraceAudit;
                                                transactionAttempt.Reference = apiResponse.Result.Reference;
                                                transactionAttempt.DisplayReceipt = "";
                                                transactionAttempt.DisplayTerminal = "";
                                                transactionAttempt.DateReceived = DateTime.Now;
                                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                                transactionAttempt.Notes += "API Maxbank Swipe Purchase Declined" + apiResponse.Result.Message;
                                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                                response.POSWSResponse.ErrNumber = apiResponse.Result.ErrNumber;
                                                response.POSWSResponse.Message = apiResponse.Result.Message;
                                                response.POSWSResponse.Status = "Declined";
                                            }
                                            else
                                            {
                                                response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Result.Message;
                                                response.POSWSResponse.ErrNumber = "2101.7";
                                                response.POSWSResponse.Status = "Declined";
                                                return response;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            // Log error
                                            var errorOnAction = "Error while " + action;
                                            var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction, "TransactionPurchase", "", "");

                                            response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + "Declined" + " " + errorOnAction + " " + ex.Message;
                                            response.POSWSResponse.ErrNumber = "2000.10";
                                            response.POSWSResponse.Status = "Declined";
                                            return response;
                                        }
                                    }
                                    else
                                    {
                                        string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                        response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }

                                #endregion MaxBank Credit

                                #region PayMaya Swipe
                                else if(mid.Switch.SwitchCode == "PayMaya")
                                {
                                    action = "processing swipe paymaya api";

                                    string d = track2;
                                    string emvIccData = SDGUtil.Functions.Base64Encode(emvDataResult.EmvIccData); //convert to Base 64
                                    decimal orgAmount = transactionAttempt.Amount;
                                    decimal finalAmount = orgAmount * 100;


                                    //try
                                    //{
                                    //    transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                    //}
                                    //catch
                                    //{
                                    //    transData.Amount = finalAmount.ToString();
                                    //}

                                    var objReq = new GatewayProcessor.PayMaya.Request
                                    {
                                        merchant = new Merchant()
                                        {
                                            id = mid.Param_2,
                                            acquiringTerminal = new AcquiringTerminal
                                            {
                                                id = mid.Param_6,
                                                inputCapability = new InputCapability
                                                {
                                                    keyEntry = false,
                                                    magstripeReader = true,
                                                    emvChip = true,
                                                    contactless = true,
                                                    contactlessMagstripe = false
                                                },
                                                onMerchantPremise = true,
                                                terminalAttended = true,
                                                cardCaptureSupported = false,
                                                cardholderActivatedTerminal = true
                                            }
                                        },
                                        payer = new Payer()
                                        {
                                            fundingInstrument = new FundingInstrument()
                                            {
                                                card = new Card()
                                                {
                                                    cardNumber = cardNumber,
                                                    expiryMonth = expMonth,
                                                    expiryYear = "20" + expYear,
                                                    firstName = request.CardDetails.NameOnCard,
                                                    lastName = "",
                                                    cardPresentFields = new CardPresentFields()
                                                    {
                                                        cardSeqNum = "",
                                                        emvIccData = emvIccData,
                                                        track2 = track2
                                                    }
                                                }
                                            }
                                        },
                                        transaction = new GatewayProcessor.PayMaya.Transaction()
                                        {
                                            amount = new Amount()
                                            {
                                                total = new Total()
                                                {
                                                    currency = request.CardDetails.Currency,
                                                    value = "" 
                                                }
                                            },
                                            description = ""
                                        }
                                    };

                                    string requestdata = new JavaScriptSerializer().Serialize(objReq);


                                    try
                                    {
                                        //call api
                                    }
                                    catch(Exception ex)
                                    {

                                    }
                                }
                                #endregion

                                else
                                {
                                    response.POSWSResponse.Message = "Transaction failed. Switch not yet supported. Please contact Support.";
                                    response.POSWSResponse.ErrNumber = "2101.9";
                                    response.POSWSResponse.Status = "Declined";
                                    return response;
                                }

                                action = "updating transaction";

                                #region Update Transaction Attempts

                                var nTransactionAttempt = _transRepo.UpdateTransactionAttempt(transactionAttempt);

                                response.MerchantId = transactionAttempt.DisplayReceipt;
                                response.TerminalId = transactionAttempt.DisplayTerminal;
                                response.TransactionNumber = Convert.ToString(transaction.TransactionId) + "-" + Convert.ToString(transactionAttempt.TransactionAttemptId);
                                response.AuthNumber = transactionAttempt.AuthNumber;
                                response.TransNumber = transactionAttempt.TransNumber;
                                response.SequenceNumber = transactionAttempt.SeqNumber;
                                response.BatchNumber = transactionAttempt.BatchNumber;
                                response.Timestamp = SDGUtil.Functions.Format_Datetime(transactionAttempt.DateReceived);  
                                response.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)transaction.TransactionEntryTypeId);
                                response.Total = Convert.ToDecimal(transactionAttempt.Amount.ToString("N2"));
                                response.CardNumber = SDGUtil.Functions.HashCardNumber(transaction.CardNumber);
                                response.Currency = request.CardDetails.Currency;

                                #endregion Update Transaction Attempts
                            }
                        }
                        else
                        {
                            throw new Exception(action);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error please
                        var errorOnAction = "Error while " + action;

                        var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "TransationPurchaseCreditSwipe", ex.StackTrace);

                        response.POSWSResponse.ErrNumber = "2101.9";
                        response.POSWSResponse.Message = "Unknown error, please contact support. Ref: " + errRefNumber;
                        response.POSWSResponse.Status = "Declined";
                    }

                    return response;

                    #endregion Handle Transaction response

                    #endregion Transaction
                }
                else if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    response.POSWSResponse.Status = "Approved";
                    response.POSWSResponse.Message = "";
                    response.POSWSResponse.ErrNumber = "0";
                    response.POSWSResponse.UpdatePending = true;

                    response.AuthNumber = "5042158454";
                    response.SequenceNumber = "0015248456";
                    response.Timestamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                    response.TransactionNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    response.SubTotal = 0;
                    response.Tax1Amount = 0;
                    response.Tax1Name = "";
                    response.Tax1Rate = 0;
                    response.Tax2Amount = 0;
                    response.Tax2Name = "";
                    response.Tax2Rate = 0;
                    response.Total = request.CardDetails.Amount;
                    response.Tips = request.Tips;
                }
                else
                {
                    response.POSWSResponse.ErrNumber = "2101.10";
                    response.POSWSResponse.Status = "Declined";
                    response.POSWSResponse.Message = "The card is expired.";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "TransactionPurchaseCreditSwipe", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message, "TransactionPurchaseCreditSwipe  ErrRef:" + errRefNumber, ex.InnerException.StackTrace);
                }

                response.POSWSResponse.ErrNumber = "2101.11";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber + " " + action;
                response.POSWSResponse.Status = "DECLINED";
            }

            return response;
        }

        public CreditVoidRefundResponse CreditTransactionVoidRefund(TransactionVoidRefundRequest VoidRequest)
        {
            string action = string.Empty;
            bool isSuccess = false;
            string refundType = string.Empty;

            CreditVoidRefundResponse response = new CreditVoidRefundResponse();

            response.POSWSResponse = new POSWSResponse();

            if (string.IsNullOrEmpty(VoidRequest.POSWSRequest.SystemMode)
                    || string.IsNullOrEmpty(VoidRequest.TransactionNumber)
                    || VoidRequest.Amount <= 0
                    || VoidRequest.VoidRefundReasonId <= 0)
            {
                response.POSWSResponse.ErrNumber = "1001.1";
                response.POSWSResponse.Message = "Missing input";
                response.POSWSResponse.Status = "DECLINED";
                return response;
            }

            response.POSWSResponse.ErrNumber = "0";

            try
            {
                if (VoidRequest.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    bool refundExists = false;
                    decimal totalAmount = VoidRequest.Amount;

                    var mobileApp = new SDGDAL.Entities.MobileApp();
                    var account = new SDGDAL.Entities.Account();

                    action = "checking mobile app availability.";

                    if (!String.IsNullOrEmpty(VoidRequest.POSWSRequest.RToken))
                    {
                        response.POSWSResponse = mobileAppFunctions.CheckStatus(VoidRequest.POSWSRequest.RToken, out mobileApp, out account);

                        if (response.POSWSResponse.Status == "Declined")
                        {
                            return response;
                        }
                    }
                    else
                    {
                        action = "retrieving mobileApp Details using Activation Code";
                        mobileApp = _mAppRepo.GetMobileAppFullDetailsByActivationCode(VoidRequest.POSWSRequest.ActivationKey);

                        if (mobileApp == null)
                        {
                            response.POSWSResponse.ErrNumber = "2201.1";
                            response.POSWSResponse.Message = "No record found.";
                            response.POSWSResponse.Status = "Declined";
                            return response;
                        }
                        else
                        {
                            if (mobileApp.IsDeleted)
                            {
                                response.POSWSResponse.ErrNumber = "2200.1";
                                response.POSWSResponse.Message = "No record found.";
                                response.POSWSResponse.Status = "Declined";
                                return response;
                            }

                            if (!mobileApp.IsActive)
                            {
                                response.POSWSResponse.ErrNumber = "2200.2";
                                response.POSWSResponse.Message = "Activation Code is not yet activated.";
                                response.POSWSResponse.Status = "Declined";
                                return response;
                            }
                        }

                        action = "retrieving Account Details of Merchant";
                        //Get the MerchantID
                        var merchantInfo = _mAppRepo.GetMobileAppDetailsByMobileAppId(mobileApp.MobileAppId);

                        if (merchantInfo != null)
                        {
                            var accountInfo = _userRepo.GetDetailsbyParentIdAndParentTypeId(Convert.ToInt32(SDGDAL.Enums.ParentType.Merchant), merchantInfo.MerchantBranchPOS.MerchantBranch.Merchant.MerchantId);
                            account.AccountId = accountInfo.AccountId;
                        }
                    }

                    MobileAppMethods mobileAppMethods = new MobileAppMethods();

                    action = "logging mobile app action.";
                    mobileAppFunctions.LogMobileAppAction("Credit Transaction Void TransactionNumber:" + VoidRequest.TransactionNumber, mobileApp.MobileAppId, account.AccountId, VoidRequest.POSWSRequest.GPSLat, VoidRequest.POSWSRequest.GPSLong);

                    int hyphen;
                    int transAttId;
                    int transId;

                    try
                    {
                        hyphen = VoidRequest.TransactionNumber.IndexOf('-');
                        transAttId = Convert.ToInt32(VoidRequest.TransactionNumber.Substring(hyphen + 1));
                        transId = Convert.ToInt32(VoidRequest.TransactionNumber.Substring(0, hyphen));
                    }
                    catch
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.ErrNumber = "2200.3";
                        response.POSWSResponse.Message = "Invalid transaction number";
                        return response;
                    }

                    action = "getting transaction by transaction id from database.";
                    var transaction = _transRepo.GetTransaction(transId);

                    if (mobileApp.MerchantBranchPOS.MerchantBranch.MerchantId != transaction.MerchantPOS.MerchantBranch.MerchantId)
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.ErrNumber = "2200.4";
                        response.POSWSResponse.Message = "Merchant Mismatch";
                        return response;
                    }

                    var transactionAttempt = _transRepo.GetTransactionAttempt(transAttId);

                    if (transactionAttempt == null)
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.ErrNumber = "2200.5";
                        response.POSWSResponse.Message = "Transaction not found.";
                        return response;
                    }

                    if (string.IsNullOrEmpty(transactionAttempt.AuthNumber))
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.ErrNumber = "2200.6";
                        response.POSWSResponse.Message = "No previous completed sale transaction found.";
                        return response;
                    }
                    else
                    {
                        if (!_transRepo.IsCreditTransactionAlreadyVoid(transId))
                        {
                            response.POSWSResponse.Status = "Declined";
                            response.POSWSResponse.ErrNumber = "2200.7";
                            response.POSWSResponse.Message = "Transaction already void.";
                            return response;
                        }

                        if (transactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Refund))
                        {
                            refundExists = true;
                            totalAmount += transactionAttempt.Amount;
                        }
                    }

                    var nTransactionAttempt = new SDGDAL.Entities.TransactionAttempt();

                    action = "verifying transaction status.";
                    if (!refundExists)
                    {
                        bool hasDeposited = (transactionAttempt.DepositDate < DateTime.Now
                                            && transactionAttempt.DepositDate > transactionAttempt.DateReceived);

                        if (hasDeposited)
                        {
                            if (transaction.FinalAmount >= VoidRequest.Amount)
                            {
                                nTransactionAttempt.Amount = VoidRequest.Amount;
                                nTransactionAttempt.DateSent = DateTime.Now;
                                nTransactionAttempt.DateReceived = DateTime.Now.AddYears(-100);
                                nTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);

                                nTransactionAttempt.Notes = "";
                                nTransactionAttempt.TransactionId = transaction.TransactionId;
                                nTransactionAttempt.TransactionChargesId = transactionAttempt.TransactionChargesId;
                                nTransactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Refund);
                                nTransactionAttempt.AuthNumber = transactionAttempt.AuthNumber;
                                nTransactionAttempt.BatchNumber = "";
                                nTransactionAttempt.DisplayReceipt = transactionAttempt.DisplayReceipt;
                                nTransactionAttempt.DisplayTerminal = transactionAttempt.DisplayTerminal;
                                nTransactionAttempt.ReturnCode = "";
                                nTransactionAttempt.SeqNumber = "";
                                nTransactionAttempt.TransNumber = transactionAttempt.TransNumber;
                            }
                            else
                            {
                                response.POSWSResponse.ErrNumber = "2200.8";
                                response.POSWSResponse.Status = "Declined";
                                response.POSWSResponse.Message = "Transaction exceeded total amount of transaction.";
                                return response;
                            }
                        }
                        else
                        {
                            if (transactionAttempt.Amount == VoidRequest.Amount)
                            {
                                // void
                                nTransactionAttempt.Amount = transaction.FinalAmount;
                                nTransactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Void);

                                refundType = "1";
                            }
                            else if (transactionAttempt.Amount > VoidRequest.Amount)
                            {
                                // refund
                                nTransactionAttempt.Amount = VoidRequest.Amount;
                                nTransactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Refund);

                                refundType = "2";
                            }
                            else
                            {
                                response.POSWSResponse.ErrNumber = "2200.8";
                                response.POSWSResponse.Status = "Declined";
                                response.POSWSResponse.Message = "Transaction exceeded total amount of transaction.";
                                return response;
                            }

                            nTransactionAttempt.DateSent = DateTime.Now;
                            nTransactionAttempt.DateReceived = DateTime.Now.AddYears(-100);
                            nTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                            nTransactionAttempt.Notes = "";
                            nTransactionAttempt.TransactionId = transactionAttempt.TransactionId;
                            nTransactionAttempt.TransactionChargesId = transactionAttempt.TransactionChargesId;
                            nTransactionAttempt.AuthNumber = "";
                            nTransactionAttempt.BatchNumber = "";
                            nTransactionAttempt.DisplayReceipt = transactionAttempt.DisplayReceipt;
                            nTransactionAttempt.DisplayTerminal = transactionAttempt.DisplayTerminal;
                            nTransactionAttempt.ReturnCode = "";
                            nTransactionAttempt.SeqNumber = "";
                            nTransactionAttempt.TransNumber = transactionAttempt.TransNumber;
                        }
                    }
                    else
                    {
                        // Refund exists
                        if (transaction.FinalAmount >= totalAmount)
                        {
                            nTransactionAttempt.Amount = VoidRequest.Amount;
                            nTransactionAttempt.DateSent = DateTime.Now;
                            nTransactionAttempt.DateReceived = DateTime.Now.AddYears(-100);
                            nTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                            nTransactionAttempt.Notes = "";
                            nTransactionAttempt.TransactionId = transactionAttempt.TransactionId;
                            nTransactionAttempt.TransactionChargesId = transactionAttempt.TransactionChargesId;
                            nTransactionAttempt.AuthNumber = "";
                            nTransactionAttempt.BatchNumber = "";
                            nTransactionAttempt.DisplayReceipt = transactionAttempt.DisplayReceipt;
                            nTransactionAttempt.DisplayTerminal = transactionAttempt.DisplayTerminal;
                            nTransactionAttempt.ReturnCode = "";
                            nTransactionAttempt.SeqNumber = "";
                            nTransactionAttempt.TransNumber = transactionAttempt.TransNumber;
                        }
                        else
                        {
                            response.POSWSResponse.ErrNumber = "2200.8";
                            response.POSWSResponse.Status = "Declined";
                            response.POSWSResponse.Message = "Transaction exceeded total amount of transaction.";
                            return response;
                        }
                    }

                    action = "decrypting card details for api transaction void/refund.";

                    string D_Card = "XXXX-XXXX-XXXX-0000";

                    try
                    {
                        D_Card = Utility.DecryptEncDataWithKeyAndIV(transaction.CardNumber, transaction.Key.Key, transaction.Key.IV);
                    }
                    catch
                    {
                        response.POSWSResponse.ErrNumber = "2200.9";
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "Unable to decrypt card details.";
                        return response;
                    }

                    transaction.CardTypeId = transaction.CardTypeId;
                    transaction.Track1 = transaction.Track1;
                    transaction.Track2 = transaction.Track2;

                    transaction.CardNumber = transaction.CardNumber;
                    transaction.ExpYear = transaction.ExpYear;
                    transaction.ExpMonth = transaction.ExpMonth;
                    transaction.NameOnCard = transaction.NameOnCard;

                    nTransactionAttempt.DeviceId = transactionAttempt.DeviceId;
                    nTransactionAttempt.MobileAppId = mobileApp.MobileAppId;
                    nTransactionAttempt.AccountId = account.AccountId;
                    nTransactionAttempt.GPSLat = VoidRequest.POSWSRequest.GPSLat;
                    nTransactionAttempt.GPSLong = VoidRequest.POSWSRequest.GPSLong;
                    nTransactionAttempt.TransactionSignatureId = transactionAttempt.TransactionSignatureId;
                    nTransactionAttempt.TransactionVoidReasonId = VoidRequest.VoidRefundReasonId;
                    nTransactionAttempt.TransactionVoidNote = VoidRequest.VoidRefundNote;
                    nTransactionAttempt.PosEntryMode = transactionAttempt.PosEntryMode;
                    nTransactionAttempt.BatchNumber = transactionAttempt.BatchNumber;
                    nTransactionAttempt.ReturnCode = null;

                    #region Handle Transaction response

                    action = "setting up transaction entry for database.";

                    try
                    {
                        action = "checking mid status before setting up transaction entry.";
                        var mid = new SDGDAL.Entities.Mid();

                        mid = _midsRepo.GetMidByPosIdAndCardTypeId(transaction.MerchantPOSId, transaction.CardTypeId);

                        if (mid == null)
                        {
                            response.POSWSResponse.ErrNumber = "2200.10";
                            response.POSWSResponse.Message = "Mid not found.";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }
                        else
                        {
                            if (!mid.IsActive || mid.IsDeleted)
                            {
                                response.POSWSResponse.ErrNumber = "2200.11";
                                response.POSWSResponse.Message = "Mid is Inactive.";
                                response.POSWSResponse.Status = "Declined";

                                return response;
                            }

                            if (!mid.Switch.IsActive)
                            {
                                response.POSWSResponse.ErrNumber = "2200.12";
                                response.POSWSResponse.Message = "Switch is Inactive.";
                                response.POSWSResponse.Status = "Declined";

                                return response;
                            }
                        }

                        action = "checking if address is required. ";
                        if (mid.Switch.IsAddressRequired)
                        {
                            action = "saving temp transaction because switch requires address.";
                            var tempTransaction = new SDGDAL.Entities.TempTransaction();

                            tempTransaction.CopyProperties(transaction);

                            var nTempTransaction = _transRepo.CreateTempTransaction(tempTransaction);

                            if (nTempTransaction.TransactionId > 0)
                            {
                                response.POSWSResponse.Status = "Declined";
                                response.POSWSResponse.Message = "This transaction require customer to enter their billing address.";
                                response.POSWSResponse.ErrNumber = "2200.13";
                                return response;
                            }
                            else
                            {
                                throw new Exception(action);
                            }
                        }

                        action = "checking if switch is active. ";
                        if (!mid.Switch.IsActive)
                        {
                            nTransactionAttempt.DateSent = DateTime.Now;
                            nTransactionAttempt.DateReceived = DateTime.Now;
                            nTransactionAttempt.DepositDate = DateTime.Now;
                            nTransactionAttempt.Notes = ((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId).ToString() + " Declined. Switch inactive.";
                            nTransactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                        }
                        else
                        {
                            nTransactionAttempt.DateSent = DateTime.Now;
                            nTransactionAttempt.DateReceived = Convert.ToDateTime("1900/01/01");
                            nTransactionAttempt.DepositDate = Convert.ToDateTime("1900/01/01");

                            if (!mid.IsActive || mid.IsDeleted)
                            {
                                nTransactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                            }

                            if (mid.Switch.SwitchName == "Transax")
                            {
                                // TODO: Correct?
                                if (((SDGDAL.Enums.TransactionType)nTransactionAttempt.TransactionTypeId) == Enums.TransactionType.PreAuth)
                                {
                                    nTransactionAttempt.TransactionTypeId = Convert.ToInt32(Enums.TransactionType.Sale);
                                }
                            }
                        }

                        var voidTable = _transRepo.GetTransactionVoidReasonById(VoidRequest.VoidRefundReasonId);

                        action = "saving void transaction to database";
                        var nnTransactionAttempt = _transRepo.CreateTransactionAttempt(nTransactionAttempt);

                        if (nnTransactionAttempt.TransactionAttemptId > 0)
                        {
                            action = "processing transaction for api integration. Transaction was successfully saved.";

                            if (((SDGDAL.Enums.TransactionType)nnTransactionAttempt.TransactionTypeId) == Enums.TransactionType.Declined)
                            {
                                response.POSWSResponse.Message = "Transaction failed. Please contact Support.";
                                response.POSWSResponse.ErrNumber = "2200.14";
                                response.POSWSResponse.Status = "Declined";
                                return response;
                            }
                            else
                            {
                                #region Master Card

                                if (mid.Switch.SwitchCode == "MasterCard")
                                {
                                    if (nnTransactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Void))
                                    {
                                        action = "processing master card.";
                                        GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                        GatewayProcessor.MasterCard.TransactionData transData = new GatewayProcessor.MasterCard.TransactionData();
                                        GatewayProcessor.MasterCard.APILogin apiLogin = new GatewayProcessor.MasterCard.APILogin();

                                        transData.AccessCode = mid.Param_1;
                                        transData.MerchantId = mid.Param_2;
                                        transData.SecureHash = mid.Param_3;
                                        apiLogin.Username = mid.Param_4;
                                        apiLogin.Password = mid.Param_5;

                                        decimal orgAmount = nnTransactionAttempt.Amount;
                                        decimal finalAmount = orgAmount * 100;

                                        try
                                        {
                                            transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                        }
                                        catch
                                        {
                                            transData.Amount = finalAmount.ToString();
                                        }

                                        transData.CardNumber = transaction.CardNumber;
                                        transData.CardExpirationDate = transaction.ExpYear.ToString() + transaction.ExpMonth.ToString();
                                        transData.CSC = transaction.CSC;
                                        transData.Currency = mid.Currency.CurrencyCode;
                                        transData.MerchTxnRef = transaction.TransactionId + "-" + nnTransactionAttempt.TransactionAttemptId;
                                        transData.OrderInfo = transaction.TransactionId + "-" + nnTransactionAttempt.TransactionAttemptId;

                                        transData.Track1 = transaction.Track1;
                                        transData.Track2 = transaction.Track2;
                                        transData.TransNumber = nnTransactionAttempt.TransNumber;

                                        var apiResponse = gateway.ProcessMasterCard(transData, apiLogin, "void", transaction.CardTypeId);

                                        action = "updating transaction attempt id:" + transactionAttempt.TransactionAttemptId + ". Api Response Status: " + apiResponse.Status + ". Api Response Message: " + apiResponse.Message + ". ";
                                        if (apiResponse.Status == "Approved")
                                        {
                                            nnTransactionAttempt.AuthNumber = apiResponse.AuthorizeId;
                                            nnTransactionAttempt.ReturnCode = apiResponse.AcqResponseCode;
                                            nnTransactionAttempt.SeqNumber = apiResponse.ReceiptNumber;
                                            nnTransactionAttempt.TransNumber = apiResponse.TransactionNumber;
                                            nnTransactionAttempt.BatchNumber = apiResponse.BatchNumber;
                                            nnTransactionAttempt.DisplayReceipt = "";
                                            nnTransactionAttempt.DisplayTerminal = "";
                                            nnTransactionAttempt.DateReceived = DateTime.Now;
                                            nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                            nnTransactionAttempt.TransactionTypeId = Convert.ToInt32((SDGDAL.Enums.TransactionType.Void));
                                            nnTransactionAttempt.Notes = " API Mastercard Void Approved";

                                            response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                                            response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType.Void));
                                            response.POSWSResponse.ErrNumber = "0";
                                            response.POSWSResponse.Message = "Transaction Successful.";
                                            response.POSWSResponse.Status = "Approved";

                                            isSuccess = true;
                                        }
                                        else if (apiResponse.Status == "Declined")
                                        {
                                            nnTransactionAttempt.AuthNumber = apiResponse.AuthorizeId;
                                            nnTransactionAttempt.ReturnCode = apiResponse.AcqResponseCode;
                                            nnTransactionAttempt.SeqNumber = apiResponse.ReceiptNumber;
                                            nnTransactionAttempt.BatchNumber = apiResponse.BatchNumber;
                                            nnTransactionAttempt.DisplayReceipt = "";
                                            nnTransactionAttempt.DisplayTerminal = "";
                                            nnTransactionAttempt.DateReceived = DateTime.Now;
                                            nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                            nnTransactionAttempt.TransactionTypeId = Convert.ToInt32((SDGDAL.Enums.TransactionType.Declined));
                                            nnTransactionAttempt.Notes = "API Mastercard Void Declined." + apiResponse.Message;

                                            response.POSWSResponse.ErrNumber = apiResponse.AcqResponseCode;
                                            response.POSWSResponse.Message = "Transaction Failed. " + apiResponse.Message;
                                            response.POSWSResponse.Status = "Declined";

                                            isSuccess = false;
                                         }
                                        else
                                        {
                                            response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Message;
                                            response.POSWSResponse.ErrNumber = "2200.14";
                                            response.POSWSResponse.Status = "Declined";
                                            return response;
                                        }
                                    }
                                    else
                                    {
                                        string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                        response.POSWSResponse.ErrNumber = "2200.15";
                                        response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }

                                #endregion Master Card

                                #region Master Card Demo

                                else if (mid.Switch.SwitchCode == "MasterCardDemo")
                                {
                                    if (nnTransactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Void))
                                    {
                                        action = "processing master card.";
                                        GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                        GatewayProcessor.MasterCard.TransactionData transData = new GatewayProcessor.MasterCard.TransactionData();
                                        GatewayProcessor.MasterCard.APILogin apiLogin = new GatewayProcessor.MasterCard.APILogin();

                                        transData.AccessCode = mid.Param_1;
                                        transData.MerchantId = mid.Param_2;
                                        transData.SecureHash = mid.Param_3;
                                        apiLogin.Username = mid.Param_4;
                                        apiLogin.Password = mid.Param_5;

                                        decimal orgAmount = nnTransactionAttempt.Amount;
                                        decimal finalAmount = orgAmount * 100;

                                        try
                                        {
                                            transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                        }
                                        catch
                                        {
                                            transData.Amount = finalAmount.ToString();
                                        }

                                        transData.CardNumber = transaction.CardNumber;
                                        transData.CardExpirationDate = transaction.ExpYear.ToString() + transaction.ExpMonth.ToString();
                                        transData.CSC = transaction.CSC;
                                        transData.Currency = mid.Currency.CurrencyCode;
                                        transData.MerchTxnRef = transaction.TransactionId + "-" + nnTransactionAttempt.TransactionAttemptId;
                                        transData.OrderInfo = transaction.TransactionId + "-" + nnTransactionAttempt.TransactionAttemptId;

                                        transData.Track1 = transaction.Track1;
                                        transData.Track2 = transaction.Track2;
                                        transData.TransNumber = nTransactionAttempt.TransNumber;

                                        var apiResponse = gateway.ProcessMasterCardDemo(transData, apiLogin, "void", transaction.CardTypeId);

                                        action = "updating transaction attempt id:" + transactionAttempt.TransactionAttemptId + ". Api Response Status: " + apiResponse.Status + ". Api Response Message: " + apiResponse.Message + ". ";
                                        if (apiResponse.Status == "Approved")
                                        {
                                            nnTransactionAttempt.AuthNumber = apiResponse.AuthorizeId;
                                            nnTransactionAttempt.ReturnCode = apiResponse.AcqResponseCode;
                                            nnTransactionAttempt.SeqNumber = apiResponse.ReceiptNumber;
                                            nnTransactionAttempt.TransNumber = apiResponse.TransactionNumber;
                                            nnTransactionAttempt.BatchNumber = apiResponse.BatchNumber;
                                            nnTransactionAttempt.DisplayReceipt = "";
                                            nnTransactionAttempt.DisplayTerminal = "";
                                            nnTransactionAttempt.DateReceived = DateTime.Now;
                                            nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                            nnTransactionAttempt.TransactionTypeId = Convert.ToInt32((SDGDAL.Enums.TransactionType.Void));
                                            nnTransactionAttempt.Notes = " API Mastercard Demo Void Approved";

                                            response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                                            response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType.Void));
                                            response.POSWSResponse.ErrNumber = "0";
                                            response.POSWSResponse.Message = "Transaction Successful.";
                                            response.POSWSResponse.Status = "Approved";

                                            isSuccess = true;
                                        }
                                        else if (apiResponse.Status == "Declined")
                                        {
                                            nnTransactionAttempt.AuthNumber = apiResponse.AuthorizeId;
                                            nnTransactionAttempt.ReturnCode = apiResponse.AcqResponseCode;
                                            nnTransactionAttempt.SeqNumber = apiResponse.ReceiptNumber;
                                            nnTransactionAttempt.BatchNumber = apiResponse.BatchNumber;
                                            nnTransactionAttempt.DisplayReceipt = "";
                                            nnTransactionAttempt.DisplayTerminal = "";
                                            nnTransactionAttempt.DateReceived = DateTime.Now;
                                            nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                            nnTransactionAttempt.TransactionTypeId = Convert.ToInt32((SDGDAL.Enums.TransactionType.Declined));
                                            nnTransactionAttempt.Notes = "API Mastercard Demo Void Declined." + apiResponse.Message;

                                            response.POSWSResponse.ErrNumber = apiResponse.AcqResponseCode;
                                            response.POSWSResponse.Message = "Transaction Failed. " + apiResponse.Message;
                                            response.POSWSResponse.Status = "Declined";

                                            isSuccess = false;
                                        }
                                        else
                                        {
                                            response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Message;
                                            response.POSWSResponse.ErrNumber = "2200.14";
                                            response.POSWSResponse.Status = "Declined";
                                            return response;
                                        }

                                        #region TODO: Should update mid balance (if already implemented)

                                        /*
                                             try
                                            {
                                                //update balance
                                                classMids.EntityMids sendBalance = new classMids.EntityMids();
                                                classMids.MidsResponse retBalance = new classMids.MidsResponse();
                                                classMids._Mids dataMids = new classMids._Mids();
                                                classMids classMids = new classMids();

                                                decimal newBalance = MidsDetail.MidsPrice.CurrentBalance + transAttData.TransAttemptDetail.Amount;

                                                dataMids.MidsPrice.ID = MidsDetail.MidsPrice.ID;
                                                dataMids.MidsPrice.Batch = MidsDetail.MidsPrice.Batch;
                                                dataMids.MidsPrice.BatchCloseTypeID = MidsDetail.MidsPrice.BatchCloseTypeID;
                                                dataMids.MidsPrice.BatchTime = MidsDetail.MidsPrice.BatchTime;
                                                dataMids.MidsPrice.Capture = MidsDetail.MidsPrice.Capture;
                                                dataMids.MidsPrice.CardNotPresent = MidsDetail.MidsPrice.CardNotPresent;
                                              //  dataMids.MidsPrice.ChargeBack = MidsDetail.MidsPrice.ChargeBack;
                                                dataMids.MidsPrice.CurrentBalance = newBalance;
                                                dataMids.MidsPrice.DailyLimit = MidsDetail.MidsPrice.DailyLimit;
                                                dataMids.MidsPrice.Purchased = MidsDetail.MidsPrice.Purchased;
                                                dataMids.MidsPrice.DiscountRate = MidsDetail.MidsPrice.DiscountRate;
                                                dataMids.MidsPrice.eCommerce = MidsDetail.MidsPrice.eCommerce;
                                                dataMids.MidsPrice.PreAuth = MidsDetail.MidsPrice.PreAuth;
                                                dataMids.MidsPrice.Refund = MidsDetail.MidsPrice.Refund;
                                                dataMids.MidsPrice.Settlement = MidsDetail.MidsPrice.Settlement;
                                                dataMids.MidsPrice.Void = MidsDetail.MidsPrice.Void;

                                                sendBalance.MidsDetail.Add(dataMids);

                                                retBalance = classMids.MidsPrice_Update(sendBalance);
                                            }
                                            catch
                                            {
                                            }
                                             */

                                        #endregion TODO: Should update mid balance (if already implemented)
                                    }
                                    else
                                    {
                                        string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                        response.POSWSResponse.ErrNumber = "2200.15";
                                        response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }

                                #endregion Master Card Demo

                                #region CTPayment VOID

                                else if (mid.Switch.SwitchCode == "CTPayment")
                                {
                                    GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                    POS_CONDITIONS posConditions;

                                    #region Header Request

                                    string msgClass = "FV";
                                    string operatorId = "   ";
                                    string posEntryMode = nTransactionAttempt.PosEntryMode.ToString().PadLeft(2, '0');
                                    string terminalId = mid.Param_6;
                                    string transType = "010";
                                    string msgVersion = "05";
                                    string posStatIndicator = "000";
                                    string seqNumber = nTransactionAttempt.BatchNumber;
                                    string seqByte = "0";

                                    HEADER_REQUEST header = new HEADER_REQUEST(msgClass, terminalId, operatorId, seqByte + seqNumber, transType, DateTime.Now, posEntryMode, msgVersion, posStatIndicator);

                                    header.GetRequestHeader();

                                    #endregion Header Request

                                    #region TRANS AMOUNT

                                    decimal orgAmount = nTransactionAttempt.Amount;
                                    decimal finalAmount = orgAmount * 100;
                                    string parseAmount;

                                    try
                                    {
                                        parseAmount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                    }
                                    catch
                                    {
                                        parseAmount = finalAmount.ToString();
                                    }

                                    if (parseAmount.Length < 2)
                                    {
                                        parseAmount = parseAmount.ToString().PadLeft(2, '0');
                                    }

                                    TRANS_AMOUNT_1 transAmount = new TRANS_AMOUNT_1(parseAmount, "");

                                    #endregion TRANS AMOUNT

                                    #region Invoice Number Field

                                    string invoiceNumberField = nTransactionAttempt.TransNumber;

                                    INVOICE_NUMBER invoiceNumber = new INVOICE_NUMBER(invoiceNumberField, "");

                                    #endregion Invoice Number Field

                                    #region APP ACCOUNT TYPE

                                    APP_ACCOUNT_TYPE appAccountType = new APP_ACCOUNT_TYPE();

                                    #endregion APP ACCOUNT TYPE

                                    #region POS CONDITIONS FIELD

                                    if (nTransactionAttempt.PosEntryMode == 2)
                                    {
                                        posConditions = new POS_CONDITIONS(0, 0, 0, 0);
                                    }
                                    else
                                    {
                                        posConditions = new POS_CONDITIONS(0, 0, CT_EMV_CLASSES.METHODS.FIDP_CARDHOLDER_AUTHENTICATION_METHOD.NOT_AUTHENTICATED, 0);
                                    }

                                    #endregion POS CONDITIONS FIELD

                                    EMV_REQUEST emvRequest = new EMV_REQUEST(header, null, transAmount, null, null, null, null, posConditions, invoiceNumber);

                                    var apiResponse = gateway.ProcessCTPaymentCreditTransaction(emvRequest.EMVRequest(), "void");

                                    if (apiResponse.Result.Status == "Approved")
                                    {
                                        nnTransactionAttempt.AuthNumber = apiResponse.Result.AuthorizeId;
                                        nnTransactionAttempt.ReturnCode = apiResponse.Result.AcqResponseCode;
                                        nnTransactionAttempt.SeqNumber = apiResponse.Result.ReceiptNumber;
                                        nnTransactionAttempt.TransNumber = apiResponse.Result.TransactionNumber;
                                        nnTransactionAttempt.BatchNumber = apiResponse.Result.BatchNumber;
                                        nnTransactionAttempt.DisplayReceipt = "";
                                        nnTransactionAttempt.DisplayTerminal = "";
                                        nnTransactionAttempt.DateReceived = DateTime.Now;
                                        nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttempt.TransactionTypeId = Convert.ToInt32((SDGDAL.Enums.TransactionType.Void));
                                        nnTransactionAttempt.Notes = " API Mastercard Demo Void Approved";

                                        response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                                        response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType.Void));
                                        response.POSWSResponse.ErrNumber = "0";
                                        response.POSWSResponse.Message = "Transaction Successful.";
                                        response.POSWSResponse.Status = "Approved";

                                        isSuccess = true;
                                    }
                                    else if (apiResponse.Result.Status == "Declined")
                                    {
                                        nnTransactionAttempt.AuthNumber = apiResponse.Result.AuthorizeId;
                                        nnTransactionAttempt.ReturnCode = apiResponse.Result.AcqResponseCode;
                                        nnTransactionAttempt.SeqNumber = apiResponse.Result.ReceiptNumber;
                                        nnTransactionAttempt.TransNumber = nnTransactionAttempt.TransNumber;
                                        nnTransactionAttempt.BatchNumber = apiResponse.Result.BatchNumber;
                                        nnTransactionAttempt.DisplayReceipt = "";
                                        nnTransactionAttempt.DisplayTerminal = "";
                                        nnTransactionAttempt.DateReceived = DateTime.Now;
                                        nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttempt.TransactionTypeId = Convert.ToInt32((SDGDAL.Enums.TransactionType.Declined));
                                        nnTransactionAttempt.Notes = "API Mastercard Demo Void Declined." + apiResponse.Result.Message;

                                        response.POSWSResponse.ErrNumber = apiResponse.Result.AcqResponseCode;
                                        response.POSWSResponse.Message = "Transaction Failed. " + apiResponse.Result.Message;
                                        response.POSWSResponse.Status = "Declined";

                                        isSuccess = false;
                                    }
                                    else
                                    {
                                        response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Result.Message;
                                        response.POSWSResponse.ErrNumber = "2200.14";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }

                                #endregion CTPayment VOID

                                #region Offline
                                else if(mid.Switch.SwitchCode == "Offline")
                                {
                                    if (nnTransactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Void))
                                    {
                                        nnTransactionAttempt.AuthNumber = DateTime.Now.ToString("hhmmss");
                                        nnTransactionAttempt.ReturnCode = "00";
                                        nnTransactionAttempt.SeqNumber = DateTime.Now.ToString("yyyyMMdd");
                                        nnTransactionAttempt.TransNumber = DateTime.Now.ToString("yyyyMMddhhmmss");
                                        nnTransactionAttempt.BatchNumber = DateTime.Now.ToString("yyyyMMdd");
                                        nnTransactionAttempt.DisplayReceipt = nTransactionAttempt.DisplayReceipt;
                                        nnTransactionAttempt.DisplayTerminal = nTransactionAttempt.DisplayTerminal;
                                        nnTransactionAttempt.DateReceived = DateTime.Now;
                                        nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttempt.TransactionTypeId = Convert.ToInt32((SDGDAL.Enums.TransactionType.Void));
                                        nnTransactionAttempt.Notes = " API Offline Void Approved";

                                        response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                                        response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType.Void));
                                        response.POSWSResponse.ErrNumber = "0";
                                        response.POSWSResponse.Message = "Transaction Successful.";
                                        response.POSWSResponse.Status = "Approved";

                                        isSuccess = true;
                                    }
                                    else
                                    {
                                        string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                        response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }
                                #endregion

                                #region TrustPay
                                else if(mid.Switch.SwitchCode == "TrustPay")
                                {
                                    GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                    GatewayProcessor.TrustPay.Transaction trans = new GatewayProcessor.TrustPay.Transaction();
                                    GatewayProcessor.TrustPay.TransactionResponse res = new GatewayProcessor.TrustPay.TransactionResponse();

                                    trans.MerNo = mid.Param_2;
                                    trans.Gatewayno = mid.Param_6;
                                    trans.tradeNo = transactionAttempt.TransNumber;
                                    trans.refundType = refundType;
                                    trans.orderAmount = Convert.ToString(transactionAttempt.Amount);
                                    trans.tradeAmount = Convert.ToString(transactionAttempt.Amount);
                                    trans.orderCurrency = mid.Currency.CurrencyCode;
                                    trans.refundReason = voidTable.VoidReason;
                                    trans.remark = VoidRequest.VoidRefundNote;

                                    var apiResponse = gateway.ProcessTrustPay(trans, "void");

                                    if (apiResponse.Status == "Approved")
                                    {
                                        transactionAttempt.TransactionAttemptId = nnTransactionAttempt.TransactionAttemptId;
                                        transactionAttempt.AuthNumber = apiResponse.tradeNo;
                                        transactionAttempt.ReturnCode = "00";
                                        transactionAttempt.SeqNumber = "";
                                        transactionAttempt.TransNumber = apiResponse.tradeNo;
                                        transactionAttempt.PosEntryMode = 0;
                                        transactionAttempt.Reference = apiResponse.tradeNo;
                                        transactionAttempt.BatchNumber = apiResponse.batchNo;
                                        transactionAttempt.DisplayTerminal = trans.tradeNo;
                                        transactionAttempt.DateReceived = DateTime.Now;
                                        transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);

                                        if (refundType == "1")
                                        {
                                            transactionAttempt.Notes = "Void Approved";
                                            response.POSWSResponse.Message = "Trustpay API Void Successful.";
                                            transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Void);
                                        }
                                        else if (refundType == "2")
                                        {
                                            transactionAttempt.Notes = "Refund Approved";
                                            response.POSWSResponse.Message = "Trustpay API Refund Successful.";
                                            transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Refund);
                                        }

                                        response.POSWSResponse.ErrNumber = "0";
                                        response.POSWSResponse.Status = "Approved";

                                        isSuccess = true;
                                    }
                                    else if (apiResponse.Status == "Declined")
                                    {
                                        transactionAttempt.PosEntryMode = 0;
                                        transactionAttempt.TransactionAttemptId = nnTransactionAttempt.TransactionAttemptId;
                                        transactionAttempt.AuthNumber = "";
                                        transactionAttempt.ReturnCode = apiResponse.responseCode;
                                        transactionAttempt.SeqNumber = null;
                                        transactionAttempt.TransNumber = null;
                                        transactionAttempt.DisplayReceipt = "";
                                        transactionAttempt.DisplayTerminal = "";
                                        transactionAttempt.DateReceived = DateTime.Now;
                                        transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        transactionAttempt.Notes = "Transaction Purchase Declined." + apiResponse.Message;
                                        transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                        response.POSWSResponse.ErrNumber = apiResponse.responseCode;
                                        response.POSWSResponse.Message = "Transaction Failed. " + apiResponse.Message;
                                        response.POSWSResponse.Status = "Declined";

                                        isSuccess = false;
                                    }
                                    else
                                    {
                                        response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Message;
                                        response.POSWSResponse.ErrNumber = "2200.14";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }

                                #endregion

                                #region NovaToPay
                                else if (mid.Switch.SwitchCode == "Nova2Pay")
                                {
                                    GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                    GatewayProcessor.NovaPay.TransactionData transData = new GatewayProcessor.NovaPay.TransactionData();
                                    GatewayProcessor.NovaPay.HashFunctions hashData = new GatewayProcessor.NovaPay.HashFunctions();

                                    string notifUrl = ConfigurationManager.AppSettings["NotifyURL"].ToString();

                                    transData.MerchantId = mid.Param_2;
                                    transData.TerminalId = mid.Param_6;
                                    transData.MerchantTradeId = transactionAttempt.TransNumber;
                                    transData.NotifyUrl = notifUrl;

                                    Hashtable sParaTemp = GatewayProcessor.NovaPay.HashFunctions.buildParamRefund(transData);
                                    Hashtable sParam = GatewayProcessor.NovaPay.HashFunctions.buildRequestRefund(sParaTemp);

                                    var apiResponse = gateway.ProcessNovaToPay(sParam, "void");

                                    if (apiResponse.Status == "Approved")
                                    {
                                        nnTransactionAttempt.AuthNumber = apiResponse.merchantTradeId;
                                        nnTransactionAttempt.ReturnCode = "00";
                                        nnTransactionAttempt.SeqNumber = "";
                                        nnTransactionAttempt.TransNumber = apiResponse.merchantTradeId;
                                        nnTransactionAttempt.PosEntryMode = 0;
                                        nnTransactionAttempt.Reference = apiResponse.merchantTradeId;
                                        nnTransactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Void);

                                        nnTransactionAttempt.BatchNumber = "";
                                        nnTransactionAttempt.DisplayReceipt = "";
                                        nnTransactionAttempt.DisplayTerminal = "";
                                        nnTransactionAttempt.DateReceived = DateTime.Now;
                                        nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttempt.Notes = "API NovaToPay Void Successful.";

                                        response.POSWSResponse.ErrNumber = "0";
                                        response.POSWSResponse.Message = "Void Successful";
                                        response.POSWSResponse.Status = "Approved";

                                        isSuccess = true;
                                    }
                                    else if (apiResponse.Status == "Declined")
                                    {
                                        nnTransactionAttempt.Reference = "";
                                        nnTransactionAttempt.PosEntryMode = 0;
                                        nnTransactionAttempt.TransactionAttemptId = nnTransactionAttempt.TransactionAttemptId;
                                        nnTransactionAttempt.AuthNumber = "";
                                        nnTransactionAttempt.ReturnCode = apiResponse.flag;
                                        nnTransactionAttempt.SeqNumber = null;
                                        nnTransactionAttempt.TransNumber = null;
                                        nnTransactionAttempt.DisplayReceipt = "";
                                        nnTransactionAttempt.DisplayTerminal = "";
                                        nnTransactionAttempt.DateReceived = DateTime.Now;
                                        nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttempt.Notes = "API NovaToPay Void Declined." + apiResponse.message;
                                        nnTransactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                        response.POSWSResponse.ErrNumber = apiResponse.flag;
                                        response.POSWSResponse.Message = "Void Failed. " + apiResponse.Message;
                                        response.POSWSResponse.Status = "Declined";

                                        isSuccess = false;
                                    }
                                    else
                                    {
                                        response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Message;
                                        response.POSWSResponse.ErrNumber = "2200.14";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }
                                #endregion

                                #region GlobalOnePay
                                else if (mid.Switch.SwitchCode == "GlobalOnePay")
                                {
                                    GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                    GatewayProcessor.GlobalOnePay.TransactionData transData = new GatewayProcessor.GlobalOnePay.TransactionData();


                                    transData.TransNumber = transactionAttempt.TransNumber;
                                    transData.TerminalId = mid.Param_6;
                                    transData.Amount = Convert.ToString(transactionAttempt.Amount);
                                    transData.Datetime = DateTime.UtcNow.ToString("dd-MM-yyyy:hh:mm:ss:fff");
                                    transData.Hash = mid.Param_3;
                                    transData.Operator = transaction.MerchantPOS.MerchantBranch.Merchant.MerchantName;
                                    transData.ReasonToRefund = voidTable.VoidReason;

                                    var apiResponse = gateway.ProcessGlobalOnePayTransaction(transData, "void");

                                    if (apiResponse.Status == "Approved")
                                    {
                                        nnTransactionAttempt.AuthNumber = apiResponse.UniqueReference;
                                        nnTransactionAttempt.ReturnCode = "00";
                                        nnTransactionAttempt.SeqNumber = "";
                                        nnTransactionAttempt.TransNumber = apiResponse.UniqueReference;
                                        nnTransactionAttempt.PosEntryMode = 0;
                                        nnTransactionAttempt.Reference = apiResponse.UniqueReference;
                                        nnTransactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Void);

                                        nnTransactionAttempt.BatchNumber = "";
                                        nnTransactionAttempt.DisplayReceipt = "";
                                        nnTransactionAttempt.DisplayTerminal = "";
                                        nnTransactionAttempt.DateReceived = DateTime.Now;
                                        nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttempt.Notes = "API GlobalOnePay Refund Approved";

                                        response.POSWSResponse.ErrNumber = "0";
                                        response.POSWSResponse.Message = "Void Successful.";
                                        response.POSWSResponse.Status = "Approved";

                                        isSuccess = true;
                                    }
                                    else if (apiResponse.Status == "Declined")
                                    {
                                        nnTransactionAttempt.Reference = "";
                                        nnTransactionAttempt.PosEntryMode = 0;
                                        nnTransactionAttempt.TransactionAttemptId = nnTransactionAttempt.TransactionAttemptId;
                                        nnTransactionAttempt.AuthNumber = "";
                                        nnTransactionAttempt.ReturnCode = apiResponse.ResponseCode;
                                        nnTransactionAttempt.SeqNumber = null;
                                        nnTransactionAttempt.TransNumber = null;
                                        nnTransactionAttempt.DisplayReceipt = "";
                                        nnTransactionAttempt.DisplayTerminal = "";
                                        nnTransactionAttempt.DateReceived = DateTime.Now;
                                        nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttempt.Notes = "API GlobalOnePay Refund Declined." + apiResponse.Message;
                                        nnTransactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                        response.POSWSResponse.ErrNumber = apiResponse.ResponseCode;
                                        response.POSWSResponse.Message = "Void Failed. " + apiResponse.Message;
                                        response.POSWSResponse.Status = "Declined";

                                        isSuccess = false;
                                    }
                                    else
                                    {
                                        response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Message;
                                        response.POSWSResponse.ErrNumber = "2200.14";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }
                                #endregion

                                #region World Net
                                else if (mid.Switch.SwitchCode == "Worldnet")
                                {
                                    GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                    GatewayProcessor.Worldnet.TransactionData transData = new GatewayProcessor.Worldnet.TransactionData();

                                    transData.TransNumber = transactionAttempt.TransNumber;
                                    transData.TerminalId = mid.Param_6;
                                    transData.Amount = Convert.ToString(transactionAttempt.Amount);
                                    transData.Datetime = DateTime.UtcNow.ToString("dd-MM-yyyy:hh:mm:ss:fff");
                                    transData.Hash = mid.Param_3;
                                    transData.Operator = transaction.MerchantPOS.MerchantBranch.Merchant.MerchantName;
                                    transData.ReasonToRefund = voidTable.VoidReason;
                                    
                                    var apiResponse = gateway.ProcessWorldnetTransaction(transData, "void");

                                    if (apiResponse.Status == "Approved")
                                    {
                                        nnTransactionAttempt.AuthNumber = apiResponse.ApprovalCode;
                                        nnTransactionAttempt.ReturnCode = apiResponse.ResponseCode;
                                        nnTransactionAttempt.SeqNumber = apiResponse.UniqueReference;
                                        nnTransactionAttempt.TransNumber = apiResponse.UniqueReference;
                                        nnTransactionAttempt.PosEntryMode = 0;

                                        nnTransactionAttempt.BatchNumber = DateTime.Now.ToString("yyyyMMdd");
                                        nnTransactionAttempt.DisplayReceipt = "";
                                        nnTransactionAttempt.DisplayTerminal = transData.TerminalId;
                                        nnTransactionAttempt.DateReceived = DateTime.Now;
                                        nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttempt.Notes = "API WorldNet Void Approved";

                                        response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                                        response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                        response.POSWSResponse.ErrNumber = "0";
                                        response.POSWSResponse.Message = "Void Successful.";
                                        response.POSWSResponse.Status = "Approved";

                                        isSuccess = true;
                                    }
                                    else if(apiResponse.Status == "Declined")
                                    {
                                        nnTransactionAttempt.PosEntryMode = 0;
                                        nnTransactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                        nnTransactionAttempt.AuthNumber = "";
                                        nnTransactionAttempt.ReturnCode = apiResponse.ResponseCode;
                                        nnTransactionAttempt.SeqNumber = null;
                                        nnTransactionAttempt.TransNumber = null;
                                        nnTransactionAttempt.DisplayReceipt = "";
                                        nnTransactionAttempt.DisplayTerminal = "";
                                        nnTransactionAttempt.DateReceived = DateTime.Now;
                                        nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttempt.Notes = " API WorldNet Void Declined." + apiResponse.Message;
                                        nnTransactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                        response.POSWSResponse.ErrNumber = apiResponse.ResponseCode;
                                        response.POSWSResponse.Message = apiResponse.Message;
                                        response.POSWSResponse.Status = "Declined";

                                        isSuccess = false;
                                    }
                                    else
                                    {
                                        response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Message;
                                        response.POSWSResponse.ErrNumber = "2200.14";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }
                                #endregion

                                #region Maxbank
                                else if(mid.Switch.SwitchCode == "Maxbank")
                                {
                                    DE_ISO8583 de = new DE_ISO8583();
                                    GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                    GatewayProcessor.VeritasPayment.CardDetails transData = new GatewayProcessor.VeritasPayment.CardDetails();

                                    transData.PrivateAdditionalData = ""; //Convert.ToString(request.CardDetails.EpbKsn.Length).PadLeft(4, '0') + request.CardDetails.EpbKsn + Convert.ToString(cardNumber.Length).PadLeft(4, '0') + cardNumber;
                                    transData.MerchantID = mid.Param_2;
                                    transData.TerminalID = mid.Param_6;
                                    transData.SystemTraceAudit = nnTransactionAttempt.TransNumber;
                                    transData.AuthorizationIDResponse = nnTransactionAttempt.AuthNumber;
                                    transData.RetrievalReferenceNumber = nnTransactionAttempt.Reference;
                                    transData.CurrencyCode = mid.Currency.IsoCode;
                                    transData.CardNumber = D_Card;

                                    decimal orgAmount = nnTransactionAttempt.Amount;
                                    decimal finalAmount = orgAmount * 100;

                                    try
                                    {
                                        transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                    }
                                    catch
                                    {
                                        transData.Amount = finalAmount.ToString();
                                    }

                                    var apiResponse = gateway.ProcessPurchaseMaxbankGateway(transData, "VOID");

                                    if (apiResponse.Result.Status == "Approved")
                                    {
                                        nnTransactionAttempt.AuthNumber = apiResponse.Result.AuthorizationID;
                                        nnTransactionAttempt.ReturnCode = apiResponse.Result.ReturnCode;
                                        nnTransactionAttempt.TransNumber = apiResponse.Result.SytemTraceAudit;
                                        nnTransactionAttempt.SeqNumber = apiResponse.Result.SytemTraceAudit;
                                        nnTransactionAttempt.BatchNumber = DateTime.Now.ToString("yyyyMMdd");
                                        nnTransactionAttempt.DisplayReceipt = transData.MerchantID;
                                        nnTransactionAttempt.DisplayTerminal = transData.TerminalID;
                                        nnTransactionAttempt.DateReceived = DateTime.Now;
                                        nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttempt.TransactionTypeId = Convert.ToInt32((SDGDAL.Enums.TransactionType.Void));
                                        nnTransactionAttempt.Notes = "API Maxbank Void Approved";

                                        response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                                        response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType.Void));
                                        response.POSWSResponse.ErrNumber = "0";
                                        response.POSWSResponse.Message = "Transaction Successful.";
                                        response.POSWSResponse.Status = "Approved";

                                        isSuccess = true;
                                    }
                                    else if (apiResponse.Result.Status == "Declined")
                                    {
                                        nnTransactionAttempt.AuthNumber = apiResponse.Result.AuthorizationID;
                                        nnTransactionAttempt.ReturnCode = apiResponse.Result.ReturnCode;
                                        nnTransactionAttempt.TransNumber = apiResponse.Result.SytemTraceAudit;
                                        nnTransactionAttempt.SeqNumber = apiResponse.Result.SytemTraceAudit;
                                        nnTransactionAttempt.BatchNumber = DateTime.Now.ToString("yyyyMMdd");
                                        nnTransactionAttempt.DisplayReceipt = transData.MerchantID;
                                        nnTransactionAttempt.DisplayTerminal = transData.TerminalID;
                                        nnTransactionAttempt.DateReceived = DateTime.Now;
                                        nnTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttempt.TransactionTypeId = Convert.ToInt32((SDGDAL.Enums.TransactionType.Declined));
                                        nnTransactionAttempt.Notes = "API Maxbank Void Declined." + apiResponse.Result.Message;

                                        response.POSWSResponse.ErrNumber = apiResponse.Result.ReturnCode;
                                        response.POSWSResponse.Message = "Transaction Failed. " + apiResponse.Result.Message;
                                        response.POSWSResponse.Status = "Declined";

                                    }
                                    else
                                    {
                                        response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Result.Message;
                                        response.POSWSResponse.ErrNumber = "2200.14";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }
                                #endregion

                                else
                                {
                                    response.POSWSResponse.Message = "Transaction failed. Switch not yet supported. Please contact Support.";
                                    response.POSWSResponse.ErrNumber = "2200.16";
                                    response.POSWSResponse.Status = "Declined";
                                    return response;
                                }

                                action = "updating transaction attempt data";
                                var voidTransactionAttempt = _transRepo.UpdateTransactionAttempt(nnTransactionAttempt);

                                if(isSuccess)
                                {
                                    action = "updating amount of previous transaction";
                                    transactionAttempt.Amount = (transactionAttempt.Amount - nnTransactionAttempt.Amount);
                                    var updateTransactionAttempt = _transRepo.UpdateTransactionAttempt(transactionAttempt);
                                }
                                
                                response.Date = Convert.ToString(nnTransactionAttempt.DateReceived);
                                response.Amount = Convert.ToString(nnTransactionAttempt.Amount);
                                response.TransactionNumber = nnTransactionAttempt.TransactionId + "-" + nnTransactionAttempt.TransactionAttemptId;
                                response.AuthNumber = nnTransactionAttempt.AuthNumber;
                                response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                                response.CardNumber = SDGUtil.Functions.HashCardNumber(D_Card);
                                response.Currency = transaction.Currency.CurrencyCode;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error please
                        var errorOnAction = "Error while " + action;

                        var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "CreditTransactionRequest", ex.StackTrace);

                        response.POSWSResponse.ErrNumber = "2200.17";
                        response.POSWSResponse.Message = "Unknown error, please contact support. Ref: " + errRefNumber;
                        response.POSWSResponse.Status = "Declined";
                    }

                    #endregion Handle Transaction response
                }
                else if (VoidRequest.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    response.POSWSResponse.ErrNumber = "0";
                    response.POSWSResponse.Status = "Approved";
                    response.POSWSResponse.Message = "";
                }
                else
                {
                    response.POSWSResponse.ErrNumber = "2200.18";
                    response.POSWSResponse.Status = "Declined";
                    response.POSWSResponse.Message = "Invalid System Mode";
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "CreditTransactionVoid", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message, "CreditTransactionVoid  ErrRef:" + errRefNumber, ex.InnerException.StackTrace);
                }

                response.POSWSResponse.ErrNumber = "2001.18";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber + " " + action;
                response.POSWSResponse.Status = "DECLINED";
            }

            return response;
        }

        public ReversalResponse ReversalTransactionCredit(ReversalRequest request)
        {
            string action = string.Empty;

            ReversalResponse response = new ReversalResponse();

            response.POSWSResponse = new POSWSResponse();

            if (string.IsNullOrEmpty(request.SystemTraceNumber))
            {
                response.POSWSResponse.ErrNumber = "1001.1";
                response.POSWSResponse.Message = "Missing input";
                response.POSWSResponse.Status = "DECLINED";
                return response;
            }

            response.POSWSResponse.ErrNumber = "0";

            try
            {
                var mobileApp = new SDGDAL.Entities.MobileApp();
                var account = new SDGDAL.Entities.Account();
                var transactionAttempt = new SDGDAL.Entities.TransactionAttempt();

                action = "checking trace number is existing";
                transactionAttempt = _transRepo.GetCreditTransactionByTraceNumber(request.SystemTraceNumber);

                if (transactionAttempt != null)
                {
                    action = "retrieving mobile App details using Activation Code";
                    mobileApp = _mAppRepo.GetMobileAppFullDetailsByActivationCode(transactionAttempt.MobileApp.ActivationCode.Replace("-", ""));

                    MobileAppMethods mobileAppMethods = new MobileAppMethods();

                    response.POSWSResponse.Status = "Reversed";
                    transactionAttempt.ReturnCode = request.ReturnCode;
                    transactionAttempt.Notes = request.Message;
                    transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Reversed);
                    transactionAttempt.DateReceived = DateTime.Now;
                    transactionAttempt.Notes = "Reversed " + request.Message;
                    transactionAttempt.DisplayReceipt = request.MerchantId;
                    transactionAttempt.DisplayTerminal = request.TerminalId;
                    transactionAttempt.ReturnCode = request.ReturnCode;
                    transactionAttempt.AuthNumber = request.AuthorizationID;
                    transactionAttempt.TransNumber = request.RetrievalReferenceNumber;
                    transactionAttempt.BatchNumber = _batchRepo.GenerateBatchNumber(mobileApp.MobileAppId, Convert.ToInt32(SDGDAL.Enums.Ref_PaymentType.Debit));

                    var nTransactionAttempt = _transRepo.UpdateTransactionAttempt(transactionAttempt);

                    response.MerchantId = transactionAttempt.DisplayReceipt;
                    response.TerminalId = transactionAttempt.DisplayTerminal;
                    response.Currency = transactionAttempt.Transaction.Currency.CurrencyCode;
                    response.TransactionNumber = Convert.ToString(transactionAttempt.TransactionId) + "-" + Convert.ToString(transactionAttempt.TransactionAttemptId);
                    response.AuthNumber = transactionAttempt.AuthNumber;
                    response.TraceNumber = transactionAttempt.TransNumber;
                    response.SequenceNumber = transactionAttempt.SeqNumber;
                    response.BatchNumber = transactionAttempt.BatchNumber;
                    response.Timestamp = SDGUtil.Functions.Format_Datetime(transactionAttempt.DateReceived);
                    response.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)transactionAttempt.Transaction.TransactionEntryTypeId);
                    response.Total = Convert.ToDecimal(transactionAttempt.Amount.ToString("N2"));
                }
                else
                {
                    response.POSWSResponse.Status = "Declined";
                    response.POSWSResponse.ErrNumber = "2600.1";
                    response.POSWSResponse.Message = "Invalid Trace Number.";
                    return response;
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, " CreditReversalTransaction", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message, "CreditReversalTransaction  ErrRef:" + errRefNumber, ex.InnerException.StackTrace);
                }

                response.POSWSResponse.ErrNumber = "2600.2";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber + " " + action;
                response.POSWSResponse.Status = "DECLINED";
            }

            return response;
        }
    }
}
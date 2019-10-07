using CT_EMV_CLASSES;
using CT_EMV_CLASSES.DOWNLOAD;
using CT_EMV_CLASSES.FINANCIAL;
using HPA_ISO8583;
using SDGDAL;
using SDGDAL.Repositories;
using SDGUtil;
using SDGWebService.Classes;
using SDGWebService.CTPaymentEMV_Classes;
using SDGWebService.TLVFunctions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace SDGWebService.WebserviceFunctions {
     public class WebserviceFunctionsEMV {
          private MobileAppRepository _mAppRepo = new MobileAppRepository();
          private MerchantRepository _merchantRepo = new MerchantRepository();
          private TransactionRepository _transRepo = new TransactionRepository();
          private MidsRepository _midsRepo = new MidsRepository();
          private MobileAppFeaturesRepository _posFeatures = new MobileAppFeaturesRepository();
          private MerchantBranchPOSRepository _posRepo = new MerchantBranchPOSRepository();
          private UserRepository _userRepo = new UserRepository();
          private DebitSystemTraceNumRepository _traceNumRepo = new DebitSystemTraceNumRepository();
          private DeviceRepository _deviceRepo = new DeviceRepository();
          private MerchantBranchRepository _branchRepo = new MerchantBranchRepository();
          private EMVCreditDebitRepository _emvCreditDebitRepo = new EMVCreditDebitRepository();
          private Functions.MobileAppFunctions mobileAppFunctions = new Functions.MobileAppFunctions();
          private DebitSystemTraceNumRepository _traceNumberRepo = new DebitSystemTraceNumRepository();
          private BatchReportRepository _batchRepo = new BatchReportRepository();

          private int _tokenExpirationInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["RTokenExpirationByMinutes"]);

          public HOST_TERMINAL_DOWNLOAD EMVDownloadHostAndTerminal(POSWSRequest POSWSRequest, string terminalId) {
               string action = string.Empty;

               HostTerminalDownloadResponse response = new HostTerminalDownloadResponse();
               HOST_TERMINAL_DOWNLOAD result = new HOST_TERMINAL_DOWNLOAD();

               try {
                    string operatorId = "   ";
                    string transType = "097";
                    string seqNumber = SDGUtil.Functions.GenerateSystemTraceAudit();
                    string seqByte = "1";

                    GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                    CTPaymentEMV_Classes.CTUtility utility = new CTPaymentEMV_Classes.CTUtility();
                    CT_EMV_CLASSES.HEADER_REQUEST request = new HEADER_REQUEST(utility.MessageClassHost, terminalId, operatorId, seqByte + seqNumber, transType, DateTime.Now, utility.PosEntryMode, utility.MessageVersion, utility.PosStatusIndicator);

                    request.GetRequestHeader();

                    action = "searching download terminal ID";
                    var emvDownload = _emvCreditDebitRepo.GetDownloadEMVParameters(terminalId);

                    if (emvDownload == null) {
                         action = "processing request to download emv host terminal from CTPayment.";
                         var apiResponse = gateway.DownloadEMVHostAndTerminals(request);

                         if (apiResponse != null && apiResponse.Result.ErrorNumber.Equals("0")) {
                              result.ErrorMessage = "Success Download";
                              result.ErrorNumber = apiResponse.Result.ErrorNumber;
                              result.HeaderResponse = apiResponse.Result.HeaderResponse;
                              result.PaySchemeParamIndicator = apiResponse.Result.PaySchemeParamIndicator;
                              result.DLDataHostParameter = apiResponse.Result.DLDataHostParameter;
                              result.DLDataTerminalParameter = apiResponse.Result.DLDataTerminalParameter;
                              result.DLDataEMVParameter = apiResponse.Result.DLDataEMVParameter;

                              action = "saving response of Emv Host-Terminal.";

                              #region Saving download data

                              #region HEADER RESPONSE

                              var emvHeaderResponse = new SDGDAL.Entities.HeaderResponse();
                              emvHeaderResponse.TerminalId = result.HeaderResponse.TID;
                              emvHeaderResponse.MessageClass = result.HeaderResponse.MsgClass;
                              emvHeaderResponse.MessageVersion = result.HeaderResponse.MsgVersion;
                              emvHeaderResponse.POSEntryMode = result.HeaderResponse.POSEntryMode;
                              emvHeaderResponse.POSResultCode = result.HeaderResponse.POSResultCode;
                              emvHeaderResponse.POSStatIndicator = result.HeaderResponse.POSStatIndicator;
                              emvHeaderResponse.ResultMessage = "Success Download";
                              emvHeaderResponse.ReturnCode = result.ErrorNumber;
                              emvHeaderResponse.SequenceNumber = result.HeaderResponse.SeqNo;
                              emvHeaderResponse.TransType = result.HeaderResponse.TranType;

                              #endregion HEADER RESPONSE

                              #region EMV HOST PARAM

                              var emvHostParam = new SDGDAL.Entities.EmvHostParameter();
                              emvHostParam.AmexJCBCLFloorLimit = result.DLDataHostParameter.AmexJCBCLFloorLimit;
                              emvHostParam.AmexJCBCLCVMLimit = result.DLDataHostParameter.AmexJCBCLCVMLimit;
                              emvHostParam.AmexJCBEMVFloorLimit = result.DLDataHostParameter.AmexJCBEMVFloorLimit;
                              emvHostParam.AmexJCBCLTxnLimit = result.DLDataHostParameter.AmexJCBCLTxnLimit;
                              emvHostParam.CashbackSubchargeLimit = result.DLDataHostParameter.CashBackSSurchargeLimit;
                              emvHostParam.CCCashbackSubcharge = result.DLDataHostParameter.CCCashBackSurcharge;
                              emvHostParam.CCRetailSubcharge = result.DLDataHostParameter.CCRetailSSurcharge;
                              emvHostParam.DialActTime = result.DLDataHostParameter.DialAcquiTime;
                              emvHostParam.DiscoverCLFloorLimit = result.DLDataHostParameter.DiscoverCLFloorLimit;
                              emvHostParam.DiscoverCLCvmLimit = result.DLDataHostParameter.DiscoverCLCvmLimit;
                              emvHostParam.DiscoverEVMFloorLimit = result.DLDataHostParameter.DiscoverEVMFloorLimit;
                              emvHostParam.DiscoverCLTxnLimit = result.DLDataHostParameter.DiscoverCLTxnLimit;
                              emvHostParam.ExtraReceiptDisplay = result.DLDataHostParameter.ExtraReceiptDisplay;
                              emvHostParam.Future1 = result.DLDataHostParameter.Future1;
                              emvHostParam.Future2 = result.DLDataHostParameter.Future2;
                              emvHostParam.Future3 = result.DLDataHostParameter.Future3;
                              emvHostParam.HostDateTime = result.DLDataHostParameter.HostDatetime;
                              emvHostParam.IDPCashbackSubcharge = result.DLDataHostParameter.IDPCashBackSurcharge;
                              emvHostParam.IDPRetailSubcharge = result.DLDataHostParameter.IDPRetailSurcharge;
                              emvHostParam.InteracCLReceiptReqLimit = result.DLDataHostParameter.InteracCLReceiptReqLimit;
                              emvHostParam.IPAddress = result.DLDataHostParameter.PrimIPAdd;
                              emvHostParam.MasterCardCLCvmLimit = result.DLDataHostParameter.MasterCardCLCvmLimit;
                              emvHostParam.MasterCardEMVFloorLimit = result.DLDataHostParameter.MasterCardEMVFloorLimit;
                              emvHostParam.MasterCardCLFloorLimit = result.DLDataHostParameter.MasterCarcCLFloorLimit;
                              emvHostParam.MasterCardCLTxnLimit = result.DLDataHostParameter.MasterCardCLTxnLimit;
                              emvHostParam.MerchantAddress = result.DLDataHostParameter.MerchantStAdd;
                              emvHostParam.MerchantCityProv = result.DLDataHostParameter.MerchantCTProvPostal;
                              emvHostParam.MerchantName = result.DLDataHostParameter.MerchantName;
                              emvHostParam.MerchantTypeIndicator = result.DLDataHostParameter.MerchantTypeIndi;
                              emvHostParam.PredialSetting = result.DLDataHostParameter.PreDialSetting;
                              emvHostParam.PrimaryPhoneNumber = result.DLDataHostParameter.PrimPhoneNo;
                              emvHostParam.RetailSubchargeLimit = result.DLDataHostParameter.RetailSSurchargeLimit;
                              emvHostParam.SecondaryIPAddress = result.DLDataHostParameter.SecIPAdd;
                              emvHostParam.SecondaryPhoneNumber = result.DLDataHostParameter.SecPhoneNo;
                              emvHostParam.TerminalAppSoftwareVer = result.DLDataHostParameter.TerminalAppSoftVersion;
                              emvHostParam.TerminalOptionStat = result.DLDataHostParameter.TerminalOptionStat;
                              emvHostParam.TerminalTransInfo = result.DLDataHostParameter.TerminalTransInfo;
                              emvHostParam.TransResponseTimer = result.DLDataHostParameter.TranResTimer;
                              emvHostParam.VisaCLFloorLimit = result.DLDataHostParameter.VisaCLFloorLimit;
                              emvHostParam.VisaCLCvmLimit = result.DLDataHostParameter.VisaCLCvmLimit;
                              emvHostParam.VisaDebitSupport = result.DLDataHostParameter.VisaDebitSupp;
                              emvHostParam.VISAEMVFloorLimit = result.DLDataHostParameter.VISAEMVFloorLimit;
                              emvHostParam.VisaCLTxnLimit = result.DLDataHostParameter.VisaCLTxnLimit;

                              #endregion EMV HOST PARAM

                              #region EMV TERMINAL PARAM

                              var emvTerminalParam = new SDGDAL.Entities.EmvTerminalParameter();
                              emvTerminalParam.AddTerminlaCapablities = result.DLDataTerminalParameter.AddTerminalCapabilities;
                              emvTerminalParam.Reserved = result.DLDataTerminalParameter.Reserved;
                              emvTerminalParam.TerminalCapabilities = result.DLDataTerminalParameter.TerminalCapabilities;
                              emvTerminalParam.TerminalCountryCode = result.DLDataTerminalParameter.TerminalCntryCode;
                              emvTerminalParam.TerminalType = result.DLDataTerminalParameter.TerminalType;
                              emvTerminalParam.TransCurrCode = result.DLDataTerminalParameter.TransCurCode;
                              emvTerminalParam.TransCurrExponent = result.DLDataTerminalParameter.TransCurExponent;
                              emvTerminalParam.TransRefCurrCode = result.DLDataTerminalParameter.TransRefCurCode;
                              emvTerminalParam.TransRefCurrConversion = result.DLDataTerminalParameter.TransRefCurConversion;
                              emvTerminalParam.TransRefCurrExponent = result.DLDataTerminalParameter.TransRefCurExponent;

                              #endregion EMV TERMINAL PARAM

                              #region EMV MASTERCARD PARAM

                              var emvMcParam = new SDGDAL.Entities.EmvMastercardParameter();
                              emvMcParam.ApplicationID = result.DLDataEMVParameter.MC.AppID;
                              emvMcParam.AppAccountSelection = result.DLDataEMVParameter.MC.AppAcctSelection;
                              emvMcParam.AppSelectionIndicator = result.DLDataEMVParameter.MC.AppSelectIndicator;
                              emvMcParam.AppVersionNumber = result.DLDataEMVParameter.MC.AppVerNo;
                              emvMcParam.ContactlessTerminalDefault = result.DLDataEMVParameter.MC.CTACDEF;
                              emvMcParam.ContactlessTerminalDenial = result.DLDataEMVParameter.MC.CLTerminalActCodeDenial;
                              emvMcParam.ContactlessTerminalOnline = result.DLDataEMVParameter.MC.CLTerminalActCodeOnline;
                              emvMcParam.MaxTarget = result.DLDataEMVParameter.MC.MaxTargetPercentBiasedRandSelection;
                              emvMcParam.TagPercent = result.DLDataEMVParameter.MC.TargetPercentRandSelection;
                              emvMcParam.ThresholdValue = result.DLDataEMVParameter.MC.ThresholdValBiasedRandSelection;
                              emvMcParam.TerminalActionDefault = result.DLDataEMVParameter.MC.TerminalActCodeDefault;
                              emvMcParam.TerminalActionDenial = result.DLDataEMVParameter.MC.TerminalActCodeDenial;
                              emvMcParam.TerminalActionOnline = result.DLDataEMVParameter.MC.TerminalActCodeOnline;

                              #endregion EMV MASTERCARD PARAM

                              #region EMV VISA PARAM

                              var emvVisaParam = new SDGDAL.Entities.EmvVisaParameter();
                              emvVisaParam.ApplicationID = result.DLDataEMVParameter.VISA.AppID;
                              emvVisaParam.AppAccountSelection = result.DLDataEMVParameter.VISA.AppAcctSelection;
                              emvVisaParam.AppSelectionIndicator = result.DLDataEMVParameter.VISA.AppSelectIndicator;
                              emvVisaParam.AppVersionNumber = result.DLDataEMVParameter.VISA.AppVerNo;
                              emvVisaParam.ContactlessTerminalDefault = result.DLDataEMVParameter.VISA.CTACDEF;
                              emvVisaParam.ContactlessTerminalDenial = result.DLDataEMVParameter.VISA.CLTerminalActCodeDenial;
                              emvVisaParam.ContactlessTerminalOnline = result.DLDataEMVParameter.VISA.CLTerminalActCodeOnline;
                              emvVisaParam.MaxTarget = result.DLDataEMVParameter.VISA.MaxTargetPercentBiasedRandSelection;
                              emvVisaParam.TagPercent = result.DLDataEMVParameter.VISA.TargetPercentRandSelection;
                              emvVisaParam.ThresholdValue = result.DLDataEMVParameter.VISA.ThresholdValBiasedRandSelection;
                              emvVisaParam.TerminalActionDefault = result.DLDataEMVParameter.VISA.TerminalActCodeDefault;
                              emvVisaParam.TerminalActionDenial = result.DLDataEMVParameter.VISA.TerminalActCodeDenial;
                              emvVisaParam.TerminalActionOnline = result.DLDataEMVParameter.VISA.TerminalActCodeOnline;

                              #endregion EMV VISA PARAM

                              #region EMV AMEX PARAM

                              var emvAmexParam = new SDGDAL.Entities.EmvAmexParameter();
                              emvAmexParam.ApplicationID = result.DLDataEMVParameter.AMEX.AppID;
                              emvAmexParam.AppAccountSelection = result.DLDataEMVParameter.AMEX.AppAcctSelection;
                              emvAmexParam.AppSelectionIndicator = result.DLDataEMVParameter.AMEX.AppSelectIndicator;
                              emvAmexParam.AppVersionNumber = result.DLDataEMVParameter.AMEX.AppVerNo;
                              emvAmexParam.ContactlessTerminalDefault = result.DLDataEMVParameter.AMEX.CTACDEF;
                              emvAmexParam.ContactlessTerminalDenial = result.DLDataEMVParameter.AMEX.CLTerminalActCodeDenial;
                              emvAmexParam.ContactlessTerminalOnline = result.DLDataEMVParameter.AMEX.CLTerminalActCodeOnline;
                              emvAmexParam.MaxTarget = result.DLDataEMVParameter.AMEX.MaxTargetPercentBiasedRandSelection;
                              emvAmexParam.TagPercent = result.DLDataEMVParameter.AMEX.TargetPercentRandSelection;
                              emvAmexParam.ThresholdValue = result.DLDataEMVParameter.AMEX.ThresholdValBiasedRandSelection;
                              emvAmexParam.TerminalActionDefault = result.DLDataEMVParameter.AMEX.TerminalActCodeDefault;
                              emvAmexParam.TerminalActionDenial = result.DLDataEMVParameter.AMEX.TerminalActCodeDenial;
                              emvAmexParam.TerminalActionOnline = result.DLDataEMVParameter.AMEX.TerminalActCodeOnline;

                              #endregion EMV AMEX PARAM

                              #region EMV INTERAC PARAM

                              var emvInteracParam = new SDGDAL.Entities.EmvInteracParameter();
                              emvInteracParam.ApplicationID = result.DLDataEMVParameter.INTERAC.AppID;
                              emvInteracParam.AppAccountSelection = result.DLDataEMVParameter.INTERAC.AppAcctSelection;
                              emvInteracParam.AppSelectionIndicator = result.DLDataEMVParameter.INTERAC.AppSelectIndicator;
                              emvInteracParam.AppVersionNumber = result.DLDataEMVParameter.INTERAC.AppVerNo;
                              emvInteracParam.ContactlessTerminalDefault = result.DLDataEMVParameter.INTERAC.CTACDEF;
                              emvInteracParam.ContactlessTerminalDenial = result.DLDataEMVParameter.INTERAC.CLTerminalActCodeDenial;
                              emvInteracParam.ContactlessTerminalOnline = result.DLDataEMVParameter.INTERAC.CLTerminalActCodeOnline;
                              emvInteracParam.MaxTarget = result.DLDataEMVParameter.INTERAC.MaxTargetPercentBiasedRandSelection;
                              emvInteracParam.TagPercent = result.DLDataEMVParameter.INTERAC.TargetPercentRandSelection;
                              emvInteracParam.ThresholdValue = result.DLDataEMVParameter.INTERAC.ThresholdValBiasedRandSelection;
                              emvInteracParam.TerminalActionDefault = result.DLDataEMVParameter.INTERAC.TerminalActCodeDefault;
                              emvInteracParam.TerminalActionDenial = result.DLDataEMVParameter.INTERAC.TerminalActCodeDenial;
                              emvInteracParam.TerminalActionOnline = result.DLDataEMVParameter.INTERAC.TerminalActCodeOnline;

                              #endregion EMV INTERAC PARAM

                              #region EMV JCB PARAM

                              var emvJcbParam = new SDGDAL.Entities.EmvJcbParameter();
                              emvJcbParam.ApplicationID = result.DLDataEMVParameter.JCB.AppID;
                              emvJcbParam.AppAccountSelection = result.DLDataEMVParameter.JCB.AppAcctSelection;
                              emvJcbParam.AppSelectionIndicator = result.DLDataEMVParameter.JCB.AppSelectIndicator;
                              emvJcbParam.AppVersionNumber = result.DLDataEMVParameter.JCB.AppVerNo;
                              emvJcbParam.ContactlessTerminalDefault = result.DLDataEMVParameter.JCB.CTACDEF;
                              emvJcbParam.ContactlessTerminalDenial = result.DLDataEMVParameter.JCB.CLTerminalActCodeDenial;
                              emvJcbParam.ContactlessTerminalOnline = result.DLDataEMVParameter.JCB.CLTerminalActCodeOnline;
                              emvJcbParam.MaxTarget = result.DLDataEMVParameter.JCB.MaxTargetPercentBiasedRandSelection;
                              emvJcbParam.TagPercent = result.DLDataEMVParameter.JCB.TargetPercentRandSelection;
                              emvJcbParam.ThresholdValue = result.DLDataEMVParameter.JCB.ThresholdValBiasedRandSelection;
                              emvJcbParam.TerminalActionDefault = result.DLDataEMVParameter.JCB.TerminalActCodeDefault;
                              emvJcbParam.TerminalActionDenial = result.DLDataEMVParameter.JCB.TerminalActCodeDenial;
                              emvJcbParam.TerminalActionOnline = result.DLDataEMVParameter.JCB.TerminalActCodeOnline;

                              #endregion EMV JCB PARAM

                              #region EMV DISCOVER PARAM

                              var emvDiscoverParam = new SDGDAL.Entities.EmvDiscoverParameter();
                              emvDiscoverParam.ApplicationID = result.DLDataEMVParameter.DISCOVER.AppID;
                              emvDiscoverParam.AppAccountSelection = result.DLDataEMVParameter.DISCOVER.AppAcctSelection;
                              emvDiscoverParam.AppSelectionIndicator = result.DLDataEMVParameter.DISCOVER.AppSelectIndicator;
                              emvDiscoverParam.AppVersionNumber = result.DLDataEMVParameter.DISCOVER.AppVerNo;
                              emvDiscoverParam.ContactlessTerminalDefault = result.DLDataEMVParameter.DISCOVER.CTACDEF;
                              emvDiscoverParam.ContactlessTerminalDenial = result.DLDataEMVParameter.DISCOVER.CLTerminalActCodeDenial;
                              emvDiscoverParam.ContactlessTerminalOnline = result.DLDataEMVParameter.DISCOVER.CLTerminalActCodeOnline;
                              emvDiscoverParam.MaxTarget = result.DLDataEMVParameter.DISCOVER.MaxTargetPercentBiasedRandSelection;
                              emvDiscoverParam.TagPercent = result.DLDataEMVParameter.DISCOVER.TargetPercentRandSelection;
                              emvDiscoverParam.ThresholdValue = result.DLDataEMVParameter.DISCOVER.ThresholdValBiasedRandSelection;
                              emvDiscoverParam.TerminalActionDefault = result.DLDataEMVParameter.DISCOVER.TerminalActCodeDefault;
                              emvDiscoverParam.TerminalActionDenial = result.DLDataEMVParameter.DISCOVER.TerminalActCodeDenial;
                              emvDiscoverParam.TerminalActionOnline = result.DLDataEMVParameter.DISCOVER.TerminalActCodeOnline;

                              #endregion EMV DISCOVER PARAM

                              var resultHeader = _emvCreditDebitRepo.SaveDownloadHostTerminal(emvHeaderResponse, emvHostParam, emvTerminalParam, emvMcParam,
                                                                                              emvVisaParam, emvAmexParam, emvInteracParam, emvJcbParam, emvDiscoverParam);

                              if (resultHeader) {
                              } else {
                                   //save logs
                              }

                              #endregion Saving download data
                         } else if (apiResponse != null) {
                              #region HEADER RESPONSE

                              result.ErrorMessage = "Failed to Download.";
                              result.ErrorNumber = apiResponse.Result.ErrorNumber;
                              result.HeaderResponse.TID = apiResponse.Result.HeaderResponse.TID;
                              result.HeaderResponse.MsgVersion = apiResponse.Result.HeaderResponse.MsgVersion;
                              result.HeaderResponse.POSEntryMode = apiResponse.Result.HeaderResponse.POSEntryMode;
                              result.HeaderResponse.POSResultCode = apiResponse.Result.HeaderResponse.POSResultCode;
                              result.HeaderResponse.POSStatIndicator = apiResponse.Result.HeaderResponse.POSStatIndicator;
                              result.HeaderResponse.MsgClass = "Failed to Download. " + apiResponse.Result.ErrorMessage;
                              result.HeaderResponse.POSResultCode = apiResponse.Result.ErrorNumber;
                              result.HeaderResponse.SeqNo = apiResponse.Result.HeaderResponse.SeqNo;
                              result.HeaderResponse.TranType = apiResponse.Result.HeaderResponse.TranType;

                              #endregion HEADER RESPONSE
                         }
                    } else {
                         result.ErrorMessage = emvDownload.ResultMessage;
                         result.ErrorNumber = emvDownload.ReturnCode;
                    }

                    return result;
               } catch (Exception ex) {
                    // Log error please
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "EMV Download Host and Terminal", ex.StackTrace);
               }

               return result;
          }

          public PurchaseResponse TransactionPurchaseCreditEMV(TransactionRequest request) {
               #region

               string action = string.Empty;

               PurchaseResponse response = new PurchaseResponse();
               response.POSWSResponse = new POSWSResponse();

               object obj = response.POSWSResponse;

               if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.TRANSACTION_PURCHASE_CREDIT_EMV, (object)request, out obj)) {
                    response.POSWSResponse = (POSWSResponse)obj;
                    return response;
               }

               var transactionAttemptId = 0;

               response.POSWSResponse.ErrNumber = "0";

               try {
                    if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE")) {
                         var mobileApp = new SDGDAL.Entities.MobileApp();
                         var account = new SDGDAL.Entities.Account();

                         action = "checking mobile app availability.";

                         response.POSWSResponse = mobileAppFunctions.CheckStatus(request.POSWSRequest.RToken, out mobileApp, out account);

                         if (response.POSWSResponse.Status == "Declined") {
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

                         SDGDAL.Enums.SwipeDevice sd = (SDGDAL.Enums.SwipeDevice)request.Device;
                         SDGDAL.Enums.CardAction cardAction = (SDGDAL.Enums.CardAction)request.Action;
                         ClassTLV emvDataResult = new ClassTLV();

                         response.POSWSResponse = GetCardDetails(request.CardDetails, sd, cardAction, out track1, out track2,
                             out cardNumber, out nameOnCard, out expDate, out expYear, out expMonth, out emvDataResult);

                         if (response.POSWSResponse.Status == "Declined") {
                              return response;
                         }

                         #region Read Tracks

                         try {
                              action = "reading track data from icc.";

                              decimal tempNum = Convert.ToDecimal(cardNumber);

                              decimal dMonth, dYear;

                              dMonth = SDGUtil.Functions.ConvertNumeric(expMonth);
                              dYear = SDGUtil.Functions.ConvertNumeric(expYear);

                              if (dMonth == -1 || dYear == -1) {
                                   response.POSWSResponse.ErrNumber = "2100.3";
                                   response.POSWSResponse.Message = "Card decode error";
                                   response.POSWSResponse.Status = "Declined";
                                   response.POSWSResponse.UpdatePending = false;
                                   return response;
                              }
                         } catch {
                              response.POSWSResponse.ErrNumber = "2100.3";
                              response.POSWSResponse.Message = "Card decode error";
                              response.POSWSResponse.Status = "Declined";
                              response.POSWSResponse.UpdatePending = false;
                              return response;
                         }

                         #endregion Read Tracks

                         action = "logging mobile app action.";
                         mobileAppFunctions.LogMobileAppAction("Purchase Credit Transaction EMV", mobileApp.MobileAppId, account.AccountId, request.POSWSRequest.GPSLat, request.POSWSRequest.GPSLong);

                         if (!mobileApp.MobileAppFeatures.CreditTransaction) // boolean, check if true / enabled - lol, wrong names
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

                         //Check Device
                         if (!_deviceRepo.HasDeviceByMasterDeviceId(request.Device)) {
                              response.POSWSResponse.Status = "Declined";
                              response.POSWSResponse.Message = "No Device Available.";
                              response.POSWSResponse.ErrNumber = "2101.3";
                              response.POSWSResponse.UpdatePending = updatePending;
                              return response;
                         }

                         // TODO:
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
                         if (cardTypeId != 0) {
                              transaction.CardTypeId = cardTypeId;
                         } else {
                              response.POSWSResponse.Status = "Declined";
                              response.POSWSResponse.Message = "Invalid card type";
                              response.POSWSResponse.ErrNumber = "2101.2";
                              response.POSWSResponse.UpdatePending = updatePending;
                              response.TransactionNumber = "0";
                              return response;
                         }

                         transaction.CardNumber = cardNumber;
                         transaction.NameOnCard = request.CardDetails.NameOnCard;
                         transaction.ExpMonth = expMonth;
                         transaction.ExpYear = expYear;
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

                         try {
                              transaction.CurrencyId = _transRepo.GetCurrencyIdByCurrencyName(request.CardDetails.Currency);
                         } catch {
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
                         transaction.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.EMV);
                         transaction.MerchantPOSId = mobileApp.MerchantBranchPOSId;
                         transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale);
                         transactionAttempt.AccountId = account.AccountId;
                         transactionAttempt.MobileAppId = mobileApp.MobileAppId;
                         transactionAttempt.DeviceId = request.Device;
                         transactionAttempt.GPSLat = request.POSWSRequest.GPSLat;
                         //transactionAttempt.
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

                         try {
                              action = "checking mid status before setting up transaction entry.";
                              var mid = new SDGDAL.Entities.Mid();

                              mid = _midsRepo.GetMidByPosIdAndCardTypeId(transaction.MerchantPOSId, transaction.CardTypeId);

                              if (mid == null) {
                                   response.POSWSResponse.ErrNumber = "2101.4";
                                   response.POSWSResponse.Message = "Mid not found.";
                                   response.POSWSResponse.Status = "Declined";

                                   return response;
                              } else {
                                   if (!mid.IsActive || mid.IsDeleted) {
                                        response.POSWSResponse.ErrNumber = "2101.5";
                                        response.POSWSResponse.Message = "Mid is Inactive.";
                                        response.POSWSResponse.Status = "Declined";

                                        return response;
                                   }

                                   if (!mid.Switch.IsActive) {
                                        response.POSWSResponse.ErrNumber = "2101.6";
                                        response.POSWSResponse.Message = "Switch is Inactive.";
                                        response.POSWSResponse.Status = "Declined";

                                        return response;
                                   }
                              }

                              action = "trying to encrypt card data.";

                              #region Encrypt Card Data

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
                              if (mid.Switch.IsAddressRequired) {
                                   action = "saving temp transaction because switch requires address.";
                                   var tempTransaction = new SDGDAL.Entities.TempTransaction();

                                   tempTransaction.CopyProperties(transaction);

                                   var nTempTransaction = _transRepo.CreateTempTransaction(tempTransaction);

                                   if (nTempTransaction.TransactionId > 0) {
                                        response.POSWSResponse.Status = "Declined";
                                        response.POSWSResponse.Message = "This transaction require customer to enter their billing address.";
                                        response.POSWSResponse.ErrNumber = "2101.7";
                                        return response;
                                   } else {
                                        throw new Exception(action);
                                   }
                              }

                              transactionAttempt.TransactionChargesId = mid.TransactionChargesId;

                              action = "checking if switch is active. ";
                              if (!mid.Switch.IsActive) {
                                   transactionAttempt.DateSent = DateTime.Now;
                                   transactionAttempt.DateReceived = DateTime.Now;
                                   transactionAttempt.DepositDate = DateTime.Now;
                                   transactionAttempt.Notes = ((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId).ToString() + " Declined. Switch inactive.";
                                   transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                              } else {
                                   transactionAttempt.DateSent = DateTime.Now;
                                   transactionAttempt.DateReceived = Convert.ToDateTime("1900/01/01");
                                   transactionAttempt.DepositDate = Convert.ToDateTime("1900/01/01");
                                   transactionAttempt.TransNumber = SDGUtil.Functions.GenerateSystemTraceAudit();

                                   if (!mid.IsActive || mid.IsDeleted) {
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

                              var rTransaction = _transRepo.CreateTransaction(nTransaction, transactionAttempt);

                              if (rTransaction.TransactionId > 0) {
                                   action = "processing transaction for api integration. Transaction was successfully saved.";
                                   transaction.TransactionId = rTransaction.TransactionId;

                                   if (((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId) == Enums.TransactionType.Declined) {
                                        response.POSWSResponse.Message = "Transaction failed. Please contact Support.";
                                        response.POSWSResponse.ErrNumber = "2101.8";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                   } else {
                                        transactionAttemptId = transactionAttempt.TransactionAttemptId;

                                        #region Master Card Demo

                                        if (mid.Switch.SwitchCode == "MasterCardDemo") {
                                             if (transactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale)) {
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

                                                  try {
                                                       transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                                  } catch {
                                                       transData.Amount = finalAmount.ToString();
                                                  }

                                                  transData.CardNumber = transaction.CardNumber;
                                                  transData.CardExpirationDate = transaction.ExpYear.ToString() + transaction.ExpMonth.ToString();
                                                  transData.CSC = transaction.CSC;
                                                  transData.Currency = request.CardDetails.Currency;
                                                  transData.TransNumber = rTransaction.TransactionId + "-" + transactionAttempt.TransactionAttemptId;
                                                  transData.OrderInfo = transData.TransNumber;
                                                  transData.SecureHash = transData.SecureHash;
                                                  transData.MerchTxnRef = transaction.TransactionId + "-" + transactionAttempt.TransactionAttemptId;
                                                  transData.Track1 = transaction.Track1;
                                                  transData.Track2 = ";" + transaction.Track2.Replace('D', '=').Replace('F', '?');
                                                  transData.EmvIccData = emvDataResult.EmvIccData;

                                                  var apiResponse = gateway.ProcessMasterCardDemoEMVCardNotPresent(transData, apiLogin, "purchase", cardTypeId);

                                                  if (apiResponse.Status == "Approved") {
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
                                                       response.POSWSResponse.Message = "sTransaction Successful.";
                                                       response.POSWSResponse.Status = "Approved";
                                                  } else if (apiResponse.Status == "Declined") {
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
                                                  } else {
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
                                             } else {
                                                  string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                                  response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                                  response.POSWSResponse.Status = "Declined";
                                                  return response;
                                             }
                                        }

                                        #endregion Master Card Demo

                                        #region Master Card
                                else if (mid.Switch.SwitchCode == "MasterCard") {
                                             if (transactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale)) {
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

                                                  try {
                                                       transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                                  } catch {
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
                                                  transData.EmvIccData = emvDataResult.EmvIccData;

                                                  var apiResponse = gateway.ProcessMasterCard(transData, apiLogin, "purchase", cardTypeId);

                                                  action = "updating transaction attempt id:" + transactionAttempt.TransactionAttemptId + ". Api Response Status: " + apiResponse.Status + ". Api Response Message: " + apiResponse.Message + ". ";
                                                  if (apiResponse.Status == "Approved") {
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
                                                  } else if (apiResponse.Status == "Declined") {
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

                                                       ApplicationLog.LogCardFormatError("SDGWebService-Migs",
                                                                                             request.CardDetails.Track1,
                                                                                             request.CardDetails.Ksn,
                                                                                             request.CardDetails.NameOnCard,
                                                                                             expYear + expMonth,
                                                                                             track2,
                                                                                             apiResponse.AcqResponseCode + "-" + apiResponse.Message,
                                                                                             transData.MerchantId + "-" + transData.TerminalId,
                                                                                             transData.TransNumber);
                                                  } else {
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
                                             } else {
                                                  string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                                  response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                                  response.POSWSResponse.Status = "Declined";
                                                  return response;
                                             }
                                        }

                                        #endregion Master Card

                                        #region CTPayment
                                else if (mid.Switch.SwitchCode == "CTPayment") {
                                             if (transactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale)) {
                                                  action = "processing emv transaction.";

                                                  GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                                  GatewayProcessor.CTPaymentGateway.TransactionEMVData emvTransData = new GatewayProcessor.CTPaymentGateway.TransactionEMVData();

                                                  #region Header Request

                                                  string msgClass = "FO";
                                                  string operatorId = "   ";
                                                  string posEntryMode = "05";
                                                  string terminalId = mid.Param_6;
                                                  string transType = "000";
                                                  string msgVersion = "05";
                                                  string posStatIndicator = "000";
                                                  string seqNumber = _traceNumRepo.GenerateSystemTraceNumber();
                                                  string seqByte = "0";

                                                  HEADER_REQUEST header = new HEADER_REQUEST(msgClass, terminalId, operatorId, seqByte + seqNumber, transType, DateTime.Now, posEntryMode, msgVersion, posStatIndicator);

                                                  header.GetRequestHeader();

                                                  #endregion Header Request

                                                  #region Track2 Equivalent Data

                                                  TRACK2_EQUIVALENT_DATA track2EquivalentData = new TRACK2_EQUIVALENT_DATA(emvDataResult.TrackData[0].CardNumber, emvDataResult.TrackData[0].ExpiryDate, emvDataResult.TrackData[0].ServiceCode, emvDataResult.TrackData[0].DiscretionaryData);
                                                  EMV emv = new EMV(track2EquivalentData);
                                                  TRACK2_REQUEST tr = new TRACK2_REQUEST(emv);
                                                  emv.TED = track2EquivalentData;
                                                  tr.EMV = emv;

                                                  #endregion Track2 Equivalent Data

                                                  #region TRANS AMOUNT

                                                  decimal orgAmount = transactionAttempt.Amount;
                                                  decimal finalAmount = orgAmount * 100;

                                                  try {
                                                       emvTransData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                                  } catch {
                                                       emvTransData.Amount = finalAmount.ToString();
                                                  }

                                                  if (emvTransData.Amount.Length < 2) {
                                                       emvTransData.Amount = emvTransData.Amount.ToString().PadLeft(2, '0');
                                                  }

                                                  TRANS_AMOUNT_1 transAmount = new TRANS_AMOUNT_1(emvTransData.Amount, "");

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

                                                  var apiResponse = gateway.ProcessCTPaymentCreditTransaction(emvRequest.EMVRequest(), "purchase");

                                                  action = "updating transaction attempt id:" + transactionAttempt.TransactionAttemptId + ". Api Response Status: " + apiResponse.Status + ". Api Response Message: " + apiResponse.Result.Message + ". ";

                                                  if (apiResponse.Result.Status == "Approved") {
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
                                                       transactionAttempt.Notes = "API EMV CTPayment Purchase Approved";

                                                       response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)rTransaction.CardTypeId);
                                                       response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                                       response.POSWSResponse.ErrNumber = "0";
                                                       response.POSWSResponse.Message = "Transaction Successful.";
                                                       response.POSWSResponse.Status = "Approved";
                                                  } else if (apiResponse.Result.Status == "Declined") {
                                                       transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                                       transactionAttempt.AuthNumber = "";
                                                       transactionAttempt.ReturnCode = apiResponse.Result.AcqResponseCode;
                                                       transactionAttempt.SeqNumber = apiResponse.Result.ReceiptNumber;
                                                       transactionAttempt.TransNumber = apiResponse.Result.TransactionNumber;
                                                       transactionAttempt.DisplayReceipt = "";
                                                       transactionAttempt.DisplayTerminal = "";
                                                       transactionAttempt.PosEntryMode = apiResponse.Result.PosEntryMode;
                                                       transactionAttempt.DateReceived = DateTime.Now;
                                                       transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                                       transactionAttempt.Notes = "API EMV CTPayment Purchase Declined." + apiResponse.Result.Message;
                                                       transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                                       response.POSWSResponse.ErrNumber = apiResponse.Result.AcqResponseCode;
                                                       response.POSWSResponse.Message = apiResponse.Result.Message;
                                                       response.POSWSResponse.Status = "Declined";
                                                  } else {
                                                       response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Result.Message;
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
                                             } else {
                                                  string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                                  response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                                  response.POSWSResponse.Status = "Declined";
                                                  return response;
                                             }
                                        }

                                        #endregion CTPayment

                                        #region Offline Switch
                                   else if (mid.Switch.SwitchCode == "Offline") {
                                        transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                             transactionAttempt.AuthNumber = "VP-001";
                                             transactionAttempt.ReturnCode = "00";
                                             transactionAttempt.SeqNumber = "621814389248";
                                             transactionAttempt.TransNumber = "1100000648";
                                             transactionAttempt.BatchNumber = DateTime.Now.ToString("yyyyMMddhhmmss");
                                             transactionAttempt.DisplayReceipt = "123456987";
                                             transactionAttempt.DisplayTerminal = "0000005";
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

                                        #region MaxBank EMV

                                else if (mid.Switch.SwitchCode == "Maxbank") {
                                             if (transactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale)) {
                                                  action = "processing debit card.";

                                                  DE_ISO8583 de = new DE_ISO8583();
                                                  GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                                  GatewayProcessor.VeritasPayment.CardDetails transData = new GatewayProcessor.VeritasPayment.CardDetails();

                                                  transData.PrivateAdditionalData = Convert.ToString(request.CardDetails.EpbKsn.Length).PadLeft(4, '0') + request.CardDetails.EpbKsn + Convert.ToString(cardNumber.Length).PadLeft(4, '0') + cardNumber;
                                                  transData.MerchantID = mid.Param_2;
                                                  transData.TerminalID = mid.Param_6;

                                                  decimal orgAmount = transactionAttempt.Amount;
                                                  decimal finalAmount = orgAmount * 100;

                                                  try {
                                                       transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                                  } catch {
                                                       transData.Amount = finalAmount.ToString();
                                                  }

                                                  transData.RetrievalReferenceNumber = SDGUtil.Functions.GenerateRetrievalNumber(12);
                                                  transData.SystemTraceAudit = transactionAttempt.TransNumber;
                                                  transData.CardNumber = cardNumber;
                                                  transData.NameOnCard = request.CardDetails.NameOnCard;
                                                  transData.Track2Data = track2.Replace('D', '=').TrimStart(';').TrimEnd('F').TrimEnd('?');
                                                  transData.Track1Data = null;
                                                  transData.ExpirationDate = (transaction.ExpMonth == null || transaction.ExpYear == null) ? null : transaction.ExpYear + transaction.ExpMonth;
                                                  transData.PinBlock = null;
                                                  transData.CurrencyCode = mid.Currency.IsoCode;
                                                  transData.ChipCardData = emvDataResult.EmvIccData;

                                                  //Fees
                                                  transData.AmountTransactionFee = "00000000";
                                                  transData.MessageAuthorizationCode = "0000000000000000";

                                                  action = "processing transaction for debit api integration. Transaction was successfully saved.";

                                                  try {
                                                       var apiResponse = gateway.ProcessPurchaseMaxbankGateway(transData, "PURCHASE");

                                                       if (apiResponse.Result.Status == "Approved") {
                                                            transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                                            transactionAttempt.AuthNumber = apiResponse.Result.AuthorizationID;
                                                            transactionAttempt.ReturnCode = apiResponse.Result.ReturnCode;
                                                            transactionAttempt.TransNumber = apiResponse.Result.SytemTraceAudit;
                                                            transactionAttempt.Reference = apiResponse.Result.Reference;
                                                            transactionAttempt.DisplayReceipt = transData.MerchantID;
                                                            transactionAttempt.DisplayTerminal = transData.TerminalID;
                                                            transactionAttempt.BatchNumber = _batchRepo.GenerateBatchNumber(mobileApp.MobileAppId, Convert.ToInt32(SDGDAL.Enums.Ref_PaymentType.Credit));
                                                            transactionAttempt.DateReceived = DateTime.Now;
                                                            transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                                            transactionAttempt.Notes = "API Credit EMV Purchase Approved";
                                                            response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                                            response.POSWSResponse.ErrNumber = "0";
                                                            response.POSWSResponse.Message = "Transaction Successful.";
                                                            response.POSWSResponse.Status = "Approved";
                                                       } else if (apiResponse.Result.Status == "Declined") {
                                                            transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                                            transactionAttempt.AuthNumber = apiResponse.Result.AuthorizationID;
                                                            transactionAttempt.ReturnCode = apiResponse.Result.ReturnCode;
                                                            transactionAttempt.TransNumber = transData.SystemTraceAudit;
                                                            transactionAttempt.Reference = apiResponse.Result.Reference;
                                                            transactionAttempt.DateReceived = DateTime.Now;
                                                            transactionAttempt.DisplayReceipt = transData.MerchantID;
                                                            transactionAttempt.DisplayTerminal = transData.TerminalID;
                                                            transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                                            transactionAttempt.Notes += "API Credit EMV Purchase Declined" + apiResponse.Result.Message;
                                                            transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                                            response.POSWSResponse.ErrNumber = apiResponse.Result.ErrNumber;
                                                            response.POSWSResponse.Message = apiResponse.Result.Message;
                                                            response.POSWSResponse.Status = "Declined";
                                                       } else {
                                                            response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Result.Message;
                                                            response.POSWSResponse.ErrNumber = "2101.7";
                                                            response.POSWSResponse.Status = "Declined";
                                                            return response;
                                                       }
                                                  } catch (Exception ex) {
                                                       // Log error
                                                       var errorOnAction = "Error while " + action;
                                                       var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction, "TransactionPurchaseEMV", "", "");

                                                       response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + "Declined" + " " + errorOnAction + " " + ex.Message;
                                                       response.POSWSResponse.ErrNumber = "2000.10";
                                                       response.POSWSResponse.Status = "Declined";
                                                       return response;
                                                  }
                                             } else {
                                                  string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                                  response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                                  response.POSWSResponse.Status = "Declined";
                                                  return response;
                                             }
                                        }

                                        #endregion MaxBank EMV

                                        #region PayMaya
                                else if (mid.Switch.SwitchCode == "PayMaya") {
                                             if (transactionAttempt.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale)) {
                                                  action = "processing api paymaya";
                                                  GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                                  GatewayProcessor.VeritasPayment.CardDetails transData = new GatewayProcessor.VeritasPayment.CardDetails();

                                                  transData.PrivateAdditionalData = Convert.ToString(request.CardDetails.EpbKsn.Length).PadLeft(4, '0') + request.CardDetails.EpbKsn + Convert.ToString(cardNumber.Length).PadLeft(4, '0') + cardNumber;
                                                  transData.MerchantID = mid.Param_2;
                                                  transData.TerminalID = mid.Param_6;

                                                  decimal orgAmount = transactionAttempt.Amount;
                                                  decimal finalAmount = orgAmount * 100;

                                                  try {
                                                       transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                                  } catch {
                                                       transData.Amount = finalAmount.ToString();
                                                  }

                                                  transData.RetrievalReferenceNumber = SDGUtil.Functions.GenerateRetrievalNumber(12);
                                                  transData.SystemTraceAudit = transactionAttempt.TransNumber;
                                                  transData.CardNumber = cardNumber;
                                                  transData.NameOnCard = request.CardDetails.NameOnCard;
                                                  transData.Track2Data = track2.Replace('D', '=').TrimStart(';').TrimEnd('F').TrimEnd('?');
                                                  transData.Track1Data = null;
                                                  transData.ExpirationDate = (transaction.ExpMonth == null || transaction.ExpYear == null) ? null : transaction.ExpYear + transaction.ExpMonth;
                                                  transData.PinBlock = null;
                                                  transData.CurrencyCode = mid.Currency.IsoCode;
                                                  transData.ChipCardData = emvDataResult.EmvIccData;

                                                  //Fees
                                                  transData.AmountTransactionFee = "00000000";
                                                  transData.MessageAuthorizationCode = "0000000000000000";

                                                  action = "processing transaction for creedit api integration. Transaction was successfully saved.";

                                                  try {
                                                       var apiResponse = gateway.ProcessPayMayaGateway(transData, "PURCHASE");

                                                       if (apiResponse.Result.Status == "Approved") {
                                                            transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                                            transactionAttempt.AuthNumber = apiResponse.Result.AuthorizationID;
                                                            transactionAttempt.ReturnCode = apiResponse.Result.ReturnCode;
                                                            transactionAttempt.TransNumber = apiResponse.Result.SytemTraceAudit;
                                                            transactionAttempt.Reference = apiResponse.Result.Reference;
                                                            transactionAttempt.DisplayReceipt = transData.MerchantID;
                                                            transactionAttempt.DisplayTerminal = transData.TerminalID;
                                                            transactionAttempt.BatchNumber = _batchRepo.GenerateBatchNumber(mobileApp.MobileAppId, Convert.ToInt32(SDGDAL.Enums.Ref_PaymentType.Credit));
                                                            transactionAttempt.DateReceived = DateTime.Now;
                                                            transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                                            transactionAttempt.Notes = "API PayMaya EMV Purchase Approved";
                                                            response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                                            response.POSWSResponse.ErrNumber = "0";
                                                            response.POSWSResponse.Message = "Transaction Successful.";
                                                            response.POSWSResponse.Status = "Approved";
                                                       } else if (apiResponse.Result.Status == "Declined") {
                                                            transactionAttempt.TransactionAttemptId = transactionAttemptId;
                                                            transactionAttempt.AuthNumber = apiResponse.Result.AuthorizationID;
                                                            transactionAttempt.ReturnCode = apiResponse.Result.ReturnCode;
                                                            transactionAttempt.TransNumber = transData.SystemTraceAudit;
                                                            transactionAttempt.Reference = apiResponse.Result.Reference;
                                                            transactionAttempt.DateReceived = DateTime.Now;
                                                            transactionAttempt.DisplayReceipt = transData.MerchantID;
                                                            transactionAttempt.DisplayTerminal = transData.TerminalID;
                                                            transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                                            transactionAttempt.Notes = "API PayMaya EMV Purchase Declined" + apiResponse.Result.Message;
                                                            transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                                            response.POSWSResponse.ErrNumber = apiResponse.Result.ErrNumber;
                                                            response.POSWSResponse.Message = apiResponse.Result.Message;
                                                            response.POSWSResponse.Status = "Declined";
                                                       } else {
                                                            response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + apiResponse.Result.Message;
                                                            response.POSWSResponse.ErrNumber = "2101.7";
                                                            response.POSWSResponse.Status = "Declined";
                                                            return response;
                                                       }
                                                  } catch (Exception ex) {
                                                       // Log error
                                                       var errorOnAction = "Error while " + action;
                                                       var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction, "TransactionPurchaseEMV", "", "");

                                                       response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + "Declined" + " " + errorOnAction + " " + ex.Message;
                                                       response.POSWSResponse.ErrNumber = "2000.10";
                                                       response.POSWSResponse.Status = "Declined";
                                                       return response;
                                                  }
                                             } else {
                                                  string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                                  response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                                  response.POSWSResponse.Status = "Declined";
                                                  return response;
                                             }
                                        }

                                        #endregion PayMaya

                                else {
                                             transactionAttempt.DateReceived = DateTime.Now;
                                             var nTransactionAttemptOffline = _transRepo.UpdateTransactionAttempt(transactionAttempt);
                                             response.POSWSResponse.Message = "Transaction failed. Switch not yet supported. Please contact Support.";
                                             response.POSWSResponse.ErrNumber = "2101.9";
                                             response.POSWSResponse.Status = "Declined";
                                             return response;
                                        }

                                        action = "updating void transaction";

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
                                   }
                              } else {
                                   throw new Exception(action);
                              }
                         } catch (Exception ex) {
                              // Log error please
                              var errorOnAction = "Error while " + action;

                              var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "TransationPurchaseCreditEMV", ex.StackTrace);

                              response.POSWSResponse.ErrNumber = "2101.9";
                              response.POSWSResponse.Message = "Unknown error, please contact support. Ref: " + errRefNumber;
                              response.POSWSResponse.Status = "Declined";
                         }

                         return response;

                         #endregion Handle Transaction response

                         #endregion Transaction
                    } else if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED")) {
                        var transaction = new SDGDAL.Entities.Transaction();
                        var transactionAttempt = new SDGDAL.Entities.TransactionAttempt();
                        var nTransaction = new SDGDAL.Entities.Transaction();
                        nTransaction.CopyProperties(transaction);
                        transactionAttemptId = transactionAttempt.TransactionAttemptId;

                        transactionAttempt.AuthNumber = "XP-001";
                        transactionAttempt.ReturnCode = "00";
                        transactionAttempt.SeqNumber = "621814389248";
                        transactionAttempt.TransNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                        transactionAttempt.BatchNumber = DateTime.Now.ToString("yyyyMMddhhmmss");
                        transactionAttempt.DisplayReceipt = "123456987";
                        transactionAttempt.DisplayTerminal = "0000005";
                        transactionAttempt.DateReceived = DateTime.Now;
                        transactionAttempt.PosEntryMode = 05;
                        transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                        transactionAttempt.Notes = "API Offline Purchase Approved";

                        response.POSWSResponse.Status = "Approved";
                         response.POSWSResponse.Message = "X Approved";
                         response.POSWSResponse.ErrNumber = "0";
                         response.POSWSResponse.UpdatePending = true;

                         response.AuthNumber = "X"+DateTime.Now.ToString("yyyyMMddhhmmss"); 
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
                                       
                        response.CardType = SDGUtil.Functions.ConvertCardTypeName(SDGUtil.Functions.GetCardType(request.CardDetails.CardNumber)).ToString();
                        response.TraceNumber = SDGUtil.Functions.GenerateSystemTraceAudit();
                        response.TransNumber = transactionAttempt.TransNumber;
                        response.TransactionEntryType = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.EMV).ToString();
                        response.TransactionType = "3"; 
                        response.BatchNumber = transactionAttempt.BatchNumber;
                        response.Currency = request.CardDetails.Currency;
                    



                } else {
                         response.POSWSResponse.ErrNumber = "2101.10";
                         response.POSWSResponse.Status = "Declined";
                         response.POSWSResponse.Message = "Invalid System Mode";
                    }
               } catch (Exception ex) {
                    // Log error please
                    string req = request.Device + "-" + request.DeviceId + "-" + action + response.POSWSResponse.ErrNumber + "-" + request.CardDetails.EmvICCData + "-" + request.CardDetails.Ksn;
                    var errorOnAction = "Error while " + action;
                    var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "TransactionPurchaseCreditSwipe", ex.StackTrace);

                    if (ex.InnerException != null) {
                         ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message, "TransactionPurchaseCreditSwipe  ErrRef:" + errRefNumber, ex.InnerException.StackTrace);
                    }

                    ApplicationLog.LogTransactionEMV("SDGWebService", request.Action.ToString(), req, ex.Message);

                    response.POSWSResponse.ErrNumber = "2101.11";
                    response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber + " " + action;
                    response.POSWSResponse.Status = "DECLINED";
               }

               return response;

               #endregion
          }

          private POSWSResponse GetCardDetails(CardDetails cd, SDGDAL.Enums.SwipeDevice swipeDevice, SDGDAL.Enums.CardAction cardAction,
              out string track1, out string track2, out string cardNumber, out string nameOnCard,
              out string expDate, out string expYear, out string expMonth, out ClassTLV emvDataResult) {
               POSWSResponse response = new POSWSResponse();
               WisepadLayer.WisePad wisepad = new WisepadLayer.WisePad();
               RamblerLayer1.Class1 rambler1 = new RamblerLayer1.Class1();
               MobileAppMethods mobileAppMethods = new MobileAppMethods();
               TLVParser tlvParser = new TLVParser();
               emvDataResult = new ClassTLV();
               track1 = "";
               track2 = "";
               cardNumber = "";
               nameOnCard = "";
               expDate = "";
               expYear = "";
               expMonth = "";
               string bdk = System.Configuration.ConfigurationManager.AppSettings["BDK"].ToString();
               switch (swipeDevice) {
                    #region WisePad 1.0 DeviceId = 3
                    case SDGDAL.Enums.SwipeDevice.WisePad1:
                    string wisepadKey = System.Configuration.ConfigurationManager.AppSettings["DKWisepad1"].ToString();
                    string wisepadKeyTlvEmv = System.Configuration.ConfigurationManager.AppSettings["DKWisepad1Emv"].ToString();

                    if (cardAction == Enums.CardAction.EMV) {
                         string decryptEmvIcc = wisepad.DecryptC5EmvData(cd.EmvICCData, wisepadKeyTlvEmv);

                         var resultTlv = TLVParser.DecodeTLV(decryptEmvIcc);
                         emvDataResult.EmvIccData = resultTlv.EmvIccData.ToUpper();

                         emvDataResult.SubField1Data = resultTlv.SubField1Data;
                         emvDataResult.SubField2Data = resultTlv.SubField2Data;
                         emvDataResult.TrackData = resultTlv.TrackData;
                         track2 = resultTlv.Track2;
                         cardNumber = resultTlv.CardNumber;
                         expYear = resultTlv.ExpiryYear;
                         expYear = resultTlv.ExpiryMonth;
                    } else {
                         response.ErrNumber = "2100.1";
                         response.Message = "Invalid Swipe device";
                         response.Status = "Declined";
                         return response;
                    }

                    if (string.IsNullOrEmpty(track2)) {
                         response.ErrNumber = "2100.2";
                         response.Message = "No track data found on the card.";
                         response.Status = "Declined";
                         return response;
                    }

                    break;
                    #endregion

                    #region WisePad 2.0 DeviceId = 4
                    case SDGDAL.Enums.SwipeDevice.WisePad2:
                    string key = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);

                    if (cardAction == Enums.CardAction.EMV) {
                         string decryptedTlv = WisepadLayer.TripleDES.decrypt_CBC(cd.EmvICCData, key);

                         var resultTlv = TLVParser.DecodeTLV(decryptedTlv);
                         emvDataResult.EmvIccData = decryptedTlv;
                         emvDataResult.SubField1Data = resultTlv.SubField1Data;
                         emvDataResult.SubField2Data = resultTlv.SubField2Data;
                         emvDataResult.TrackData = resultTlv.TrackData;
                         track2 = resultTlv.Track2;
                         cardNumber = resultTlv.CardNumber;
                         expYear = resultTlv.ExpiryYear;
                         expMonth = resultTlv.ExpiryMonth;
                    } else {
                         response.ErrNumber = "2100.1";
                         response.Message = "Invalid Swipe device";
                         response.Status = "Declined";
                         return response;
                    }

                    if (string.IsNullOrEmpty(track2)) {
                         response.ErrNumber = "2100.2";
                         response.Message = "No track data found on the card.";
                         response.Status = "Declined";
                         return response;
                    }

                    break;
                    #endregion

                    #region Chipper Audio 1.0 DeviceId = 5
                    case SDGDAL.Enums.SwipeDevice.ChipperAudioI:
                    string keyChipper = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);

                    if (cardAction == Enums.CardAction.EMV) {
                         string decryptedTlv = WisepadLayer.TripleDES.decrypt_CBC(cd.EmvICCData, keyChipper);

                         var resultTlv = TLVParser.DecodeTLV(decryptedTlv);
                         emvDataResult.EmvIccData = resultTlv.EmvIccData;
                         emvDataResult.SubField1Data = resultTlv.SubField1Data;
                         emvDataResult.SubField2Data = resultTlv.SubField2Data;
                         emvDataResult.TrackData = resultTlv.TrackData;
                         track2 = resultTlv.Track2;
                         cardNumber = resultTlv.CardNumber;
                         expYear = resultTlv.ExpiryYear;
                         expMonth = resultTlv.ExpiryMonth;
                    } else {
                         response.ErrNumber = "2100.1";
                         response.Message = "Invalid Swipe device";
                         response.Status = "Declined";
                         return response;
                    }

                    if (string.IsNullOrEmpty(track2)) {
                         response.ErrNumber = "2100.2";
                         response.Message = "No track data found on the card.";
                         response.Status = "Declined";
                         return response;
                    }

                    break;
                    #endregion

                    #region Chipper Audio 2.0 DeviceId = 6
                    case SDGDAL.Enums.SwipeDevice.ChipperAudioII:
                    string dataKey = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);

                    if (cardAction == Enums.CardAction.EMV) {
                         string decryptedTlv = WisepadLayer.TripleDES.decrypt_CBC(cd.EmvICCData, dataKey);

                         var resultTlv = TLVParser.DecodeTLV(decryptedTlv);
                         emvDataResult.EmvIccData = resultTlv.EmvIccData;
                         emvDataResult.SubField1Data = resultTlv.SubField1Data;
                         emvDataResult.SubField2Data = resultTlv.SubField2Data;
                         emvDataResult.TrackData = resultTlv.TrackData;
                         track2 = resultTlv.Track2;
                         cardNumber = resultTlv.CardNumber;
                         expYear = resultTlv.ExpiryYear;
                         expMonth = resultTlv.ExpiryMonth;
                    } else {
                         response.ErrNumber = "2100.1";
                         response.Message = "Invalid Swipe device";
                         response.Status = "Declined";
                         return response;
                    }

                    if (string.IsNullOrEmpty(track2)) {
                         response.ErrNumber = "2100.2";
                         response.Message = "No track data found on the card.";
                         response.Status = "Declined";
                         return response;
                    }

                    break;
                    #endregion

                    #region WisePos DeviceId = 7
                    case SDGDAL.Enums.SwipeDevice.WisePos:
                    string dataKeyWisepos = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);

                    if (cardAction == Enums.CardAction.EMV) {
                         string decryptedTlv = WisepadLayer.TripleDES.decrypt_CBC(cd.EmvICCData, dataKeyWisepos);

                         var resultTlv = TLVParser.DecodeTLV(decryptedTlv);
                         emvDataResult.EmvIccData = resultTlv.EmvIccData;
                         emvDataResult.SubField1Data = resultTlv.SubField1Data;
                         emvDataResult.SubField2Data = resultTlv.SubField2Data;
                         emvDataResult.TrackData = resultTlv.TrackData;
                         track2 = resultTlv.Track2;
                         cardNumber = resultTlv.CardNumber;
                         expYear = resultTlv.ExpiryYear;
                         expMonth = resultTlv.ExpiryMonth;
                    } else {
                         response.ErrNumber = "2100.1";
                         response.Message = "Invalid Swipe device";
                         response.Status = "Declined";
                         return response;
                    }

                    if (string.IsNullOrEmpty(track2)) {
                         response.ErrNumber = "2100.2";
                         response.Message = "No track data found on the card.";
                         response.Status = "Declined";
                         return response;
                    }

                    break;
                    #endregion

                    #region WisePad Plus DeviceId = 8
                    case SDGDAL.Enums.SwipeDevice.WisePadPlus:
                    string dataKeyWisepadPlus = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);

                    if (cardAction == Enums.CardAction.EMV) {
                         string decryptedTlv = WisepadLayer.TripleDES.decrypt_CBC(cd.EmvICCData, dataKeyWisepadPlus);

                         var resultTlv = TLVParser.DecodeTLV(decryptedTlv);
                         emvDataResult.EmvIccData = resultTlv.EmvIccData;
                         emvDataResult.SubField1Data = resultTlv.SubField1Data;
                         emvDataResult.SubField2Data = resultTlv.SubField2Data;
                         emvDataResult.TrackData = resultTlv.TrackData;
                         track2 = resultTlv.Track2;
                         cardNumber = resultTlv.CardNumber;
                         expYear = resultTlv.ExpiryYear;
                         expMonth = resultTlv.ExpiryMonth;
                    } else {
                         response.ErrNumber = "2100.1";
                         response.Message = "Invalid Swipe device";
                         response.Status = "Declined";
                         return response;
                    }

                    if (string.IsNullOrEmpty(track2)) {
                         response.ErrNumber = "2100.2";
                         response.Message = "No track data found on the card.";
                         response.Status = "Declined";
                         return response;
                    }

                    break;
                    #endregion

                    #region Chipper BT 1.0 DeviceId = 9
                    case SDGDAL.Enums.SwipeDevice.ChipperBT1:
                    string dataKeyBT = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);

                    if (cardAction == Enums.CardAction.EMV) {
                         string decryptedTlv = WisepadLayer.TripleDES.decrypt_CBC(cd.EmvICCData, dataKeyBT);

                         var resultTlv = TLVParser.DecodeTLV(decryptedTlv);
                         emvDataResult.EmvIccData = resultTlv.EmvIccData;
                         emvDataResult.SubField1Data = resultTlv.SubField1Data;
                         emvDataResult.SubField2Data = resultTlv.SubField2Data;
                         emvDataResult.TrackData = resultTlv.TrackData;
                         track2 = resultTlv.Track2;
                         cardNumber = resultTlv.CardNumber;
                         expYear = resultTlv.ExpiryYear;
                         expMonth = resultTlv.ExpiryMonth;
                    } else {
                         response.ErrNumber = "2100.1";
                         response.Message = "Invalid Swipe device";
                         response.Status = "Declined";
                         return response;
                    }

                    if (string.IsNullOrEmpty(track2)) {
                         response.ErrNumber = "2100.2";
                         response.Message = "No track data found on the card.";
                         response.Status = "Declined";
                         return response;
                    }

                    break;
                    #endregion

                    #region Chipper BT 2.0 DeviceId = 10
                    case SDGDAL.Enums.SwipeDevice.ChipperBT2:
                    string dataKeyBT2 = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);

                    if (cardAction == Enums.CardAction.EMV) {
                         string decryptedTlv = WisepadLayer.TripleDES.decrypt_CBC(cd.EmvICCData, dataKeyBT2);

                         var resultTlv = TLVParser.DecodeTLV(decryptedTlv);
                         emvDataResult.EmvIccData = resultTlv.EmvIccData;
                         emvDataResult.SubField1Data = resultTlv.SubField1Data;
                         emvDataResult.SubField2Data = resultTlv.SubField2Data;
                         emvDataResult.TrackData = resultTlv.TrackData;
                         track2 = resultTlv.Track2;
                         cardNumber = resultTlv.CardNumber;
                         expYear = resultTlv.ExpiryYear;
                         expMonth = resultTlv.ExpiryMonth;
                    } else {
                         response.ErrNumber = "2100.1";
                         response.Message = "Invalid Swipe device";
                         response.Status = "Declined";
                         return response;
                    }

                    if (string.IsNullOrEmpty(track2)) {
                         response.ErrNumber = "2100.2";
                         response.Message = "No track data found on the card.";
                         response.Status = "Declined";
                         return response;
                    }

                    break;
                    #endregion

                    #region PosMini DeviceId = 11
                    case SDGDAL.Enums.SwipeDevice.QPosMini:
                    string iPek = ConfigurationManager.AppSettings["IPEK"].ToString();
                    string keyIpek = DSPREADQPosLayer.EMVSwipeTLV.DUKPTServer.GetDataKeyFromIPEK(cd.Ksn, iPek);

                    if (cardAction == Enums.CardAction.EMV) {
                         string decryptedTlv = DSPREADQPosLayer.EMVSwipeTLV.TripleDES.decrypt_CBC(cd.EmvICCData, keyIpek);

                         var resultTlv = TLVParser.DecodeTLV(decryptedTlv);
                         emvDataResult.EmvIccData = decryptedTlv;
                         emvDataResult.SubField1Data = resultTlv.SubField1Data;
                         emvDataResult.SubField2Data = resultTlv.SubField2Data;
                         emvDataResult.TrackData = resultTlv.TrackData;
                         track2 = resultTlv.Track2;
                         cardNumber = resultTlv.CardNumber;
                         expYear = resultTlv.ExpiryYear;
                         expMonth = resultTlv.ExpiryMonth;
                    } else {
                         response.ErrNumber = "2100.1";
                         response.Message = "Invalid Swipe device";
                         response.Status = "Declined";
                         return response;
                    }

                    if (string.IsNullOrEmpty(track2)) {
                         response.ErrNumber = "2100.2";
                         response.Message = "No track data found on the card.";
                         response.Status = "Declined";
                         return response;
                    }

                    break;
                    #endregion

                    default:
                    response.ErrNumber = "2100.1";
                    response.Message = "Invalid Device";
                    response.Status = "Declined";
                    return response;
               }

               return response;
          }
     }
}
using SDGUtil;
using SDGDAL.Repositories;
using SDGWebService.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDGDAL;
using System.Configuration;
using System.Collections;

namespace SDGWebService.WebserviceFunctions
{
    public class WebserviceFunctionsCreditManual
    {
        private Functions.MobileAppFunctions mobileAppFunctions = new Functions.MobileAppFunctions();
        private MidsRepository _midsRepo = new MidsRepository();
        private TransactionRepository _transRepo = new TransactionRepository();
        private ReferenceRepository _refRepo = new ReferenceRepository();
        private MobileAppRepository _mAppRepo = new MobileAppRepository();
        private UserRepository _userRepo = new UserRepository();

        public PurchaseResponse TransactionPurchaseCreditManual(CreditManualRequest request)
        {
            string action = string.Empty;
            PurchaseResponse response = new PurchaseResponse();
            response.POSWSResponse = new POSWSResponse();

            object obj = response.POSWSResponse;
            if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.TRANSACTION_PURCHASE_MANUAL, (object)request, out obj))
            {
                response.POSWSResponse = (POSWSResponse)obj;
                return response;
            }

            response.POSWSResponse.ErrNumber = "0";

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    var mobileApp = new SDGDAL.Entities.MobileApp();
                    var account = new SDGDAL.Entities.Account();
                    var card = new CreditCardDetails();

                    action = "checking mobile app availability.";

                    if(!String.IsNullOrEmpty(request.POSWSRequest.RToken))
                    {
                        response.POSWSResponse = mobileAppFunctions.CheckStatus(request.POSWSRequest.RToken, out mobileApp, out account);

                        if (response.POSWSResponse.Status == "Declined")
                        {
                            return response;
                        }
                    }
                    else
                    {
                        action = "retrieving mobileApp Details using Activation Code";
                        mobileApp = _mAppRepo.GetMobileAppFullDetailsByActivationCode(request.POSWSRequest.ActivationKey);

                        if (mobileApp == null)
                        {
                            response.POSWSResponse.ErrNumber = "2001.1";
                            response.POSWSResponse.Message = "No record found.";
                            response.POSWSResponse.Status = "Declined";
                            return response;
                        }
                        else
                        {
                            if (mobileApp.IsDeleted)
                            {
                                response.POSWSResponse.ErrNumber = "2001.1";
                                response.POSWSResponse.Message = "No record found.";
                                response.POSWSResponse.Status = "Declined";
                                return response;
                            }

                            if (!mobileApp.IsActive)
                            {
                                response.POSWSResponse.ErrNumber = "2001.2";
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
                    
                    action = "logging mobile app action.";
                    MobileAppMethods mobileAppMethods = new MobileAppMethods();
                    mobileAppFunctions.LogMobileAppAction("Purchase Credit Manual Transaction", mobileApp.MobileAppId, account.AccountId, request.POSWSRequest.GPSLat, request.POSWSRequest.GPSLong);

                    if (!mobileApp.MobileAppFeatures.CreditTransaction)
                    {
                        response.POSWSResponse.ErrNumber = "2900.1";
                        response.POSWSResponse.Message = "Credit transactions are currently disabled on this device";
                        response.POSWSResponse.Status = "Declined";
                        return response;
                    }

                    #region Get Country Name using country Code
                    var countries = _refRepo.GetCountryNameByCode(request.CustomerInfo.Country);

                    if(countries == null)
                    {
                        response.POSWSResponse.ErrNumber = "2900.2";
                        response.POSWSResponse.Message = "Invalid Country";
                        response.POSWSResponse.Status = "Declined";
                        return response;
                    }
                    #endregion

                    #region Decrypt and Parse Credit card Data
                    try
                    {
                        action = "parse credit card data";

                        var cardDetails = Functions.CryptorEngine.Decrypt(request.EncCardDetails);

                        card.CardNumber = cardDetails.CardNumber;
                        card.ExpiryMonth = cardDetails.ExpiryMonth;
                        card.ExpiryYear = cardDetails.ExpiryYear;
                        card.CVV = cardDetails.CVV;
                    }
                    catch(Exception ex)
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "Error occurs while processing the encrypted data.";
                        response.POSWSResponse.ErrNumber = "2900.5";
                        response.TransactionNumber = "0";
                        return response;
                    }
                    #endregion

                    action = "getting transaction  and transactionattempt credit details.";
                    var transaction = new SDGDAL.Entities.Transaction();
                    var transactionAttempt = new SDGDAL.Entities.TransactionAttempt();

                    #region checking card type id
                    action = "get cardtype and do simple card validate";

                    int cardTypeId = SDGUtil.Functions.GetCardType(card.CardNumber);
                    if (cardTypeId != 0)
                    {
                        transaction.CardTypeId = cardTypeId;
                    }
                    else
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "Invalid card type";
                        response.POSWSResponse.ErrNumber = "2900.5";
                        response.TransactionNumber = "0";
                        return response;
                    }
                    #endregion

                    #region checking mid status
                    action = "checking mid status before setting up transaction entry.";
                    var mid = new SDGDAL.Entities.Mid();

                    mid = _midsRepo.GetMidByPosIdAndCardTypeId(mobileApp.MerchantBranchPOSId, transaction.CardTypeId); 

                    if (mid == null)
                    {
                        response.POSWSResponse.ErrNumber = "2900.2";
                        response.POSWSResponse.Message = "Mid not found.";
                        response.POSWSResponse.Status = "Declined";

                        return response;
                    }
                    else
                    {
                        if (!mid.IsActive || mid.IsDeleted)
                        {
                            response.POSWSResponse.ErrNumber = "2900.3";
                            response.POSWSResponse.Message = "Mid is Inactive.";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }

                        if (!mid.Switch.IsActive)
                        {
                            response.POSWSResponse.ErrNumber = "2900.4";
                            response.POSWSResponse.Message = "Switch is Inactive.";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }
                    }
                    #endregion

                    #region Encrypt Card Data

                    action = "trying to encrypt card data.";
                    //ENCRYPT CARD DATA
                    string NE_CARD = card.CardNumber;
                    string NE_EMONTH = card.ExpiryMonth;
                    string NE_EYEAR = card.ExpiryYear;
                    string NE_CSC = card.CVV;
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

                    try
                    {
                        transaction.CurrencyId = _transRepo.GetCurrencyIdByCurrencyName(request.Currency);
                    }
                    catch
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "Invalid currency.";
                        response.POSWSResponse.ErrNumber = "2900.6";
                        response.TransactionNumber = "0";
                        return response;
                    }

                    transaction.MerchantPOSId = mobileApp.MerchantBranchPOSId;
                    transaction.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.CreditManual);
                    transaction.Notes = string.Empty;
                    transaction.RefNumSales = request.RefNumberSale;
                    transaction.RefNumApp = request.RefNumberApp;
                    transaction.OriginalAmount = request.Amount;
                    transaction.TaxAmount = 0;
                    transaction.MidId = mid.MidId;
                    transaction.FinalAmount = request.Amount;
                    transaction.DateCreated = DateTime.Now;
                    transaction.CardNumber = E_CARD;
                    transaction.ExpMonth = null;
                    transaction.ExpYear = null;
                    transaction.CSC = null;

                    transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale);
                    transactionAttempt.AccountId = account.AccountId;
                    transactionAttempt.MobileAppId = mobileApp.MobileAppId;
                    transactionAttempt.GPSLat = request.POSWSRequest.GPSLat;
                    transactionAttempt.GPSLong = request.POSWSRequest.GPSLong;
                    transactionAttempt.Amount = transaction.OriginalAmount;
                    transactionAttempt.TransactionChargesId = mid.TransactionChargesId;
                    transactionAttempt.Notes = string.Empty;
                    transactionAttempt.DateSent = DateTime.Now;
                    transactionAttempt.DateReceived = Convert.ToDateTime("1900/01/01");
                    transactionAttempt.DepositDate = Convert.ToDateTime("1900/01/01");
                    transactionAttempt.DeviceId = 1;

                    action = "saving transaction details to database.";
                    var nTransaction = new SDGDAL.Entities.Transaction();
                    nTransaction.CopyProperties(transaction);

                    var savedTransaction = _transRepo.CreateTransaction(nTransaction, transactionAttempt);

                    if (savedTransaction.TransactionId > 0)
                    {
                        action = "processing transaction for api integration. Transaction was successfully saved.";

                        #region Offline Switch
                        if (mid.Switch.SwitchCode == "Offline")
                        {
                            transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                            transactionAttempt.AuthNumber = DateTime.Now.ToString("hhmmss");
                            transactionAttempt.ReturnCode = "00";
                            transactionAttempt.SeqNumber = DateTime.Now.ToString("yyyyMMdd");
                            transactionAttempt.TransNumber = DateTime.Now.ToString("yyyyMMddhhmmss");
                            transactionAttempt.BatchNumber = DateTime.Now.ToString("yyyyMMdd");
                            transactionAttempt.DisplayReceipt = "";
                            transactionAttempt.DisplayTerminal = "";
                            transactionAttempt.DateReceived = DateTime.Now;
                            transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                            transactionAttempt.Notes = " API Offline Demo Purchase Approved";

                            response.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)transaction.TransactionEntryTypeId);
                            response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                            response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                            response.POSWSResponse.ErrNumber = "0";
                            response.POSWSResponse.Message = "Transaction Successful.";
                            response.POSWSResponse.Status = "Approved";
                        }
                        #endregion

                        #region ISignThis

                        else if (mid.Switch.SwitchCode == "IsignThis")
                        {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.IsignThis.auth_request authrequest = new GatewayProcessor.IsignThis.auth_request();
                            GatewayProcessor.IsignThis.cardholder cardinfo = new GatewayProcessor.IsignThis.cardholder();
                            GatewayProcessor.IsignThis.client client = new GatewayProcessor.IsignThis.client();
                            GatewayProcessor.IsignThis.merchant merchant = new GatewayProcessor.IsignThis.merchant();
                            GatewayProcessor.IsignThis.transactions transactionData = new GatewayProcessor.IsignThis.transactions();

                            string ip = SDGUtil.Functions.GetIPAddress();
                            client.ip = ip;

                            cardinfo.pan = card.CardNumber;
                            cardinfo.name = request.NameOnCard;
                            cardinfo.expiration_date = card.ExpiryMonth + card.ExpiryYear;
                            cardinfo.cvv = card.CVV;

                            merchant.id = "10001";
                            merchant.terminal_id = "0000001";
                            merchant.name = mid.Merchant.MerchantName;

                            transactionData.bank_id = "4365294759";
                            transactionData.amount = Convert.ToString(transaction.OriginalAmount);
                            transactionData.datetime = Convert.ToString(DateTime.Now);
                            transactionData.id = transactionAttempt.TransactionId + "-" + transactionAttempt.TransactionAttemptId;
                            transactionData.currency = request.Currency;
                            transactionData.reference = transaction.TransactionId + "-" + transactionAttempt.TransactionAttemptId;

                            authrequest.acquirer_id = "12345";
                            authrequest.client = client;
                            authrequest.merchant = merchant;
                            authrequest.transaction = transactionData;
                            authrequest.cardholder = cardinfo;

                            var apiResponse = gateway.ProcessEcomTransactionKYC(authrequest);
                        }

                        #endregion

                        #region GlobalOnePay
                        else if (mid.Switch.SwitchCode == "GlobalOnePay")
                        {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.GlobalOnePay.TransactionData transData = new GatewayProcessor.GlobalOnePay.TransactionData();
     
                            transData.Amount = Convert.ToString(transaction.OriginalAmount);
                            transData.CardExpiry = card.ExpiryMonth + card.ExpiryYear;
                            transData.CardHolderName = request.NameOnCard;
                            transData.CardNumber = card.CardNumber;
                            transData.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId).ToUpper();
                            transData.Currency = request.Currency;
                            transData.Cvv = card.CVV;
                            transData.Hash = mid.Param_3;
                            transData.OrderId = Convert.ToString(transactionAttempt.TransactionId + "-" + transactionAttempt.TransactionAttemptId);
                            transData.TerminalId = mid.Param_6;

                            var apiResponse = gateway.ProcessGlobalOnePayTransaction(transData, "purchase");

                            if(apiResponse.Status == "Approved")
                            {
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = DateTime.Now.ToString("yyyyMMddhhmmss"); //apiResponse.ApprovalCode
                                transactionAttempt.ReturnCode = apiResponse.ResponseCode;
                                transactionAttempt.SeqNumber = apiResponse.UniqueReference;
                                transactionAttempt.TransNumber = apiResponse.UniqueReference;
                                transactionAttempt.PosEntryMode = 0;

                                transactionAttempt.BatchNumber = DateTime.Now.ToString("yyyyMMdd");
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = transData.TerminalId;
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = " API Global One Pay Purchase Approved";

                                response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                                response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                response.POSWSResponse.ErrNumber = "0";
                                response.POSWSResponse.Message = "Transaction Successful.";
                                response.POSWSResponse.Status = "Approved";
                            }
                            else if(apiResponse.Status == "Declined")
                            {
                                transactionAttempt.PosEntryMode = 0;
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = "";
                                transactionAttempt.ReturnCode = apiResponse.ResponseCode;
                                transactionAttempt.SeqNumber = null;
                                transactionAttempt.TransNumber = null;
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = " API Global One Pay Purchase Declined." + apiResponse.Message;
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                response.POSWSResponse.ErrNumber = apiResponse.ResponseCode;
                                response.POSWSResponse.Message = apiResponse.Message;
                                response.POSWSResponse.Status = "Declined";
                            }
                            else
                            {
                                response.POSWSResponse.ErrNumber = "2900.8";
                                response.POSWSResponse.Message = apiResponse.Status;
                                response.POSWSResponse.Status = apiResponse.Status;
                            }
                        }
                        #endregion

                        #region PayEco
                        else if (mid.Switch.SwitchCode == "PayEco")
                        {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.Payeco.TransactionData trans = new GatewayProcessor.Payeco.TransactionData();
                            GatewayProcessor.Payeco.RequestXML requestXML = new GatewayProcessor.Payeco.RequestXML();

                            string ProcCode = "0200";
                            string merchantPwd = mid.Param_3;
                            string bankCardNo = "";
                            string processCode = "";
                            string RespCode = "";
                            string terminalNo = mid.Param_6;
                            string merchantno = mid.Param_2;
                            string MerchantOrderNo = DateTime.Now.ToString("yyyyMMddHHmmss");
                            string OrderState = string.Empty;

                            string src = "";

                            src = ProcCode
                                + SDGUtil.Functions.getString(bankCardNo)
                                + SDGUtil.Functions.getString(processCode)
                                + SDGUtil.Functions.getString(request.Amount.ToString())
                                + SDGUtil.Functions.getString(DateTime.Now.ToString("yyyyMMddHHmmss"))
                                + SDGUtil.Functions.getString(DateTime.Now.ToString("HHmmss"))
                                + SDGUtil.Functions.getString("")
                                + SDGUtil.Functions.getString("")
                                + SDGUtil.Functions.getString("REFERENCE")
                                + SDGUtil.Functions.getString(RespCode)
                                + SDGUtil.Functions.getString(terminalNo)
                                + SDGUtil.Functions.getString(merchantno)
                                + SDGUtil.Functions.getString(MerchantOrderNo)
                                + SDGUtil.Functions.getString(OrderState);

                            string macSrc = (src + " " + merchantPwd).ToUpper();

                            string MAC = SDGUtil.Functions.ComputeMD5Hash(macSrc);

                            //string srcXML = requestXML.
                        }
                        #endregion

                        #region Worldnet Pay
                        else if (mid.Switch.SwitchCode == "Worldnet")
                        {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.Worldnet.TransactionData transData = new GatewayProcessor.Worldnet.TransactionData();

                            transData.Amount = Convert.ToString(transaction.OriginalAmount);
                            transData.CardExpiry = card.ExpiryMonth + card.ExpiryYear;
                            transData.CardHolderName = request.NameOnCard;
                            transData.CardNumber = card.CardNumber;
                            transData.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId).ToUpper();
                            transData.Currency = request.Currency;
                            transData.Cvv = card.CVV;
                            transData.Hash = mid.Param_3;
                            transData.OrderId = Convert.ToString(transactionAttempt.TransactionId + "-" + transactionAttempt.TransactionAttemptId);
                            transData.TerminalId = mid.Param_6;

                            var apiResponse = gateway.ProcessWorldnetTransaction(transData, "purchase");

                            if (apiResponse.Status == "Approved")
                            {
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = DateTime.Now.ToString("yyyyMMddhhmmss");
                                transactionAttempt.ReturnCode = apiResponse.ResponseCode;
                                transactionAttempt.SeqNumber = apiResponse.UniqueReference;
                                transactionAttempt.TransNumber = apiResponse.UniqueReference;
                                transactionAttempt.PosEntryMode = 0;

                                transactionAttempt.BatchNumber = "";
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = transData.TerminalId;
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = " API WorldNet Purchase Approved";

                                response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                                response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                response.POSWSResponse.ErrNumber = "0";
                                response.POSWSResponse.Message = "Transaction Successful.";
                                response.POSWSResponse.Status = "Approved";
                            }
                            else
                            {
                                transactionAttempt.PosEntryMode = 0;
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = "";
                                transactionAttempt.ReturnCode = apiResponse.ResponseCode;
                                transactionAttempt.SeqNumber = null;
                                transactionAttempt.TransNumber = null;
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = " API WorldNet Purchase Declined." + apiResponse.Message;
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                response.POSWSResponse.ErrNumber = apiResponse.ResponseCode;
                                response.POSWSResponse.Message = apiResponse.Message;
                                response.POSWSResponse.Status = "Declined";
                            }
                        }
                        #endregion

                        #region NovaPay
                        else if (mid.Switch.SwitchCode == "Nova2Pay")
                        {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.NovaPay.TransactionData transData = new GatewayProcessor.NovaPay.TransactionData();

                            string notifUrl = ConfigurationManager.AppSettings["NotifyURL"].ToString();
                            transData.Address = request.CustomerInfo.Address;
                            transData.City = request.CustomerInfo.City;
                            transData.Country = request.CustomerInfo.Country;
                            transData.State = request.CustomerInfo.State;
                            transData.Zip = request.CustomerInfo.Zip;

                            transData.FirstName = request.CustomerInfo.FirstName;
                            transData.LastName = request.CustomerInfo.LastName;
                            transData.Phone = request.CustomerInfo.PhoneNumber;
                            transData.Email = request.CustomerInfo.Email;

                            transData.AmountFee = Convert.ToString(request.Amount);
                            transData.Currency = request.Currency;
                            transData.GoodsTitle = "N/A";
                            transData.IssuingBank = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId).ToUpper();
                            transData.MerchantId = mid.Param_2;
                            transData.TerminalId = mid.Param_6;
                            transData.MerchantTradeId = DateTime.Now.ToString("MMddyyyyhhmmss");
                            transData.NotifyUrl = notifUrl;
                            transData.SecureHash = mid.Param_3;
                            transData.SignType = "RSA";

                            transData.CardNumber = card.CardNumber;
                            transData.ExpiryDate = card.ExpiryMonth + card.ExpiryYear;
                            transData.Cvv = card.CVV;

                            Hashtable sParamTemp = Functions.Hastable.buildParamRequest(transData);
                            Hashtable sParam = Functions.Hastable.buildSignRequest(sParamTemp);

                            var apiResponse = gateway.ProcessNovaToPay(sParam, "purchase");

                            if(apiResponse.Status == "Approved")
                            {
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = apiResponse.merchantTradeId;
                                transactionAttempt.ReturnCode = "00";
                                transactionAttempt.SeqNumber = "";
                                transactionAttempt.TransNumber = apiResponse.merchantTradeId;
                                transactionAttempt.PosEntryMode = 0;

                                transactionAttempt.BatchNumber = "";
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = "Transaction Purchase Approved";

                                response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                                response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                response.POSWSResponse.ErrNumber = "0";
                                response.POSWSResponse.Message = "Transaction Successful.";
                                response.POSWSResponse.Status = "Approved";
                            }
                            else
                            {
                                transactionAttempt.PosEntryMode = 0;
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = "";
                                transactionAttempt.ReturnCode = apiResponse.errorCode;
                                transactionAttempt.SeqNumber = null;
                                transactionAttempt.TransNumber = apiResponse.merchantTradeId;
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = " API NovaToPay Purchase Declined." + apiResponse.errorMsg;
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                response.POSWSResponse.ErrNumber = apiResponse.errorCode;
                                response.POSWSResponse.Message = apiResponse.errorMsg;
                                response.POSWSResponse.Status = "Declined";
                            }
                        }
                        #endregion

                        #region TrustPay
                        else if (mid.Switch.SwitchCode == "TrustPay")
                        {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.TrustPay.Transaction trans = new GatewayProcessor.TrustPay.Transaction();

                            string returnUrl = ConfigurationManager.AppSettings["ReturnURL"].ToString();

                            trans.csid = "csid";
                            trans.orderNo = DateTime.Now.ToString("MMddyyyyhhmmss");
                            trans.orderAmount = Convert.ToString(request.Amount);

                            trans.firstName = request.CustomerInfo.FirstName;
                            trans.lastName = request.CustomerInfo.LastName;
                            trans.ip = SDGUtil.Functions.GetIPAddress();
                            trans.zip = request.CustomerInfo.Zip;
                            trans.state = request.CustomerInfo.State;
                            trans.email = request.CustomerInfo.Email;
                            trans.address = request.CustomerInfo.Address;
                            trans.city = request.CustomerInfo.City;
                            trans.country = countries.CountryName;
                            trans.phone = request.CustomerInfo.PhoneNumber;
                            trans.orderCurrency = request.Currency;
                            trans.MerNo = mid.Param_2;
                            trans.Gatewayno = mid.Param_6;
                            trans.returnUrl = returnUrl;
                            trans.Date = DateTime.Now;

                            trans.cardSecurityCode = card.CVV;
                            trans.cardExpireMonth = card.ExpiryMonth;
                            trans.cardExpireYear = "20" + card.ExpiryYear;
                            trans.cardNo = card.CardNumber;
                            trans.issuingBank = request.BankName != null || request.BankName != "" ? request.BankName : "N/A";

                            var apiResponse = gateway.ProcessTrustPay(trans, "purchase");

                            if(apiResponse.Status == "Approved")
                            {
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = apiResponse.orderNo;
                                transactionAttempt.ReturnCode = "00";
                                transactionAttempt.SeqNumber = "";
                                transactionAttempt.TransNumber = apiResponse.tradeNo;
                                transactionAttempt.PosEntryMode = 0;

                                transactionAttempt.BatchNumber = "";
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = "Transaction Purchase Approved";

                                response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)transaction.CardTypeId);
                                response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);
                                response.POSWSResponse.ErrNumber = "0";
                                response.POSWSResponse.Message = "Transaction Successful.";
                                response.POSWSResponse.Status = "Approved";
                            }
                            else if (apiResponse.Status == "Declined")
                            {
                                transactionAttempt.PosEntryMode = 0;
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = "";
                                transactionAttempt.ReturnCode = apiResponse.responseCode;
                                transactionAttempt.SeqNumber = null;
                                transactionAttempt.TransNumber = null;
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = " API NovaToPay Purchase Declined." + apiResponse.orderInfo + ":" + apiResponse.responseCode;
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                response.POSWSResponse.ErrNumber = apiResponse.responseCode;
                                response.POSWSResponse.Message = apiResponse.orderInfo;
                                response.POSWSResponse.Status = "Declined";
                            }
                            else
                            {
                                response.POSWSResponse.ErrNumber = "2900.8";
                                response.POSWSResponse.Message = apiResponse.Status;
                                response.POSWSResponse.Status = apiResponse.Status;
                            }
                        }
                        #endregion

                        else
                        {
                            response.POSWSResponse.ErrNumber = "2900.7";
                            response.POSWSResponse.Message = "Invalid Switch.";
                            response.POSWSResponse.Status = "Declined";
                        }
                    }

                    action = "updating Transaction Attempt Cash details from gateway.";

                    var nTransactionAttempt = _transRepo.UpdateTransactionAttempt(transactionAttempt);

                    response.TransactionNumber = Convert.ToString(savedTransaction.TransactionId) + "-" + Convert.ToString(transactionAttempt.TransactionAttemptId);
                    response.AuthNumber = transactionAttempt.AuthNumber;
                    response.TransNumber = transactionAttempt.TransNumber;
                    response.SequenceNumber = transactionAttempt.SeqNumber;
                    response.BatchNumber = transactionAttempt.BatchNumber;
                    response.Timestamp = SDGUtil.Functions.Format_Datetime(transactionAttempt.DateReceived);
                    response.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)transaction.TransactionEntryTypeId);
                    response.Total = Convert.ToDecimal(transactionAttempt.Amount.ToString("N2"));
                    response.CardNumber = SDGUtil.Functions.HashCardNumber(card.CardNumber);
                    response.Currency = request.Currency; 
                }
                else if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    PurchaseResponse result = new PurchaseResponse();
                    result.POSWSResponse = new POSWSResponse();

                    result.AuthNumber = "1001";
                    result.BatchNumber = "0000001";
                    result.Currency = "PHP";
                    result.Timestamp = Convert.ToString(DateTime.Now);
                    result.TransactionNumber = "0000-0000";
                    result.TraceNumber = "0000";
                    result.Total = request.Amount;
                    result.TransactionEntryType = response.TransactionEntryType = Convert.ToString(SDGDAL.Enums.TransactionEntryType.CreditManual);
                }
                else
                {
                    response.POSWSResponse.ErrNumber = "2900.8";
                    response.POSWSResponse.Status = "Declined";
                    response.POSWSResponse.Message = "Invalid System Mode.";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message.ToString() , "TransactionPurchaseCreditManual", ex.Message);

                response.POSWSResponse.ErrNumber = "2900.9";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.POSWSResponse.Status = "DECLINED";
            }

            return response;
        }
    }
}
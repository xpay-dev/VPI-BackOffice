using SDGBackOffice.Models;
using SDGDAL;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using SDGUtil;
using SDGUtil.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace SDGBackOffice.Controllers
{
    public class PaymentController : Controller
    {
        MerchantRepository _merchantRepo;
        UserRepository _userRepo;
        ResellerRepository _resellerRepo;
        MerchantBranchRepository _branchRepo;
        PartnerRepository _partnerRepo;
        ReferenceRepository _refRepo;
        MerchantBranchPOSRepository _posRepo;
        MobileAppFeaturesRepository _posFeatures;
        AccountRepository _accountRepository;
        MidsRepository _midsRepository;
        MobileAppRepository _mobileAppRepo;
        MidsRepository _midsRepo;
        GatewayProcessor.Gateways _gateway;
        TransactionRepository _transRepo;

        private static string payecoUrl = string.Empty;
        private static string apiResponseXML = string.Empty;
        string action = string.Empty;

        public PaymentController()
        {
            _posRepo = new MerchantBranchPOSRepository();
            _merchantRepo = new MerchantRepository();
            _userRepo = new UserRepository();
            _resellerRepo = new ResellerRepository();
            _branchRepo = new MerchantBranchRepository();
            _partnerRepo = new PartnerRepository();
            _refRepo = new ReferenceRepository();
            _posFeatures = new MobileAppFeaturesRepository();
            _accountRepository = new AccountRepository();
            _midsRepository = new MidsRepository();
            _mobileAppRepo = new MobileAppRepository();
            _midsRepo = new MidsRepository();
            _gateway = new GatewayProcessor.Gateways();
            _transRepo = new TransactionRepository();
        }

        public ActionResult Index()
        {
            TransactionModel model = new TransactionModel();

            GetddlData();

            var merchant = _merchantRepo.GetDetailsbyMerchantId(CurrentUser.ParentId);
            model.MerchantName = merchant.MerchantName;
            model.InvoiceNumber = DateTime.Now.ToString("yyyyMMddhhmmss");

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(TransactionModel model)
        {
            try
            {
                var transaction = new SDGDAL.Entities.Transaction();
                var transactionAttempt = new SDGDAL.Entities.TransactionAttempt();

                var merchant = _merchantRepo.GetDetailsbyMerchantId(CurrentUser.ParentId);
                model.MerchantName = merchant.MerchantName;

                action = "iniatializing data.";

                #region checking card type id
                action = "get cardtype and do simple card validate";

                int cardTypeId = model.CardTypeId == 7 ? 7 : SDGUtil.Functions.GetCardType(model.CardNumber);
                if (cardTypeId != 0)
                {
                    transaction.CardTypeId = cardTypeId;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: 2900.5 Invalid card type");
                    return View(model);
                }
                #endregion

                #region Checking Mids

                action = "checking mid status before setting up transaction entry.";
                var mid = new SDGDAL.Entities.Mid();

                mid = _midsRepo.GetMidByPosIdAndCardTypeId(model.MerchantPosId, model.CardTypeId); 

                if (mid == null)
                {
                    ModelState.AddModelError(string.Empty, "Mid not found.");
                    return View(model);
                }
                else
                {
                    if (!mid.IsActive || mid.IsDeleted)
                    {
                        ModelState.AddModelError(string.Empty, "Mid is Inactive.");
                        return View(model);
                    }

                    if (!mid.Switch.IsActive)
                    {
                        ModelState.AddModelError(string.Empty, "Switch is Inactive.");
                        return View(model);
                    }
                }
                #endregion

                #region Checking Currency
                try
                {
                    transaction.CurrencyId = _transRepo.GetCurrencyIdByCurrencyName(model.Currency);
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error 2900.6: Invalid currency.");
                    return View(model);
                }
                #endregion

                if (ModelState.IsValid && model != null)
                {
                    action = "getting transaction  and transactionattempt credit details.";

                    #region Encrypt Card Data

                    action = "trying to encrypt card data.";
                    //ENCRYPT CARD DATA
                    string NE_CARD = model.CardNumber;
                    string NE_EMONTH = model.ExpiryMonth;
                    string NE_EYEAR = model.ExpiryYear;
                    string NE_CSC = model.Cvv;
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

                    transaction.MerchantPOSId = model.MerchantPosId;
                    transaction.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.CreditManual);
                    transaction.Notes = string.Empty;
                    transaction.RefNumSales = model.InvoiceNumber;
                    transaction.RefNumApp = DateTime.Now.ToString("MMddyyyyhhmmss");
                    transaction.OriginalAmount = Convert.ToDecimal(model.Amount);
                    transaction.TaxAmount = 0;
                    transaction.MidId = mid.MidId;
                    transaction.FinalAmount = Convert.ToDecimal(model.Amount);
                    transaction.DateCreated = DateTime.Now;
                    transaction.CardNumber = E_CARD;
                    transaction.ExpMonth = model.ExpiryMonth;
                    transaction.ExpYear = model.ExpiryYear;
                    transaction.CSC = null;
                    transaction.NameOnCard = model.FirstName + " " + model.LastName;

                    transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale);
                    transactionAttempt.AccountId = CurrentUser.AccountId;
                    transactionAttempt.MobileAppId = model.MerchantPosId;
                    transactionAttempt.GPSLat = 0;
                    transactionAttempt.GPSLong = 0;
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

                        #region NovaToPay

                        if(mid.Switch.SwitchCode == "Nova2Pay") {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.NovaPay.TransactionData transData = new GatewayProcessor.NovaPay.TransactionData();
                            GatewayProcessor.NovaPay.HashFunctions hashData = new GatewayProcessor.NovaPay.HashFunctions();

                            string notifUrl = ConfigurationManager.AppSettings["NotifyURL"].ToString();
                            transData.Address = model.Address;
                            transData.City = model.City;
                            transData.Country = model.Country;
                            transData.State = model.State;
                            transData.Zip = model.Zip;

                            transData.FirstName = model.FirstName;
                            transData.LastName = model.LastName;
                            transData.Phone = model.Phone;
                            transData.Email = model.Email;

                            transData.AmountFee = Convert.ToString(transaction.OriginalAmount);
                            transData.Currency = model.Currency;
                            transData.GoodsTitle = model.GoodsTitle;
                            transData.IssuingBank = Convert.ToString((SDGDAL.Enums.MobileAppCardType) model.CardTypeId).ToUpper();
                            transData.MerchantId = mid.Param_2;
                            transData.TerminalId = mid.Param_6;
                            transData.PrivateKey = mid.Param_3;
                            transData.MerchantTradeId = DateTime.Now.ToString("MMddyyyyhhmmss");
                            transData.NotifyUrl = notifUrl;
                            transData.SecureHash = mid.Param_3;
                            transData.SignType = "RSA";

                            transData.CardNumber = model.CardNumber;
                            transData.ExpiryDate = model.ExpiryMonth + model.ExpiryYear;
                            transData.Cvv = model.Cvv;

                            Hashtable sParamTemp = GatewayProcessor.NovaPay.HashFunctions.buildParamRequest(transData);
                            Hashtable sParam = GatewayProcessor.NovaPay.HashFunctions.buildSignRequest(sParamTemp);

                            var apiResponse = gateway.ProcessNovaToPay(sParam, "purchase");

                            if(apiResponse.Status == "Approved") {
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = apiResponse.merchantTradeId;
                                transactionAttempt.ReturnCode = "00";
                                transactionAttempt.SeqNumber = "";
                                transactionAttempt.TransNumber = apiResponse.merchantTradeId;
                                transactionAttempt.PosEntryMode = 0;
                                transactionAttempt.Reference = model.InvoiceNumber;

                                transactionAttempt.BatchNumber = "";
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = "Transaction Purchase Approved";

                                Session ["Success"] = "Successful Transaction";
                            } else {
                                transactionAttempt.Reference = model.InvoiceNumber;
                                transactionAttempt.PosEntryMode = 0;
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = "";
                                transactionAttempt.ReturnCode = apiResponse.errorCode;
                                transactionAttempt.SeqNumber = null;
                                transactionAttempt.TransNumber = null;
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = "Transaction Purchase Declined." + apiResponse.errorMsg;
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                Session ["Error"] = "Declined:" + apiResponse.errorCode + "-" + apiResponse.errorMsg;
                            }
                        }
                        #endregion

                        #region GlobalOnePay
                        else if(mid.Switch.SwitchCode == "GlobalOnePay") {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.GlobalOnePay.TransactionData transData = new GatewayProcessor.GlobalOnePay.TransactionData();

                            transData.Amount = Convert.ToString(transaction.OriginalAmount);
                            transData.CardExpiry = model.ExpiryMonth + model.ExpiryYear;
                            transData.CardHolderName = model.FirstName + " " + model.LastName;
                            transData.CardNumber = model.CardNumber;
                            transData.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType) transaction.CardTypeId).ToUpper();
                            transData.Currency = model.Currency;
                            transData.Cvv = model.Cvv;
                            transData.Hash = mid.Param_3;
                            transData.OrderId = Convert.ToString(transactionAttempt.TransactionId + "-" + transactionAttempt.TransactionAttemptId);
                            transData.TerminalId = mid.Param_6;

                            var apiResponse = gateway.ProcessGlobalOnePayTransaction(transData, "purchase");

                            if(apiResponse.Status == "Approved") {
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = apiResponse.ApprovalCode;
                                transactionAttempt.ReturnCode = apiResponse.ResponseCode;
                                transactionAttempt.SeqNumber = apiResponse.UniqueReference;
                                transactionAttempt.TransNumber = apiResponse.UniqueReference;
                                transactionAttempt.PosEntryMode = 0;

                                transactionAttempt.BatchNumber = "";
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = " API Global One Pay Purchase Approved";

                                transactionAttempt.BatchNumber = "";
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = "Transaction Purchase Approved";

                                Session ["Success"] = "Successful Transaction";
                            } else {
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

                                Session ["Error"] = "Declined:" + apiResponse.ResponseCode + "-" + apiResponse.Message;
                            }
                        }
                        #endregion

                        #region Worldnet Pay
                        else if(mid.Switch.SwitchCode == "Worldnet") {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.Worldnet.TransactionData transData = new GatewayProcessor.Worldnet.TransactionData();

                            transData.Amount = Convert.ToString(transaction.OriginalAmount);
                            transData.CardExpiry = model.ExpiryMonth + model.ExpiryYear;
                            transData.CardHolderName = model.FirstName + " " + model.ExpiryYear;
                            transData.CardNumber = model.CardNumber;
                            transData.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType) transaction.CardTypeId).ToUpper();
                            transData.Currency = model.Currency;
                            transData.Cvv = model.Cvv;
                            transData.Hash = mid.Param_3;
                            transData.OrderId = Convert.ToString(transactionAttempt.TransactionId + "-" + transactionAttempt.TransactionAttemptId);
                            transData.TerminalId = mid.Param_6;

                            var apiResponse = gateway.ProcessWorldnetTransaction(transData, "purchase");

                            if(apiResponse.Status == "Approved") {
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = apiResponse.ApprovalCode;
                                transactionAttempt.ReturnCode = apiResponse.ResponseCode;
                                transactionAttempt.SeqNumber = apiResponse.UniqueReference;
                                transactionAttempt.TransNumber = apiResponse.UniqueReference;
                                transactionAttempt.PosEntryMode = 0;

                                transactionAttempt.BatchNumber = "";
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = " API WorldNet Purchase Approved";

                                Session ["Success"] = "Successful Transaction";
                            } else {
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

                                Session ["Error"] = "Declined:" + apiResponse.ResponseCode + "-" + apiResponse.Message;
                            }
                        }
                        #endregion

                        #region TrustPay
                        else if(mid.Switch.SwitchCode == "TrustPay") {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.TrustPay.Transaction trans = new GatewayProcessor.TrustPay.Transaction();

                            string returnUrl = ConfigurationManager.AppSettings["ReturnURL"].ToString();

                            trans.csid = "csid";
                            trans.orderNo = DateTime.Now.ToString("MMddyyyyhhmmss");
                            trans.orderAmount = Convert.ToString(model.Amount);

                            trans.firstName = model.FirstName;
                            trans.lastName = model.LastName;
                            trans.ip = SDGUtil.Functions.GetIPAddress();
                            trans.zip = model.Zip;
                            trans.state = model.State;
                            trans.email = model.Email;
                            trans.address = model.Address;
                            trans.city = model.City;
                            trans.country = model.Country;
                            trans.phone = model.Phone;
                            trans.orderCurrency = model.Currency;
                            trans.MerchantID = mid.Param_2;
                            trans.MerNo = mid.Param_2;
                            trans.Gatewayno = mid.Param_6;
                            trans.returnUrl = returnUrl;
                            trans.Date = DateTime.Now;

                            trans.cardSecurityCode = model.Cvv;
                            trans.cardExpireMonth = model.ExpiryMonth;
                            trans.cardExpireYear = "20" + model.ExpiryYear;
                            trans.cardNo = model.CardNumber;
                            trans.issuingBank = model.IssuingBank;

                            var apiResponse = gateway.ProcessTrustPay(trans, "purchase");

                            if(apiResponse.Status == "Approved") {
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
                                transactionAttempt.Notes = "TrustPay Transaction Purchase Approved";

                                Session ["Success"] = "Approved:" + apiResponse.orderInfo;
                            } else if(apiResponse.Status == "Declined") {
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
                                transactionAttempt.Notes = "API TrustPay Purchase Declined: " + apiResponse.responseCode + " " + apiResponse.orderInfo;

                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                                Session ["Error"] = "Declined:" + apiResponse.responseCode + "-" + apiResponse.orderInfo;
                            } else {
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                                Session ["Error"] = apiResponse.Status + "-" + apiResponse.responseCode;
                            }
                        }
                        #endregion

                        #region PayEco
                        else if(mid.Switch.SwitchCode == "PayEco") {
                            GatewayProcessor.Payeco.TransactionData transData = new GatewayProcessor.Payeco.TransactionData();
                            payecoUrl = ConfigurationManager.AppSettings ["PayEcoURL"].ToString();

                            string syncAddress = ConfigurationManager.AppSettings["SynAddress"].ToString();
                            string asyncAddress = ConfigurationManager.AppSettings["AsynAddress"].ToString();
                            string merchantPassword = ConfigurationManager.AppSettings["MerchantPassword"].ToString();

                            string merchantId = mid.Param_2;

                            MerchantMessage msg = new MerchantMessage();
                            msg.setVersion(ConfigurationManager.AppSettings ["Version"].ToString());
                            msg.setProcCode(ConfigurationManager.AppSettings ["ProcCode"].ToString());
                            msg.setProcessCode(ConfigurationManager.AppSettings ["ProcessCode"].ToString());
                            msg.setMerchantNo(mid.Param_2);
                            msg.setMerchantOrderNo(Convert.ToString(transactionAttempt.TransactionId + "-" + transactionAttempt.TransactionAttemptId));
                            msg.setOrderNo(DateTime.Now.ToString("yyyyMMddhhmmss"));
                            msg.setAcqSsn(DateTime.Now.ToString("HHmmss"));
                            msg.setTransDatetime(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            msg.setAmount(model.Amount);
                            msg.setCurrency(model.Currency);
                            msg.setDescription(model.GoodsTitle);
                            msg.setSynAddress(syncAddress);
                            msg.setAsynAddress(asyncAddress);

                            String mac = msg.computeMac(merchantPassword);
                            msg.setMac(mac.ToUpper());

                            string srcXml = msg.ToXML();

                            apiResponseXML = _gateway.ProcessPayEco(srcXml);

                            return RedirectToAction("RedirectToPayeco");
                        }
                        #endregion

                        #region PayOnline
                        else if(mid.Switch.SwitchCode == "PayOnline") {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.PayOnline.TransactionRequest trans = new GatewayProcessor.PayOnline.TransactionRequest();

                            trans.MerchantId = mid.Param_2;
                            trans.OrderId = transactionAttempt.TransactionId + "-" + transactionAttempt.TransactionAttemptId;
                            trans.Amount = Convert.ToString(model.Amount);
                            trans.Currency = model.Currency;
                            trans.SecurityKey = "";
                            trans.Ip = SDGUtil.Functions.GetIPAddress();
                            trans.Email = model.Email;
                            trans.CardHolderName = model.FirstName + " " + model.LastName;
                            trans.CardNumber = model.CardNumber;
                            trans.CardExpiry = model.ExpiryMonth + model.ExpiryYear;
                            trans.Cvv = model.Cvv;
                            trans.Country = model.Country; //2 letter initials
                            trans.City = model.City;
                            trans.Address = model.Address;
                            trans.Zip = model.Zip;
                            trans.State = model.State;
                            trans.Phone = model.Phone;
                            trans.Issuer = model.IssuingBank;

                            var apiResponse = gateway.ProcessPayOnline(trans, "purchase");

                            if(apiResponse.Status == "Approved") {
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = apiResponse.Id;
                                transactionAttempt.ReturnCode = "00";
                                transactionAttempt.SeqNumber = "";
                                transactionAttempt.TransNumber = apiResponse.Result;
                                transactionAttempt.PosEntryMode = 0;

                                transactionAttempt.BatchNumber = "";
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = "TrustPay Transaction Purchase Approved";

                                Session ["Success"] = "Approved:" + apiResponse.Id;
                            } else if(apiResponse.Status == "Declined") {
                                transactionAttempt.PosEntryMode = 0;
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = "";
                                transactionAttempt.ReturnCode = apiResponse.ErrorCode;
                                transactionAttempt.SeqNumber = null;
                                transactionAttempt.TransNumber = null;
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = "API TrustPay Purchase Declined: " + apiResponse.ErrorCode + " " + apiResponse.Result;

                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                                Session ["Error"] = "Declined:" + apiResponse.ErrorCode + "-" + apiResponse.Result;
                            } else {
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                                Session ["Error"] = apiResponse.Status + "-" + apiResponse.ErrorCode;
                            }
                        }
                        #endregion

                        #region PayMaya
                        else if(mid.Switch.SwitchCode.ToUpper() == "PAYMAYA") {
                            return View();
                        }
                        #endregion

                        else {
                            ModelState.AddModelError(string.Empty, "Invalid Switch");
                            return RedirectToAction("Result");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error in saving transactions");
                        return RedirectToAction("Result");
                    }

                    action = "updating Transaction Attempt Cash details from gateway.";

                    var nTransactionAttempt = _transRepo.UpdateTransactionAttempt(transactionAttempt);

                    TempData["TransactionId"] = Convert.ToString(transactionAttempt.TransactionId) + "-" + Convert.ToString(nTransactionAttempt.TransactionAttemptId);

                    return RedirectToAction("Result");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "AssignDevice", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Assign Device";
                err.Method = "Assign";
                err.ErrText = ex.Message;
                err.StackTrace = ex.StackTrace;
                err.DateCreated = DateTime.Now;

                if (ex.InnerException != null)
                {
                    err.InnerException = ex.InnerException.Message;
                    err.InnerExceptionStackTrace = ex.InnerException.StackTrace;
                }

                err = _refRepo.LogError(err);
            }
            finally
            {
                GetddlData();
            }
            
            return View(model);
        }
        public ActionResult AsynResponse()
        {
            try
            {
                string merchantPassword = ConfigurationManager.AppSettings["PayEcoMerchantPassword"].ToString();
                String response_text = Request["response_text"];
                string result = string.Empty;
                if(string.IsNullOrEmpty(response_text))
                {
                    result = response_text;
                }
                else
                {
                    result = "No data has been received";
                }

                //notify Payeco Recived
                Response.Write(result);
                Response.Flush();
                //Response.Close(); //尽量用End()
                Response.End();

                //TransactionResponseModel model = new TransactionResponseModel();
                //var transaction = new SDGDAL.Entities.Transaction();
                //var transactionAttempt = new SDGDAL.Entities.TransactionAttempt();

                //if (!string.IsNullOrEmpty(response_text))
                //{
                //    string response = string.Empty;
                //    string xmlString = string.Empty;

                //    string urlText = HttpUtility.UrlDecode(response_text);
                //    xmlString = Toolkit.Base64ToString(urlText);

                //    XmlDocument xmlDoc = new XmlDocument();
                //    xmlDoc.LoadXml(xmlString);
                //    XmlNodeList nodeList = xmlDoc.GetElementsByTagName("x:NetworkRequest");
                //    XmlNode node = nodeList[0];

                //    string procCode = node["ProcCode"].InnerText;
                //    string processCode = node["ProcessCode"].InnerText;
                //    string accountNo = node["AccountNo"].InnerText;
                //    string accountType = node["AccountType"].InnerText;
                //    string amount = node["Amount"].InnerText;
                //    string currency = node["Currency"].InnerText;
                //    string terminalNo = node["TerminalNo"].InnerText;
                //    string merchantNo = node["MerchantNo"].InnerText;
                //    string merchantOrderNo = node["MerchantOrderNo"].InnerText;
                //    string orderNo = node["OrderNo"].InnerText;
                //    string orderState = node["OrderState"].InnerText;
                //    string responseCode = node["RespCode"].InnerText;
                //    string responseMessage = node["Remark"].InnerText;
                //    string mac = node["MAC"].InnerText;
                //    string transDatetime = node["TransDatetime"].InnerText;
                //    string acqSsn = node["AcqSsn"].InnerText;
                //    string transData = node["TransData"].InnerText;
                //    string reference = node["Reference"].InnerText;

                //    String src = procCode
                //        + Toolkit.getString(accountNo)
                //        + Toolkit.getString(processCode)
                //        + Toolkit.getString(amount)
                //        + Toolkit.getString(transDatetime)
                //        + Toolkit.getString(acqSsn)
                //        + Toolkit.getString(orderNo)
                //        + Toolkit.getString(transData)
                //        + Toolkit.getString(reference)
                //        + Toolkit.getString(responseCode)
                //        + Toolkit.getString(terminalNo)
                //        + Toolkit.getString(merchantNo)
                //        + Toolkit.getString(merchantOrderNo)
                //        + Toolkit.getString(orderState) + " " + merchantPassword;

                //    String MAC = Toolkit.ComputeMD5Hash(src);

                //    if (MAC.Equals(mac))
                //    {
                //        // get proper trans_attempt id
                //        int hyphen;
                //        int transAttId;
                //        int transId;

                //        if (merchantNo.Contains('-'))
                //        {
                //            hyphen = merchantNo.IndexOf('-');
                //            transAttId = Convert.ToInt32(merchantNo.Substring(hyphen + 1));
                //            transId = Convert.ToInt32(merchantNo.Substring(0, hyphen));

                //            transactionAttempt.TransactionId = transId;
                //            transactionAttempt.TransactionAttemptId = transAttId;

                //            if (responseCode.Equals("0000"))
                //            {
                //                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                //                transactionAttempt.AuthNumber = acqSsn;
                //                transactionAttempt.ReturnCode = "00";
                //                transactionAttempt.SeqNumber = "";
                //                transactionAttempt.TransNumber = orderNo;
                //                transactionAttempt.PosEntryMode = 0;
                //                transactionAttempt.Reference = reference;
                //                transactionAttempt.BatchNumber = DateTime.Now.ToString("yyyyMMddhhmmss");
                //                transactionAttempt.DisplayReceipt = merchantNo;
                //                transactionAttempt.DisplayTerminal = terminalNo;
                //                transactionAttempt.DateReceived = DateTime.Now;
                //                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                //                transactionAttempt.Notes = "API Payeco Transaction Purchase Approved";
                //                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale);

                //                Session["Success"] = "Successful Transaction";
                //            }
                //            else
                //            {
                //                transactionAttempt.Reference = reference;
                //                transactionAttempt.PosEntryMode = 0;
                //                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                //                transactionAttempt.AuthNumber = "";
                //                transactionAttempt.ReturnCode = responseCode;
                //                transactionAttempt.SeqNumber = null;
                //                transactionAttempt.TransNumber = orderNo;
                //                transactionAttempt.DisplayReceipt = merchantNo;
                //                transactionAttempt.DisplayTerminal = terminalNo;
                //                transactionAttempt.DateReceived = DateTime.Now;
                //                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                //                transactionAttempt.Notes = "API Payeco Transaction Purchase Declined:" + responseCode + " " + responseMessage;
                //                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                //                Session["Error"] = "Declined:" + responseCode + "-" + responseMessage;
                //            }

                //            action = "updating Transaction Attempt Credit details from gateway.";

                //            var nTransactionAttempt = _transRepo.UpdateTransactionAttempt(transactionAttempt);

                //            TempData["TransactionId"] = Convert.ToString(transactionAttempt.TransactionId) + "-" + Convert.ToString(nTransactionAttempt.TransactionAttemptId);

                //            return RedirectToAction("Result");
                //        }
                //        else
                //        {
                //            //log error
                //        }
                //    }
                //}
                //else
                //{
                //    //No Response text value
                //}
            }
            catch
            {

            }

            return View();
        }

        public ActionResult Result()
        {
            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            if (Session["Error"] != null)
            {
                ViewBag.Error = Session["Error"];
            }

            Session["Success"] = null;
            Session["Error"] = null;

            TransactionResponseModel model = new TransactionResponseModel();
            string transNumber = TempData["TransactionId"].ToString();

            try
            {
                // get proper trans_attempt id
                int hyphen;
                int transAttId;
                int transId;

                if (transNumber.Contains('-'))
                {
                    hyphen = transNumber.IndexOf('-');
                    transAttId = Convert.ToInt32(transNumber.Substring(hyphen + 1));
                    transId = Convert.ToInt32(transNumber.Substring(0, hyphen));

                    var resultCredit = _transRepo.GetAllCreditTransactionAttemptByTransNumber(transId, transAttId);

                    if(resultCredit != null)
                    {
                        model.ErrorCode = resultCredit.ReturnCode;
                        model.ErrorMessage = resultCredit.Notes;

                        model.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType)resultCredit.Transaction.CardTypeId).ToUpper();
                        model.AuthNumber = resultCredit.AuthNumber;
                        model.InvoiceNumber = resultCredit.Reference;
                        model.OrderNumber = resultCredit.TransNumber;
                        model.TransactionNumber = transId + "-" + transAttId;
                        model.Timestamp = SDGUtil.Functions.Format_Datetime(resultCredit.DateReceived);
                        model.Total = resultCredit.Transaction.Currency.CurrencyCode + " " + Convert.ToString(resultCredit.Amount);
                    }
                }
            }
            catch(Exception ex)
            {
                //log error
            }

            return View(model);
        }

        public ActionResult PageNotif()
        {
            return View();
        }

        public ActionResult Refund()
        {
            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            if (Session["Error"] != null)
            {
                ViewBag.Error = Session["Error"];
            }

            Session["Success"] = null;
            Session["Error"] = null;

            GetddlData();

            TransactionModel model = new TransactionModel();

            int merchantId = CurrentUser.ParentId;
            var merchant = _merchantRepo.GetDetailsbyMerchantId(CurrentUser.ParentId);
            model.MerchantName = merchant.MerchantName;

            return View(model);
        }

        [HttpPost]
        public ActionResult Refund(TransactionModel model)
        {
            GetddlData();
            int merchantId = CurrentUser.ParentId;
            var merchant = _merchantRepo.GetDetailsbyMerchantId(CurrentUser.ParentId);
            model.MerchantName = merchant.MerchantName;

            bool refundExists = false;
            decimal totalAmount = 0;
            int hyphen;
            int transAttId = 0;
            int transId = 0;

            try
            {
                hyphen = model.TransactionNumber.IndexOf('-');
                transAttId = Convert.ToInt32(model.TransactionNumber.Substring(hyphen + 1));
                transId = Convert.ToInt32(model.TransactionNumber.Substring(0, hyphen));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Error: Invalid transaction number");
                return View(model);
            }

            action = "getting reason to refund";
            var refReason = _transRepo.GetTransactionVoidReasonById(Convert.ToInt32(model.ReasonId));

            action = "getting transaction by transaction id from database.";
            var transaction = _transRepo.GetTransaction(transId);

            var transactionAttempt = _transRepo.GetTransactionAttempt(transAttId);

            if (transactionAttempt == null)
            {
                ModelState.AddModelError(string.Empty, "Error:2100.4 Transaction not found.");
                return View(model);
            }

            if (string.IsNullOrEmpty(transactionAttempt.AuthNumber))
            {
                ModelState.AddModelError(string.Empty, "Error:2100.5 No previous completed sale transaction found.");
                return View(model);
            }
            else
            {
                if (!_transRepo.IsCreditTransactionAlreadyVoid(transId))
                {
                    ModelState.AddModelError(string.Empty, "Error:2100.6 Transaction already void.");
                    return View(model);
                }
            }

            if (CurrentUser.ParentId != transaction.MerchantPOS.MerchantBranch.MerchantId)
            {
                ModelState.AddModelError(string.Empty, "Error:2100.3 Merchant Mismatch");
                return View(model);
            }

            action = "checking mid status before setting up transaction entry.";
            var mid = new SDGDAL.Entities.Mid();

            mid = _midsRepo.GetMidByPosIdAndCardTypeId(transaction.MerchantPOSId, transaction.CardTypeId);

            if (mid == null)
            {
                ModelState.AddModelError(string.Empty, "Error:2001.8 Mid not found.");
                return View(model);
            }
            else
            {
                if (!mid.IsActive || mid.IsDeleted)
                {
                    ModelState.AddModelError(string.Empty, "Error:2101.5 Mid is Inactive.");
                    return View(model);
                }

                if (!mid.Switch.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Error:2101.6 Switch is Inactive.");
                    return View(model);
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
                ModelState.AddModelError(string.Empty, "Error:2100.10 Unable to decrypt card details.");
                return View(model);
            }

            if (ModelState.IsValid && model != null)
            {
                var nTransactionAttempt = new SDGDAL.Entities.TransactionAttempt();

                action = "verifying transaction status.";
                // void
                nTransactionAttempt.Amount = transaction.FinalAmount;
                nTransactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Void);

                nTransactionAttempt.DateSent = DateTime.Now;
                nTransactionAttempt.DateReceived = DateTime.Now.AddYears(-100);
                nTransactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                nTransactionAttempt.Notes = "";
                nTransactionAttempt.TransactionId = transactionAttempt.TransactionId;
                nTransactionAttempt.TransactionChargesId = transactionAttempt.TransactionChargesId;
                nTransactionAttempt.AuthNumber = transactionAttempt.AuthNumber;
                nTransactionAttempt.BatchNumber = "";
                nTransactionAttempt.DisplayReceipt = "";
                nTransactionAttempt.DisplayTerminal = "";
                nTransactionAttempt.ReturnCode = "";
                nTransactionAttempt.SeqNumber = "";
                nTransactionAttempt.TransNumber = transactionAttempt.TransNumber;

                transaction.CardTypeId = transaction.CardTypeId;
                transaction.Track1 = transaction.Track1;
                transaction.Track2 = transaction.Track2;

                transaction.CardNumber = transaction.CardNumber;
                transaction.ExpYear = transaction.ExpYear;
                transaction.ExpMonth = transaction.ExpMonth;
                transaction.NameOnCard = transaction.NameOnCard;

                nTransactionAttempt.DeviceId = transactionAttempt.DeviceId;
                nTransactionAttempt.MobileAppId = transaction.MerchantPOSId;
                nTransactionAttempt.AccountId = transactionAttempt.AccountId;
                nTransactionAttempt.GPSLat = transactionAttempt.GPSLat;
                nTransactionAttempt.GPSLong = transactionAttempt.GPSLong;
                nTransactionAttempt.TransactionSignatureId = transactionAttempt.TransactionSignatureId;
                nTransactionAttempt.TransactionVoidReasonId = Convert.ToInt32(model.ReasonId);
                nTransactionAttempt.TransactionVoidNote = model.Note;
                nTransactionAttempt.PosEntryMode = transactionAttempt.PosEntryMode;
                nTransactionAttempt.BatchNumber = transactionAttempt.BatchNumber;
                nTransactionAttempt.ReturnCode = null;

                nTransactionAttempt.DateSent = DateTime.Now;
                nTransactionAttempt.DateReceived = Convert.ToDateTime("1900/01/01");
                nTransactionAttempt.DepositDate = Convert.ToDateTime("1900/01/01");

                #region Handle Transaction response

                action = "setting up transaction entry for database.";

                try
                {
                    var nnTransactionAttempt = _transRepo.CreateTransactionAttempt(nTransactionAttempt);

                    nTransactionAttempt.TransactionSignatureId = nnTransactionAttempt.TransactionSignatureId;
                    nnTransactionAttempt.TransactionVoidReasonId = Convert.ToInt32(model.ReasonId);
                    nnTransactionAttempt.TransactionVoidNote = model.Note;

                    if (nnTransactionAttempt.TransactionAttemptId > 0)
                    {
                        action = "processing transaction for api integration. Transaction was successfully saved.";

                        #region Nova2Pay Refund
                        if (mid.Switch.SwitchCode == "Nova2Pay")
                        {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.NovaPay.TransactionData transData = new GatewayProcessor.NovaPay.TransactionData();
                            GatewayProcessor.NovaPay.HashFunctions hashData = new GatewayProcessor.NovaPay.HashFunctions();

                            string notifUrl = ConfigurationManager.AppSettings["NotifyURL"].ToString();

                            transData.PrivateKey = transaction.Mid.Param_3;
                            transData.MerchantId = transaction.Mid.Param_2;
                            transData.TerminalId = transaction.Mid.Param_6;
                            transData.MerchantTradeId = transactionAttempt.TransNumber;
                            transData.NotifyUrl = notifUrl;

                            Hashtable sParaTemp = GatewayProcessor.NovaPay.HashFunctions.buildParamRefund(transData);
                            Hashtable sParam = GatewayProcessor.NovaPay.HashFunctions.buildRequestRefund(sParaTemp);

                            var apiResponse = gateway.ProcessNovaToPay(sParam, "void");

                            if (apiResponse.Status == "Approved")
                            {
                                transactionAttempt.TransactionAttemptId = nnTransactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = apiResponse.merchantTradeId;
                                transactionAttempt.ReturnCode = "00";
                                transactionAttempt.SeqNumber = "";
                                transactionAttempt.TransNumber = apiResponse.merchantTradeId;
                                transactionAttempt.PosEntryMode = 0;
                                transactionAttempt.Reference = apiResponse.merchantTradeId;
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Void);

                                transactionAttempt.BatchNumber = "";
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = "Refund Approved";

                                Session["Success"] = "Refund Approved";
                            }
                            else if(apiResponse.Status == "Declined")
                            {
                                transactionAttempt.Reference = model.InvoiceNumber;
                                transactionAttempt.PosEntryMode = 0;
                                transactionAttempt.TransactionAttemptId = nnTransactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = "";
                                transactionAttempt.ReturnCode = apiResponse.flag;
                                transactionAttempt.SeqNumber = null;
                                transactionAttempt.TransNumber = null;
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = "Transaction Purchase Declined." + apiResponse.message;
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                Session["Error"] = "Declined:" + apiResponse.flag + "-" + apiResponse.message;
                            }
                            else
                            {
                                Session["Error"] = "Declined:" + apiResponse.flag + "-" + apiResponse.message;
                            }
                        }
                        #endregion

                        #region TrustsPay Refund
                        else if (mid.Switch.SwitchCode == "TrustPay")
                        {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                            GatewayProcessor.TrustPay.Transaction trans = new GatewayProcessor.TrustPay.Transaction();
                            GatewayProcessor.TrustPay.TransactionResponse response = new GatewayProcessor.TrustPay.TransactionResponse();

                            trans.MerNo = mid.Param_2;
                            trans.Gatewayno = mid.Param_6;
                            trans.tradeNo = transactionAttempt.TransNumber;
                            trans.refundType = "1";
                            trans.orderAmount = Convert.ToString(transactionAttempt.Amount);
                            trans.tradeAmount = Convert.ToString(transactionAttempt.Amount);
                            trans.orderCurrency = mid.Currency.CurrencyCode;
                            trans.refundReason = refReason.VoidReason;
                            trans.remark = model.Note;

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
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Void);

                                transactionAttempt.BatchNumber = "";
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = "Refund Approved";

                                Session["Success"] = "Refund Approved";
                            }
                            else if (apiResponse.Status == "Declined")
                            {
                                transactionAttempt.Reference = model.InvoiceNumber;
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

                                Session["Error"] = "Declined:" + apiResponse.responseCode + "-" + apiResponse.Message;
                            }
                            else
                            {
                                Session["Error"] = "Declined:" + apiResponse.responseCode + "-" + apiResponse.Message;
                            }
                        }
                        #endregion

                        else
                        {
                            ModelState.AddModelError(string.Empty, "Error:2001.8 Transaction failed. Switch not yet supported. Please contact Support.");
                            return View(model);
                        }
                        
                        action = "updating Transaction Attempt Cash details from gateway.";

                        var savedRefund = _transRepo.UpdateTransactionAttempt(transactionAttempt);

                        TempData["TransactionId"] = Convert.ToString(savedRefund.TransactionId) + "-" + Convert.ToString(transactionAttempt.TransactionAttemptId);

                        return RedirectToAction("Refund");
                    }
                }
                catch (Exception ex)
                {
                    // Log error please
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackoffice", errorOnAction + "\n" + ex.Message, "CreditTransactionEcom", ex.StackTrace);

                    ModelState.AddModelError(string.Empty, "Error: IUnknown error, please contact support. Ref: " + errRefNumber);
                }

                #endregion Handle Transaction response
            }

            return View(model);
        }

        public ActionResult ReturnUrl()
        {
            return View();
        }

        private void GetddlData()
        {
            action = "fetching the ddl list";
            var val = 0;

            #region ddl Reasons for refund
            var ddlreasons = new List<SelectListItem>();

            ddlreasons.Add(new SelectListItem()
            {
                Value = "1",
                Text = "A mistake has been made"
            });

            ddlreasons.Add(new SelectListItem()
            {
                Value = "2",
                Text = "Broken Product"
            });

            ddlreasons.Add(new SelectListItem()
            {
                Value = "3",
                Text = "Wrong product delivered"
            });

            ViewBag.Reasons = ddlreasons;

            #endregion

            #region ddl Branches

            var merchantBranches = _branchRepo.GetAllBranchesByMerchant(CurrentUser.ParentId, "");
            var ddlBranches = new List<SelectListItem>();

            if (merchantBranches.Count > 0)
            {
                if (merchantBranches.Count == 1)
                {
                    ddlBranches.AddRange(merchantBranches.Select(p => new SelectListItem()
                    {
                        Value = p.MerchantBranchId.ToString(),
                        Text = p.MerchantBranchName + " (M: " + p.Merchant.MerchantName + ")"
                    }).ToList());
                }
                else
                {
                    ddlBranches.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "Select a Branch"
                    });

                    ddlBranches.AddRange(merchantBranches.Select(p => new SelectListItem()
                    {
                        Value = p.MerchantBranchId.ToString(),
                        Text = p.MerchantBranchName + " (M: " + p.Merchant.MerchantName + ")"
                    }).ToList());
                }
            }
            else
            {
                ddlBranches.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "No Merchant Branches available"
                });
            }

            ViewBag.Branches = ddlBranches;
            #endregion

            #region ddl Currency
            var currencies = _refRepo.GetAllCurrencies();
            var ddlcurrency = new List<SelectListItem>();

            //ddlcurrency.Add(new SelectListItem()
            //{
            //    Value = "0",
            //    Text = "Select Currency",
            //    Selected = val == 0
            //});

            ddlcurrency.AddRange(currencies.Select(c => new SelectListItem()
            {
                Value = c.CurrencyCode,
                Text = c.CurrencyCode,
                Selected = c.CurrencyId == 2
            }).ToList());

            ViewBag.Currencies = ddlcurrency;
            #endregion

            #region ddl Expiry Month
            var ddlExpiryMonth = new List<SelectListItem>();

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "0",
                Text = "Expiry date: MM"
            });

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "01",
                Text = "1"
            });

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "02",
                Text = "2"
            });

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "03",
                Text = "3"
            });

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "04",
                Text = "4"
            });

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "05",
                Text = "5"
            });

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "06",
                Text = "6"
            });

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "07",
                Text = "7"
            });

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "08",
                Text = "8"
            });

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "09",
                Text = "9"
            });

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "10",
                Text = "10"
            });


            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "11",
                Text = "11"
            });

            ddlExpiryMonth.Add(new SelectListItem()
            {
                Value = "12",
                Text = "12"
            });
            ViewBag.ExpiryMonth = ddlExpiryMonth;

            #endregion

            #region ddl Expiry Year
            var ddlExpiryYear = new List<SelectListItem>();

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "0",
                Text = "Expiry date: YY"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "17",
                Text = "2017"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "18",
                Text = "2018"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "19",
                Text = "2019"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "20",
                Text = "2020"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "21",
                Text = "2021"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "22",
                Text = "2022"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "23",
                Text = "2023"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "24",
                Text = "2024"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "25",
                Text = "2025"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "26",
                Text = "2026"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "27",
                Text = "2027"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "28",
                Text = "2028"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "29",
                Text = "2029"
            });

            ddlExpiryYear.Add(new SelectListItem()
            {
                Value = "30",
                Text = "2030"
            });

            ViewBag.ExpiryYear = ddlExpiryYear;

            #endregion

            #region User DDL
            var mAppDetails = _mobileAppRepo.GetMobileAppDetailsByPosId(CurrentUser.ParentId);

            var users = _userRepo.GetUsersbyMerchantId(CurrentUser.ParentId);

            var ddlUsers = new List<SelectListItem>();

            ddlUsers.AddRange(users.Select(u => new SelectListItem()
            {
                Value = u.AccountId.ToString(),
                Text = u.User.FirstName + " " + u.User.LastName
            }).ToList());

            ViewBag.Users = ddlUsers;
            #endregion

            #region Card Types
            var cardTypes = _refRepo.GetAllCardTypes();

            ViewBag.CardTypes = cardTypes.Select(c => new SelectListItem()
            {
                Value = c.CardTypeId.ToString(),
                Text = c.TypeName
            }).ToList();

            #endregion

            #region ddlCountry
            var countries = _refRepo.GetAllCountries();

            ViewBag.Countries = countries.Select(c => new SelectListItem()
            {
                Value = c.CountryCode,
                Text = c.CountryName
            }).ToList();
            #endregion
        }

        public void RedirectToPayeco()
        {
            String method = "POST";
            StringBuilder html = new StringBuilder();
            html.Append("<html><head></head><body>")
            .Append("<form id=\"pay_form\" name=\"pay_form\" action=\"")
            .Append(payecoUrl).Append("\" method=\"" + method + "\">\n")
            .Append("<input type=\"hidden\" name=\"" + apiResponseXML + "\">\n")
            .Append("</form>\n")
            .Append("<script language=\"javascript\">window.onload=function(){document.pay_form.submit();}</script>\n")
            .Append("</body></html>");
          Response.Write(html.ToString());
        }
    }
}
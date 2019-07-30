using SDGDAL;
using SDGDAL.Repositories;
using SDGWebService.Classes;
using System;
using System.Configuration;
using System.Linq;

namespace SDGWebService.Functions
{
    public class MobileAppFunctions
    {
        #region Instances

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

        private int _tokenExpirationInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["RTokenExpirationByMinutes"]);

        #endregion Instances

        #region PRIVATE METHODS

        public bool CheckObject(object obj)
        {
            return obj == null ? true : false;
        }

        public bool CheckDetails(Functions.Enums.METHODS eMethod, object request, out object oResponse)
        {
            POSWSResponse response = new POSWSResponse();
            bool retValue = false;
            switch (eMethod)
            {
                case Functions.Enums.METHODS.ACTIVATE_APP:
                    MobileDeviceInfo mobileInfo = (MobileDeviceInfo)request;
                    if (CheckObject(mobileInfo.POSWSRequest)
                        || string.IsNullOrEmpty(mobileInfo.Platform)
                        || string.IsNullOrEmpty(mobileInfo.POSWSRequest.ActivationKey)
                        || string.IsNullOrEmpty(mobileInfo.PosType))
                    {
                        response = GetError(out retValue);
                    }
                    break;

                case Functions.Enums.METHODS.TRANSACTION_VOID_REASON:
                case Functions.Enums.METHODS.UPDATE_APP:
                case Functions.Enums.METHODS.UPDATE_APP_COMPLETED:
                    POSWSRequest posRequest = (POSWSRequest)request;
                    if (string.IsNullOrEmpty(posRequest.SystemMode)
                        || string.IsNullOrEmpty(posRequest.ActivationKey))
                    {
                        response = GetError(out retValue);
                    }
                    break;

                case Functions.Enums.METHODS.INSTALL_APP:
                    InstallAppRequest insAppRequest = (InstallAppRequest)request;
                    if (CheckObject(insAppRequest.POSWSRequest)
                        || string.IsNullOrEmpty(insAppRequest.POSWSRequest.SystemMode)
                        || string.IsNullOrEmpty(insAppRequest.POSWSRequest.ActivationKey)
                        || string.IsNullOrEmpty(insAppRequest.PackageName))
                    {
                        response = GetError(out retValue);
                    }
                    break;

                case Functions.Enums.METHODS.LOGIN:
                    LoginRequest loginRequest = (LoginRequest)request;
                    if (CheckObject(loginRequest.POSWSRequest)
                        || string.IsNullOrEmpty(loginRequest.POSWSRequest.SystemMode)
                        || string.IsNullOrEmpty(loginRequest.POSWSRequest.ActivationKey)
                        || string.IsNullOrEmpty(loginRequest.Username)
                        || string.IsNullOrEmpty(loginRequest.Password)
                        || string.IsNullOrEmpty(loginRequest.AppVersion))
                    {
                        response = GetError(out retValue);
                    }
                    break;

                case Functions.Enums.METHODS.ISALIVE:
                    string MobAppVersion = System.Configuration.ConfigurationManager.AppSettings["MobAppVersion"].ToString();

                    if (MobAppVersion != Convert.ToString(request))
                    {
                        response.Status = "Declined";
                        response.Message = "A newer version of this APP is available, please download the latest version.";
                        response.ErrNumber = "1002.1";
                        response.UpdatePending = false;
                        retValue = true;
                    }
                    break;

                case Functions.Enums.METHODS.TRANS_LOOKUP:
                    TransLookupRequest transLookRequest = (TransLookupRequest)request;
                    if (CheckObject(transLookRequest.POSWSRequest)
                        || string.IsNullOrEmpty(transLookRequest.POSWSRequest.SystemMode)
                        || string.IsNullOrEmpty(transLookRequest.POSWSRequest.ActivationKey)
                        || string.IsNullOrEmpty(transLookRequest.POSWSRequest.RToken)
                        || string.IsNullOrEmpty(transLookRequest.SearchCriteria))
                    {
                        response = GetError(out retValue);
                    }
                    break;

                case Functions.Enums.METHODS.SEND_RECEIPT:

                    break;

                case Functions.Enums.METHODS.SEND_SMS_RECEIPT:
                    SmsRequest smsRequest = (SmsRequest)request;
                    if (CheckObject(smsRequest.POSWSRequest)
                        || string.IsNullOrEmpty(smsRequest.POSWSRequest.SystemMode)
                        || string.IsNullOrEmpty(smsRequest.POSWSRequest.ActivationKey)
                        || string.IsNullOrEmpty(smsRequest.TransNumber)
                        || string.IsNullOrEmpty(smsRequest.CountryCode)
                        || string.IsNullOrEmpty(smsRequest.PhoneNumber))
                    {
                        response = GetError(out retValue);
                    }
                    break;

                case Functions.Enums.METHODS.EMAIL_RECEIPT:
                    EmailReceiptRequest emailRequest = (EmailReceiptRequest)request;
                    if (CheckObject(emailRequest.POSWSRequest)
                        || string.IsNullOrEmpty(emailRequest.POSWSRequest.SystemMode)
                        || string.IsNullOrEmpty(emailRequest.POSWSRequest.ActivationKey)
                        || string.IsNullOrEmpty(emailRequest.TransNumber)
                        || string.IsNullOrEmpty(emailRequest.EmailDetails.Email)
                        || string.IsNullOrEmpty(emailRequest.EmailDetails.ReceiptHeader)
                        || string.IsNullOrEmpty(emailRequest.EmailDetails.ReceiptSec1)
                        || string.IsNullOrEmpty(emailRequest.EmailDetails.ReceiptSec2)
                        || string.IsNullOrEmpty(emailRequest.EmailDetails.ReceiptSec3)
                        || string.IsNullOrEmpty(emailRequest.EmailDetails.ReceiptFooter))
                    {
                        response = GetError(out retValue);
                    }
                    break;

                case Functions.Enums.METHODS.PRINT_RECEIPT: break;
                case Functions.Enums.METHODS.CREATE_TICKET:
                    CreateTicketRequest ticketRequest = (CreateTicketRequest)request;
                    if (CheckObject(ticketRequest.EmailDetails)
                        || CheckObject(ticketRequest.POSWSRequest)
                        || string.IsNullOrEmpty(ticketRequest.POSWSRequest.SystemMode)
                        || string.IsNullOrEmpty(ticketRequest.POSWSRequest.ActivationKey)
                        || string.IsNullOrEmpty(ticketRequest.TicketNumber)
                        || string.IsNullOrEmpty(ticketRequest.EmailDetails.Email)
                        || string.IsNullOrEmpty(ticketRequest.EmailDetails.ReceiptHeader)
                        || string.IsNullOrEmpty(ticketRequest.EmailDetails.ReceiptSec1)
                        || string.IsNullOrEmpty(ticketRequest.EmailDetails.ReceiptSec2)
                        || string.IsNullOrEmpty(ticketRequest.EmailDetails.ReceiptSec3)
                        || string.IsNullOrEmpty(ticketRequest.EmailDetails.ReceiptFooter))
                    {
                        response = GetError(out retValue);
                    }
                    break;

                case Functions.Enums.METHODS.TRANSACTION_PURCHASE_CREDIT_SWIPE:
                    TransactionRequest creditRequest = (TransactionRequest)request;
                    if (CheckObject(creditRequest.POSWSRequest)
                        || string.IsNullOrEmpty(creditRequest.POSWSRequest.RToken)
                        || string.IsNullOrEmpty(creditRequest.POSWSRequest.SystemMode)
                        || creditRequest.Device <= 0
                        || string.IsNullOrEmpty(creditRequest.LanguageUser)
                        || string.IsNullOrEmpty(creditRequest.CardDetails.Currency)
                        || string.IsNullOrEmpty(creditRequest.CardDetails.RefNumberApp)
                        || string.IsNullOrEmpty(creditRequest.CardDetails.Ksn)
                        || string.IsNullOrEmpty(creditRequest.CardDetails.ExpMonth)
                        || string.IsNullOrEmpty(creditRequest.CardDetails.ExpYear)
                        || creditRequest.CardDetails.Amount <= 0
                        || creditRequest.DeviceId <= 0)
                    {
                        response = GetError(out retValue);
                    }
                    break;

                case Functions.Enums.METHODS.TRANSACTION_PURCHASE_CREDIT_EMV:
                    TransactionRequest emvCreditReq = (TransactionRequest)request;
                    if (CheckObject(emvCreditReq.POSWSRequest)
                        || string.IsNullOrEmpty(emvCreditReq.POSWSRequest.RToken)
                        || string.IsNullOrEmpty(emvCreditReq.POSWSRequest.SystemMode)
                        || emvCreditReq.Device <= 0
                        || string.IsNullOrEmpty(emvCreditReq.LanguageUser)
                        || string.IsNullOrEmpty(emvCreditReq.CardDetails.Currency)
                        || string.IsNullOrEmpty(emvCreditReq.CardDetails.EmvICCData)
                        || string.IsNullOrEmpty(emvCreditReq.CardDetails.RefNumberApp)
                        || emvCreditReq.CardDetails.Amount <= 0
                        || emvCreditReq.DeviceId <= 0)
                    {
                        response = GetError(out retValue);
                    }

                    break;

                case Functions.Enums.METHODS.TRANSACTION_PURCHASE_CASH:
                    CashTransactionRequest cashReq = (CashTransactionRequest)request;
                    if (CheckObject(cashReq.POSWSRequest)
                        || string.IsNullOrEmpty(cashReq.POSWSRequest.RToken)
                        || string.IsNullOrEmpty(cashReq.POSWSRequest.SystemMode)
                        || string.IsNullOrEmpty(cashReq.LanguageUser)
                        || string.IsNullOrEmpty(cashReq.Currency)
                        || string.IsNullOrEmpty(cashReq.RefNumberApp)
                        || cashReq.Amount <= 0)
                    {
                        response = GetError(out retValue);
                    }

                    break;

                case Functions.Enums.METHODS.TRANSACTION_PURCHASE_MANUAL:
                    CreditManualRequest creditreq = (CreditManualRequest)request;
                    if (CheckObject(creditreq.POSWSRequest)
                        || string.IsNullOrEmpty(creditreq.POSWSRequest.SystemMode)
                        || string.IsNullOrEmpty(creditreq.LanguageUser)
                        || string.IsNullOrEmpty(creditreq.NameOnCard)
                        || string.IsNullOrEmpty(creditreq.Currency)
                        || string.IsNullOrEmpty(creditreq.RefNumberApp)
                        || string.IsNullOrEmpty(creditreq.EncCardDetails)
                        || string.IsNullOrEmpty(creditreq.CustomerInfo.Address)
                        || string.IsNullOrEmpty(creditreq.CustomerInfo.City)
                        || string.IsNullOrEmpty(creditreq.CustomerInfo.Country)
                        || string.IsNullOrEmpty(creditreq.CustomerInfo.FirstName)
                        || string.IsNullOrEmpty(creditreq.CustomerInfo.LastName)
                        || string.IsNullOrEmpty(creditreq.CustomerInfo.PhoneNumber)
                        || string.IsNullOrEmpty(creditreq.CustomerInfo.State)
                        || string.IsNullOrEmpty(creditreq.CustomerInfo.Zip)
                        || creditreq.Amount <= 0)
                    {
                        response = GetError(out retValue);
                    }

                    break;
                
            }
            oResponse = response;
            return retValue;
        }

        public POSWSResponse GetError(out bool oValue)
        {
            POSWSResponse response = new POSWSResponse();
            response.ErrNumber = "1001.1";
            response.Message = "Missing input";
            response.Status = "DECLINED";
            oValue = true;
            return response;
        }

        public POSWSResponse CheckMobileAppStatus(string activationKey, out SDGDAL.Entities.MobileApp mobileApp)
        {
            POSWSResponse response = new POSWSResponse();

            mobileApp = _mAppRepo.GetMobileAppFullDetailsByActivationCode(activationKey);

            if (mobileApp == null)
            {
                response.ErrNumber = "2001.1";
                response.Message = "No record found.";
                response.Status = "Declined";
            }
            else
            {
                if (mobileApp.IsDeleted
                    || mobileApp.MerchantBranchPOS.IsDeleted
                    || mobileApp.MerchantBranchPOS.MerchantBranch.IsDeleted
                    || mobileApp.MerchantBranchPOS.MerchantBranch.Merchant.IsDeleted)
                {
                    response.ErrNumber = "2001.1";
                    response.Message = "No record found.";
                    response.Status = "Declined";
                }
                else
                {
                    if (!mobileApp.IsActive
                        || !mobileApp.MerchantBranchPOS.IsActive
                        || !mobileApp.MerchantBranchPOS.MerchantBranch.IsActive
                        || !mobileApp.MerchantBranchPOS.MerchantBranch.Merchant.IsActive)
                    {
                        response.ErrNumber = "2001.5";
                        response.Message = "POS is currently disabled.";
                        response.Status = "Declined";
                    }
                }
            }

            return response;
        }

        public POSWSResponse CheckStatus(string rTokenLog, out SDGDAL.Entities.MobileApp mobileApp, out SDGDAL.Entities.Account account)
        {
            POSWSResponse response = new POSWSResponse();

            var tokenLog = _mAppRepo.GetMobileAppTokenLog(rTokenLog);

            _mAppRepo = new MobileAppRepository();

            mobileApp = new SDGDAL.Entities.MobileApp();
            account = new SDGDAL.Entities.Account();

            mobileApp = tokenLog.MobileApp;
            account = tokenLog.Account;

            if (mobileApp == null || account == null)
            {
                response.ErrNumber = "2001.1";
                response.Message = "No record found.";
                response.Status = "Declined";
            }
            else
            {
                if (mobileApp.IsDeleted
                    || mobileApp.MerchantBranchPOS.IsDeleted
                    || mobileApp.MerchantBranchPOS.MerchantBranch.IsDeleted
                    || mobileApp.MerchantBranchPOS.MerchantBranch.Merchant.IsDeleted)
                {
                    response.ErrNumber = "2001.1";
                    response.Message = "No record found.";
                    response.Status = "Declined";
                }
                else
                {
                    if (!mobileApp.IsActive
                        || !mobileApp.MerchantBranchPOS.IsActive
                        || !mobileApp.MerchantBranchPOS.MerchantBranch.IsActive
                        || !mobileApp.MerchantBranchPOS.MerchantBranch.Merchant.IsActive)
                    {
                        response.ErrNumber = "2001.5";
                        response.Message = "POS is currently disabled.";
                        response.Status = "Declined";
                    }
                }

                if (!account.IsActive || account.IsDeleted)
                {
                    response.ErrNumber = "2001.6";
                    response.Message = "Employee account is currently disabled.";
                    response.Status = "Declined";
                }

                if (tokenLog.ExpirationDate < DateTime.Now)
                {
                    response.ErrNumber = "2001.7";
                    response.Message = "Session expired, please login again.";
                    response.Status = "Declined";
                }
                else
                {
                    // Update token expiration
                    var ntokenLog = new SDGDAL.Entities.MobileAppTokenLog();

                    tokenLog.ExpirationDate = DateTime.Now.AddMinutes(_tokenExpirationInMinutes);

                    ntokenLog = _mAppRepo.UpdateMobileAppTokenLog(tokenLog);
                }
            }

            return response;
        }

        public POSWSResponse CheckMidsStatusForTransaction(int posId, int cardTypeId, out SDGDAL.Entities.Mid mid)
        {
            POSWSResponse response = new POSWSResponse();

            mid = _midsRepo.GetMidByPosIdAndCardTypeId(posId, cardTypeId);

            if (mid == null)
            {
                response.ErrNumber = "2110.1";
                response.Message = "No mids found.";
                response.Status = "Declined";
            }
            else
            {
                if (mid.IsDeleted || !mid.IsActive)
                {
                    response.ErrNumber = "2110.1";
                    response.Message = "No mids found.";
                    response.Status = "Declined";
                }
                else
                {
                    //if (!mid.Switch.IsActive)
                    //{
                    //    response.ErrNumber = "2110.2";
                    //    response.Message = "Mid's switch is currently disabled.";
                    //    response.Status = "Declined";
                    //}
                    //else
                    //{
                    //    // TODO: Check for Merchant and Merchant Branch status
                    //}
                }
            }

            return response;
        }

        public string GenerateRequestToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());

            return Utility.Encrypt(token);
        }

        public void LogMobileAppAction(string action, int mobileAppId, int accountId, decimal gpsLat, decimal gpsLong)
        {
            try
            {
                var mobileAppLog = new SDGDAL.Entities.MobileAppLog();

                mobileAppLog.LogDetails = action;
                mobileAppLog.AccountId = mobileAppId;
                mobileAppLog.DateLogged = DateTime.Now;
                mobileAppLog.MobileAppId = mobileAppId;
                mobileAppLog.GPSLat = gpsLat;
                mobileAppLog.GPSLong = gpsLong;

                var rLog = _mAppRepo.LogMobileAppAction(mobileAppLog);

                if (rLog.MobileAppLogId > 0)
                {
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                // For now, do nothing
            }
        }

        #endregion PRIVATE METHODS

        #region ENUM Methods

        private enum METHODS
        {
            ACTIVATE_APP = 1,
            UPDATE_APP = 2,
            UPDATE_APP_COMPLETED = 3,
            INSTALL_APP = 4,
            LOGIN = 5,
            ISALIVE = 6,
            TRANS_LOOKUP = 7,
            SEND_RECEIPT = 8,
            SEND_SMS_RECEIPT = 9,
            EMAIL_RECEIPT = 10,
            PRINT_RECEIPT = 11,
            CREATE_TICKET = 12,
            TRANSACTION_PURCHASE_CREDIT_SWIPE = 13,
            CREDIT_TRANSACTION_VOID_REFUND_SWIPE = 14,
            TRANSACTION_PURCHASE_DEBIT_SWIPE = 15,
            TRANSACTION_VOID_REASON = 16,
            TRANSACTION_PURCHASE_CREDIT_EMV = 17,
            TRANSACTION_PURCHASE_CASH = 18,
        }

        #endregion ENUM Methods
    }
}
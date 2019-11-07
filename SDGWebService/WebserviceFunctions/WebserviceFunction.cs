using Newtonsoft.Json.Linq;
using SDGDAL;
using SDGDAL.Repositories;
using SDGUtil;
using SDGWebService.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using JWT;
using JWT.Serializers;
using Newtonsoft.Json;

namespace SDGWebService.WebserviceFunctions
{
    public class WebserviceFunction
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
        private ReferenceRepository _rerRepo = new ReferenceRepository();
        private BatchReportRepository _batchRepo = new BatchReportRepository();

        private int _tokenExpirationInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["RTokenExpirationByMinutes"]);

        public POSWSResponse ActivateApp(MobileDeviceInfo posInfo)
        {
            POSWSResponse response = new POSWSResponse();

            string action = string.Empty;
            object obj = response;

            if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.ACTIVATE_APP, (object)posInfo, out obj))
            {
                return (POSWSResponse)obj;
            }

            try
            {
                var mobileAppLog = new SDGDAL.Entities.MobileAppLog();
                action = "fetching full details of mobile app based on activation key.";
                var mobileApp = _mAppRepo.GetMobileAppFullDetailsByActivationCode(posInfo.POSWSRequest.ActivationKey);

                action = "";
                if (mobileApp == null)
                {
                    response.ErrNumber = "2001.1";
                    response.Message = "No record found.";
                    response.Status = "Declined";
                    return response;
                }
                else
                {
                    if (mobileApp.IsDeleted)
                    {
                        response.ErrNumber = "2001.1";
                        response.Message = "No record found.";
                        response.Status = "Declined";
                        return response;
                    }
                    else
                    {
                        if (mobileApp.IsActive)
                        {
                            response.ErrNumber = "2001.2";
                            response.Message = "The code is already been used, please contact support.";
                            response.Status = "Declined";
                            return response;
                        }
                        else
                        {
                            if (mobileApp.MerchantBranchPOS.IsDeleted
                                || mobileApp.MerchantBranchPOS.MerchantBranch.IsDeleted
                                || mobileApp.MerchantBranchPOS.MerchantBranch.Merchant.IsDeleted)
                            {
                                response.ErrNumber = "2001.3";
                                response.Message = "No record found.";
                                response.Status = "Declined";
                                return response;
                            }
                            else if (!mobileApp.MerchantBranchPOS.IsActive
                                || !mobileApp.MerchantBranchPOS.MerchantBranch.IsActive
                                || !mobileApp.MerchantBranchPOS.MerchantBranch.Merchant.IsActive)
                            {
                                response.ErrNumber = "2001.4";
                                response.Message = "There was a problem activating the app, please contact support.";
                                response.Status = "Declined";
                                return response;
                            }
                            else
                            {
                                // Update Mobile App
                                action = "setting mobile app info for update.";
                                mobileApp.DateActivated = DateTime.Now;
                                mobileApp.ExpirationDate = DateTime.Now.AddYears(1);
                                mobileApp.GPSLat = posInfo.POSWSRequest.GPSLat;
                                mobileApp.GPSLong = posInfo.POSWSRequest.GPSLong;
                                mobileApp.IsActive = true;
                                mobileApp.UpdatePending = true;

                                action = "setting mobile device info for mobile app.";
                                var deviceInfo = new SDGDAL.Entities.MobileDeviceInfo();

                                deviceInfo.IMEI = posInfo.IMEI;
                                deviceInfo.IP = posInfo.IP;
                                deviceInfo.OS = posInfo.OS;
                                deviceInfo.Model = posInfo.Model;
                                deviceInfo.Manufacturer = posInfo.Manufacturer;
                                deviceInfo.PhoneNumber = posInfo.PhoneNumber;
                                deviceInfo.Platform = posInfo.Platform;
                                deviceInfo.DateCreated = DateTime.Now;

                                mobileApp.MobileDeviceInfo = deviceInfo;

                                action = "updating mobile app with new details and device info.";
                                var nMobileApp = _mAppRepo.UpdateMobileApp(mobileApp);

                                if (nMobileApp != null)
                                {
                                    try
                                    {
                                        action = "log mobile app action.";

                                        mobileAppLog.LogDetails = "App Activation";
                                        mobileAppLog.DateLogged = DateTime.Now;
                                        mobileAppLog.MobileAppId = mobileApp.MobileAppId;
                                        mobileAppLog.GPSLat = posInfo.POSWSRequest.GPSLat;
                                        mobileAppLog.GPSLong = posInfo.POSWSRequest.GPSLong;

                                        var rLog = _mAppRepo.LogMobileAppAction(mobileAppLog);

                                        if (rLog.MobileAppLogId > 0)
                                        {
                                        }

                                        //Get the MerchantID
                                        var merchantInfo = _mAppRepo.GetMobileAppDetailsByMobileAppId(mobileApp.MobileAppId);

                                        if (merchantInfo != null)
                                        {
                                            var accountInfo = _userRepo.GetDetailsbyParentIdAndParentTypeId(Convert.ToInt32(SDGDAL.Enums.ParentType.Merchant), merchantInfo.MerchantBranchPOS.MerchantBranch.Merchant.MerchantId);
                                            mobileAppLog.AccountId = accountInfo.AccountId;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        response.ErrNumber = "-1";
                                        response.Status = "Declined";
                                        response.Message = ex.Message + " while " + action;

                                        return response;
                                    }

                                    response.AccountId = Convert.ToString(mobileAppLog.AccountId);
                                    response.MobileAppId = Convert.ToString(nMobileApp.MobileAppId);
                                    response.ErrNumber = "0";
                                    response.Status = "Approved";
                                    response.Message = "";
                                    response.UpdatePending = true;
                                }
                                else
                                {
                                    throw new Exception("Unknown error, need to debug.");
                                }

                                return response;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "ActivateApp", ex.StackTrace);

                response.ErrNumber = "1001.2";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber + ex.Message;
                response.Status = "Declined";
                return response;
            }
        }

        public POSFeaturesResponse UpdateApp(POSWSRequest request)
        {
            string action = string.Empty;

            POSFeaturesResponse featuresResponse = new POSFeaturesResponse();

            POSWSResponse response = new POSWSResponse();
            object obj = response;
            if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.UPDATE_APP, (object)request, out obj))
            {
                featuresResponse.POSWSResponse = (POSWSResponse)obj;
                return featuresResponse;
            }

            try
            {
                if (request.SystemMode.ToUpper().Equals("LIVE"))
                {
                    var mobileApp = new SDGDAL.Entities.MobileApp();

                    action = "checking mobile app availability.";
                    response = mobileAppFunctions.CheckMobileAppStatus(request.ActivationKey, out mobileApp);

                    if (response.Status == "Declined")
                    {
                        featuresResponse.POSWSResponse = response;
                        return featuresResponse;
                    }

                    action = "log mobile app action.";
                    mobileAppFunctions.LogMobileAppAction("Update App", mobileApp.MobileAppId, 0, request.GPSLat, request.GPSLong);

                    action = "";

                    if (mobileApp == null)
                    {
                        response.ErrNumber = "2002.1";
                        response.Message = "No record found.";
                        response.Status = "Declined";
                        response.UpdatePending = true;
                    }
                    else
                    {
                        action = "setting pos features for response.";
                        featuresResponse.HelpOpenHour = System.Configuration.ConfigurationManager.AppSettings["AppHelpOpenHour"].ToString();
                        featuresResponse.HelpClosedHour = System.Configuration.ConfigurationManager.AppSettings["AppHelpClosedHour"].ToString();
                        featuresResponse.HelpGMT = System.Configuration.ConfigurationManager.AppSettings["AppHelpGMT"].ToString();
                        featuresResponse.HelpContactNumber = System.Configuration.ConfigurationManager.AppSettings["AppHelpTeleNumber"].ToString();

                        // Default features
                        if (mobileApp.MobileAppFeaturesId == null)
                        {
                            featuresResponse.SystemMode = "LIVE";
                            featuresResponse.GPSEnabled = true;

                            featuresResponse.ReferenceNumber = true;

                            featuresResponse.CreditTransaction = true;
                            featuresResponse.DebitTransaction = false;
                            featuresResponse.ChequeTransaction = false;
                            featuresResponse.CreditSignature = true;
                            featuresResponse.DebitSignature = false;
                            featuresResponse.ChequeSignature = false;
                            featuresResponse.DebitRefund = true;
                            featuresResponse.ChequeProofId = true;
                            featuresResponse.ChequeType = false;
                            featuresResponse.CashSignature = false;
                            featuresResponse.CashTransaction = true;

                            featuresResponse.GiftAllowed = false;
                            featuresResponse.EmailAllowed = true;
                            featuresResponse.SMSEnabled = false;
                            featuresResponse.PrintAllowed = true;
                            featuresResponse.TipsEnabled = false;

                            featuresResponse.Currency = "PHP"; // TODO: For now
                        }
                        else
                        {
                            featuresResponse.SystemMode = mobileApp.MobileAppFeatures.SystemMode;
                            featuresResponse.GPSEnabled = mobileApp.MobileAppFeatures.GPSEnabled;

                            featuresResponse.ReferenceNumber = mobileApp.MobileAppFeatures.ReferenceNumber;

                            featuresResponse.CreditTransaction = mobileApp.MobileAppFeatures.CreditTransaction;
                            featuresResponse.DebitTransaction = mobileApp.MobileAppFeatures.DebitTransaction;
                            featuresResponse.ChequeTransaction = mobileApp.MobileAppFeatures.ChequeTransaction;
                            featuresResponse.CreditSignature = mobileApp.MobileAppFeatures.CreditSignature;
                            featuresResponse.DebitSignature = mobileApp.MobileAppFeatures.DebitSignature;
                            featuresResponse.ChequeSignature = mobileApp.MobileAppFeatures.ChequeSignature;
                            featuresResponse.DebitRefund = mobileApp.MobileAppFeatures.DebitRefund;
                            featuresResponse.ChequeProofId = mobileApp.MobileAppFeatures.ProofId;
                            featuresResponse.ChequeType = mobileApp.MobileAppFeatures.ChequeType;
                            featuresResponse.CashSignature = mobileApp.MobileAppFeatures.CashSignature;
                            featuresResponse.CashTransaction = mobileApp.MobileAppFeatures.CashTransaction;
                            featuresResponse.DebitBalanceCheck = mobileApp.MobileAppFeatures.BalanceInquiry;
                            featuresResponse.BillsPayment = mobileApp.MobileAppFeatures.BillsPayment;

                            featuresResponse.GiftAllowed = mobileApp.MobileAppFeatures.GiftAllowed;
                            featuresResponse.EmailAllowed = mobileApp.MobileAppFeatures.EmailAllowed;
                            featuresResponse.SMSEnabled = mobileApp.MobileAppFeatures.SMSEnabled;
                            featuresResponse.PrintAllowed = mobileApp.MobileAppFeatures.PrintAllowed;
                            featuresResponse.TipsEnabled = mobileApp.MobileAppFeatures.TipsEnabled;
                            featuresResponse.Currency = mobileApp.MobileAppFeatures.Currency.CurrencyCode;

                            if (featuresResponse.TipsEnabled)
                            {
                                // Tips (specific amount for verification to give tips)
                                featuresResponse.Amount1 = mobileApp.MobileAppFeatures.Amount1;
                                featuresResponse.Amount2 = mobileApp.MobileAppFeatures.Amount2;
                                featuresResponse.Amount3 = mobileApp.MobileAppFeatures.Amount3;

                                // Tips (percentage for tips when specific amount is achieved)
                                featuresResponse.Percentage1 = mobileApp.MobileAppFeatures.Percentage1;
                                featuresResponse.Percentage2 = mobileApp.MobileAppFeatures.Percentage2;
                                featuresResponse.Percentage2 = mobileApp.MobileAppFeatures.Percentage3;
                            }
                        }

                        featuresResponse.MerchantDetails = new MerchantDetails();
                        var merchant = mobileApp.MerchantBranchPOS.MerchantBranch.Merchant;

                        featuresResponse.MerchantDetails.MerchantId = Convert.ToString(merchant.MerchantId);
                        featuresResponse.MerchantDetails.MerchantName = merchant.MerchantName;
                        featuresResponse.MerchantDetails.Address = merchant.ContactInformation.Address;
                        featuresResponse.MerchantDetails.City = merchant.ContactInformation.City;
                        featuresResponse.MerchantDetails.State = merchant.ContactInformation.StateProvince;
                        featuresResponse.MerchantDetails.Zip = merchant.ContactInformation.ZipCode;
                        featuresResponse.MerchantDetails.PrimaryContactNumber = merchant.ContactInformation.PrimaryContactNumber;
                        featuresResponse.MerchantDetails.Country = merchant.ContactInformation.Country.CountryName;
                        featuresResponse.MerchantDetails.DisclaimerEnabled = merchant.MerchantFeatures.DisclaimerEnabled;
                        featuresResponse.MerchantDetails.TOSEnabled = merchant.MerchantFeatures.TermsOfServiceEnabled;
                        featuresResponse.MerchantDetails.BranchName = mobileApp.MerchantBranchPOS.MerchantBranch.MerchantBranchName;

                        action = "fetching devices for merchants";
                        var devicesResult = new List<SDGDAL.Entities.DeviceMerchantLink>();

                        Devices devices = new Devices();
                        List<Devices> MerchantDevices = new List<Devices>();

                        devicesResult = _deviceRepo.GetDevicesByMerchantId(merchant.MerchantId);

                        foreach (var res in devicesResult)
                        {
                            MerchantDevices.Add(new Devices()
                            {
                                DeviceFlowType = res.Device.MasterDevice.FlowType.FlowType,
                                DeviceFlowTypeId = Convert.ToString(res.Device.MasterDevice.FlowType.DeviceFlowTypeId),
                                DeviceId = Convert.ToString(res.DeviceId),
                                SerialNumber = res.Device.SerialNumber,
                                DeviceType = res.Device.MasterDevice.DeviceType.TypeName,
                                MasterDeviceId = Convert.ToString(res.Device.MasterDeviceId)
                            });
                        }

                        featuresResponse.Devices = MerchantDevices;

                        action = "retrieving list of countries";
                        var countryResult = new List<SDGDAL.Entities.Country>();
                        Countries countries = new Countries();
                        List<Countries> countryList = new List<Countries>();

                        countryResult = _rerRepo.GetAllCountries(); 

                        foreach (var res in countryResult)
                        {
                            countryList.Add(new Countries()
                            {
                                CountryCode = res.CountryCode,
                                CountryName = res.CountryName
                            });
                        }

                        featuresResponse.Countries = countryList;

                        #region fetching agreements details
                        //action = "fetching agreements details.";

                        //var agreements = _merchantRepo.GetAgreements(merchant.MerchantId);

                        //if (agreements.Count != 0)
                        //{
                        //    bool useAgreementDefault = true;
                        //    string languageCode = "EN-CA";

                        //    if (merchant.MerchantFeatures != null)
                        //    {
                        //        if (!merchant.MerchantFeatures.UseDefaultAgreements)
                        //        {
                        //            useAgreementDefault = false;
                        //        }

                        //        if (!string.IsNullOrEmpty(merchant.MerchantFeatures.LanguageCode))
                        //        {
                        //            languageCode = merchant.MerchantFeatures.LanguageCode;
                        //        }
                        //    }

                        //    featuresResponse.MerchantDetails.AppLanguage = languageCode;

                        //    if (useAgreementDefault)
                        //    {
                        //        if (agreements.Any(aa => aa.MerchantId == null && aa.LanguageCode == languageCode))
                        //        {
                        //            var agreementDefault = agreements.SingleOrDefault(aa => aa.MerchantId == null && aa.LanguageCode == languageCode);

                        //            if (agreementDefault.Disclaimer == null)
                        //            {
                        //                featuresResponse.Disclaimer = null;
                        //            }
                        //            else
                        //            {
                        //                featuresResponse.Disclaimer = agreementDefault.Disclaimer;
                        //            }

                        //            if (agreementDefault.TermsOfService == null)
                        //            {
                        //                featuresResponse.TermsOfService = null;
                        //            }
                        //            else
                        //            {
                        //                featuresResponse.TermsOfService = agreementDefault.TermsOfService;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            var agreementDefault = agreements.SingleOrDefault(aa => aa.MerchantId == null && aa.LanguageCode == "EN-CA");

                        //            featuresResponse.Disclaimer = string.Empty;
                        //            featuresResponse.TermsOfService = string.Empty;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (agreements.Any(aa => aa.MerchantId == merchant.MerchantId && aa.LanguageCode == languageCode))
                        //        {
                        //            var merchantAgreement = agreements.SingleOrDefault(aa => aa.MerchantId == merchant.MerchantId && aa.LanguageCode == languageCode);

                        //            if (featuresResponse.Disclaimer == null)
                        //            {
                        //                featuresResponse.Disclaimer = null;
                        //            }
                        //            else
                        //            {
                        //                featuresResponse.Disclaimer = merchantAgreement.Disclaimer;
                        //            }

                        //            if (featuresResponse.TermsOfService == null)
                        //            {
                        //                featuresResponse.TermsOfService = null;
                        //            }
                        //            else
                        //            {
                        //                featuresResponse.TermsOfService = merchantAgreement.TermsOfService;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            var merchantAgreement = agreements.SingleOrDefault(aa => aa.MerchantId == merchant.MerchantId && aa.LanguageCode == "EN-CA");

                        //            if (featuresResponse.Disclaimer == null)
                        //            {
                        //                featuresResponse.Disclaimer = null;
                        //            }
                        //            else
                        //            {
                        //                featuresResponse.Disclaimer = merchantAgreement.Disclaimer;
                        //            }

                        //            if (featuresResponse.TermsOfService == null)
                        //            {
                        //                featuresResponse.TermsOfService = null;
                        //            }
                        //            else
                        //            {
                        //                featuresResponse.TermsOfService = merchantAgreement.TermsOfService;
                        //            }
                        //        }
                        //    }
                        //}

                        #endregion

                        response.Status = "Approved";
                        response.Message = "";
                        response.ErrNumber = "0";
                        response.UpdatePending = true;
                    }
                }
                else if (request.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    response.Status = "Approved";
                    response.Message = "";
                    response.ErrNumber = "0";
                }
                else
                {
                    response.Status = "Declined";
                    response.Message = "An error occured while performing the update.";
                    response.ErrNumber = "0";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "UpdateApp", ex.StackTrace);

                response.ErrNumber = "1002.2";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.Status = "Declined";
            }

            featuresResponse.POSWSResponse = response;
            return featuresResponse;
        }

        public POSWSResponse UpdateAppCompleted(POSWSRequest request)
        {
            string action = string.Empty;
            POSWSResponse response = new POSWSResponse();
            object obj = response;

            if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.UPDATE_APP_COMPLETED, (object)request, out obj))
            {
                return (POSWSResponse)obj;
            }

            try
            {
                if (request.SystemMode.ToUpper().Equals("LIVE"))
                {
                    action = "fetching full details of mobile app based on activation key.";
                    var mobileApp = _mAppRepo.GetMobileAppFullDetailsByActivationCode(request.ActivationKey);

                    if (mobileApp == null)
                    {
                        response.ErrNumber = "2003.1";
                        response.Message = "No record found.";
                        response.Status = "Declined";

                        return response;
                    }
                    else
                    {
                        if (mobileApp.IsDeleted || mobileApp.MerchantBranchPOS.IsDeleted)
                        {
                            response.ErrNumber = "2003.1";
                            response.Message = "No record found.";
                            response.Status = "Declined";

                            return response;
                        }
                        else
                        {
                            if (!mobileApp.IsActive || !mobileApp.MerchantBranchPOS.IsActive)
                            {
                                response.ErrNumber = "2003.2";
                                response.Message = "POS is currently disabled.";
                                response.Status = "Declined";

                                return response;
                            }
                        }
                    }

                    try
                    {
                        action = "log mobile app action.";
                        var mobileAppLog = new SDGDAL.Entities.MobileAppLog();

                        mobileAppLog.LogDetails = "Complete update app";
                        mobileAppLog.AccountId = 0;
                        mobileAppLog.DateLogged = DateTime.Now;
                        mobileAppLog.MobileAppId = mobileApp.MobileAppId;
                        mobileAppLog.GPSLat = request.GPSLat;
                        mobileAppLog.GPSLong = request.GPSLong;

                        var rLog = _mAppRepo.LogMobileAppAction(mobileAppLog);

                        if (rLog.MobileAppLogId > 0)
                        {
                        }
                    }
                    catch (Exception ex)
                    {
                        string result = ex.Message;
                    }

                    action = "";

                    mobileApp.UpdatePending = false;

                    mobileApp = _mAppRepo.UpdateMobileAppPendingStatus(mobileApp);

                    if (mobileApp != null)
                    {
                        response.ErrNumber = "0";
                        response.Status = "Approved";
                        response.UpdatePending = false;
                    }
                    else
                    {
                        response.ErrNumber = "0";
                        response.Status = "Declined";
                        response.UpdatePending = true;
                    }
                }
                else if (request.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    response.Status = "Approved";
                    response.Message = "";
                    response.ErrNumber = "0";
                }
                else
                {
                    response.Status = "Declined";
                    response.Message = "An error occured while performing the update.";
                    response.ErrNumber = "2003.4";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "UpdateAppCompleted", ex.StackTrace);

                response.ErrNumber = "2003.5";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.Status = "Declined";
            }

            return response;
        }

        public AppInstallResponse InstallApp(InstallAppRequest request) // NewInstallApp
        {
            string action = string.Empty;

            POSWSResponse response = new POSWSResponse();
            AppInstallResponse appInstallResponse = new AppInstallResponse();
            object obj = response;

            if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.INSTALL_APP, request, out obj))
            {
                appInstallResponse.WSResponse = (POSWSResponse)obj;
                return appInstallResponse;
            }

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    action = "fetching details of mobile app version.";

                    var result = _androidAppVersionRepo.GetAndroidVersionAppByPackageName(request.PackageName);

                    if (result != null)
                    {
                        appInstallResponse.PackageName = result.PackageName;
                        appInstallResponse.VersionBuild = result.VersionBuild;
                        appInstallResponse.VersionCode = result.VersionCode;
                        appInstallResponse.VersionName = result.VersionName;
                        appInstallResponse.AppName = result.AppName;

                        response.ErrNumber = "0";
                        response.Message = "Version is compatible.";
                        response.Status = "Approved";
                        response.UpdatePending = true;
                    }
                    else
                    {
                        response.Status = "Declined";
                        response.Message = "A newer version of this APP is available, please download the latest version.";
                        response.ErrNumber = "2004.1";
                        response.UpdatePending = false;
                    }
                }
                else if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    response.Status = "Approved";
                    response.Message = "";
                }
                else
                {
                    response.Status = "Declined";
                    response.Message = "Failed installing the app.";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "InstallApp", ex.StackTrace);

                response.ErrNumber = "2004.2";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.Status = "Declined";
            }

            appInstallResponse.WSResponse = response;
            return appInstallResponse;
        }

        public RegistrationResponse Registration(RegistrationRequest request)
        {
            string action = string.Empty;
            POSWSResponse response = new POSWSResponse();
            RegistrationResponse registrationResponse = new RegistrationResponse();

            response.ErrNumber = "0";

            if (string.IsNullOrEmpty(request.FullName)
                    || string.IsNullOrEmpty(request.POSWSRequest.SystemMode)
                    || string.IsNullOrEmpty(request.PhoneNumber)
                    || string.IsNullOrEmpty(request.MerchantEmail)
                    || string.IsNullOrEmpty(request.Username)
                    || string.IsNullOrEmpty(request.Password)
                    || string.IsNullOrEmpty(request.ConfirmPassword))
            {
                response.ErrNumber = "1003.1";
                response.Message = "Missing input";
                response.Status = "DECLINED";

                registrationResponse.POSWSResponse = response;
                return registrationResponse;
            }

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    var mobileApp = new SDGDAL.Entities.MobileApp();
                    var account = new SDGDAL.Entities.Account();

                    action = "checking merchant email.";
                    var merchant = _merchantRepo.CheckMerchant(request.MerchantEmail);

                    if (merchant == null || merchant.Count() == 0)
                    {
                        response.ErrNumber = "2005.1";
                        response.Message = "Invalid Merchant Email.";
                        response.Status = "DECLINED";

                        registrationResponse.POSWSResponse = response;
                        return registrationResponse;
                    }

                    action = "checking merchant branch.";
                    var merchantBranches = _merchantRepo.CheckMerchantBranch(merchant[0].MerchantId);

                    if (merchantBranches == null || merchantBranches.Count() == 0)
                    {
                        response.ErrNumber = "2005.2";
                        response.Message = "No branch available to this merchant.";
                        response.Status = "DECLINED";

                        registrationResponse.POSWSResponse = response;
                        return registrationResponse;
                    }

                    if (!_userRepo.IsUserNameAvailable(request.Username))
                    {
                        response.ErrNumber = "2005.3";
                        response.Message = "Username is not available.";
                        response.Status = "DECLINED";

                        registrationResponse.POSWSResponse = response;
                        return registrationResponse;
                    }

                    if (request.Password != request.ConfirmPassword)
                    {
                        response.ErrNumber = "2005.4";
                        response.Message = "Password does not match.";
                        response.Status = "DECLINED";

                        registrationResponse.POSWSResponse = response;
                        return registrationResponse;
                    }

                    action = "generating verification codes/activation codes";
                    var codeGenerator = new SDGUtil.CodeGenerator();

                    string ActivationCode = codeGenerator.FormatCode(4, codeGenerator.GenerateCode(16), "-");
                    string POSName = "POS " + codeGenerator.GenerateCode(4);

                    action = "sending verification code";

                    #region SMS Registration Old
                    //Dictionary<string, string> postData = new Dictionary<string, string>();
                    //postData.Add("To", request.PhoneNumber);
                    //postData.Add("Message", "Your verification code is: " + ActivationCode);

                    //string url = System.Configuration.ConfigurationManager.AppSettings["SMSApiGateway"].ToString();

                    //GatewayProcessor.SMS_Gateway.SendVerificationCode sendCode = new GatewayProcessor.SMS_Gateway.SendVerificationCode();

                    //string result = sendCode.Post(postData, url).Result;

                    //dynamic parseResult = JObject.Parse(result);

                    //var sendResponse = parseResult.SendResponse;
                    #endregion

                    #region Save MobileApp Features

                    action = "creating mobile pos features";
                    var posFeatures = new SDGDAL.Entities.MobileAppFeatures();

                    posFeatures.SystemMode = "LIVE";
                    posFeatures.LanguageCode = "EN-CA";
                    posFeatures.CurrencyId = 1; //PHP
                    posFeatures.GPSEnabled = false;
                    posFeatures.SMSEnabled = false;
                    posFeatures.EmailAllowed = false;
                    posFeatures.EmailLimit = 0;
                    posFeatures.CheckForEmailDuplicates = false;
                    posFeatures.BillingCyclesCheckEmail = 0;
                    posFeatures.PrintAllowed = false;
                    posFeatures.PrintLimit = 0;
                    posFeatures.CheckForPrintDuplicates = false;
                    posFeatures.BillingCyclesCheckPrint = 0;
                    posFeatures.ReferenceNumber = false;
                    posFeatures.CreditSignature = true;
                    posFeatures.DebitSignature = false;
                    posFeatures.ChequeSignature = false;
                    posFeatures.CashSignature = false;
                    posFeatures.CreditTransaction = true;
                    posFeatures.DebitTransaction = true;
                    posFeatures.ChequeTransaction = false;
                    posFeatures.CashTransaction = false;
                    posFeatures.ProofId = false;
                    posFeatures.ChequeType = false;
                    posFeatures.DebitRefund = false;
                    posFeatures.TOSRequired = false;
                    posFeatures.DisclaimerRequired = false;
                    posFeatures.Percentage1 = 0;
                    posFeatures.Percentage2 = 0;
                    posFeatures.Percentage3 = 0;
                    posFeatures.TipsEnabled = false;
                    posFeatures.Amount1 = 0;
                    posFeatures.Amount2 = 0;
                    posFeatures.Amount3 = 0;

                    var features = _posFeatures.CreateMobileAppFeatures(posFeatures);

                    #endregion Save MobileApp Features

                    #region Save POS

                    List<SDGDAL.Entities.MobileApp> mApps = new List<SDGDAL.Entities.MobileApp>();
                    mApps.Add(new SDGDAL.Entities.MobileApp()
                    {
                        ActivationCode = ActivationCode,
                        DateCreated = DateTime.Now,
                        ExpirationDate = DateTime.Now.AddYears(1),
                        UpdatePending = true,
                        MobileAppFeaturesId = features.MobileAppFeaturesId,
                        MerchantBranchPOS = new SDGDAL.Entities.MerchantBranchPOS()
                        {
                            IsActive = true,
                            MerchantPOSName = POSName,
                            DateCreated = DateTime.Now,
                            MerchantBranchId = merchantBranches[0].MerchantBranchId
                        }
                    });

                    bool success = _posRepo.CreateMerchantBranchPOSs(mApps);

                    #endregion Save POS

                    if (success)
                    {
                        action = "creating account and User";
                        var m = new SDGDAL.Entities.Merchant();
                        var u = new SDGDAL.Entities.User();

                        u.FirstName = request.FullName;
                        u.LastName = request.FullName;
                        u.MiddleName = request.FullName;
                        u.EmailAddress = request.MerchantEmail;

                        u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                        {
                            Address = "N/S",
                            City = "N/S",
                            StateProvince = "N/S",
                            CountryId = 185,
                            ZipCode = "N/S",
                            PrimaryContactNumber = request.PhoneNumber,
                            Fax = "N/S",
                            MobileNumber = request.PhoneNumber,
                            NeedsUpdate = true
                        };

                        var acc = new SDGDAL.Entities.Account();
                        acc.Username = request.Username;
                        acc.Password = request.Password;
                        acc.User = u;
                        acc.ParentTypeId = 3;
                        acc.RoleId = 1;

                        m.MerchantId = merchant[0].MerchantId;
                        acc.PIN = "None";

                        var newUser = _merchantRepo.CreateMerchantUser(m, acc);

                        if ((newUser.AccountId > 0) && (newUser.UserId > 0))
                        {
                            try
                            {
                                action = "Checking Email Server Settings";
                                string strMesageBody = "";
                                string strCCEmails = String.Empty;

                                string email = System.Configuration.ConfigurationManager.AppSettings["Email"].ToString();
                                string password = System.Configuration.ConfigurationManager.AppSettings["EmailPassword"].ToString();
                                int emailPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                                string emailHost = System.Configuration.ConfigurationManager.AppSettings["Host"].ToString();
                                bool emailSSL = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["SSL"].ToString());
                                string emailSubject = System.Configuration.ConfigurationManager.AppSettings["EmailSubj"].ToString();
                                string emailServerName = System.Configuration.ConfigurationManager.AppSettings["ServerName"].ToString();

                                string strSender = ""; //partnerEmailServer[0].Email;

                                MailMessage mm = new MailMessage(email, email);

                                mm.Subject = emailSubject;

                                string newReceiptHeader = "Hello " + request.FullName;
                                string newReceiptSec1 = ("Your activation code is ") + ActivationCode;
                                string newReceiptSec2 = "";
                                string newReceiptSec3 = "Thank You,";
                                string newReceiptFooter = "";

                                strMesageBody = "<table> <tr> <td>" + newReceiptHeader + "<br/><br/></td> </tr>" + "<tr><td>" + newReceiptSec1 + "<br/><br/></td> </tr>" + "<tr><td>" + newReceiptSec2 + "<br/><br/></td> </tr>" + "<tr><td>" + newReceiptSec3 + "<br/></td> </tr>" + "<tr><td><br/>" + newReceiptFooter + "</td> </tr></table>";

                                mm.Body = strMesageBody;
                                mm.IsBodyHtml = true;

                                var fromAddressTemp = new MailAddress(email, emailServerName);
                                var fromAddress = new MailAddress(email, password);
                                var toAddress = new MailAddress(request.UserEmail, "");

                                var smtp2 = new SmtpClient
                                {
                                    Host = emailHost,
                                    Port = emailPort,
                                    EnableSsl = emailSSL,
                                    DeliveryMethod = SmtpDeliveryMethod.Network,
                                    UseDefaultCredentials = false,
                                    Credentials = new System.Net.NetworkCredential(email, password),
                                };
                                using (var message = new System.Net.Mail.MailMessage(fromAddress, toAddress)
                                {
                                    IsBodyHtml = true,
                                    Subject = mm.Subject,
                                    Body = mm.Body,
                                    From = fromAddressTemp,
                                })
                                {
                                    try
                                    {
                                        action = "sending email.";
                                        message.CC.Add(request.MerchantEmail);
                                        smtp2.Send(message);
                                    }
                                    catch(Exception ex)
                                    {
                                        response.ErrNumber = "2700.2";
                                        response.Message = "Email not sent. Please try again." + ex.Message;
                                        response.Status = "DECLINED";

                                        registrationResponse.POSWSResponse = response;
                                        return registrationResponse;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log error please
                                var errorOnAction = "Error while " + action;

                                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "RequestAddUser", ex.StackTrace);

                                response.Status = "DECLINED";
                                response.Message = "Unknown error, please contact Support. ";
                                response.ErrNumber = "2700.3";
                            }

                            response.Status = "Approved";
                            response.ErrNumber = "0";
                            response.Message = "Registration Succcessful.Verification code will send to your email";
                        }
                    }
                    else
                    {
                        response.Status = "Declined";
                        response.Message = "Registration failed.";
                        response.ErrNumber = "";
                    }
                }
                else if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    response.Status = "Approved";
                    response.Message = "In test mode. Registration approved.";
                    response.UpdatePending = false;
                    response.ErrNumber = "0";
                }
                else
                {
                    response.Status = "Declined";
                    response.Message = "Registration failed.";
                    response.ErrNumber = "-1";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "Registration", ex.StackTrace);

                response.ErrNumber = "2005.5";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber + " " + errorOnAction;
                response.Status = "DECLINED";
            }

            registrationResponse.POSWSResponse = response;
            return registrationResponse;
        }

        public POSWSResponse ChangePassword(ChangePasswordRequest request)
        {
            string action = String.Empty;
            POSWSResponse response = new POSWSResponse();

            if (string.IsNullOrEmpty(request.OldPassword)
                    || string.IsNullOrEmpty(request.POSWSRequest.SystemMode)
                    || string.IsNullOrEmpty(request.NewPassword)
                    || string.IsNullOrEmpty(request.ConfirmPassword)
                    || request.AccountId <= 0)
            {
                response.ErrNumber = "1001.1";
                response.Message = "Missing input";
                response.Status = "DECLINED";

                return response;
            }

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    var mobileApp = new SDGDAL.Entities.MobileApp();
                    var account = new SDGDAL.Entities.Account();

                    action = "checking mobile app availability.";

                    response = mobileAppFunctions.CheckStatus(request.POSWSRequest.RToken, out mobileApp, out account);

                    if (response.Status == "Declined")
                    {
                        return response;
                    }

                    if (request.NewPassword != request.OldPassword)
                    {
                        response.Status = "DECLINED";
                        response.Message = "New password should not match the current password.";
                        response.ErrNumber = "2006.1";
                    }

                    if (request.NewPassword != request.ConfirmPassword)
                    {
                        response.Status = "DECLINED";
                        response.Message = "Password does not match.";
                        response.ErrNumber = "2006.2";
                    }

                    action = "checking account";
                    var acc = _userRepo.GetUserByAccountId(request.AccountId);

                    if (acc != null)
                    {
                        if (Utility.Decrypt(acc.Password) == request.OldPassword && acc.AccountId == request.AccountId)
                        {
                            var updatAcc = new SDGDAL.Entities.Account();
                            updatAcc.UserId = acc.UserId;
                            updatAcc.Password = request.NewPassword;
                            updatAcc.NeedsUpdate = false;
                            updatAcc.PasswordExpirationDate = acc.PasswordExpirationDate;

                            var result = _userRepo.UpdatePasswordByUserId(updatAcc);

                            if (result != null)
                            {
                                response.Status = "Approved";
                                response.Message = "Password has been changed.";
                                response.ErrNumber = "0";
                            }
                        }
                        else
                        {
                            response.Status = "DECLINED";
                            response.Message = "Invalid User.";
                            response.ErrNumber = "2006.3";
                        }
                    }
                    else
                    {
                        response.Status = "DECLINED";
                        response.Message = "No record found.";
                        response.ErrNumber = "2006.4";
                    }
                }
                else if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    response.Status = "Approved";
                    response.Message = "In test mode. Change Password approved.";
                    response.UpdatePending = false;
                    response.ErrNumber = "0";
                }
                else
                {
                    response.Status = "Declined";
                    response.Message = "Change Password failed.";
                    response.ErrNumber = "2006.5";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "ChangePassword", ex.StackTrace);

                response.ErrNumber = "2006.6";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.Status = "DECLINED";
            }

            return response;
        }

        public POSWSResponse ResendVerificationCode(ResendVerificationRequest request)
        {
            POSWSResponse response = new POSWSResponse();
            string action = String.Empty;

            if (string.IsNullOrEmpty(request.POSWSRequest.SystemMode)
                    || string.IsNullOrEmpty(request.PhoneNumber)
                    || string.IsNullOrEmpty(request.POSWSRequest.ActivationKey))
            {
                response.ErrNumber = "1001.1";
                response.Message = "Missing input";
                response.Status = "DECLINED";

                return response;
            }

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    action = "sending verification code";
                    Dictionary<string, string> postData = new Dictionary<string, string>();
                    postData.Add("To", request.PhoneNumber);
                    postData.Add("Message", "Your verification code is: " + request.POSWSRequest.ActivationKey);

                    string url = System.Configuration.ConfigurationManager.AppSettings["SMSApiGateway"].ToString();

                    GatewayProcessor.SMS_Gateway.SendVerificationCode sendCode = new GatewayProcessor.SMS_Gateway.SendVerificationCode();

                    string result = sendCode.Post(postData, url).Result;

                    dynamic parseResult = JObject.Parse(result);

                    var sendResult = parseResult.SendResponse;

                    if (sendResult != null)
                    {
                        int responseCode = parseResult.SendResponse.ResponseNo;
                        string responseMessage = parseResult.SendResponse.Message;

                        if (responseCode == 20300)
                        {
                            response.Status = "Approved";
                            response.ErrNumber = "0";
                            response.Message = "Verification code will send to your mobile number.";
                        }
                        else
                        {
                            response.Status = "DECLINED";
                            response.ErrNumber = responseCode.ToString();
                            response.Message = responseMessage;
                        }
                    }
                    else
                    {
                        response.Status = "DECLINED";
                        response.ErrNumber = parseResult.LoginResponse.ResponseNo;
                        response.Message = "SMS Gateway Error:" + parseResult.LoginResponse.Message;
                        response.UpdatePending = false;
                    }
                }
                else
                {
                    response.Status = "Declined";
                    response.Message = "Send Verification failed.";
                    response.ErrNumber = "2007.1";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "Resend Verification Code", ex.StackTrace);

                response.ErrNumber = "2006.2";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.Status = "DECLINED";
            }

            return response;
        }

        public POSWSResponse Login(LoginRequest request)
        {
            string action = string.Empty;
            POSWSResponse response = new POSWSResponse();
            object obj = response;
            if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.LOGIN, (object)request, out obj))
            {
                return (POSWSResponse)obj;
            }

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    var account = new SDGDAL.Entities.Account();

                    action = "fetching mobile app details and account details.";
                    var mobileApp = _mAppRepo.GetMobileAppDetailsByCredentialsAndActivationCode(request.Username, request.Password, request.POSWSRequest.ActivationKey, out account);

                    if (mobileApp == null)
                    {
                        response.ErrNumber = "2007.1";
                        response.Message = "Login failed. (0)";
                        response.Status = "Declined";
                        response.UpdatePending = false;
                        return response;
                    }
                    else if (account == null)
                    {
                        response.ErrNumber = "2007.2";
                        response.Message = "Login failed. (1)";
                        response.Status = "Declined";
                        response.UpdatePending = false;
                        return response;
                    }
                    else
                    {
                        if (mobileApp.IsDeleted || mobileApp.MerchantBranchPOS.IsDeleted)
                        {
                            response.ErrNumber = "2007.3";
                            response.Message = "Login failed. (2)";
                            response.Status = "Declined";
                            response.UpdatePending = false;
                            return response;
                        }
                        else
                        {
                            if (!mobileApp.IsActive || !mobileApp.MerchantBranchPOS.IsActive)
                            {
                                response.ErrNumber = "2007.4";
                                response.Message = "Login failed. (3)";
                                response.Status = "Declined";
                                response.UpdatePending = false;
                                return response;
                            }
                        }

                        if (!account.IsActive || account.IsDeleted)
                        {
                            response.ErrNumber = "2007.5";
                            response.Message = "Login failed. (4)";
                            response.Status = "Declined";
                            response.UpdatePending = false;
                            return response;
                        }
                    }

                    var rToken = mobileAppFunctions.GenerateRequestToken();

                    if (string.IsNullOrEmpty(rToken))
                    {
                        throw new Exception("There was a problem creating a new token.");
                    }

                    response.RToken = HttpUtility.UrlEncode(rToken);

                    action = "log mobile app action.";
                    mobileAppFunctions.LogMobileAppAction("Login", mobileApp.MobileAppId, account.AccountId, request.POSWSRequest.GPSLat, request.POSWSRequest.GPSLong);

                    response.ErrNumber = "0";
                    response.Status = "Approved";
                    response.Message = "Login successful.";
                    response.UpdatePending = mobileApp.UpdatePending;
                    response.AccountId = Convert.ToString(account.AccountId);
                    response.SequenceNumber = "";

                    action = "creating mobile app token log";
                    SDGDAL.Entities.MobileAppTokenLog tokenLog = new SDGDAL.Entities.MobileAppTokenLog();
                    tokenLog.RequestToken = response.RToken;
                    tokenLog.MobileAppId = mobileApp.MobileAppId;
                    tokenLog.AccountId = account.AccountId;
                    tokenLog.DateCreated = DateTime.Now;
                    tokenLog.ExpirationDate = DateTime.Now.AddMinutes(_tokenExpirationInMinutes);

                    var ntokenLog = _mAppRepo.CreateMobileAppTokenLog(tokenLog);

                    if (ntokenLog == null)
                    {
                        throw new Exception("Unable to create token.");
                    }
                }
                else if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    if (request.Pin == "1234")
                    {
                        response.Status = "Approved";
                        response.Message = "In test mode. PIN approved.";
                        response.UpdatePending = false;
                        response.ErrNumber = "0";
                        response.SequenceNumber = "12312321";
                    }
                    else
                    {
                        response.Status = "Declined";
                        response.Message = "Login failed. PIN declined";
                        response.ErrNumber = "-1";
                    }
                }
                else
                {
                    response.Status = "Declined";
                    response.Message = "Login failed. PIN declined";
                    response.ErrNumber = "-1";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "Login", ex.StackTrace);

                response.ErrNumber = "2007.6";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.Status = "DECLINED";
            }

            return response;
        }

        public POSWSResponse LoginByMobilePin(LoginRequest request)
        {
            string action = string.Empty;
            POSWSResponse response = new POSWSResponse();

            if (string.IsNullOrEmpty(request.POSWSRequest.SystemMode)
                    || string.IsNullOrEmpty(request.POSWSRequest.ActivationKey)
                    || string.IsNullOrEmpty(request.AppVersion)
                    || string.IsNullOrEmpty(request.Pin))
            {
                response.ErrNumber = "1001.1";
                response.Message = "Missing input";
                response.Status = "Declined";
                response.UpdatePending = false;
                return response;
            }

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    var account = new SDGDAL.Entities.Account();

                    action = "fetching mobile app details and account details.";
                    var mobileApp = _mAppRepo.GetMobileAppDetailsByPinAndActivationCode(request.Pin, request.POSWSRequest.ActivationKey, out account);

                    if (mobileApp == null)
                    {
                        response.ErrNumber = "2007.1";
                        response.Message = "Login failed. (0)";
                        response.Status = "Declined";
                        response.UpdatePending = false;
                        return response;
                    }
                    else if (account == null)
                    {
                        response.ErrNumber = "2007.2";
                        response.Message = "Login failed. (1)";
                        response.Status = "Declined";
                        response.UpdatePending = false;
                        return response;
                    }
                    else
                    {
                        if (mobileApp.IsDeleted || mobileApp.MerchantBranchPOS.IsDeleted)
                        {
                            response.ErrNumber = "2007.3";
                            response.Message = "Login failed. (2)";
                            response.Status = "Declined";
                            response.UpdatePending = false;
                            return response;
                        }
                        else
                        {
                            if (!mobileApp.IsActive || !mobileApp.MerchantBranchPOS.IsActive)
                            {
                                response.ErrNumber = "2007.4";
                                response.Message = "Login failed. (3)";
                                response.Status = "Declined";
                                response.UpdatePending = false;
                                return response;
                            }
                        }

                        if (!account.IsActive || account.IsDeleted)
                        {
                            response.ErrNumber = "2007.5";
                            response.Message = "Login failed. (4)";
                            response.Status = "Declined";
                            response.UpdatePending = false;
                            return response;
                        }
                    }

                    var rToken = mobileAppFunctions.GenerateRequestToken();

                    if (string.IsNullOrEmpty(rToken))
                    {
                        throw new Exception("There was a problem creating a new token.");
                    }

                    response.RToken = HttpUtility.UrlEncode(rToken);

                    action = "log mobile app action.";
                    mobileAppFunctions.LogMobileAppAction("Login", mobileApp.MobileAppId, account.AccountId, request.POSWSRequest.GPSLat, request.POSWSRequest.GPSLong);

                    response.ErrNumber = "0";
                    response.Status = "Approved";
                    response.Message = "Login successful.";
                    response.UpdatePending = mobileApp.UpdatePending;
                    response.AccountId = Convert.ToString(account.AccountId);
                    response.MobileAppId = Convert.ToString(mobileApp.MobileAppId);
                    response.SequenceNumber = "";

                    action = "creating mobile app token log";
                    SDGDAL.Entities.MobileAppTokenLog tokenLog = new SDGDAL.Entities.MobileAppTokenLog();
                    tokenLog.RequestToken = response.RToken;
                    tokenLog.MobileAppId = mobileApp.MobileAppId;
                    tokenLog.AccountId = account.AccountId;
                    tokenLog.DateCreated = DateTime.Now;
                    tokenLog.ExpirationDate = DateTime.Now.AddMinutes(_tokenExpirationInMinutes);

                    var ntokenLog = _mAppRepo.CreateMobileAppTokenLog(tokenLog);

                    if (ntokenLog == null)
                    {
                        throw new Exception("Unable to create token.");
                    }
                }
                else if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    if (request.Pin == "1234")
                    {
                        response.Status = "Approved";
                        response.Message = "In test mode. PIN approved.";
                        response.UpdatePending = false;
                        response.ErrNumber = "0";
                        response.SequenceNumber = "12312321";
                    }
                    else
                    {
                        response.Status = "Declined";
                        response.Message = "Login failed. PIN declined";
                        response.ErrNumber = "-1";
                    }
                }
                else
                {
                    response.Status = "Declined";
                    response.Message = "Login failed. PIN declined";
                    response.ErrNumber = "-1";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "Login", ex.StackTrace);

                response.ErrNumber = "2007.6";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.Status = "DECLINED";
            }

            return response;
        }

        public POSFeaturesResponse IsAlive(POSWSRequest request, string appVersion)
        {
            POSFeaturesResponse response = new POSFeaturesResponse();

            POSWSResponse posResponse = new POSWSResponse();

            object obj = (object)posResponse;
            if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.ISALIVE, (object)appVersion, out obj))
            {
                response.POSWSResponse = (POSWSResponse)obj;
                return response;
            }
            else
            {
                return UpdateApp(request);
            }
        }

        public TransLookupResponse TransLookUp(TransLookupRequest request)
        {
            string action = string.Empty;

            POSWSResponse response = new POSWSResponse();

            TransLookupResponse searchResponse = new TransLookupResponse();
            var resultCredit = new List<SDGDAL.Entities.TransactionAttempt>();
            var resultDebit = new List<SDGDAL.Entities.TransactionAttemptDebit>();
            var resultCash = new List<SDGDAL.Entities.TransactionAttemptCash>();

            object obj = response;
            response.ErrNumber = "0";

            try
            {
                if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.TRANS_LOOKUP, (object)request, out obj))
                {
                    searchResponse.POSWSResponse = (POSWSResponse)obj;
                    return searchResponse;
                }

                var mobileApp = new SDGDAL.Entities.MobileApp();
                var account = new SDGDAL.Entities.Account();

                var statusResponse = mobileAppFunctions.CheckStatus(request.POSWSRequest.RToken, out mobileApp, out account);

                if (statusResponse.Status == "Declined")
                {
                    searchResponse.POSWSResponse = statusResponse;
                    return searchResponse;
                }

                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    if (request.SearchUsing != Convert.ToDecimal(SDGDAL.Enums.MobileAppTransSearchBy.TransactionNum)
                        && request.SearchUsing != Convert.ToDecimal(SDGDAL.Enums.MobileAppTransSearchBy.MobileAppId)
                        && request.SearchUsing != Convert.ToDecimal(SDGDAL.Enums.MobileAppTransSearchBy.RefNumApp))
                    {
                        response.ErrNumber = "2009.2";
                        response.Message = "Invalid search option";
                        response.Status = "Declined";

                        searchResponse.POSWSResponse = response;
                        return searchResponse;
                    }

                    var resMerchActivation = _mAppRepo.GetMobileAppDetailsByActivationCode(request.POSWSRequest.ActivationKey);

                    Transaction trans = new Transaction();
                    List<Transaction> listTransactions = new List<Transaction>();

                    if (resMerchActivation != null)
                    {
                        #region Search last 10 transaction Using AccountId and MobileAppId

                        if (request.SearchUsing == Convert.ToDecimal(SDGDAL.Enums.MobileAppTransSearchBy.MobileAppId))
                        {
                            #region Credit Transaction

                            if (request.MobileAppTransType == Convert.ToInt32(SDGDAL.Enums.MobileAppTransType.Credit))
                            {
                                action = "searching credit transaction for last 10 transactions";
                                resultCredit = _transRepo.GetCreditTransactionByMobileAppId(request.MobileAppId);

                                if (resultCredit == null || resultCredit.Count <= 0)
                                {  
                                    response.Status = "DECLINED";
                                    response.ErrNumber = "2009.3";
                                    response.Message = "There are no transactions made today.";

                                    searchResponse.POSWSResponse = response;
                                    return searchResponse;
                                }

                                foreach (var res in resultCredit)
                                {
                                    listTransactions.Add(new Transaction()
                                    {
                                        TransactionNumber = Convert.ToString(res.TransactionId) + "-" + Convert.ToString(res.TransactionAttemptId),
                                        Timestamp = Convert.ToString(res.DateReceived),
                                        Total = res.Transaction.OriginalAmount.ToString("N2"),
                                        Currency = res.Transaction.Currency.CurrencyCode,
                                        TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)res.TransactionTypeId)
                                    });
                                }
                            }

                            #endregion

                            #region Debit Transaction

                            else if (request.MobileAppTransType == Convert.ToInt32(SDGDAL.Enums.MobileAppTransType.Debit))
                            {
                                action = "searching debit transaction using Mobile App Id";
                                resultDebit = _transRepo.GetDebitTransactionByMobileAppId(request.MobileAppId);

                                if (resultDebit == null || resultDebit.Count <= 0)
                                {
                                    response.Status = "DECLINED";
                                    response.ErrNumber = "2009.4";
                                    response.Message = "There are no transactions made today.";

                                    searchResponse.POSWSResponse = response;
                                    return searchResponse;
                                }

                                foreach (var res in resultDebit)
                                {
                                    listTransactions.Add(new Transaction()
                                    {
                                        TransactionNumber = Convert.ToString(res.TransactionDebitId) + "-" + Convert.ToString(res.TransactionAttemptDebitId),
                                        Timestamp = Convert.ToString(res.DateReceived),
                                        Total = res.TransactionDebit.OriginalAmount.ToString("N2"),
                                        Currency = res.TransactionDebit.Currency.CurrencyCode,
                                        TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)res.TransactionTypeId)
                                });
                                }
                            }

                            #endregion

                            #region Cash Transaction
                            else if (request.MobileAppTransType == Convert.ToInt32(SDGDAL.Enums.MobileAppTransType.Cash))
                            {
                                action = "searching cash transaction using Mobile App Id";
                                resultCash = _transRepo.GetCashTransactionByMobileAppId(request.MobileAppId);

                                if (resultCash == null || resultCash.Count <= 0)
                                {
                                    response.Status = "DECLINED";
                                    response.ErrNumber = "2009.5";
                                    response.Message = "There are no transactions made today.";

                                    searchResponse.POSWSResponse = response;
                                    return searchResponse;
                                }

                                foreach (var res in resultCash)
                                {
                                    listTransactions.Add(new Transaction()
                                    {
                                        TransactionNumber = Convert.ToString(res.TransactionCashId) + "-" + Convert.ToString(res.TransactionAttemptCashId),
                                        Timestamp = Convert.ToString(res.DateReceived),
                                        Total = res.TransactionCash.OriginalAmount.ToString("N2"),
                                        Currency = res.TransactionCash.Currency.CurrencyCode,
                                        TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)res.TransactionTypeId)
                                    });
                                }
                            }

                            #endregion

                            else
                            {
                                response.ErrNumber = "2009.6";
                                response.Message = "Invalid Mobile App Transaction Type.";
                                response.Status = "Declined";

                                searchResponse.POSWSResponse = response;
                                return searchResponse;
                            }
                        }

                        #endregion Search last 5 transaction Using AccountId and MobileAppId

                        #region Search Using Transaction Number

                        else if (request.SearchUsing == Convert.ToDecimal(SDGDAL.Enums.MobileAppTransSearchBy.TransactionNum))
                        {
                            // get proper trans_attempt id
                            int hyphen;
                            int transAttId;
                            int transId;
                            int totalrecords = 0;

                            if (!request.SearchCriteria.Contains('-'))
                            {
                                #region Search Credit Transaction for TransNumber

                                if (request.MobileAppTransType == Convert.ToDecimal(SDGDAL.Enums.MobileAppTransType.Credit))
                                {
                                    resultCredit = _transRepo.GetCreditTransactionAllByMobileAppId(request.MobileAppId, request.SearchCriteria, out totalrecords);

                                    foreach (var res in resultCredit)
                                    {
                                        listTransactions.Add(new Transaction()
                                        {
                                            TransactionNumber = Convert.ToString(res.TransactionId) + "-" + Convert.ToString(res.TransactionAttemptId),
                                            Timestamp = Convert.ToString(res.DateReceived),
                                            Total = res.Transaction.OriginalAmount.ToString("N2"),
                                            Currency = res.Transaction.Currency.CurrencyCode,
                                            TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)res.TransactionTypeId)
                                        });
                                    }
                                }

                                #endregion

                                #region Search Debit Transaction Using Transaction Number
                                else if (request.MobileAppTransType == Convert.ToDecimal(SDGDAL.Enums.MobileAppTransType.Debit))
                                {
                                    resultDebit = _transRepo.GetDebitTransactionAllByMobileAppId(request.MobileAppId, request.SearchCriteria, out totalrecords);

                                    foreach (var res in resultDebit)
                                    {
                                        listTransactions.Add(new Transaction()
                                        {
                                            TransactionNumber = Convert.ToString(res.TransactionDebitId) + "-" + Convert.ToString(res.TransactionAttemptDebitId),
                                            Timestamp = Convert.ToString(res.DateReceived),
                                            Total = res.TransactionDebit.OriginalAmount.ToString("N2"),
                                            TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)res.TransactionTypeId)
                                        });
                                    }
                                }
                                #endregion

                                #region Search Cash Transaction using Transaction Number
                                else if (request.MobileAppTransType == Convert.ToDecimal(SDGDAL.Enums.MobileAppTransType.Cash))
                                {
                                    resultCash = _transRepo.GetCashTransactionAllByMobileAppId(request.MobileAppId, request.SearchCriteria, out totalrecords);

                                    foreach (var res in resultCash)
                                    {
                                        listTransactions.Add(new Transaction()
                                        {
                                            TransactionNumber = Convert.ToString(res.TransactionCashId) + "-" + Convert.ToString(res.TransactionAttemptCashId),
                                            Timestamp = Convert.ToString(res.DateReceived),
                                            Total = res.TransactionCash.OriginalAmount.ToString("N2"),
                                            Currency = res.TransactionCash.Currency.CurrencyCode,
                                            TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)res.TransactionTypeId)
                                        });
                                    }
                                }
                                #endregion

                                else
                                {
                                    response.ErrNumber = "2009.7";
                                    response.Message = "Invalid Mobile App Transaction Type.";
                                    response.Status = "Declined";

                                    searchResponse.POSWSResponse = response;
                                    return searchResponse;
                                }
                            }
                            else
                            {
                                ///search by transaction number
                                try
                                {
                                    hyphen = request.SearchCriteria.IndexOf('-');
                                    transAttId = Convert.ToInt32(request.SearchCriteria.Substring(hyphen + 1));
                                    transId = Convert.ToInt32(request.SearchCriteria.Substring(0, hyphen));
                                }
                                catch
                                {
                                    response.Status = "DECLINED";
                                    response.ErrNumber = "2009.8";
                                    response.Message = "Invalid transaction number";
                                    response.UpdatePending = false;

                                    searchResponse.POSWSResponse = response;
                                    return searchResponse;
                                }

                                #region Search Credit Transaction Using complete transaction number

                                #region Credit Transaction Number
                                if (request.MobileAppTransType == Convert.ToDecimal(SDGDAL.Enums.MobileAppTransType.Credit))
                                {
                                    resultCredit = _transRepo.GetCreditTransactionAttemptByTransNumber(transId, transAttId);

                                    if (resultCredit == null || resultCredit.Count <= 0 || string.IsNullOrEmpty(resultCredit[0].ReturnCode))
                                    {
                                        response.Status = "DECLINED";
                                        response.ErrNumber = "2009.12";
                                        response.Message = "No transaction found.";

                                        searchResponse.POSWSResponse = response;
                                        return searchResponse;
                                    }
                                    else
                                    {
                                        //Compare merchant id extracted from activation code with the transaction's merchant id.
                                        if (resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.MerchantId != resultCredit[0].MobileApp.MerchantBranchPOS.MerchantBranch.Merchant.MerchantId)
                                        {
                                            response.Status = "DECLINED";
                                            response.ErrNumber = "2009.13";
                                            response.Message = "Merchant Mismatch";
                                            response.UpdatePending = false;

                                            searchResponse.POSWSResponse = response;
                                            return searchResponse;
                                        }
                                    }

                                    //Decrypt Card Number
                                    string Key = resultCredit[0].Transaction.Key.Key.ToString();
                                    string KeyIV = resultCredit[0].Transaction.Key.IV;
                                    string E_Card = resultCredit[0].Transaction.CardNumber.ToString();

                                    String E_CARD = Utility.DecryptEncDataWithKeyAndIV(E_Card, Key, KeyIV);

                                    try
                                    {
                                        trans.BatchNumber = resultCredit[0].BatchNumber;
                                    }
                                    catch
                                    {
                                        trans.BatchNumber = "";
                                    }

                                    if (string.IsNullOrEmpty(resultCredit[0].TransNumber))
                                    {
                                        response.Status = "DECLINED";
                                        response.ErrNumber = resultCredit[0].ReturnCode;
                                    }
                                    else
                                    {
                                        response.Status = "APPROVED";
                                        response.ErrNumber = "0";
                                    }

                                    response.Message = resultCredit[0].Notes;
                                    string transtype;

                                    try
                                    {
                                        transtype = Convert.ToString((SDGDAL.Enums.TransactionType)resultCredit[0].TransactionTypeId);
                                    }
                                    catch
                                    {
                                        transtype = "";
                                    }

                                    try
                                    {
                                        if (String.IsNullOrEmpty(resultCredit[0].TransactionSignature.FileData))
                                        {
                                            string path = resultCredit[0].TransactionSignature.Path + "/" + resultCredit[0].TransactionSignature.FileName;
                                            Image imageSignature = Image.FromFile(path);

                                            trans.TransactionSignature = SDGUtil.Functions.ConvertImageToHex(imageSignature);
                                        }
                                        else
                                        {
                                            trans.TransactionSignature = resultCash[0].TransactionSignature.FileData;
                                        }
                                    }
                                    catch
                                    {
                                        trans.TransactionSignature = "";
                                    }

                                    trans.Timestamp = SDGUtil.Functions.Format_Datetime(resultCredit[0].DateReceived);
                                    trans.AuthNumber = resultCredit[0].AuthNumber;
                                    trans.SequenceNumber = resultCredit[0].SeqNumber;
                                    trans.TransactionNumber = resultCredit[0].TransactionId + "-" + resultCredit[0].TransactionAttemptId;
                                    trans.Total = resultCredit[0].Amount.ToString("N2");
                                    trans.NameOnCard = resultCredit[0].Transaction.NameOnCard;
                                    trans.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)resultCredit[0].Transaction.TransactionEntryTypeId);
                                    trans.CardType = resultCredit[0].Transaction.CardType.TypeName;
                                    trans.CardNumber = SDGUtil.Functions.HashCardNumber(E_CARD);
                                    trans.TransactionType = transtype;
                                    trans.ApprovalCode = resultCredit[0].ReturnCode;
                                    trans.ResponseIsoCode = resultCredit[0].ReturnCode;
                                    trans.Currency = resultCredit[0].Transaction.Currency.CurrencyCode;
                                    trans.MobileAppTransType = Convert.ToString(request.MobileAppTransType);
                                    trans.MerchantId = resultCredit[0].DisplayReceipt;
                                    trans.TerminalId = resultCredit[0].DisplayTerminal;

                                    listTransactions.Add(trans);
                                }
                                #endregion

                                #region Debit Transaction Number

                                else if (request.MobileAppTransType == Convert.ToDecimal(SDGDAL.Enums.MobileAppTransType.Debit))
                                {
                                    resultDebit = _transRepo.GetDebitTransactionAttemptByTransNumber(transId, transAttId);

                                    if (resultDebit == null || resultDebit.Count <= 0 || string.IsNullOrEmpty(resultDebit[0].ReturnCode))
                                    {
                                        response.Status = "DECLINED";
                                        response.ErrNumber = "2009.12";
                                        response.Message = "No transaction found.";

                                        searchResponse.POSWSResponse = response;
                                        return searchResponse;
                                    }
                                    else
                                    {
                                        //Compare merchant id extracted from activation code with the transaction's merchant id.
                                        if (resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.MerchantId != resultDebit[0].MobileApp.MerchantBranchPOS.MerchantBranch.Merchant.MerchantId)
                                        {
                                            response.Status = "DECLINED";
                                            response.ErrNumber = "2009.13";
                                            response.Message = "Merchant Mismatch";
                                            response.UpdatePending = false;

                                            searchResponse.POSWSResponse = response;
                                            return searchResponse;
                                        }
                                    }

                                    //Decrypt Card Number
                                    string Key = resultDebit[0].TransactionDebit.Key.Key;
                                    string KeyIV = resultDebit[0].TransactionDebit.Key.IV;
                                    string E_Card = resultDebit[0].TransactionDebit.CardNumber.ToString();

                                    String E_CARD = Utility.DecryptEncDataWithKeyAndIV(E_Card, Key, KeyIV);

                                    try
                                    {
                                        trans.BatchNumber = resultDebit[0].BatchNumber;
                                    }
                                    catch
                                    {
                                        trans.BatchNumber = "";
                                    }

                                    if (string.IsNullOrEmpty(resultDebit[0].TraceNumber))
                                    {
                                        response.Status = "DECLINED";
                                        response.ErrNumber = resultDebit[0].ReturnCode;
                                    }
                                    else
                                    {
                                        response.Status = "APPROVED";
                                        response.ErrNumber = "0";
                                    }

                                    response.Message = resultDebit[0].Notes;
                                    string transtype;

                                    try
                                    {
                                        transtype = Convert.ToString((SDGDAL.Enums.TransactionType)resultDebit[0].TransactionTypeId);
                                    }
                                    catch
                                    {
                                        transtype = "";
                                    }

                                    try
                                    {
                                        if (String.IsNullOrEmpty(resultDebit[0].TransactionSignature.FileData))
                                        {
                                            string path = resultDebit[0].TransactionSignature.Path + "/" + resultDebit[0].TransactionSignature.FileName;
                                            Image imageSignature = Image.FromFile(path);

                                            trans.TransactionSignature = SDGUtil.Functions.ConvertImageToHex(imageSignature);
                                        }
                                        else
                                        {
                                            trans.TransactionSignature = resultCash[0].TransactionSignature.FileData;
                                        }
                                    }
                                    catch
                                    {
                                        trans.TransactionSignature = "";
                                    }

                                    trans.Timestamp = SDGUtil.Functions.Format_Datetime(resultDebit[0].DateReceived);
                                    trans.AuthNumber = resultDebit[0].AuthNumber;
                                    trans.SequenceNumber = resultDebit[0].SeqNumber;
                                    trans.TransactionNumber = resultDebit[0].TransactionDebitId + "-" + resultDebit[0].TransactionAttemptDebitId;
                                    trans.Total = resultDebit[0].Amount.ToString("N2");
                                    trans.NameOnCard = resultDebit[0].TransactionDebit.NameOnCard;
                                    trans.CardType = Convert.ToString(SDGDAL.Enums.CardTypes.Debit);
                                    trans.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)resultDebit[0].TransactionDebit.TransactionEntryTypeId);
                                    trans.CardNumber = SDGUtil.Functions.HashCardNumber(E_CARD);
                                    trans.TransactionType = transtype;
                                    trans.ApprovalCode = resultDebit[0].ReturnCode;
                                    trans.ResponseIsoCode = resultDebit[0].ReturnCode;
                                    trans.Currency = resultDebit[0].TransactionDebit.Currency.CurrencyCode;
                                    trans.MobileAppTransType = Convert.ToString(request.MobileAppTransType);
                                    trans.MerchantId = resultDebit[0].DisplayReceipt;
                                    trans.TerminalId = resultDebit[0].DisplayTerminal;

                                    listTransactions.Add(trans);
                                }
                                #endregion

                                #region Cash Transaction Number
                                else if (request.MobileAppTransType == Convert.ToDecimal(SDGDAL.Enums.MobileAppTransType.Cash))
                                {
                                    resultCash = _transRepo.GetCashTransactionAttemptByTransNumber(transId, transAttId);

                                    if (resultCash == null || resultCash.Count <= 0 || string.IsNullOrEmpty(resultCash[0].ReturnCode))
                                    {
                                        response.Status = "DECLINED";
                                        response.ErrNumber = "2009.12";
                                        response.Message = "No transaction found.";

                                        searchResponse.POSWSResponse = response;
                                        return searchResponse;
                                    }
                                    else
                                    {
                                        //Compare merchant id extracted from activation code with the transaction's merchant id.
                                        if (resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.MerchantId != resultCash[0].MobileApp.MerchantBranchPOS.MerchantBranch.Merchant.MerchantId)
                                        {
                                            response.Status = "DECLINED";
                                            response.ErrNumber = "2009.13";
                                            response.Message = "Merchant Mismatch";
                                            response.UpdatePending = false;

                                            searchResponse.POSWSResponse = response;
                                            return searchResponse;
                                        }
                                    }

                                    try
                                    {
                                        trans.BatchNumber = resultCash[0].BatchNumber;
                                    }
                                    catch
                                    {
                                        trans.BatchNumber = "";
                                    }

                                    if (string.IsNullOrEmpty(trans.BatchNumber))
                                    {
                                        response.Status = "DECLINED";
                                        response.ErrNumber = resultCash[0].ReturnCode;
                                    }
                                    else
                                    {
                                        response.Status = "APPROVED";
                                        response.ErrNumber = "0";
                                    }

                                    response.Message = resultCash[0].Notes;
                                    string transtype;

                                    try
                                    {
                                        transtype = Convert.ToString((SDGDAL.Enums.TransactionType)resultCash[0].TransactionTypeId);
                                    }
                                    catch
                                    {
                                        transtype = "";
                                    }

                                    try
                                    {
                                        if (String.IsNullOrEmpty(resultCash[0].TransactionSignature.FileData))
                                        {
                                            string path = resultCash[0].TransactionSignature.Path + "/" + resultCash[0].TransactionSignature.FileName;
                                            Image imageSignature = Image.FromFile(path);

                                            trans.TransactionSignature = SDGUtil.Functions.ConvertImageToHex(imageSignature);
                                        }
                                        else
                                        {
                                            trans.TransactionSignature = resultCash[0].TransactionSignature.FileData;
                                        }
                                    }
                                    catch
                                    {
                                        trans.TransactionSignature = "";
                                    }

                                    trans.Timestamp = SDGUtil.Functions.Format_Datetime(resultCash[0].DateReceived);
                                    trans.AuthNumber = resultCash[0].AuthNumber;
                                    trans.SequenceNumber = resultCash[0].SeqNumber;
                                    trans.TransactionNumber = resultCash[0].TransactionCashId + "-" + resultCash[0].TransactionAttemptCashId;
                                    trans.Total = resultCash[0].Amount.ToString("N2");
                                    trans.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)resultCash[0].TransactionCash.TransactionEntryTypeId);
                                    trans.TransactionType = transtype;
                                    trans.ApprovalCode = resultCash[0].ReturnCode;
                                    trans.ResponseIsoCode = resultCash[0].ReturnCode;
                                    trans.Currency = resultCash[0].TransactionCash.Currency.CurrencyCode;
                                    trans.MobileAppTransType = Convert.ToString(request.MobileAppTransType);
                                    trans.MerchantId = resultCash[0].DisplayReceipt;
                                    trans.TerminalId = resultCash[0].DisplayTerminal;

                                    listTransactions.Add(trans);

                                }
                                #endregion

                                else
                                {
                                    response.ErrNumber = "2009.10";
                                    response.Message = "Invalid Mobile App Transaction Type.";
                                    response.Status = "Declined";

                                    searchResponse.POSWSResponse = response;
                                    return searchResponse;
                                }

                                #endregion 
                            }
                        }

                        #endregion Search Using Transaction Number

                        #region Search Using RefNumApp

                        else if (request.SearchUsing == Convert.ToDecimal(SDGDAL.Enums.MobileAppTransSearchBy.RefNumApp))
                        {
                            if (request.SearchCriteria != null || request.SearchCriteria != string.Empty)
                            {
                                if (request.MobileAppTransType == Convert.ToInt32(SDGDAL.Enums.MobileAppTransType.Credit))
                                {
                                    resultCredit = _transRepo.GetTransactionAttemptByRefNumApp(request.SearchCriteria);
                                }
                                else if (request.MobileAppTransType == Convert.ToInt32(SDGDAL.Enums.MobileAppTransType.Debit))
                                {
                                    resultDebit = _transRepo.GetDebitTransactionAttemptByRefNumApp(request.SearchCriteria);
                                }
                                else if (request.MobileAppTransType == Convert.ToInt32(SDGDAL.Enums.MobileAppTransType.Cash))
                                {
                                    resultCash = _transRepo.GetCashTransactionAttemptByRefNumApp(request.SearchCriteria);
                                }
                                else
                                {
                                    response.ErrNumber = "2009.9";
                                    response.Message = "Invalid Transaction Type";
                                    response.Status = "Declined";
                                }
                            }
                            else
                            {
                                response.ErrNumber = "2009.10";
                                response.Message = "Invalid App Reference Number.";
                                response.Status = "Declined";

                                searchResponse.POSWSResponse = response;
                                return searchResponse;
                            }
                        }

                        #endregion Search Using RefNumApp

                        else
                        {
                            response.ErrNumber = "2009.11";
                            response.Message = "Invalid Search Type.";
                            response.Status = "Declined";

                            searchResponse.POSWSResponse = response;
                            return searchResponse;
                        }

                        searchResponse.Transactions = listTransactions;
                    }
                    else
                    {
                        response.ErrNumber = "2009.14";
                        response.Message = "Activation Code is not registered.";
                        response.Status = "Declined";

                        searchResponse.POSWSResponse = response;
                        return searchResponse;
                    }
                }
                else if (request.POSWSRequest.SystemMode.Equals("TESTAPPROVED"))
                {
                    response.Status = "Approved";
                    response.Message = "";
                    response.ErrNumber = "0";
                    response.UpdatePending = true;
                }
                else
                {
                    response.Status = "Declined";
                    response.Message = "";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "TransLookup", ex.StackTrace);

                response.ErrNumber = "2009.15";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.Status = "DECLINED";
            }

            searchResponse.POSWSResponse = response;
            return searchResponse;
        }

        public POSWSResponse EmailReceipt(EmailReceiptRequest request)
        {
            string action = string.Empty;
            POSWSResponse response = new POSWSResponse();
            object obj = response;
            try
            {
                if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.EMAIL_RECEIPT, (object)request, out obj))
                {
                    return (POSWSResponse)obj;
                }

                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    int limit, partnerId;
                    bool checkDuplicate;
                    int checkNumCycle;

                    action = "fetching MobilePOS Settings";
                    var resMerchActivation = _mAppRepo.GetMobileAppDetailsByActivationCode(request.POSWSRequest.ActivationKey);

                    if (resMerchActivation != null)
                    {
                        #region Get Email Settings

                        if (resMerchActivation.MobileAppFeatures.EmailAllowed)
                        {
                            partnerId = resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.PartnerId;
                            limit = resMerchActivation.MobileAppFeatures.EmailLimit;
                            checkDuplicate = resMerchActivation.MobileAppFeatures.CheckForEmailDuplicates;
                            checkNumCycle = resMerchActivation.MobileAppFeatures.BillingCyclesCheckEmail;
                        }
                        else
                        {
                            response.ErrNumber = "2011.2";
                            response.Message = "Email receipts are currently disabled on this device, please contact Support to enable this feature.";
                            response.Status = "DECLINED";
                            return response;
                        }

                        #endregion Get Email Settings
                    }
                    else
                    {
                        response.ErrNumber = "2011.3";
                        response.Message = "Activation Code is not registered.";
                        response.Status = "DECLINED";
                        return response;
                    }

                    //TODO EMAIL CHARGE

                    #region Send Email Receipt

                    try
                    {
                        action = "Checking Email Server Settings";
                        string strSender = "";
                        string strMesageBody = "";

                        var partnerEmailServer = _emailServerRepo.GetEmailServerByPartnerId(partnerId);

                        if (partnerEmailServer != null && partnerEmailServer.Count != 0)
                        {
                            string maskEmail = System.Configuration.ConfigurationManager.AppSettings["SendEmailName"].ToString();

                            strSender = partnerEmailServer[0].Email;

                            // instance of MailMessage
                            MailMessage mm = new MailMessage(strSender, request.EmailDetails.Email);

                            // set SUBJECT
                            mm.Subject = resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.MerchantName + " - #" + request.TransNumber;

                            string newReceiptHeader = request.EmailDetails.ReceiptHeader.Replace(";", "<br/>");
                            string newReceiptSec1 = request.EmailDetails.ReceiptSec1.Replace(";", "<br/>");
                            string newReceiptSec2 = request.EmailDetails.ReceiptSec2.Replace(";", "<br/>");
                            string newReceiptSec3 = request.EmailDetails.ReceiptSec3.Replace(";", "<br/>");
                            string newReceiptFooter = request.EmailDetails.ReceiptFooter.Replace(";", "<br/>");

                            strMesageBody = "<table> <tr> <td><strong>" + newReceiptHeader + "</strong><br/></td> </tr>" + "<tr><td>" + newReceiptSec1 + "<br/></td> </tr>" + "<tr><td>-------------------------------------<br/>" + newReceiptSec2 + "<br/></td> </tr>" + "<tr><td>" + newReceiptSec3 + "<br/></td> </tr>" + "<tr><td>-------------------------------------<br/>" + newReceiptFooter + "</td> </tr></table>";

                            // Set BODY
                            mm.Body = strMesageBody;
                            mm.IsBodyHtml = true;

                            var fromAddressTemp = new MailAddress(maskEmail, partnerEmailServer[0].EmailServerName);
                            var fromAddress = new MailAddress(partnerEmailServer[0].Username, partnerEmailServer[0].Password);
                            var toAddress = new MailAddress(request.EmailDetails.Email, "");

                            var smtp = new SmtpClient
                            {
                                Host = partnerEmailServer[0].Host,
                                Port = partnerEmailServer[0].Port,
                                EnableSsl = partnerEmailServer[0].UseSSL,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = partnerEmailServer[0].DefaultCredential,
                                Credentials = new System.Net.NetworkCredential(partnerEmailServer[0].Username, partnerEmailServer[0].Password),
                            };
                            using (var message = new System.Net.Mail.MailMessage(fromAddress, toAddress)
                            {
                                IsBodyHtml = true,
                                Subject = mm.Subject,
                                Body = mm.Body,
                                From = fromAddressTemp
                            })
                            {
                                try
                                {
                                    action = "sending email.";
                                    smtp.Send(message);
                                }
                                catch
                                {
                                    response.Message = "Email not sent. Please try again.";
                                    response.Status = "Declined";
                                    response.ErrNumber = "2011.7";
                                    return response;
                                }
                            }

                            //For Future. Log the Events

                            response.Message = "This receipt will be emailed to the address you provided.";
                            response.Status = "APPROVED";
                            response.ErrNumber = "0";
                        }
                        else
                        {
                            response.Message = "Email Not sent, no available SMTP settings. Please contact Support ";
                            response.Status = "Email Not Sent";
                            response.ErrNumber = "2011.4";
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error please
                        var errorOnAction = "Error while " + action;

                        var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "Email Receipt", ex.StackTrace);

                        response.Status = "DECLINED";
                        response.Message = "Unknown error, please contact Support. ";
                        response.ErrNumber = "2011.5";
                    }

                    #endregion Send Email Receipt
                }
                else
                {
                    if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                    {
                        response.Status = "The receipt will be sent to the contact information you have provided.";
                        response.Message = "";
                        response.ErrNumber = "0";
                    }
                    else
                    {
                        response.Status = "DECLINED";
                        response.Message = "An error occured.";
                        response.ErrNumber = "2011.6";
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "Email Receipt", ex.StackTrace);

                response.Status = "DECLINED";
                response.Message = "Unknown error, please contact Support. ";
                response.ErrNumber = "2011.7";
            }

            return response;
        }

        public POSWSResponse CreateTicket(CreateTicketRequest request)
        {
            string action = string.Empty;
            POSWSResponse response = new POSWSResponse();

            try
            {
                if (string.IsNullOrEmpty(request.POSWSRequest.SystemMode)
                    || string.IsNullOrEmpty(request.POSWSRequest.ActivationKey)
                    || string.IsNullOrEmpty(request.TicketNumber)
                    || string.IsNullOrEmpty(request.EmailDetails.Email)
                    || string.IsNullOrEmpty(request.EmailDetails.ReceiptHeader)
                    || string.IsNullOrEmpty(request.EmailDetails.ReceiptSec1)
                    || string.IsNullOrEmpty(request.EmailDetails.ReceiptSec2)
                    || string.IsNullOrEmpty(request.EmailDetails.ReceiptSec3)
                    || string.IsNullOrEmpty(request.EmailDetails.ReceiptFooter))
                {
                    response.ErrNumber = "2012.1";
                    response.Message = "Missing input";
                    response.Status = "DECLINED";

                    return response;
                }

                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    int partnerId;

                    action = "fetching MobilePOS Settings";
                    var resMerchActivation = _mAppRepo.GetMobileAppDetailsByActivationCode(request.POSWSRequest.ActivationKey);

                    if (resMerchActivation != null)
                    {
                        #region Get Email Settings

                        if (resMerchActivation.MobileAppFeatures.EmailAllowed)
                        {
                            partnerId = resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.PartnerId;
                        }
                        else
                        {
                            response.ErrNumber = "2012.2";
                            response.Message = "Email receipts are currently disabled on this device, please contact Support to enable this feature.";
                            response.Status = "DECLINED";
                            return response;
                        }

                        #endregion Get Email Settings
                    }
                    else
                    {
                        response.ErrNumber = "2012.3";
                        response.Message = "Activation Code is not registered.";
                        response.Status = "DECLINED";
                        return response;
                    }

                    //TODO EMAIL CHARGE

                    #region Send Email Receipt

                    try
                    {
                        action = "Checking Email Server Settings";
                        string strSender = "";
                        string strMesageBody = "";
                        string strCCEmails = String.Empty;

                        var partnerEmailServer = _emailServerRepo.GetEmailServerByPartnerId(partnerId);

                        if (partnerEmailServer != null && partnerEmailServer.Count != 0)
                        {
                            string ccEmails = System.Configuration.ConfigurationManager.AppSettings["SupportEmailCC"].ToString();
                            string maskEmail = System.Configuration.ConfigurationManager.AppSettings["SendEmailName"].ToString();

                            strSender = partnerEmailServer[0].Email;

                            MailMessage mm = new MailMessage(strSender, request.EmailDetails.Email);

                            mm.Subject = resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.MerchantName + " - #" + request.TicketNumber;

                            string newReceiptHeader = request.EmailDetails.ReceiptHeader.Replace(";", "<br/>");
                            string newReceiptSec1 = request.EmailDetails.ReceiptSec1.Replace(";", "<br/>");
                            string newReceiptSec2 = request.EmailDetails.ReceiptSec2.Replace(";", "<br/>");
                            string newReceiptSec3 = request.EmailDetails.ReceiptSec3.Replace(";", "<br/>");
                            string newReceiptFooter = request.EmailDetails.ReceiptFooter.Replace(";", "<br/>");

                            strMesageBody = "<table> <tr> <td><strong>" + newReceiptHeader + "</strong><br/></td> </tr>" + "<tr><td>" + newReceiptSec1 + "<br/></td> </tr>" + "<tr><td>-----------------------------------------------<br/>" + newReceiptSec2 + "<br/></td> </tr>" + "<tr><td>" + newReceiptSec3 + "<br/></td> </tr>" + "<tr><td>-----------------------------------------------<br/>" + newReceiptFooter + "</td> </tr></table>";

                            mm.Body = strMesageBody;
                            mm.IsBodyHtml = true;

                            var fromAddressTemp = new MailAddress(maskEmail, request.EmailServerName);
                            var fromAddress = new MailAddress(partnerEmailServer[0].Username, partnerEmailServer[0].Password);
                            var toAddress = new MailAddress(request.EmailDetails.Email, "");

                            var smtp2 = new SmtpClient
                            {
                                Host = partnerEmailServer[0].Host,
                                Port = partnerEmailServer[0].Port,
                                EnableSsl = partnerEmailServer[0].UseSSL,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = partnerEmailServer[0].DefaultCredential,
                                Credentials = new System.Net.NetworkCredential(partnerEmailServer[0].Username, partnerEmailServer[0].Password),
                            };
                            using (var message = new System.Net.Mail.MailMessage(fromAddress, toAddress)
                            {
                                IsBodyHtml = true,
                                Subject = mm.Subject,
                                Body = mm.Body,
                                From = fromAddressTemp,
                            })
                            {
                                try
                                {
                                    action = "sending email.";
                                    message.CC.Add(ccEmails);
                                    smtp2.Send(message);
                                }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                                catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                                {
                                    response.Message = "Email not sent. Please try again.";
                                    response.Status = "Declined";
                                    response.ErrNumber = "2012.7";
                                    return response;
                                }
                            }

                            response.Message = "This receipt will be emailed to the address you provided.";
                            response.Status = "APPROVED";
                            response.ErrNumber = "0";
                        }
                        else
                        {
                            response.Message = "Email Not sent, no available SMTP settings. Please contact Support ";
                            response.Status = "Email Not Sent";
                            response.ErrNumber = "2012.4";
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error please
                        var errorOnAction = "Error while " + action;

                        var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "CreateTicket", ex.StackTrace);

                        response.Status = "DECLINED";
                        response.Message = "Unknown error, please contact Support. ";
                        response.ErrNumber = "2012.5";
                    }

                    #endregion Send Email Receipt
                }
                else
                {
                    if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                    {
                        response.Status = "The receipt will be sent to the contact information you have provided.";
                        response.Message = "";
                        response.ErrNumber = "0";
                    }
                    else
                    {
                        response.Status = "DECLINED";
                        response.Message = "An error occured.";
                        response.ErrNumber = "2012.6";
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "CreateTicket", ex.StackTrace);

                response.Status = "DECLINED";
                response.Message = "Unknown error, please contact Support. ";
                response.ErrNumber = "2012.7";
            }

            return response;
        }

        public POSWSResponse ForgotMobilePin(POSWSRequest request, string UserEmail, int AccountId)
        {
            string action = string.Empty;
            string usersPin = string.Empty;
            bool IsExists = false;

            POSWSResponse response = new POSWSResponse();

            try
            {
                action = "checking the email and account ID.";
                if (string.IsNullOrEmpty(UserEmail)
                   || AccountId <= 0)
                {
                    response.ErrNumber = "2500.1";
                    response.Message = "Missing Input";
                    response.Status = "DECLINED";
                    return response;
                }

                if (request.SystemMode.ToUpper().Equals("LIVE"))
                {
                    action = "checking account information";
                    var account = _userRepo.GetUserByAccountId(AccountId);

                    if (account == null)
                    {
                        response.ErrNumber = "2500.2";
                        response.Message = "Your account does not exists.";
                        response.Status = "DECLINED";

                        return response;
                    }

                    var merchantListEmail = _userRepo.GetDetailsbyParentIdAndParentTypeIdAndEmail(Convert.ToInt32(SDGDAL.Enums.ParentType.Merchant), account.ParentId, UserEmail);

                    var branchList = _branchRepo.GetAllBranchesByMerchant(account.ParentId, "");

                    for (int i = 0; i < branchList.Count; i++)
                    {
                        var branchListEmail = _userRepo.GetDetailsbyParentIdAndParentTypeIdAndEmail(Convert.ToInt32(SDGDAL.Enums.ParentType.Branch), branchList[i].MerchantBranchId, UserEmail);
                        if (branchListEmail.Count > 0)
                        {
                            IsExists = true;
                            break;
                        }
                    }

                    if (merchantListEmail.Count == 0 && IsExists == false)
                    {
                        response.ErrNumber = "2500.2";
                        response.Message = "Your email is not registered.";
                        response.Status = "DECLINED";

                        return response;
                    }

                    var users = _userRepo.GetDetailsbyParentIdAndParentTypeIdAccountId(account.ParentTypeId, account.ParentId, AccountId);

                    if (users != null && users.Count() > 0)
                    {
                        for (int i = 0; i < users.Count(); i++)
                        {
                            usersPin += "<br/>" + "Retrieve Pin User " + (i + 1) + ": " + "<strong>" + Utility.Decrypt(users[i].PIN) + "</strong>" + "<br/>";
                        }
                    }

                    action = "fetching MobilePOS Settings";
                    var resMerchActivation = _mAppRepo.GetMobileAppDetailsByActivationCode(request.ActivationKey);

                    #region Send Email Receipt

                    try
                    {
                        action = "Checking Email Server Settings";

                        string maskEmail = System.Configuration.ConfigurationManager.AppSettings["SendEmailName"].ToString();

                        string strSender = "";
                        string strMesageBody = "";

                        var partnerEmailServer = _emailServerRepo.GetEmailServerByPartnerId(resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.PartnerId);

                        if (partnerEmailServer != null && partnerEmailServer.Count != 0)
                        {
                            strSender = partnerEmailServer[0].Email;

                            MailMessage mm = new MailMessage(strSender, UserEmail);

                            mm.Subject = resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.MerchantName + " - " + "Forgot PIN";

                            string newReceiptHeader = resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.MerchantName + "<br/>"
                                                   + resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.ContactInformation.Address + "<br/>"
                                                   + resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.ContactInformation.City + ", " + resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.ContactInformation.Country.CountryName + "<br />"
                                                   + resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.ContactInformation.PrimaryContactNumber + "<br /><br />";

                            string newReceiptSec1 = "Dear Valued Client," + "<br/> <br/>"
                                                    + "Your request for pin update is successful." + "<br/>";

                            string newReceiptSec2 = "Merchant PIN: " + "<strong>" + Utility.Decrypt(account.PIN) + "</strong>" + "<br/>";

                            string newReceiptFooter = "Thank you." + "<br/><br/>";

                            strMesageBody = "<table> <tr> <td><strong>" + newReceiptHeader + "</strong><br/></td> </tr>" + "<tr><td>" + newReceiptSec1 + "<br/></td></tr>" + "<tr><td>---------------------------------------------------------<br/><br/>" + newReceiptSec2 + usersPin + "<br/></td> </tr>" + "<tr><td>---------------------------------------------------------<br/>" + newReceiptFooter + "</td> </tr> </table>";

                            mm.Body = strMesageBody;
                            mm.IsBodyHtml = true;

                            var fromAddressTemp = new MailAddress(maskEmail, "PayMaya");
                            var fromAddress = new MailAddress(partnerEmailServer[0].Username, partnerEmailServer[0].Password);
                            var toAddress = new MailAddress(UserEmail, "");

                            var smtp = new SmtpClient
                            {
                                Host = partnerEmailServer[0].Host,
                                Port = partnerEmailServer[0].Port,
                                EnableSsl = partnerEmailServer[0].UseSSL,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = partnerEmailServer[0].DefaultCredential,
                                Credentials = new System.Net.NetworkCredential(partnerEmailServer[0].Username, partnerEmailServer[0].Password),
                            };
                            using (var message = new System.Net.Mail.MailMessage(fromAddress, toAddress)
                            {
                                IsBodyHtml = true,
                                Subject = mm.Subject,
                                Body = mm.Body,
                                From = fromAddressTemp
                            })
                            {
                                try
                                {
                                    action = "sending pin to an email.";
                                    smtp.Send(message);
                                }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                                catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                                {
                                    response.Message = "Email Not sent.";
                                    response.Status = "Declined";
                                    response.ErrNumber = "2500.6";
                                    return response;
                                }
                            }

                            response.Message = "An email was sent to " + UserEmail + " to retrieve your PIN.";
                            response.Status = "APPROVED";
                            response.ErrNumber = "0";
                        }
                        else
                        {
                            response.Message = "Email Not sent, no available SMTP settings. Please contact Support ";
                            response.Status = "Email Not Sent";
                            response.ErrNumber = "2500.3";
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error please
                        var errorOnAction = "Error while " + action;

                        var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "Email Receipt", ex.StackTrace);

                        response.Status = "DECLINED";
                        response.Message = "Unknown error, please contact Support. ";
                        response.ErrNumber = "2500.4";
                    }

                    #endregion Send Email Receipt
                }
                else
                {
                    response.Status = "Mobile PIN will be sent to the contact information you have provided.";
                    response.Message = "";
                    response.ErrNumber = "0";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "Forgot Mobile Pin", ex.StackTrace);

                response.Status = "DECLINED";
                response.Message = "Unknown error, please contact Support. ";
                response.ErrNumber = "2500.5";
            }

            return response;
        }

        public POSWSResponse TransSignatureCapture(TransactionSignatureRequest request)
        {
            string action = string.Empty;
            POSWSResponse response = new POSWSResponse();

            if (string.IsNullOrEmpty(request.POSWSRequest.SystemMode)
                || string.IsNullOrEmpty(request.POSWSRequest.ActivationKey)
                || string.IsNullOrEmpty(request.POSWSRequest.RToken)
                || string.IsNullOrEmpty(request.TransNumber)
                || string.IsNullOrEmpty(request.Image)
                || string.IsNullOrEmpty(request.ImageLen)
                || request.MobileAppTransType <= 0)
            {
                response.ErrNumber = "1001.1";
                response.Message = "Missing input";
                response.Status = "Declined";

                return response;
            }

            response.ErrNumber = "0";

            var mobileApp = new SDGDAL.Entities.MobileApp();
            var account = new SDGDAL.Entities.Account();

            action = "checking mobileApp status.";
            var statusResponse = mobileAppFunctions.CheckStatus(request.POSWSRequest.RToken, out mobileApp, out account);

            if (statusResponse.Status == "Declined")
            {
                response.Message = statusResponse.Message;
                return response;
            }

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    #region Get TransactionId and TransactionAttempId

                    int hyphen;
                    int transAttId;
                    int transId;

                    try
                    {
                        action = "checking transaction id and transaction attempt id";
                        hyphen = request.TransNumber.IndexOf('-');
                        transAttId = Convert.ToInt32(request.TransNumber.Substring(hyphen + 1));
                        transId = Convert.ToInt32(request.TransNumber.Substring(0, hyphen));
                    }
                    catch
                    {
                        response.Status = "DECLINED";
                        response.ErrNumber = "2300.1";
                        response.Message = "Invalid transaction number";
                        response.UpdatePending = false;

                        return response;
                    }

                    #endregion Get TransactionId and TransactionAttempId

                    if (int.Parse(request.ImageLen) == request.Image.Length)
                    {
                        #region Save Image

                        action = "initializing signature.";

                        string hex = request.Image;
                        hex = hex.Replace("-", "");

                        var transSign = new SDGDAL.Entities.TransactionSignature();

                        transSign.FileSize = request.Image.Length;
                        transSign.FileName = Convert.ToString(transId) + Convert.ToString(transAttId) + DateTime.Now.ToString("MMddyyyyhhmmss") + ".jpg";
                        transSign.FileData = null;

                        byte[] byteImage = SDGUtil.Functions.ConvertHexToByteArray(hex);

                        try
                        {
                            var saveDir = System.Web.Hosting.HostingEnvironment.MapPath("~/Signatures");
                            string path = saveDir + "/" + transSign.FileName ;
                            transSign.Path = saveDir;

                            if (!Directory.Exists(saveDir))
                            {
                                Directory.CreateDirectory(saveDir);
                            }

                            action = "saving image to folder path " + path;

                            System.Drawing.Image newImage;

                           if (byteImage != null)
                           {
                                using (MemoryStream stream = new MemoryStream(byteImage))
                                {
                                    newImage = System.Drawing.Image.FromStream(stream);
                                    newImage.Save(path);
                                }
                           }
                        }
                        catch(Exception ex)
                        {
                            response.ErrNumber = "-1";
                            response.Status = "Declined";
                            response.Message = "Error in saving image to path. " + ex.Message;

                            return response;
                        }

                        if (request.MobileAppTransType == Convert.ToInt32(SDGDAL.Enums.MobileAppTransType.Credit))
                        {
                            action = "saving path to database";

                            var trans = new SDGDAL.Entities.TransactionAttempt();
                            trans.TransactionId = transId;
                            trans.TransactionAttemptId = transAttId;

                            var resultCredit = _transRepo.CreateTransactionSignature(transSign, trans);

                            if (resultCredit.TransactionSignatureId > 0)
                            {
                                response.ErrNumber = "0";
                                response.Status = "Approved";
                                response.Message = "";
                            }
                            else
                            {
                                response.ErrNumber = "2300.2";
                                response.Status = "Declined";
                                response.Message = "Signature does not saved.";
                            }
                        }
                        else if (request.MobileAppTransType == Convert.ToInt32(SDGDAL.Enums.MobileAppTransType.Debit))
                        {
                            action = "saving path to database";
                            var trans = new SDGDAL.Entities.TransactionAttemptDebit();
                            trans.TransactionDebitId = transId;
                            trans.TransactionAttemptDebitId = transAttId;

                            var resultDebit = _transRepo.CreateTransactionSignatureDebit(transSign, trans);

                            if (resultDebit.TransactionSignatureId > 0)
                            {
                                response.ErrNumber = "0";
                                response.Status = "Approved";
                                response.Message = "";
                            }
                            else
                            {
                                response.ErrNumber = "2300.2";
                                response.Status = "Declined";
                                response.Message = "Signature does not saved.";
                            }
                        }
                        else if (request.MobileAppTransType == Convert.ToInt32(SDGDAL.Enums.MobileAppTransType.Cash))
                        {

                            var trans = new SDGDAL.Entities.TransactionAttemptCash();
                            trans.TransactionCashId = transId;
                            trans.TransactionAttemptCashId = transAttId;

                            var resultCash = _transRepo.CreateTransactionSignatureCash(transSign, trans);

                            if(resultCash.TransactionSignatureId > 0)
                            {
                                response.ErrNumber = "0";
                                response.Status = "Approved";
                                response.Message = "";
                            }
                            else
                            {
                                response.ErrNumber = "2300.2";
                                response.Status = "Declined";
                                response.Message = "Signature does not saved.";
                            }
                        }
                        else
                        {
                            response.ErrNumber = "2300.3";
                            response.Status = "Declined";
                            response.Message = "Invalid Mobile App Transaction Type";
                        }

                        #endregion Save Image
                    }
                    else
                    {
                        response.Message = "Invalid length of received file";
                        response.ErrNumber = "2300.4";
                    }
                }
                else if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    response.ErrNumber = "0";
                    response.Status = "Declined";
                    response.Message = "Test Approved";
                }
                else
                {
                    response.ErrNumber = "2300.4";
                    response.Status = "Declined";
                    response.Message = "";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "TransSignatureCapture", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message, "TransSignatureCapture  ErrRef:" + errRefNumber, ex.InnerException.StackTrace);
                }

                response.ErrNumber = "2300.5";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber + " " + action + " " + ex.Message;
                response.Status = "DECLINED";
            }

            return response;
        }

        public TransactionVoidReasonResponse TransactionVoidReason(POSWSRequest request)
        {
            string action = String.Empty;

            POSWSResponse response = new POSWSResponse();
            TransactionVoidReasonResponse voidReasonResponse = new TransactionVoidReasonResponse();

            var result = new List<SDGDAL.Entities.TransactionVoidReason>();

            object obj = response;
            response.ErrNumber = "0";

            if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.TRANSACTION_VOID_REASON, (object)request, out obj))
            {
                voidReasonResponse.POSWSResponse = (POSWSResponse)obj;
                return voidReasonResponse;
            }

            try
            {
                var mobileApp = new SDGDAL.Entities.MobileApp();
                var account = new SDGDAL.Entities.Account();

                action = "checking mobile app availability.";

                voidReasonResponse.POSWSResponse = mobileAppFunctions.CheckStatus(request.RToken, out mobileApp, out account);

                if (voidReasonResponse.POSWSResponse.Status == "Declined")
                {
                    return voidReasonResponse;
                }

                if (request.SystemMode.ToUpper().Equals("LIVE"))
                {
                    action = "retrieving all reasons for void transactions";
                    result = _transRepo.GetAllTransactionVoidReason();

                    List<TransactionVoidReason> listVoidReasons = new List<TransactionVoidReason>();

                    foreach (var res in result)
                    {
                        listVoidReasons.Add(new TransactionVoidReason()
                        {
                            ID = Convert.ToString(res.TransactionVoidReasonId),
                            VoidReason = res.VoidReason
                        });
                    }

                    voidReasonResponse.VoidReason = listVoidReasons;
                }
                else if (request.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    response.Status = "Approved";
                    response.Message = "";
                    response.ErrNumber = "0";
                }
                else
                {
                    response.Status = "Declined";
                    response.Message = "An error occured while performing the update.";
                    response.ErrNumber = "2400.1";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "UpdateApp", ex.StackTrace);

                response.ErrNumber = "2400.2";
                response.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.Status = "Declined";
            }

            voidReasonResponse.POSWSResponse = response;
            return voidReasonResponse;
        }

        public POSWSResponse RequestAddUser(EmailDetails request)
        {
            string action = string.Empty;

            POSWSResponse response = new POSWSResponse();
            response.ErrNumber = "0";

            if (string.IsNullOrEmpty(request.Email)
                    || string.IsNullOrEmpty(request.ReceiptSec2))
            {
                response.ErrNumber = "2700.1";
                response.Message = "Missing input";
                response.Status = "DECLINED";

                return response;
            }

            #region Send Email Receipt

            try
            {
                action = "Checking Email Server Settings";
                string strMesageBody = "";
                string strCCEmails = String.Empty;

                //string ccEmails = System.Configuration.ConfigurationManager.AppSettings["EmailCC"].ToString();
                string email = System.Configuration.ConfigurationManager.AppSettings["Email"].ToString();
                string emailPassword = System.Configuration.ConfigurationManager.AppSettings["Password"].ToString();
                int emailPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                string emailHost = System.Configuration.ConfigurationManager.AppSettings["Host"].ToString();
                bool emailSSL = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["SSL"].ToString());
                string emailSubject = System.Configuration.ConfigurationManager.AppSettings["EmailSubj"].ToString();
                string emailServerName = System.Configuration.ConfigurationManager.AppSettings["ServerName"].ToString();

                string strSender = ""; //partnerEmailServer[0].Email;

                MailMessage mm = new MailMessage(email, email);

                mm.Subject = emailSubject;

                string newReceiptHeader = "Hello Admin,";
                string newReceiptSec1 = ("Here are the informations to register new user for mobile POS.");
                string newReceiptSec2 = request.ReceiptSec2.Replace(";", "<br/>");
                string newReceiptSec3 = "Thank You,";
                string newReceiptFooter = request.Email;

                strMesageBody = "<table> <tr> <td>" + newReceiptHeader + "<br/><br/></td> </tr>" + "<tr><td>" + newReceiptSec1 + "<br/><br/></td> </tr>" + "<tr><td>" + newReceiptSec2 + "<br/><br/></td> </tr>" + "<tr><td>" + newReceiptSec3 + "<br/></td> </tr>" + "<tr><td><br/>" + newReceiptFooter + "</td> </tr></table>";

                mm.Body = strMesageBody;
                mm.IsBodyHtml = true;

                var fromAddressTemp = new MailAddress(request.Email, emailServerName);
                var fromAddress = new MailAddress(email, emailPassword);
                var toAddress = new MailAddress(email, "");

                var smtp2 = new SmtpClient
                {
                    Host = emailHost,
                    Port = emailPort,
                    EnableSsl = emailSSL,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(email, emailPassword),
                };
                using (var message = new System.Net.Mail.MailMessage(fromAddress, toAddress)
                {
                    IsBodyHtml = true,
                    Subject = mm.Subject,
                    Body = mm.Body,
                    From = fromAddressTemp,
                })
                {
                    try
                    {
                        action = "sending email.";
                        //message.CC.Add(ccEmails);
                        smtp2.Send(message);
                    }
                    catch
                    {
                        response.Message = "Email not sent. Please try again.";
                        response.Status = "Declined";
                        response.ErrNumber = "2700.2";
                        return response;
                    }
                }

                response.Message = "Successfully sent.";
                response.Status = "APPROVED";
                response.ErrNumber = "0";
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "RequestAddUser", ex.StackTrace);

                response.Status = "DECLINED";
                response.Message = "Unknown error, please contact Support. ";
                response.ErrNumber = "2700.3";
            }

            #endregion Send Email Receipt

            return response;
        }

        public POSWSResponse GiftReceipt(EmailReceiptRequest request)
        {
            string action = string.Empty;
            POSWSResponse response = new POSWSResponse();
            object obj = response;
            try
            {
                if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.EMAIL_RECEIPT, (object)request, out obj))
                {
                    return (POSWSResponse)obj;
                }

                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    int partnerId;

                    action = "fetching MobilePOS Settings";
                    var resMerchActivation = _mAppRepo.GetMobileAppDetailsByActivationCode(request.POSWSRequest.ActivationKey);

                    if (resMerchActivation != null)
                    {
                        #region Get Gift Settings

                        if (resMerchActivation.MobileAppFeatures.GiftAllowed)
                        {
                            partnerId = resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.PartnerId;
                        }
                        else
                        {
                            response.ErrNumber = "2800.2";
                            response.Message = "Gift receipts are currently disabled on this device, please contact Support to enable this feature.";
                            response.Status = "DECLINED";
                            return response;
                        }

                        #endregion Get Email Settings
                    }
                    else
                    {
                        response.ErrNumber = "2800.3";
                        response.Message = "Activation Code is not registered.";
                        response.Status = "DECLINED";
                        return response;
                    }

                    //TODO EMAIL CHARGE

                    #region Send Email Receipt

                    try
                    {
                        action = "Checking Email Server Settings";
                        string strSender = "";
                        string strMesageBody = "";

                        var partnerEmailServer = _emailServerRepo.GetEmailServerByPartnerId(partnerId);

                        if (partnerEmailServer != null && partnerEmailServer.Count != 0)
                        {
                            string maskEmail = System.Configuration.ConfigurationManager.AppSettings["SendEmailName"].ToString();

                            strSender = partnerEmailServer[0].Email;

                            // instance of MailMessage
                            MailMessage mm = new MailMessage(strSender, request.EmailDetails.Email);

                            // set SUBJECT
                            mm.Subject = resMerchActivation.MerchantBranchPOS.MerchantBranch.Merchant.MerchantName + " - #" + request.TransNumber;

                            string newReceiptHeader = request.EmailDetails.ReceiptHeader.Replace(";", "<br/>");
                            string newReceiptSec1 = request.EmailDetails.ReceiptSec1.Replace(";", "<br/>");
                            string newReceiptSec2 = request.EmailDetails.ReceiptSec2.Replace(";", "<br/>");
                            string newReceiptSec3 = request.EmailDetails.ReceiptSec3.Replace(";", "<br/>");
                            string newReceiptFooter = request.EmailDetails.ReceiptFooter.Replace(";", "<br/>");

                            strMesageBody = "<table> <tr> <td><strong>" + newReceiptHeader + "</strong><br/></td> </tr>" + "<tr><td>" + newReceiptSec1 + "<br/></td> </tr>" + "<tr><td>-------------------------------------<br/>" + newReceiptSec2 + "<br/></td> </tr>" + "<tr><td>" + newReceiptSec3 + "<br/></td> </tr>" + "<tr><td>-------------------------------------<br/>" + newReceiptFooter + "</td> </tr></table>";

                            // Set BODY
                            mm.Body = strMesageBody;
                            mm.IsBodyHtml = true;

                            var fromAddressTemp = new MailAddress(maskEmail, partnerEmailServer[0].EmailServerName);
                            var fromAddress = new MailAddress(partnerEmailServer[0].Username, partnerEmailServer[0].Password);
                            var toAddress = new MailAddress(request.EmailDetails.Email, "");

                            var smtp = new SmtpClient
                            {
                                Host = partnerEmailServer[0].Host,
                                Port = partnerEmailServer[0].Port,
                                EnableSsl = partnerEmailServer[0].UseSSL,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = partnerEmailServer[0].DefaultCredential,
                                Credentials = new System.Net.NetworkCredential(partnerEmailServer[0].Username, partnerEmailServer[0].Password),
                            };
                            using (var message = new System.Net.Mail.MailMessage(fromAddress, toAddress)
                            {
                                IsBodyHtml = true,
                                Subject = mm.Subject,
                                Body = mm.Body,
                                From = fromAddressTemp
                            })
                            {
                                try
                                {
                                    action = "sending email.";
                                    smtp.Send(message);
                                }
                                catch
                                {
                                    response.Message = "Email not sent. Please try again.";
                                    response.Status = "Declined";
                                    response.ErrNumber = "2800.4";
                                    return response;
                                }
                            }

                            response.Message = "This receipt will be emailed to the address you provided.";
                            response.Status = "APPROVED";
                            response.ErrNumber = "0";
                        }
                        else
                        {
                            response.Message = "Email Not sent, no available SMTP settings. Please contact Support ";
                            response.Status = "Email Not Sent";
                            response.ErrNumber = "2800.5";
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorOnAction = "Error while " + action;

                        var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "Gift Receipt", ex.StackTrace);

                        response.Status = "DECLINED";
                        response.Message = "Unknown error, please contact Support. ";
                        response.ErrNumber = "2800.6";
                    }

                    #endregion Send Email Receipt
                }
                else
                {
                    if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                    {
                        response.Status = "The receipt will be sent to the contact information you have provided.";
                        response.Message = "";
                        response.ErrNumber = "0";
                    }
                    else
                    {
                        response.Status = "DECLINED";
                        response.Message = "An error occured.";
                        response.ErrNumber = "2800.7";
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "Gift Receipt", ex.StackTrace);

                response.Status = "DECLINED";
                response.Message = "Unknown error, please contact Support. ";
                response.ErrNumber = "2800.8";
            }

            return response;
        }

        public Response MerchantPushPayments(Request request)
        {
            string action = string.Empty;
            Response response = new Response();
            
            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    var mobileApp = new SDGDAL.Entities.MobileApp();
                    var account = new SDGDAL.Entities.Account();
                    var card = new CreditCardDetails();

                    action = "checking mobile app availability.";

                    if (!String.IsNullOrEmpty(request.POSWSRequest.RToken))
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

                    #region Decrypt and Parse Credit card Data
                    try
                    {
                        action = "decrypting and parsing card data.";

                        string key = "7o#$rlf{@gXI{YKwsYAhGwB/uljVkUk1qrcIQGM$";
                        var serializer = new JsonNetSerializer();

                        JsonWebToken.JsonSerializer = serializer;

                        string decCardData = JsonWebToken.Decode(request.EncCardDetails, key, verify: false);
                        var cardDetails = JsonConvert.DeserializeObject<CreditCardDetails>(decCardData);

                        card.CardNumber = cardDetails.CardNumber;
                    }
                    catch (Exception ex)
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
                    string E_CARD;
                    byte[] desKey;
                    byte[] desIV;

                    //card number masking
                    string s = NE_CARD.Substring(NE_CARD.Length - 4);
                    string r = new string('*', NE_CARD.Length);
                    string MASK_CARD = r + s;

                    E_CARD = Utility.GenerateSymmetricKeyAndEcryptData(MASK_CARD, out desKey, out desIV);

                    transaction.Key = new SDGDAL.Entities._Key();
                    transaction.Key.Key = Convert.ToBase64String(desKey);
                    transaction.Key.IV = Convert.ToBase64String(desIV);

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
                    transaction.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.Contactless);
                    transaction.Notes = string.Empty;
                    transaction.RefNumSales = DateTime.Now.ToString("yyyyMMddhhmmss");
                    transaction.RefNumApp = request.RefNumApp;
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

                        #region MVisa Offline
                        if (mid.Switch.SwitchCode == "MVisaOffline")
                        {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();

                            string traceNumber = SDGUtil.Functions.GenerateSystemTraceAudit();
                            string retrievalRefNum = SDGUtil.Functions.GenerateRefNumUsingDate() + traceNumber;
                            var objMerchantPayment = new VisaDirectRequest
                            {
                                acquirerCountryCode = SDGUtil.Functions.GetNumericCountryCode(mid.Country), 
                                acquiringBin = Convert.ToInt32(mid.AcquiringBin),
                                amount = Convert.ToDecimal(request.Amount),
                                businessApplicationId = "MP",
                                cardAcceptor = new CardAcceptor
                                {
                                    address = new Address()
                                    {
                                        city = mid.City, 
                                        country = mid.Country 
                                    },
                                    idCode = "CA-IDCode-" + SDGUtil.Functions.GenerateRetrievalNumber(5),
                                    name = mid.Merchant.MerchantName
                                },
                                localTransactionDateTime = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"),
                                purchaseIdentifier = new PurchaseIdentifier()
                                {
                                    referenceNumber = transactionAttempt.TransactionId + "-" + transactionAttempt.TransactionAttemptId, //Transaction number
                                    type = 1
                                },
                                recipientName = mid.Merchant.MerchantName,
                                recipientPrimaryAccountNumber = mid.Param_12, 
                                retrievalReferenceNumber = retrievalRefNum,
                                senderAccountNumber = "4027290077881587", //request from enc card data of consumer
                                senderName = request.Name,
                                systemsTraceAuditNumber = traceNumber,
                                transactionCurrencyCode = request.Currency
                            };

                            string requestData = new JavaScriptSerializer().Serialize(objMerchantPayment);
                            string baseUri = "visadirect/";
                            string resourcePath = "mvisa/v1/merchantpushpayments";

                            var apiResponse = gateway.ProcessVisaDirect(baseUri + resourcePath, requestData);

                            if (apiResponse.Status == "Approved")
                            {
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = apiResponse.approvalCode;
                                transactionAttempt.ReturnCode = apiResponse.actionCode;
                                transactionAttempt.SeqNumber = "";
                                transactionAttempt.TransNumber = apiResponse.retrievalReferenceNumber;
                                transactionAttempt.PosEntryMode = 0;

                                transactionAttempt.BatchNumber = _batchRepo.GenerateBatchNumber(mobileApp.MobileAppId, Convert.ToInt32(SDGDAL.Enums.Ref_PaymentType.Credit));
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = apiResponse.cardAcceptor.terminalId;
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
                                transactionAttempt.AuthNumber = apiResponse.approvalCode;
                                transactionAttempt.ReturnCode = apiResponse.actionCode;
                                transactionAttempt.TransNumber = apiResponse.retrievalReferenceNumber;
                                transactionAttempt.SeqNumber = null;
                                transactionAttempt.TransNumber = null;
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = " API NovaToPay Purchase Declined." + apiResponse.Message + ":" + apiResponse.responseCode;
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                response.POSWSResponse.ErrNumber = apiResponse.responseCode;
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
                    response.Timestamp = SDGUtil.Functions.Format_Datetime(transactionAttempt.DateReceived);
                    response.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)transaction.TransactionEntryTypeId);
                    response.Total = Convert.ToDecimal(transactionAttempt.Amount.ToString("N2"));
                    response.CardNumber = SDGUtil.Functions.HashCardNumber(card.CardNumber);
                    response.Currency = request.Currency;
                }
            }
            catch(Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message.ToString(), "MerchantPushPayments", ex.Message);

                response.POSWSResponse.ErrNumber = "2900.9";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.POSWSResponse.Status = "DECLINED";
            }

            return response;
        }

        public Response CashInPushPayments(Request request)
        {
            string action = string.Empty;
            Response response = new Response();

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    var mobileApp = new SDGDAL.Entities.MobileApp();
                    var account = new SDGDAL.Entities.Account();
                    var card = new CreditCardDetails();

                    action = "checking mobile app availability.";

                    if (!String.IsNullOrEmpty(request.POSWSRequest.RToken))
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

                    #region Decrypt and Parse Credit card Data
                    try
                    {
                        action = "decrypting and parsing card data.";

                        string key = "7o#$rlf{@gXI{YKwsYAhGwB/uljVkUk1qrcIQGM$";
                        var serializer = new JsonNetSerializer();

                        JsonWebToken.JsonSerializer = serializer;

                        string decCardData = JsonWebToken.Decode(request.EncCardDetails, key, verify: false);
                        var cardDetails = JsonConvert.DeserializeObject<CreditCardDetails>(decCardData);

                        card.CardNumber = cardDetails.CardNumber;
                    }
                    catch (Exception ex)
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
                    string E_CARD;
                    byte[] desKey;
                    byte[] desIV;

                    //card number masking
                    string s = NE_CARD.Substring(NE_CARD.Length - 4);
                    string r = new string('*', NE_CARD.Length);
                    string MASK_CARD = r + s;

                    E_CARD = Utility.GenerateSymmetricKeyAndEcryptData(MASK_CARD, out desKey, out desIV);

                    transaction.Key = new SDGDAL.Entities._Key();
                    transaction.Key.Key = Convert.ToBase64String(desKey);
                    transaction.Key.IV = Convert.ToBase64String(desIV);

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
                    transaction.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.Contactless);
                    transaction.Notes = string.Empty;
                    transaction.RefNumSales = DateTime.Now.ToString("yyyyMMddhhmmss");
                    transaction.RefNumApp = request.RefNumApp;
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

                        #region MVisa Offline
                        if (mid.Switch.SwitchCode == "MVisaOffline")
                        {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();

                            string traceNumber = SDGUtil.Functions.GenerateSystemTraceAudit();
                            string retrievalRefNum = SDGUtil.Functions.GenerateRefNumUsingDate() + traceNumber;
                            var objMerchantPayment = new VisaDirectRequest
                            {
                                acquirerCountryCode = SDGUtil.Functions.GetNumericCountryCode(mid.Country),
                                acquiringBin = Convert.ToInt32(mid.AcquiringBin),
                                amount = Convert.ToDecimal(request.Amount),
                                businessApplicationId = "MP",
                                cardAcceptor = new CardAcceptor
                                {
                                    address = new Address()
                                    {
                                        city = mid.City,
                                        country = mid.Country
                                    },
                                    idCode = "CA-IDCode-" + SDGUtil.Functions.GenerateRetrievalNumber(5),
                                    name = mid.Merchant.MerchantName
                                },
                                localTransactionDateTime = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"),
                                purchaseIdentifier = new PurchaseIdentifier()
                                {
                                    referenceNumber = transactionAttempt.TransactionId + "-" + transactionAttempt.TransactionAttemptId, //Transaction number
                                    type = 1
                                },
                                recipientName = mid.Merchant.MerchantName,
                                recipientPrimaryAccountNumber = mid.Param_12,
                                retrievalReferenceNumber = retrievalRefNum,
                                senderAccountNumber = "4027290077881587", //request from enc card data of consumer
                                senderName = request.Name,
                                systemsTraceAuditNumber = traceNumber,
                                transactionCurrencyCode = request.Currency
                            };

                            string requestData = new JavaScriptSerializer().Serialize(objMerchantPayment);
                            string baseUri = "visadirect/";
                            string resourcePath = "mvisa/v1/merchantpushpayments";

                            var apiResponse = gateway.ProcessVisaDirect(baseUri + resourcePath, requestData);

                            if (apiResponse.Status == "Approved")
                            {
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = apiResponse.approvalCode;
                                transactionAttempt.ReturnCode = apiResponse.actionCode;
                                transactionAttempt.SeqNumber = "";
                                transactionAttempt.TransNumber = apiResponse.retrievalReferenceNumber;
                                transactionAttempt.PosEntryMode = 0;

                                transactionAttempt.BatchNumber = _batchRepo.GenerateBatchNumber(mobileApp.MobileAppId, Convert.ToInt32(SDGDAL.Enums.Ref_PaymentType.Credit));
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = apiResponse.cardAcceptor.terminalId;
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
                                transactionAttempt.AuthNumber = apiResponse.approvalCode;
                                transactionAttempt.ReturnCode = apiResponse.actionCode;
                                transactionAttempt.TransNumber = apiResponse.retrievalReferenceNumber;
                                transactionAttempt.SeqNumber = null;
                                transactionAttempt.TransNumber = null;
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = " API NovaToPay Purchase Declined." + apiResponse.Message + ":" + apiResponse.responseCode;
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                response.POSWSResponse.ErrNumber = apiResponse.responseCode;
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
                    response.Timestamp = SDGUtil.Functions.Format_Datetime(transactionAttempt.DateReceived);
                    response.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)transaction.TransactionEntryTypeId);
                    response.Total = Convert.ToDecimal(transactionAttempt.Amount.ToString("N2"));
                    response.CardNumber = SDGUtil.Functions.HashCardNumber(card.CardNumber);
                    response.Currency = request.Currency;
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message.ToString(), "MerchantPushPayments", ex.Message);

                response.POSWSResponse.ErrNumber = "2900.9";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.POSWSResponse.Status = "DECLINED";
            }

            return response;
        }

        public Response CashOutPayments(Request request)
        {
            string action = string.Empty;
            Response response = new Response();

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    var mobileApp = new SDGDAL.Entities.MobileApp();
                    var account = new SDGDAL.Entities.Account();
                    var card = new CreditCardDetails();

                    action = "checking mobile app availability.";

                    if (!String.IsNullOrEmpty(request.POSWSRequest.RToken))
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

                    #region Decrypt and Parse Credit card Data
                    try
                    {
                        action = "decrypting and parsing card data.";

                        string key = "7o#$rlf{@gXI{YKwsYAhGwB/uljVkUk1qrcIQGM$";
                        var serializer = new JsonNetSerializer();

                        JsonWebToken.JsonSerializer = serializer;

                        string decCardData = JsonWebToken.Decode(request.EncCardDetails, key, verify: false);
                        var cardDetails = JsonConvert.DeserializeObject<CreditCardDetails>(decCardData);

                        card.CardNumber = cardDetails.CardNumber;
                    }
                    catch (Exception ex)
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
                    string E_CARD;
                    byte[] desKey;
                    byte[] desIV;

                    //card number masking
                    string s = NE_CARD.Substring(NE_CARD.Length - 4);
                    string r = new string('*', NE_CARD.Length);
                    string MASK_CARD = r + s;

                    E_CARD = Utility.GenerateSymmetricKeyAndEcryptData(MASK_CARD, out desKey, out desIV);

                    transaction.Key = new SDGDAL.Entities._Key();
                    transaction.Key.Key = Convert.ToBase64String(desKey);
                    transaction.Key.IV = Convert.ToBase64String(desIV);

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
                    transaction.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.Contactless);
                    transaction.Notes = string.Empty;
                    transaction.RefNumSales = DateTime.Now.ToString("yyyyMMddhhmmss");
                    transaction.RefNumApp = request.RefNumApp;
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

                        #region MVisa Offline
                        if (mid.Switch.SwitchCode == "MVisaOffline")
                        {
                            GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();

                            string traceNumber = SDGUtil.Functions.GenerateSystemTraceAudit();
                            string retrievalRefNum = SDGUtil.Functions.GenerateRefNumUsingDate() + traceNumber;
                            var objMerchantPayment = new VisaDirectRequest
                            {
                                acquirerCountryCode = SDGUtil.Functions.GetNumericCountryCode(mid.Country),
                                acquiringBin = Convert.ToInt32(mid.AcquiringBin),
                                amount = Convert.ToDecimal(request.Amount),
                                businessApplicationId = "MP",
                                cardAcceptor = new CardAcceptor
                                {
                                    address = new Address()
                                    {
                                        city = mid.City,
                                        country = mid.Country
                                    },
                                    idCode = "CA-IDCode-" + SDGUtil.Functions.GenerateRetrievalNumber(5),
                                    name = mid.Merchant.MerchantName
                                },
                                localTransactionDateTime = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"),
                                purchaseIdentifier = new PurchaseIdentifier()
                                {
                                    referenceNumber = transactionAttempt.TransactionId + "-" + transactionAttempt.TransactionAttemptId, //Transaction number
                                    type = 1
                                },
                                recipientName = mid.Merchant.MerchantName,
                                recipientPrimaryAccountNumber = mid.Param_12,
                                retrievalReferenceNumber = retrievalRefNum,
                                senderAccountNumber = "4027290077881587", //request from enc card data of consumer
                                senderName = request.Name,
                                systemsTraceAuditNumber = traceNumber,
                                transactionCurrencyCode = request.Currency
                            };

                            string requestData = new JavaScriptSerializer().Serialize(objMerchantPayment);
                            string baseUri = "visadirect/";
                            string resourcePath = "mvisa/v1/merchantpushpayments";

                            var apiResponse = gateway.ProcessVisaDirect(baseUri + resourcePath, requestData);

                            if (apiResponse.Status == "Approved")
                            {
                                transactionAttempt.TransactionAttemptId = transactionAttempt.TransactionAttemptId;
                                transactionAttempt.AuthNumber = apiResponse.approvalCode;
                                transactionAttempt.ReturnCode = apiResponse.actionCode;
                                transactionAttempt.SeqNumber = "";
                                transactionAttempt.TransNumber = apiResponse.retrievalReferenceNumber;
                                transactionAttempt.PosEntryMode = 0;

                                transactionAttempt.BatchNumber = _batchRepo.GenerateBatchNumber(mobileApp.MobileAppId, Convert.ToInt32(SDGDAL.Enums.Ref_PaymentType.Credit));
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = apiResponse.cardAcceptor.terminalId;
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
                                transactionAttempt.AuthNumber = apiResponse.approvalCode;
                                transactionAttempt.ReturnCode = apiResponse.actionCode;
                                transactionAttempt.TransNumber = apiResponse.retrievalReferenceNumber;
                                transactionAttempt.SeqNumber = null;
                                transactionAttempt.TransNumber = null;
                                transactionAttempt.DisplayReceipt = "";
                                transactionAttempt.DisplayTerminal = "";
                                transactionAttempt.DateReceived = DateTime.Now;
                                transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                transactionAttempt.Notes = " API NovaToPay Purchase Declined." + apiResponse.Message + ":" + apiResponse.responseCode;
                                transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                response.POSWSResponse.ErrNumber = apiResponse.responseCode;
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
                    response.Timestamp = SDGUtil.Functions.Format_Datetime(transactionAttempt.DateReceived);
                    response.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)transaction.TransactionEntryTypeId);
                    response.Total = Convert.ToDecimal(transactionAttempt.Amount.ToString("N2"));
                    response.CardNumber = SDGUtil.Functions.HashCardNumber(card.CardNumber);
                    response.Currency = request.Currency;
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message.ToString(), "MerchantPushPayments", ex.Message);

                response.POSWSResponse.ErrNumber = "2900.9";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.POSWSResponse.Status = "DECLINED";
            }

            return response;
        }
    }
}
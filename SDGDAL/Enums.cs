namespace SDGDAL
{
    public class Enums
    {
        #region HelpSupport

        public enum HelpSupportType
        {
            Technical = 1,
            Customer = 2
        }

        public enum HelpSupportItemType
        {
            CallCenter = 1,
            User = 2
        }

        public enum HelpSupportSearchBy
        {
            All = 0,
            ID = 1
        }

        public enum HelpSupportErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3
        }

        public enum SupportContactBackMethod
        {
            Email = 1,
            Phone = 2
        }

        #endregion HelpSupport

        #region MobileApp

        public enum MobileAppUpdate
        {
            All = 0,
            UpdatePending = 1
        }

        public enum MobileAppSearchBy
        {
            All = 0,
            ID = 1,
            ActivationCode = 2,
            MobileAppSetting_PosId = 3,
            ActivationCodePin = 4,
            LanguageID = 5,
            MobileAppID = 6,
            RefHybridPosTypeID = 7
        }

        public enum MobileAppLinkSearchBy
        {
            All = 0,
            ID = 1,
            UserID = 2,
            MobileAppID = 3,
            UserIdAndMobileAppID = 4
        }

        public enum MobileAppErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4,
            Inactive = 5,
            InvalidUpdateBy = 6,
            InsertFailed = 7,
            UpdateFailed = 8
        }

        public enum MobileAppDeleteType
        {
            HideMobileApp = 0,
            DeleteMobileApp = 1
        }

        public enum MobileAppTransSearchBy
        {
            NotSet = 0,
            TransactionNum = 1,
            MobileAppId = 2,
            RefNumApp = 3
        }

        public enum MobileAppTransType
        {
            Credit = 1,
            Debit = 2,
            Cash = 3
        }

        public enum MobileAppCardEntryType
        {
            Swipe = 1,
            Manual = 2
        }

        public enum MobileAppTransaxTransType
        {
            Sale = 1,
            Return = 2,
            Void = 3
        }

        public enum MobileAppCardType
        {
            MasterCard = 1,
            Visa = 2,
            AmericanExpress = 3,
            Diners = 4,
            Discover = 5,
            JCB = 6,
            Chinabank = 7,
            Citibank = 8,
            Barclaycard = 9,
            Debit = 10,
            Cheque = 11,
            Cash = 12,
            EBT = 13
        }

        public enum MobileDeviceSearchType
        {
            All = 1,
            MobileAppID = 2,
            Platform = 3,
            PhoneNumber = 4
        }

        #endregion MobileApp

        #region Language

        public enum LanguageSearchBy
        {
            All = 0,
            ID = 1,
            NewInstallID = 2,
            MobileAppSettingsLanguageID = 3,
            MobileAppSettingsLanguageDefaultID = 4,
            IsoCode = 5,
            MerchantID = 6
        }

        public enum LanguageDeleteType
        {
            DeleteLanguage = 0,
            All = 1,
            ID = 2,
            AllByMobileAppSettingsLanguageID = 3,
            AllByMobAppNewInstallID = 4
        }

        public enum LanguageErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            AlreadyExists = 4
        }

        #endregion Language

        #region Address

        public enum AddressSearchBy
        {
            All = 0,
            ID = 1
        }

        public enum AddressErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4
        }

        public enum AddressDeleteType
        {
            HideAddress = 0,
            DeleteAddress = 1
        }

        #endregion Address

        #region Switch

        public enum SwitchSearchBy
        {
            All = 0,
            ID = 1,
            Switch_ID = 2,
            Partner_ID = 3
        }

        public enum SwitchErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4,
            Deactivated = 5
        }

        //TODO: To be deleted in favour of Switches
        //public enum SwitchAPITypes
        //{
        //    CTPayment = 1,
        //    Moneris = 2,
        //    DataCollection = 3,
        //    PaymentData_Usd = 4,
        //    TransaxPayemet = 5
        //}

        public enum Switches
        {
            CTPayment = 24,
            Moneris = 2,
            DataCollection = 26,
            PaymentData_Usd = 4,
            TransaxPayment = 23,
            VaultPosGateway = 28,
            Unichannel = 38,
            ApiMastercard = 50,
            ApiPesoPay = 51,
            TransactaEvertec = 52,
            ApiMastercardDemo = 53,
            IPS = 54,
            DatacCollectAddress = 55,
            SuperPay = 56,
            ApiElavon = 68,
            PlanetPay = 69,
            VaultPosGateway1_5 = 70,
            FroogalPay = 71,
            HybridSwitch = 74,
            CardConnect = 75,
            DataCash = 76,
            CTPayment2 = 77,
            CardConnect_OneTrack = 78
        }

        public enum SwitchAPITypes_SearchByType
        {
            All = 0,
            ApiName = 1,
            isActive = 2,
            MerchantID = 3
        }

        public enum SwitchDeleteType
        {
            HideSwitch = 0,
            DeleteSwitch = 1
        }

        public enum MidsGroupTypes
        {
            Ordered = 1,
            Balanced = 2
        }

        #endregion Switch

        #region Reseller

        public enum ResellerSearchBy
        {
            All = 0,
            ID = 1,
            Reseller_ID = 2,
            ID_Reseller_Only = 3
        }

        public enum ResellerErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4,
            InsertFailed = 8,
            DeleteFailed = 9
        }

        public enum ResellerDeleteType
        {
            HideReseller = 0,
            DeleteReseller = 1
        }

        #endregion Reseller

        #region Merchant

        public enum MerchantSearchBy
        {
            All = 0,
            ID = 1,
            MerchantID = 2,
            MerchantLocationID = 3,
            MerchantMidTypeId = 4,
            MerchantIdAndMidId = 5,
            MerchantLocationIdAndMidId = 6,
            MerchantLocationPosID = 7,
            TaxID = 8,
            ApiLogin = 9,
            ApiKey = 10
        }

        public enum MerchantErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4,
            Inactive = 5,
            InsertFailed = 6,
            UpdateFailed = 7,
            AuthenticationFail = 8,
            NotUnique = 9
        }

        public enum MerchantDeleteType
        {
            HideMerchant = 0,
            DeleteMerchant = 1,
            DeleteMerchantById = 2
        }

        #endregion Merchant

        #region Partner

        public enum PartnerSearchBy
        {
            All = 0,
            ID = 1,
            Partner_ID = 2,
            ID_Partner_Only = 3
        }

        public enum PartnerErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4,
            InsertFailed = 8,
            DeleteFailed = 9
        }

        public enum PartnerDeleteType
        {
            HidePartner = 0,
            DeletePartner = 1
        }

        #endregion Partner

        #region Parent (eg Hybrid, Reseller, etc) Associations

        public enum ParentType
        {
            Partner = 1,
            Reseller = 2,
            Merchant = 3,
            Branch = 4
        }

        #endregion Parent (eg Hybrid, Reseller, etc) Associations

        #region Specific Associations

        public enum SpecificAssociationType
        {
            HybridPaytech = 1,
            Merchant = 2,
            MerchantLocation = 3,
            POS = 4,
            Reseller = 5,
            Partner = 6
        }

        #endregion Specific Associations

        #region Permission

        public enum PermissionSet
        {
            Admin = 1,
            Manager = 2,
            Employee = 3
        }

        #endregion Permission

        #region Users

        public enum UsersRoles
        {
            Admin = 1,
            CallCenter = 2,
            Reseller = 3,
            Merchant = 4
        }

        public enum UsersSearchBy
        {
            All = 0,
            ID = 1,
            MerchantID = 2,
            UserID = 3,
            MerchantLocationID = 4,
            ResellerID = 5,
            UserNamePW = 6,
            ActivationCodePin = 7,
            PinAndMerchID = 8,
            UserName = 9
        }

        public enum AccessSearchBy
        {
            ID = 0,
            MerchantID = 1,
            AccessGroupID = 2,
            AccessLevelID = 3
        }

        public enum UsersErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4,
            Duplicate = 5,
            Unauthorized = 6,
            Inactive = 7,
            InsertFailed = 8,
            DeleteFailed = 9
        }

        public enum UsersDeleteType
        {
            HideUser = 0,
            DeleteUser = 1,
            DeleteUserOhter = 2
        }

        public enum UsersAccessLevel
        {
            Merchant = 1,
            MerchantLocation = 2,
            MerchantLocationPOS = 3
        }

        #endregion Users

        #region Transaction

        public enum AccountType
        {
            Chequing = 1,
            Savings = 2,
            Current = 3,
            BalanceChequing = 4,
            BalanceSavings = 5
        }

        public enum ChequeType
        {
            Personal = 1,
            Corporate = 2,
            Governemnt = 3
        }

        public enum TransactionType
        {
            Capture = 1,
            PreAuth = 2,
            Sale = 3,
            Void = 4,
            Refund = 5,
            Declined = 6,
            BalanceInquiry = 7,
            CashBack = 8,
            Reversed = 9
        }

        public enum TransactionEntryType
        {
            CreditPin = 1,
            CreditSignature = 2,
            CreditManual = 3,
            Debit = 4,
            CreditSwipe = 5,
            Cheque = 6,
            EBTSwipePin = 7,
            Cash = 8,
            EMV = 9,
            Contactless = 10
        }

        public enum TransactionSearchBy
        {
            All = 0,
            ID = 1,
            TransNum = 2,
            AuthNum = 3,
            RefNum = 4,
            AppRefNum = 5,
            MerchantID = 6,
            MerchantLocationID = 7,
            MerchantPOS = 8,
            MerchantUser = 9,
            DeviceID = 10,
            TransactionID = 11,
            CurrencyAbbr = 12,
            IdForBilling = 13,
            SalesRefNum = 14,
            RefPaymentTypeAndTransactionID = 15
        }

        public enum TransactionErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4,
            Declined_Mid_Deactivated = 5,
            Declined_Switch_Deactivated = 6,
            Declined = 7,
            RequestAddress = 8,
            InsertFailed = 9,
            UpdateFailed = 10
        }

        public enum DeleteTransactionType
        {
            DeleteTransaction = 0,
            RollbackTransaction = 1
        }

        public enum SendReceiptType
        {
            SMS = 0,
            Email = 1
        }

        public enum SwipeDevice
        {
            NotSet = 0,
            Rambler = 1,
            RamblerDemo = 2, //Rambler
            WisePad1 = 3, //Black Wisepad
            WisePad2 = 4, //White WisePad
            ChipperAudioI = 5, 
            ChipperAudioII = 6,
            WisePos = 7, //Device built-in to phone
            WisePadPlus = 8,//With Printer
            ChipperBT1 = 9, //Mini BT Chipper
            ChipperBT2 = 10, 
            QPosMini = 11
        }

        public enum CardTypes
        {
            MasterCard = 1,
            Visa = 2,
            AmericanExpress = 3,
            Diners = 4,
            Discover = 5,
            JCB = 6,
            ChinaUnionPay = 7,
            Citibank = 8,
            Barclaycard = 9,
            Debit = 10,
            Cheque = 11,
            Cash = 12,
            EBT = 13
        }

        public enum Currency
        {
            CanadianDollar = 1,
            UnitedSatesDollar = 3,
            EuropeanEuro = 4,
            BritishPound = 5,
            JapaneseYen = 6,
            PhillipinesPeso = 7,
            SwissFranc = 8,
            AustralianDollar = 9,
            SwedishKrona = 10,
            HongKongDollar = 11,
            ArgentinePeso = 12,
            BrazilianReal = 13,
            ColumbianPeso = 14,
            VenezuelanBolivar = 15,
            ChineseYuan = 16,
            SingaporeDollar = 17,
            TurkishLira = 18
        }

        public enum CardAction
        {
            NotSet = 0,
            SwipeCard = 1,
            EMV = 2,
        }

        public enum CurrencyISOCode
        {
            PHP = 608,
            CAD = 124,
            USD = 840,
            EUR = 4,
            GBP = 5,
            JPY = 6,
            CHF = 8,
            AUD = 9,
            SEK = 10,
            HKD = 11,
            ARS = 12,
            BRL = 13,
            COP = 14,
            VEF = 15,
            CNY = 16,
            SGD = 17,
            TRY = 18
        }

        #endregion Transaction

        #region Billing

        public enum InvoiceSearchBy
        {
            All = 0,
            ID = 1,
            MerchantID = 2,
            ResellerID = 3
        }

        public enum BillingErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4,
            Duplicate = 5,
            UpdateFailed = 6
        }

        public enum ReceiptSearchBy
        {
            All = 0,
            ID = 1,
            MerchantID = 2,
            Trans = 3,
            MerchID_IsActive = 4
        }

        public enum BillingCycleType
        {
            Weekly = 1,
            BiWeekly = 2,
            Monthly = 3,
            Quarterly = 4,
            Annually = 5
        }

        #endregion Billing

        #region Reports

        public enum ReportsSearchBy
        {
            All = 0,
            Merchant = 1,
            Location = 2,
            POS = 3,
            User = 4,
            MobileApp = 5,
            TransType = 6,
            Date = 7,
            ID = 8,
            TransID = 9,
            Reseller = 10
        }

        public enum ReportsErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4
        }

        #endregion Reports

        #region Encryption

        public enum EncryptionErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2
        }

        #endregion Encryption

        #region License

        public enum LicenseType
        {
            Manager = 1,
            User = 2,
            Device = 3,
            App = 4
        }

        public enum LicenseSearchBy
        {
            All = 0,
            ID = 1
        }

        public enum LicenseErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3
        }

        #endregion License

        #region MobileAppAuthentification

        public enum MobileAppAuthentificationUpdate
        {
            All = 0,
            UpdatePending = 1
        }

        public enum MobileAppAuthentificationSearchBy
        {
            All = 0,
            ID = 1,
            EmailAndPassword = 2,
            UserID = 3
        }

        public enum MobileAppAuthentificationErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            InvalidUpdateBy = 4
        }

        public enum MobileAppAuthentificationDeleteType
        {
            HideMobileApp = 0,
            DeleteMobileApp = 1
        }

        #endregion MobileAppAuthentification

        #region Mids

        public enum MidsSearchBy
        {
            All = 0,
            ID = 1,
            MidsGroup_ID = 2,
            POS_ID = 3,
            Location_ID = 4,
            Merchant_ID = 5,
            POS_ID_AND_CardType_ID = 6,
            RolloverGroup_ID = 7,
            Mids_ID = 8,
            RolloverGroup_ID_AND_ORDER = 9,
            MidsGroup_ID_AND_ORDER = 10,
        }

        public enum MidsErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4,
            Deactivated = 5
        }

        public enum MidsBatchCloseType
        {
            Auto = 1,
            Manual = 2
        }

        #endregion Mids

        #region Mid Groups

        public enum MidGroupType
        {
            Ordered = 1,
            Balanced = 2
        }

        #endregion Mid Groups

        #region Tax

        public enum TaxCalc
        {
            Single = 1, // One tax only
            Both = 2, // Taxes calculated SEPERATELY
            Combination = 3, // Second tax calculated ON TOP of first tax (EG: a la Quebec)
            DoNotShow = 4 // Taxes not calculated or displayed
        }

        #endregion Tax

        #region Terms of Service

        public enum TOS_SearchBy
        {
            All = 0,
            ID = 2,
            MerchantID = 3,
            MerchantAndLanguage = 4
        }

        #endregion Terms of Service

        #region Global Settings

        public enum GlobalSettingsErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoBalance = 4,
            NoResultFound = 5,
            Disabled = 6,
            All = 7,
            ID = 8
        }

        #endregion Global Settings

        #region Features

        public enum FeaturesErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoBalance = 4,
            NoResultFound = 5,
            Disabled = 6,
            InsertFailed = 7,
            UpdateFailed = 8
        }

        public enum Features_SearchBy
        {
            All = 0,
            ID = 1,
            Merchant_ID = 2,
            User_ID = 3,
            Transaction = 4,
            MerchantSmsPackage_ID = 5,
            SmsPackage_ID = 6,
            IsActive = 7,
            ActiveByMerchant = 8,
            ByTrans = 9,
            ForBilling = 10,
            CountryCode = 11,
            Continent = 12,
            MobileAppSettingFeature_ID = 13,
            Email_ID = 14,
            MobileApp_ID = 15,
            Print_ID = 16,
            Disclaimer_ID = 17,
            TOS_ID = 18,
            Enable = 19,
            Language = 20,
            Merchant_and_Language = 21,
            Country_ID = 22,
            StateISOCode = 23,
            State_Name = 24,
            ISO_Code = 25,
            POS_ID = 26,
            FeatDebitCashBack_ID = 27,
            FeatCloseBatch_ID = 28,
            EmailServer_ID = 29,
            Tip_ID = 30,
            EmailServerMerchant_ID = 31
        }

        #region SMS

        public enum SMS_Packages_SearchBy
        {
            All = 0,
            ID = 1,
            CountryCode = 2,
            MerchantID = 3
        }

        public enum Merchant_SMS_Packages_SearchBy
        {
            All = 0,
            ID = 1,
            MerchantID = 2,
            SmsPackageID = 3
        }

        #endregion SMS

        #endregion Features

        #region Continents

        public enum Continents
        {
            NorthAmerica = 1,
            SountAmerica = 2,
            Asia = 3,
            Europe = 4,
            Oceania = 5,
            Africa = 6
        }

        #endregion Continents

        #region Countries

        public enum Country_SearchBy
        {
            All = 0,
            ID = 1,
            Continent = 2,
            CountryName = 3,
            CountryCode = 4,
            ISOCode = 5
        }

        #endregion Countries

        #region MobileAudit

        public enum MobileAuditErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4
        }

        #endregion MobileAudit

        #region Device Management 2013-06-20

        public enum Ref_Platform
        {
            Android = 1,
            BlackBerry = 2,
            iOS = 3,
            WindowsMobile6 = 4,
            WindowsPhone7 = 5,
            WindowsPhone8 = 6
        }

        public enum MasterDevice
        {
            Rover = 1,
            Rambler = 2,
            MotorolaSwipe = 3,
            ZebraRW220Swipe = 4,
            ZebraEM220Swipe = 5,
            InfinaTab = 6,
            Koamtac = 7,
            MotorolaEMV = 8,
            ZebraMZ220 = 9,
            ZebraiMZ220 = 10,
            ZebraRW220Printer = 11,
            ZebraEM220Printer = 12,
            ZebraEM220iiPrinter = 13,
            ZebraMZ220wifi = 14,
            DailySystems = 15,
            CaptuvoSL22 = 16,
            ZebraEM220iiSwipe = 17
        }

        public enum Ref_PaymentType
        {
            Credit = 1,
            Debit = 2,
            Cash = 3,
            EMV = 4,
            ACH = 5,
            All = 6
        }

        public enum Ref_FlowType
        {
            HybridBackOffice = 1,
            HybridEMV = 2,
            Resto = 3
        }

        public enum DeviceSearchBy
        {
            All = 1,
            ID = 2,
            DeviceID = 3,
            MasterDeviceID = 4,
            MobileAppID = 5,
            MerchantID = 6,
            DeviceIdAndMobileAppID = 7,
            SerialNumber = 8,
            RefDeviceTypeID = 9,
            RefFlowTypeID = 10,
            RefHybridPosTypeID = 11,
            RefPlatformID = 12,
            RefPaymentTypeID = 13,
            RefDeviceID = 14
        }

        public enum DeviceError
        {
            NoError = 0,
            dbError = 1,
            InvalidSearchBy = 2,
            NoResultFound = 3,
            UnknownError = 4,
            InsertFailed = 5,
            UpdateFailed = 6
        }

        public enum Ref_HybridPosType
        {
            HybridPosCC = 1,
            HybridPosPIN = 2,
            HybridPosEMV = 3,
            HybridPos4 = 4
        }

        public enum Ref_DeviceType
        {
            PaymentDevice = 1,
            Printer = 2,
            Miscellaneous = 3
        }

        public enum Ref_Device
        {
            Rover = 1,
            Rambler = 2,
            MotorolaSnapOn = 3,
            InfineaTabIPC = 4,
            Koamtec = 5,
            DailySystemDS247 = 6,
            CaptuvoSL22 = 7,
            RW220 = 8,
            EM220 = 9,
            EM220II = 10,
            MZ220 = 11,
            MZ220WIFI = 12,
            IMZ220 = 13,
            EM220SwipeAndPrint = 17,
            RW220SwipeAndPrint = 19,
            EM220iiSwipe = 20
        }

        #endregion Device Management 2013-06-20

        #region Batch 2013-09-26

        public enum BatchError
        {
            NoError = 0,
            dbError = 1,
            InvalidSearchBy = 2,
            NoResultFound = 3,
            UnknownError = 4,
            InsertFailed = 5,
            UpdateFailed = 6
        }

        public enum BatchSearchBy
        {
            All = 1,
            ID = 2,
            BatchNumber = 3,
            Batch_ID = 4,
            Merchant_ID = 5,
            Location_ID = 6,
            POS_ID = 7
        }

        public enum BatchCloseBy
        {
            Auto = 1,
            MobileApp = 2,
            BackOffice = 3
        }

        #endregion Batch 2013-09-26

        #region EMV 2013-06-28

        public enum HybridSwitch_SearchBy
        {
            All = 1,
            ID = 2,
            MerchantID = 3,
            MobileAppID = 4,
            HybridSwitchMerchantID = 5,
            HybridSwitchMerchantNum = 6,
            HybridSwitchTerminalNum = 7,
            MerchantAndStoreID = 8,
            TerminalID = 9,
            LastByMerchant = 10
        }

        #endregion EMV 2013-06-28

        #region Authentication Error

        public enum AuthenticationStatus
        {
            Success = 1,
            SessionExpired = 2,
            NotAllowedToFolder = 3,
            NewSession = 4,
            NotAllowedToSpecificFolder = 5, /* if merchantLocation to access merchant page*/
            NoPermission = 6, /* if employee to access admin page*/
            Error = 9
        }

        #endregion Authentication Error

        #region Transaction Reference Search By [SDS 28]

        public enum TransactionReferenceSearchBy
        {
            All = 1,
            ID = 2,
            AppRefNum = 3,
            SalesRefNum_And_MerchantID = 4,
            MerchantID = 5,
            RefPaymentTypeAndTransID = 6
        }

        #endregion Transaction Reference Search By [SDS 28]

        public enum RollbackTable
        {
            Address = 1,
            Telephone = 2,
            ProfileTelephoneNumber = 3,
            SpecificAssociationLink = 4,
            User = 5,
            UserLink = 6,
            Reseller = 7,
            ResellerTelephone = 8,
            Partner = 9,
            PartnerTelephone = 10,
            POS = 11
        }

        public enum RollbackErrNum
        {
            NoError = 0,
            UnknownError = 1,

            //InsertFailed = 8,
            DeleteFailed = 9
        }

        #region Ref_WebServices

        public enum Ref_WebServices
        {
            ActivateApp = 1,
            Close_Batch = 2,
            CollectDeviceInfo = 3,
            CreditTransPurchaseSwipe = 4,
            DebitBalanceInquiry = 5,
            DebitTransCashBack = 6,
            DebitTransPurchase = 7,
            IsAlive = 8,
            Login = 9,
            NewActivateApp = 10,
            NewAdditionalEmailReceipt = 11,
            NewCashTransPurchase = 12,
            NewCashTransRefund = 13,
            NewChequeTransPurchase = 14,
            NewChequeTransRefund = 15,
            NewCreditTransAddress = 16,
            NewCreditTransPurchase = 17,
            NewCreditTransPurchaseSwipe = 18,
            NewCreditTransVoidRefund = 19,
            NewCreditTransVoidRefund_Manual = 20,
            NewCreditTransVoidRefund_Swipe = 21,
            NewDebitTransPurchase = 22,
            NewDebitTransRefund = 23,
            NewEbtTransPurchase = 24,
            NewEmailReceipt = 25,
            NewInstallApp = 26,
            PrintAdditionalReceipt = 27,
            PrintReceipt = 28,
            RecordTransaxCreditTransRefundResult = 29,
            RecordTransaxCreditTransRefundStart = 30,
            RecordTransaxDebitRefundForInsufficientFundResult = 31,
            RecordTransaxDebitRefundForInsufficientFundStart = 32,
            RecordTransaxDebitTransRefundResult = 33,
            RecordTransaxDebitTransRefundStart = 34,
            RecordTransaxTransactionResult = 35,
            RecordTransaxTransactionStart = 36,
            SendAdditonalEmailReceipt = 37,
            SendEmailReceipt = 38,
            SendSmsReceipt = 39,
            TermOfServiceAccepted = 40,
            TicketCreate = 41,
            TransLookup = 42,
            TransPurchase_ACH = 43,
            TransPurchase_Cash = 44,
            TransPurchase_Credit_Address = 45,
            TransPurchase_Credit_Manual = 46,
            TransPurchase_Credit_Swipe = 47,
            TransPurchase_Debit = 48,
            TransPurchase_EBT = 49,
            TransRefund_ACH = 50,
            TransRefund_Cash = 51,
            TransRefund_Debit = 52,
            TransSignatureCapture = 53,
            TransVoidRefund_Credit_Manual = 54,
            TransVoidRefund_Credit_Swipe = 55,
            TransactionSearch = 56,
            UpdateApp = 57,
            UpdateAppCompleted = 58,
            HybridAPI_Login = 60,
            HybridAPI_CreditTrans_Purchase_Swipe = 61,
            HybridAPI_CreditTrans_Purchase_Manual = 62,
            HybridAPI_CreditTrans_Purchase_Swipe_Address = 63,
            HybridAPI_CreditTrans_Purchase_Manual_Address = 64,
            HybridAPI_DebitTrans_Purchase = 65,
            HybridAPI_ChequeTrans_Purchase = 66,
            HybridAPI_TransSignatureCapture = 67,
            HybridAPI_Search_Transaction_ByInvNum = 68
        }

        #endregion Ref_WebServices

        #region GPSCoordinates_SearchBy

        public enum GPSCoordinates_SearchBy
        {
            All = 1,
            ID = 2,
            RefWebServiceID = 3,
            MerchantID = 4,
            MobileAppID = 5,
            UserID = 6,
            RefPaymentTypeAndTransAttemptID = 7
        }

        #endregion GPSCoordinates_SearchBy

        #region Ref_ReceiptType

        public enum Ref_ReceiptType
        {
            Email = 1,
            Print = 2,
            SMS = 3
        }

        #endregion Ref_ReceiptType

        public enum AuditTrailErrNum
        {
            NoError = 0,
            UnknownError = 1,
            dbError = 2,
            InvalidSearchBy = 3,
            NoResultFound = 4,
            InsertFailed = 5,
            UpdateFailed = 6
        }

        public enum ResponseStatus
        {
            NoError = 0,
            NoResultFound = 1,
            dbError = 2,
            dbError_InsertFail = 3,
            dbError_UpdateFail = 4,
            dbError_SelectFail = 5,
            UnknownError = 6,
        }

        #region CTPayment Enums

        public enum CommandType
        {
            A = 'A',
            U = 'U',
            D = 'D'
        }

        #endregion CTPayment Enums

        #region BatchReport
        public enum ReportsCriteria
        {
            NotSet = 0,
            SummaryReport = 1,
            DetailedReport = 2,
            EndOfTheDay = 3
        }
        #endregion
    }

    public static class HyRegex
    {
        public const string MerchantAppLanguage = @"^[a-zA-Z0-9.,:\#\@'\(\)\-\s]{1,500}$";
    }
}
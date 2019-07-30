using SDGDAL.Entities;
using System.Data.Entity;

namespace SDGDAL
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base(AppSettings.SDGroupConnString)
        {
            #region database creation

            if (!this.Database.Exists())
            {
                this.Database.Create();
            }

            //if (!this.Database.Exists())
            //{
            //    this.Database.Create();

            //    #region Countries
            //    this.Countries.Add(new Country()
            //    {
            //        CountryCode = "PH",
            //        CountryName = "Philippines",
            //        DateCreated = DateTime.Now
            //    });
            //    this.SaveChanges();
            //    #endregion

            //#region Roles
            //this.Roles.Add(new Role()
            //{
            //    RoleName = "Administrator",
            //    DateCreated = DateTime.Now
            //});

            //this.Roles.Add(new Role()
            //{
            //    RoleName = "Manager",
            //    DateCreated = DateTime.Now
            //});

            //this.Roles.Add(new Role()
            //{
            //    RoleName = "Employee",
            //    DateCreated = DateTime.Now
            //});

            //this.Roles.Add(new Role()
            //{
            //    RoleName = "Developer",
            //    DateCreated = DateTime.Now
            //});
            //#endregion

            //#region First Partner (us)
            //this.Partners.Add(new Partner()
            //{
            //    CompanyName = "SD Group",
            //    CompanyEmail = "gerald.toco@gmail.com",
            //    DateCreated = DateTime.Now,
            //    ContactInformation = new ContactInformation()
            //    {
            //        Address = "Unit 1902, 139 Corporate Center, 139 Valero St.",
            //        City = "Makati City",
            //        StateProvince = "Metro Manila",
            //        CountryId = 1,
            //        ZipCode = "1201",
            //        PrimaryContactNumber = "+639333491863"
            //    }
            //});

            //this.Accounts.Add(new Account()
            //{
            //    Username = "SDGAdmin",
            //    Password = Utility.Encrypt("Admin1234!"),
            //    DateCreated = DateTime.Now,
            //    PasswordExpirationDate = DateTime.Now.AddDays(AppSettings.PasswordExpirationbyDays),
            //    RoleId = 1,
            //    ParentId = 1,
            //    ParentTypeId = 1,
            //    IsActive = true,
            //    PIN = "None",
            //    User = new User()
            //    {
            //        FirstName = "Ralph Gerald",
            //        LastName = "Toco",
            //        EmailAddress = "gerald.toco@gmail.com",
            //        Price = 0,
            //        ContactInformation = new ContactInformation()
            //        {
            //            Address = "Unit 1902, 139 Corporate Center, 139 Valero St.",
            //            City = "Makati City",
            //            StateProvince = "Metro Manila",
            //            CountryId = 1,
            //            ZipCode = "1201",
            //            PrimaryContactNumber = "+639333491863"
            //        }
            //    }
            //});
            //#endregion

            //#region Device Types
            //this.DeviceTypes.Add(new DeviceType()
            //{
            //    TypeName = "PaymentDevice"
            //});

            //this.DeviceTypes.Add(new DeviceType()
            //{
            //    TypeName = "Printer"
            //});

            //this.DeviceTypes.Add(new DeviceType()
            //{
            //    TypeName = "Miscellaneous"
            //});
            //#endregion

            //#region Device Flow Types
            //this.DeviceFlowTypes.Add(new DeviceFlowType()
            //{
            //    FlowType = "BackOffice"
            //});

            //this.DeviceFlowTypes.Add(new DeviceFlowType()
            //{
            //    FlowType = "EMV"
            //});
            //#endregion

            //#region Device Platforms
            //this.DevicePlatforms.Add(new DevicePlatform()
            //{
            //    Platform = "Android",
            //    DateCreated = DateTime.Now,
            //    IsActive = true,
            //    IsDeleted = false
            //});

            //this.DevicePlatforms.Add(new DevicePlatform()
            //{
            //    Platform = "Blackberry",
            //    DateCreated = DateTime.Now,
            //    IsActive = true,
            //    IsDeleted = false
            //});

            //this.DevicePlatforms.Add(new DevicePlatform()
            //{
            //    Platform = "iOS",
            //    DateCreated = DateTime.Now,
            //    IsActive = true,
            //    IsDeleted = false
            //});
            //#endregion

            //#region POS Types
            //this.POSTypes.Add(new POSType()
            //{
            //    TypeName = "CC"
            //});

            //this.POSTypes.Add(new POSType()
            //{
            //    TypeName = "PIN"
            //});

            //this.POSTypes.Add(new POSType()
            //{
            //    TypeName = "EMV"
            //});

            //this.POSTypes.Add(new POSType()
            //{
            //    TypeName = "4"
            //});
            //#endregion

            //#region Payment Types
            //this.PaymentTypes.Add(new PaymentType()
            //{
            //    TypeName = "ACH",
            //    DateCreated = DateTime.Now,
            //    IsActive = true
            //});

            //this.PaymentTypes.Add(new PaymentType()
            //{
            //    TypeName = "Cash",
            //    DateCreated = DateTime.Now,
            //    IsActive = true
            //});

            //this.PaymentTypes.Add(new PaymentType()
            //{
            //    TypeName = "Credit",
            //    DateCreated = DateTime.Now,
            //    IsActive = true
            //});

            //this.PaymentTypes.Add(new PaymentType()
            //{
            //    TypeName = "Debit",
            //    DateCreated = DateTime.Now,
            //    IsActive = true
            //});

            //this.PaymentTypes.Add(new PaymentType()
            //{
            //    TypeName = "EMV",
            //    DateCreated = DateTime.Now,
            //    IsActive = true
            //});

            //this.PaymentTypes.Add(new PaymentType()
            //{
            //    TypeName = "All",
            //    DateCreated = DateTime.Now,
            //    IsActive = true
            //});
            //#endregion

            //#region Card Types
            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "MasterCard"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "Visa"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "AmericanExpress"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "Diners"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "Discover"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "JCB"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "Chinabank"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "Citibank"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "Barclaycard"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "Debit"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "Cheque"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "Cash"
            //});

            //this.CardTypes.Add(new CardType()
            //{
            //    TypeName = "EBT"
            //});
            //#endregion

            //#region PaymentAccountTypes
            //this.PaymentAccountTypes.Add(new PaymentAccountType()
            //{
            //    AccountType = "Chequing"
            //});

            //this.PaymentAccountTypes.Add(new PaymentAccountType()
            //{
            //    AccountType = "Savings"
            //});

            //this.PaymentAccountTypes.Add(new PaymentAccountType()
            //{
            //    AccountType = "Credit"
            //});

            //this.PaymentAccountTypes.Add(new PaymentAccountType()
            //{
            //    AccountType = "Checking"
            //});
            //#endregion

            //#region Switch API Types
            //this.SwitchAPITypes.Add(new SwitchAPIType()
            //{
            //    APIName = "CT Payments",
            //    IsActive = true
            //});

            //this.SwitchAPITypes.Add(new SwitchAPIType()
            //{
            //    APIName = "Moneris",
            //    IsActive = false
            //});

            //this.SwitchAPITypes.Add(new SwitchAPIType()
            //{
            //    APIName = "Data Collection",
            //    IsActive = true
            //});

            //this.SwitchAPITypes.Add(new SwitchAPIType()
            //{
            //    APIName = "PaymentData Usd",
            //    IsActive = true
            //});

            //this.SwitchAPITypes.Add(new SwitchAPIType()
            //{
            //    APIName = "TransaxPayment",
            //    IsActive = false
            //});
            //#endregion

            //#region Mids Group Types
            //this.MidsGroupTypes.Add(new MidsGroupType()
            //{
            //    GroupType = "Ordered"
            //});

            //this.MidsGroupTypes.Add(new MidsGroupType()
            //{
            //    GroupType = "Balanced"
            //});
            //#endregion

            //#region Billing Cycles
            //this.BillingCycles.Add(new BillingCycle()
            //{
            //    CycleType = "Weekly"
            //});

            //this.BillingCycles.Add(new BillingCycle()
            //{
            //    CycleType = "Bi-Weekly"
            //});

            //this.BillingCycles.Add(new BillingCycle()
            //{
            //    CycleType = "Monthly"
            //});

            //this.BillingCycles.Add(new BillingCycle()
            //{
            //    CycleType = "Quarterly"
            //});

            //this.BillingCycles.Add(new BillingCycle()
            //{
            //    CycleType = "Annually"
            //});
            //#endregion

            //#region TransactionTypes
            //this.TransactionTypes.Add(new TransactionType()
            //{
            //    Name = "Capture"
            //});

            //this.TransactionTypes.Add(new TransactionType()
            //{
            //    Name = "Pre Auth"
            //});

            //this.TransactionTypes.Add(new TransactionType()
            //{
            //    Name = "Purchased"
            //});

            //this.TransactionTypes.Add(new TransactionType()
            //{
            //    Name = "Void"
            //});

            //this.TransactionTypes.Add(new TransactionType()
            //{
            //    Name = "Refund"
            //});

            //this.TransactionTypes.Add(new TransactionType()
            //{
            //    Name = "Declined"
            //});
            //#endregion

            //#region TransactionEntryTypes
            //this.TransactionEntryTypes.Add(new TransactionEntryType()
            //{
            //    EntryType = "Credit PIN"
            //});

            //this.TransactionEntryTypes.Add(new TransactionEntryType()
            //{
            //    EntryType = "Credit Signature"
            //});

            //this.TransactionEntryTypes.Add(new TransactionEntryType()
            //{
            //    EntryType = "Credit Manual"
            //});

            //this.TransactionEntryTypes.Add(new TransactionEntryType()
            //{
            //    EntryType = "Debit PIN"
            //});

            //this.TransactionEntryTypes.Add(new TransactionEntryType()
            //{
            //    EntryType = "Credit Swipe"
            //});

            //this.TransactionEntryTypes.Add(new TransactionEntryType()
            //{
            //    EntryType = "Cheque"
            //});

            //this.TransactionEntryTypes.Add(new TransactionEntryType()
            //{
            //    EntryType = "EBTSwipePIN"
            //});
            //#endregion

            //this.SaveChanges();
            //}

            #endregion database creation
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Partner>()
                        .HasRequired(c => c.ContactInformation)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reseller>()
                        .HasRequired(s => s.ContactInformation)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Merchant>()
                        .HasRequired(s => s.ContactInformation)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<MerchantBranch>()
                        .HasRequired(c => c.ContactInformation)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                        .HasRequired(c => c.ContactInformation)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<MerchantFeatures>()
                       .HasRequired(c => c.Currency)
                       .WithMany()
                       .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mid>()
                        .HasRequired(c => c.Currency)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mid>()
                        .HasRequired(c => c.Merchant)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<MerchantBranch>()
                        .HasRequired(c => c.Merchant)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
                        .HasRequired(c => c.Currency)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
                        .HasRequired(c => c.CardType)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
                        .HasRequired(c => c.MerchantPOS)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
                        .HasRequired(c => c.TransactionEntryType)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionAttempt>()
                        .HasRequired(c => c.Transaction)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionAttempt>()
                        .HasRequired(c => c.MobileApp)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionAttempt>()
                        .HasRequired(c => c.Device)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionAttempt>()
                        .HasRequired(c => c.Account)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<MasterDevice>()
                        .HasRequired(c => c.DeviceType)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<MasterDevice>()
                        .HasRequired(c => c.FlowType)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<SwitchPartnerLink>()
                        .HasRequired(c => c.Switch)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<SwitchPartnerLink>()
                        .HasRequired(c => c.Partner)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TempTransaction>()
                        .HasRequired(c => c.Mid)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TempTransaction>()
                        .HasRequired(c => c.Key)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TempTransaction>()
                        .HasRequired(c => c.Currency)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionDebit>()
                        .HasRequired(c => c.MerchantPOS)
                        .WithMany()
                        .WillCascadeOnDelete(true);

            modelBuilder.Entity<TransactionDebit>()
                        .HasRequired(c => c.Currency)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionDebit>()
                        .HasRequired(c => c.AccountType)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionDebit>()
                        .HasRequired(c => c.TransactionEntryType)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionAttemptDebit>()
                        .HasRequired(c => c.TransactionDebit)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionAttemptDebit>()
                        .HasRequired(c => c.MobileApp)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionAttemptDebit>()
                        .HasRequired(c => c.Device)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionAttemptDebit>()
                        .HasRequired(c => c.Account)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionCash>()
                        .HasRequired(c => c.MerchantPOS)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionCash>()
                        .HasRequired(c => c.Currency)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionCash>()
                        .HasRequired(c => c.TransactionEntryType)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionAttemptCash>()
                        .HasRequired(c => c.TransactionCash)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionAttemptCash>()
                        .HasRequired(c => c.MobileApp)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionAttemptCash>()
                        .HasRequired(c => c.Account)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Batch>()
                        .HasRequired(c => c.MobileApp)
                        .WithMany()
                        .WillCascadeOnDelete(false);

        }

        public DbSet<AccountType> AccountType { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ActionLog> ActionLogs { get; set; }
        public DbSet<Agreements> Agreements { get; set; }
        public DbSet<BillingCycle> BillingCycles { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<ContactInformation> ContactInformation { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CountryIP> CountryIPs { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<DbSettings> DbSettings { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<DeviceFlowType> DeviceFlowTypes { get; set; }
        public DbSet<DeviceMerchantLink> DeviceMerchantLink { get; set; }
        public DbSet<DevicePlatform> DevicePlatforms { get; set; }
        public DbSet<DevicePOSLink> DevicePOSLink { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<EmailServer> EmailServers { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        //public DbSet<_Key> Keys { get; set; }
        public DbSet<KeyInjected> KeyInjecteds { get; set; }

        public DbSet<Language> Languages { get; set; }
        public DbSet<MasterDevice> MasterDevices { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<MerchantBranch> MerchantBranches { get; set; }
        public DbSet<MerchantBranchPOS> MerchantPOSs { get; set; }
        public DbSet<MerchantFeatures> MerchantFeatures { get; set; }
        public DbSet<MerchantMobileAppPricePoint> MerchantMobileAppPricePoints { get; set; }
        public DbSet<MerchantUserPricePoint> MerchantUserPricePoints { get; set; }
        public DbSet<Mid> Mids { get; set; }
        public DbSet<MidsGroup> MidsGroups { get; set; }
        public DbSet<MidsGroupType> MidsGroupTypes { get; set; }
        public DbSet<MidsMerchantBranches> MidsMerchantBranches { get; set; }
        public DbSet<MidsMerchantBranchPOSs> MidsMerchantBranchPOSs { get; set; }
        public DbSet<MobileApp> MobileApps { get; set; }
        public DbSet<MobileAppFeatures> MobileAppFeatures { get; set; }
        public DbSet<MobileAppLog> MobileAppLogs { get; set; }
        public DbSet<MobileAppTokenLog> MobileAppTokenLogs { get; set; }
        public DbSet<MobileDeviceInfo> MobileDeviceInfos { get; set; }
        public DbSet<ParentType> ParentTypes { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<PaymentAccountType> PaymentAccountTypes { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<POSType> POSTypes { get; set; }
        public DbSet<Provinces> Provinces { get; set; }
        public DbSet<Reseller> Resellers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Switch> Switches { get; set; }
        public DbSet<SwitchAPIType> SwitchAPITypes { get; set; }
        public DbSet<SwitchParameter> SwitchParameters { get; set; }
        public DbSet<SwitchParameterType> SwitchParameterTypes { get; set; }
        public DbSet<SwitchPartnerLink> SwitchPartnerLinks { get; set; }
        public DbSet<TaxType> TaxTypes { get; set; }
        public DbSet<TempTransaction> TempTransactions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionAttempt> TransactionAttempts { get; set; }

        //public DbSet<TransactionReference> TransactionReference { get; set; }
        public DbSet<TransactionCharges> TransactionCharges { get; set; }

        public DbSet<TransactionEntryType> TransactionEntryTypes { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TransactionSignature> TransactionSignature { get; set; }
        public DbSet<TransactionDebit> TransactionDebit { get; set; }
        public DbSet<TransactionAttemptDebit> TransactionAttemptDebit { get; set; }
        public DbSet<TempSystemTraceNumber> TempSystemTraceNumber { get; set; }
        public DbSet<TransactionVoidReason> TransactionVoidReason { get; set; }
        public DbSet<MerchantOnBoardResponse> MerchantOnBoardResponse { get; set; }
        public DbSet<MerchantOnBoardRequest> MerchantOnBoardRequest { get; set; }
        public DbSet<MerchantOnBoardResponseLink> MerchantOnBoardResponseLink { get; set; }
        public DbSet<RequestedMerchant> RequestedMerchants { get; set; }
        public DbSet<HeaderResponse> HeaderResponse { get; set; }
        public DbSet<EmvHostParameter> EmvHostParameters { get; set; }
        public DbSet<EmvTerminalParameter> EmvTerminalParameters { get; set; }
        public DbSet<EmvMastercardParameter> EmvMastercardParameters { get; set; }
        public DbSet<EmvVisaParameter> EmvVisaParameters { get; set; }
        public DbSet<EmvAmexParameter> EmvAmexParameters { get; set; }
        public DbSet<EmvInteracParameter> EmvInteracParameters { get; set; }
        public DbSet<EmvJcbParameter> EmvJcbParameters { get; set; }
        public DbSet<EmvDiscoverParameter> EmvDiscoverParameters { get; set; }
        public DbSet<AndroidAppVersion> AndroidAppVersion { get; set; }
        public DbSet<TransactionCash> TransactionCash { get; set; }
        public DbSet<TransactionAttemptCash> TransactionAttemptCash { get; set; }
        public DbSet<Batch> Batch { get; set; }
    }
}
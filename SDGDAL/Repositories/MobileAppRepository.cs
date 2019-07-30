using SDGDAL.Entities;
using System;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class MobileAppRepository
    {
        public MobileApp GetMobileAppDetailsByActivationCode(string activationCode)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var mobileApp = context.MobileApps
                                       .Include("MerchantBranchPOS")
                                       .Include("MerchantBranchPOS.MerchantBranch")
                                       .Include("MerchantBranchPOS.MerchantBranch.Merchant")
                                       .Include("MerchantBranchPOS.MerchantBranch.Merchant.ContactInformation")
                                       .Include("MerchantBranchPOS.MerchantBranch.Merchant.ContactInformation.Country")
                                       .Include("MobileAppFeatures")
                                       .Include("MobileAppFeatures.Currency")

                                       .SingleOrDefault(ma => ma.ActivationCode.Replace("-", "") == activationCode);

                    return mobileApp;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            throw new NotImplementedException();
        }

        public MobileApp GetMobileAppFullDetailsByActivationCode(string activationCode)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    var mobileApp = context.MobileApps
                                           .Include("MerchantBranchPOS")
                                           .Include("MerchantBranchPOS.MerchantBranch")
                                           .Include("MerchantBranchPOS.MerchantBranch.Merchant")
                                           .Include("MerchantBranchPOS.MerchantBranch.Merchant.ContactInformation")
                                           .Include("MerchantBranchPOS.MerchantBranch.Merchant.ContactInformation.Country")
                                           .Include("MerchantBranchPOS.MerchantBranch.Merchant.MerchantFeatures")
                                           .Include("MobileAppFeatures")
                                           .Include("MobileAppFeatures.Currency")
                                           .SingleOrDefault(ma => ma.ActivationCode.Replace("-", "") == activationCode);

                    return mobileApp;
                }
            }
            catch (Exception ex)
            {
                ex.Data["MethodCalled"] = "MobileAppRepository.GetMobileAppFullDetailsByActivationCode";
                throw ex;
            }
        }

        public MobileApp GetMobileAppDetailsByCredentialsAndActivationCode(string username, string password, string activationCode, out Account account)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    var mobileApp = context.MobileApps
                                           .Include("MerchantBranchPOS")
                                           .Include("MerchantBranchPOS.MerchantBranch")
                                           .Include("MerchantBranchPOS.MerchantBranch.Merchant")
                                           .SingleOrDefault(ma => ma.ActivationCode.Replace("-", "") == activationCode.Replace("-", ""));

                    try
                    {
                        account = context.Accounts
                                             .Include("User")
                                             .SingleOrDefault(acc => (acc.Username == username)
                                                                    && ((acc.ParentTypeId == 3 && acc.ParentId == mobileApp.MerchantBranchPOS.MerchantBranch.MerchantId) // Merchant
                                                                        || (acc.ParentTypeId == 4 && acc.ParentId == mobileApp.MerchantBranchPOS.MerchantBranchId))); // Merchant Branch

                        if (account != null)
                        {
                            if (Utility.Decrypt(account.Password) != password)
                            {
                                account = null;
                            }
                        }
                    }
                    catch
                    {
                        account = null;
                    }

                    return mobileApp;
                }
            }
            catch (Exception ex)
            {
                ex.Data["MethodCalled"] = "MobileAppRepository.GetMobileAppDetailsByCredentialsAndActivationCode";
                throw ex;
            }
        }

        public MobileApp GetMobileAppDetailsByPinAndActivationCode(string pin, string activationCode, out Account account)
        {
            using (DataContext context = new DataContext())
            {
                var mobileApp = context.MobileApps
                                       .Include("MerchantBranchPOS")
                                       .Include("MerchantBranchPOS.MerchantBranch")
                                       .Include("MerchantBranchPOS.MerchantBranch.Merchant")
                                       .SingleOrDefault(ma => ma.ActivationCode.Replace("-", "") == activationCode.Replace("-", ""));

                try
                {
                    account = null;
                    using (DataContext ctxuser = new DataContext())
                    {
                        var u = ctxuser.Accounts.Include("User")
                            .Where(acc => (acc.PIN != "None") && (acc.ParentTypeId == 3 && (acc.ParentId == mobileApp.MerchantBranchPOS.MerchantBranch.MerchantId) // Merchant
                                                                    || (acc.ParentTypeId == 4 && acc.ParentId == mobileApp.MerchantBranchPOS.MerchantBranchId))).ToList();

                        if (u != null)
                        {
                            for (int i = 0; i < u.Count(); i++)
                            {
                                int accId = u[i].AccountId;

                                if (Utility.Decrypt(u[i].PIN) != pin)
                                {
                                    account = null;
                                }
                                else
                                {
                                    account = ctxuser.Accounts.FirstOrDefault(acc => (acc.AccountId == accId));
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    account = null;
                }

                return mobileApp;
            }
        }

        public MobileApp UpdateMobileApp(MobileApp mobileApp)
        {
            using (DataContext context = new DataContext())
            {
                var mobileDeviceInfo = context.MobileDeviceInfos.Add(mobileApp.MobileDeviceInfo);
                context.SaveChanges();

                mobileApp.MobileDeviceInfo = mobileDeviceInfo;
                mobileApp.MobileDeviceInfoId = mobileDeviceInfo.MobileDeviceInfoId;
                //Update Mobile App
                var uMP = context.MobileApps.Attach(mobileApp);
                var entryA = context.Entry(mobileApp);

                entryA.Property(e => e.DateActivated).IsModified = true;
                entryA.Property(e => e.IsActive).IsModified = true;
                entryA.Property(e => e.ExpirationDate).IsModified = true;
                entryA.Property(e => e.GPSLat).IsModified = true;
                entryA.Property(e => e.GPSLong).IsModified = true;
                entryA.Property(e => e.UpdatePending).IsModified = true;
                entryA.Property(e => e.MobileDeviceInfoId).IsModified = true;

                context.SaveChanges();

                return mobileApp;
            }
        }

        public MobileApp UpdateMobileAppPendingStatus(MobileApp mobileApp)
        {
            using (DataContext context = new DataContext())
            {
                //Update Mobile App
                var uMP = context.MobileApps.Attach(mobileApp);
                var entryA = context.Entry(mobileApp);

                entryA.Property(e => e.UpdatePending).IsModified = true;

                context.SaveChanges();

                return mobileApp;
            }
        }

        public MobileDeviceInfo CreateMobileDeviceInfo(MobileDeviceInfo deviceInfo)
        {
            using (DataContext context = new DataContext())
            {
                var mobileDevice = context.MobileDeviceInfos.Add(deviceInfo);

                context.SaveChanges();

                return mobileDevice;
            }
        }

        public MobileAppLog LogMobileAppAction(MobileAppLog mobileAppLog)
        {
            using (DataContext context = new DataContext())
            {
                var log = context.MobileAppLogs.Add(mobileAppLog);

                context.SaveChanges();

                return log;
            }
        }

        public MobileApp GetMobileAppDetailsByPosId(int posId)
        {
            using (DataContext context = new DataContext())
            {
                var mobileApp = context.MobileApps
                    .Include("MerchantBranchPOS")
                    .Include("MobileAppFeatures")
                    .Include("MerchantBranchPOS.MerchantBranch")
                    .Include("MerchantBranchPOS.MerchantBranch.Merchant")
                    .SingleOrDefault(ma => ma.MerchantBranchPOSId == posId);

                return mobileApp;
            }
        }

        public MobileApp GetMobileAppDetailsByMobileAppId(int mobileAppId)
        {
            using (DataContext context = new DataContext())
            {
                var mobileApp = context.MobileApps
                    .Include("MerchantBranchPOS")
                    .Include("MerchantBranchPOS.MerchantBranch")
                    .Include("MerchantBranchPOS.MerchantBranch.Merchant")
                    .Include("MobileAppFeatures")
                    .FirstOrDefault(ma => ma.MobileAppId == mobileAppId);

                return mobileApp;
            }
        }

        public MobileAppTokenLog CreateMobileAppTokenLog(MobileAppTokenLog tokenLog)
        {
            using (DataContext context = new DataContext())
            {
                var log = context.MobileAppTokenLogs.Add(tokenLog);

                context.SaveChanges();

                return log;
            }
        }

        public MobileAppTokenLog UpdateMobileAppTokenLog(MobileAppTokenLog tokenLog)
        {
            using (DataContext context = new DataContext())
            {
                var ul = context.MobileAppTokenLogs.Attach(tokenLog);
                var entryL = context.Entry(tokenLog);

                entryL.Property(e => e.ExpirationDate).IsModified = true;

                context.SaveChanges();

                return tokenLog;
            }
        }

        public MobileAppTokenLog GetMobileAppTokenLog(string token)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    return context.MobileAppTokenLogs
                        .Include("MobileApp")
                        .Include("MobileApp.MerchantBranchPOS")
                        .Include("MobileApp.MerchantBranchPOS.MerchantBranch")
                        .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                        .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant.ContactInformation")
                        .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant.ContactInformation.Country")
                        .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant.MerchantFeatures")
                        .Include("MobileApp.MobileAppFeatures")
                        .Include("MobileApp.MobileAppFeatures.Currency")
                        .Include("Account")
                        .SingleOrDefault(i => i.RequestToken == token);
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string msg = string.Format("{0} : {1}", validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(msg, raise);
                    }
                }
                throw raise;
            }
        }

        public bool CheckExistingActivationCode(string activationCode)
        {
            using (DataContext context = new DataContext())
            {
                var actiCode = context.MobileApps
                    .SingleOrDefault(m => m.ActivationCode == activationCode);

                if (actiCode == null)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
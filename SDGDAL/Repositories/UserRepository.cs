using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class UserRepository
    {
        public Account GetUserByCredentials(string username, string password, string IPAddress = "")
        {
            try
            {
                string msg = "";

                using (DataContext context = new DataContext())
                {
                    var acc = context.Accounts
                                     .Include("User")
                                     .SingleOrDefault(a => a.Username == username
                                                      && a.IsDeleted == false);

                    if (acc == null)
                    {
                        Exception e = new Exception("Incorrect Username.");
                        e.Data["ErrorCode"] = "Login001";
                        e.Data["ErrorType"] = "Custom";
                        throw e;
                    }
                    else if (acc.IsActive != true)
                    {
                        Exception e = new Exception("Your account is deactivated. Please contact suppport for assistance.");
                        e.Data["ErrorCode"] = "Login002";
                        e.Data["ErrorType"] = "Custom";
                        throw e;
                    }
                    else
                    {
                        acc.IPAddress = IPAddress;
                        acc.LastLoggedIn = DateTime.Now;

                        if (Utility.Decrypt(acc.Password) != password)
                        {
                            msg = "Incorrect Password.";

                            if (acc.AccountAvailableDate.HasValue)
                            {
                                if (acc.AccountAvailableDate > DateTime.Now)
                                {
                                    msg = "Account is locked. Please contact administrator to unlock immediately.";
                                }
                                else
                                {
                                    acc.LogTries = 0;
                                }
                            }

                            if (acc.LogTries < AppSettings.MaxLoginTries)
                                acc.LogTries++;
                            else
                                acc.AccountAvailableDate = DateTime.Now.AddMinutes(AppSettings.AccountLockedByMinutes);

                            context.SaveChanges();

                            Exception e = new Exception(msg);
                            e.Data["ErrorCode"] = "Login003";
                            e.Data["ErrorType"] = "Custom";
                            throw e;
                        }

                        if (acc.PasswordExpirationDate <= DateTime.Now)
                        {
                            Exception e = new Exception("Password expired. Please contact support for assistance.");
                            e.Data["ErrorCode"] = "Login004";
                            e.Data["ErrorType"] = "Custom";
                            throw e;
                        }

                        if (acc.AccountAvailableDate > DateTime.Now)
                        {
                            msg = "Account is locked. Please contact administrator to unlock immediately.";
                            Exception e = new Exception(msg);
                            e.Data["ErrorCode"] = "Login005";
                            e.Data["ErrorType"] = "Custom";
                            throw e;
                        }

                        acc.LogTries = 0;

                        context.SaveChanges();
                    }

                    return acc;
                }
            }
            catch (Exception ex)
            {
                ex.Data["MethodCalled"] = "UserRepository.GetUserByCredentials";
                throw ex;
            }
        }

        public Account GetUserByUserAndPassword(string username, string password)
        {
            using (DataContext context = new DataContext())
            {
                string encPassword = Utility.Encrypt(password);
                var acc = context.Accounts
                                 .Include("User")
                                 .SingleOrDefault(a => a.Username == username);

                if (Utility.Decrypt(acc.Password) != password)
                {
                    return null;
                }

                return acc;
            }
        }

        public Account GetUserByAccountId(int accountId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var acc = context.Accounts
                                .Include("User")
                                .SingleOrDefault(a => a.AccountId == accountId);
                    return acc;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public bool IsUserNameAvailable(string Username)
        {
            using (DataContext context = new DataContext())
            {
                return context.Accounts.SingleOrDefault(acc => acc.Username == Username) == null;
            }
        }

        public bool IsBranchNameAvailable(string BranchName)
        {
            using (DataContext context = new DataContext())
            {
                return context.MerchantBranches.SingleOrDefault(acc => acc.MerchantBranchName == BranchName) == null;
            }
        }

        public bool IsEmailAvailable(string EmailAddress)
        {
            using (DataContext context = new DataContext())
            {
                return context.Merchants.SingleOrDefault(acc => acc.MerchantEmail == EmailAddress) == null;
            }
        }

        public void UploadUserPhoto(int userId, string fileName)
        {
            using (DataContext context = new DataContext())
            {
                var user = context.Users.SingleOrDefault(u => u.UserId == userId);

                user.PhotoUrl = fileName;

                //context.Users.Attach(user);
                context.SaveChanges();
            }
        }

        public Account GetDetailsbyUserId(int userId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var user = context.Accounts
                                        .Include("User")
                                        .SingleOrDefault(u => u.UserId == userId);

                    return user;
                }
                catch
                {
                    return null;
                }
            }
        }

        public Account GetDetailsbyAccountId(int aId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var user = context.Accounts
                        .Include("User")
                        .Include("User.ContactInformation")
                        .Include("User.ContactInformation.Country")
                        .SingleOrDefault(u => u.AccountId == aId);

                    return user;
                }
                catch
                {
                    return null;
                }
            }
        }

        public Account GetDetailsbyParentIdAndParentTypeId(int parentTypeId, int parentId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var acc = context.Accounts.FirstOrDefault(u => u.ParentTypeId == parentTypeId && u.ParentId == parentId);

                    return acc;
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<Account> GetDetailsbyParentIdAndParentTypeIdAccountId(int parentTypeId, int parentId, int accountId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var acc = context.Accounts
                              .Include("User").Where(u => u.ParentTypeId == parentTypeId && u.ParentId == parentId && u.AccountId != accountId && u.PIN != "None");

                    return acc.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<Account> GetDetailsbyParentIdAndParentTypeIdAndEmail(int parentTypeId, int parentId, string email)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var acc = context.Accounts
                              .Include("User").Where(u => u.ParentTypeId == parentTypeId && u.ParentId == parentId && u.User.EmailAddress == email);

                    return acc.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<Account> GetUsersbyMerchantIdAndBranchId(int merchantId, int branchId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var users = context.Accounts
                                .Include("User").Where(u => (u.ParentTypeId == 3 && u.ParentId == merchantId) || (u.ParentTypeId == 4 && u.ParentId == branchId));

                    return users.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<Account> GetUsersbyMerchantId(int merchantId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var users = context.Accounts
                                .Include("User").Where(u => (u.ParentTypeId == 3 && u.ParentId == merchantId));

                    return users.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public bool IsValidUserCredentials(string userName, string email)
        {
            using (DataContext context = new DataContext())
            {
                return context.Accounts.Include("User").SingleOrDefault(u => u.Username == userName && u.User.EmailAddress == email) == null;
            }
        }

        public Account UpdateUserPasswordByUsername(Account acc)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    Account account = context.Accounts.Single(i => i.Username == acc.Username);
                    account.Password = Utility.Encrypt(acc.Password);
                    account.NeedsUpdate = acc.NeedsUpdate;
                    context.SaveChanges();

                    return account;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Account UpdatePasswordByUserId(Account acc)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    DateTime dt = acc.PasswordExpirationDate;
                    TimeSpan d = dt.Subtract(DateTime.Today);

                    Account account = context.Accounts.Single(i => i.UserId == acc.UserId);
                    account.Password = Utility.Encrypt(acc.Password);
                    account.NeedsUpdate = acc.NeedsUpdate;

                    if (d.Days <= 3)
                    {
                        account.PasswordExpirationDate = acc.PasswordExpirationDate.AddYears(1);
                    }

                    context.SaveChanges();

                    return account;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Account UpdatePasswordByAccountId(Account acc)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    DateTime dt = acc.PasswordExpirationDate;
                    TimeSpan d = dt.Subtract(DateTime.Today);

                    Account account = context.Accounts.Single(i => i.AccountId == acc.AccountId);
                    account.Password = Utility.Encrypt(acc.Password);
                    account.NeedsUpdate = acc.NeedsUpdate;

                    if (d.Days <= 3)
                    {
                        account.PasswordExpirationDate = acc.PasswordExpirationDate.AddMonths(3);
                    }

                    context.SaveChanges();

                    return account;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public bool IsPinAvailable(string pin, int MerchantId)
        {
            using (DataContext context = new DataContext())
            {
                var accountMerchant = context.Accounts.Include("User")
                            .Where(acc => (acc.PIN != "None") && (acc.PIN != "0192") && (acc.ParentTypeId == 3 && (acc.ParentId == MerchantId))).ToList();

                if (Utility.Decrypt(accountMerchant[0].PIN) == pin)
                {
                    return false;
                }

                var merchantBranches = context.MerchantBranches.Where(m => m.MerchantId == MerchantId).ToList();

                for (int i = 0; i < merchantBranches.Count; i++)
                {
                    int branchId = merchantBranches[i].MerchantBranchId;

                    var accountBranch = context.Accounts.Include("User")
                            .Where(acc => (acc.PIN != "None") && (acc.PIN != "0192") && (acc.ParentTypeId == 4 && (acc.ParentId == branchId))).ToList();

                    if (Utility.Decrypt(accountBranch[0].PIN) == pin)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public List<Account> GetUserDetailsUserType(int parentId, int parentTypeId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Account> users = new List<Entities.Account>();
            using (DataContext context = new DataContext())
            {
                var q = context.Accounts.Include("User")
                    .Include("User.ContactInformation")
                    .Include("User.ContactInformation.Country")
                    .Where(p => p.ParentId == parentId && p.ParentTypeId == parentTypeId);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                users = q.ToList();
            }

            return users;
        }

        public Account UpdateUserStatus(Account account)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(account).State = EntityState.Modified;
                context.SaveChanges();
                return account;
            }
        }

        public Account UpdateUserAvailability(Account account)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(account).State = EntityState.Modified;
                context.SaveChanges();
                return account;
            }
        }

        public Account UpdateAccountInfo(Account account)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(account).State = EntityState.Modified;
                context.SaveChanges();

                return account;
            }
        }

        public Account GetUserByParentIdAndparentTypeId(int parentTypeId, int parentId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var accountUser = context.Accounts
                                      .Include("User").SingleOrDefault(u => u.ParentTypeId == parentTypeId && u.ParentId == parentId);

                    return accountUser;
                }
                catch
                {
                    return null;
                }
            }
        }

        public User UpdateUserInfo(User users)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(users).State = EntityState.Modified;
                context.SaveChanges();

                return users;
            }
        }

        public User UpdateUser(User user)
        {
            using (DataContext context = new DataContext())
            {
                var uc = context.ContactInformation.Attach(user.ContactInformation);
                var entryC = context.Entry(user.ContactInformation);

                entryC.Property(e => e.Address).IsModified = true;
                entryC.Property(e => e.City).IsModified = true;
                entryC.Property(e => e.StateProvince).IsModified = true;
                entryC.Property(e => e.ZipCode).IsModified = true;
                entryC.Property(e => e.CountryId).IsModified = true;
                entryC.Property(e => e.PrimaryContactNumber).IsModified = true;
                entryC.Property(e => e.MobileNumber).IsModified = true;
                entryC.Property(e => e.Fax).IsModified = true;
                entryC.Property(e => e.ProvIsoCode).IsModified = true;
                entryC.Property(e => e.NeedsUpdate).IsModified = true;

                context.SaveChanges();

                return user;
            }
        }

        public User GetMerchantByContactInfoId(int contactInfoId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var user = context.Users.SingleOrDefault(e => e.ContactInformationId == contactInfoId);

                    return user;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
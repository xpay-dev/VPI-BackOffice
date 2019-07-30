using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class MerchantRepository
    {
        private UserRepository _userRepo;

        public MerchantRepository()
        {
            _userRepo = new UserRepository();
        }

        public List<Agreements> GetAgreements(int merchantId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Agreements.Where(aa => aa.MerchantId == merchantId || aa.MerchantId == null).ToList();
            }
        }

        public Merchant GetMerchantDetails(int merchantId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Merchants.SingleOrDefault(m => m.MerchantId == merchantId);
            }
        }

        public Merchant CreateMerchantWithUser(Merchant merchant, Account account)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        merchant.DateCreated = DateTime.Now;
                        merchant.MerchantFeatures.CurrencyId = 1;
                        merchant.MerchantFeatures.LanguageCode = "EN-CA";
                        var savedMerchant = context.Merchants.Add(merchant);
                        context.SaveChanges();

                        account.Password = Utility.Encrypt(account.Password);
                        account.DateCreated = DateTime.Now;
                        account.IsActive = true;
                        account.ParentId = savedMerchant.MerchantId;
                        account.ParentTypeId = 3; // Merchant
                        account.PasswordExpirationDate = DateTime.Now.AddDays(AppSettings.PasswordExpirationbyDays);
                        account.NeedsUpdate = false;

                        var savedAccount = context.Accounts.Add(account);
                        context.SaveChanges();

                        trans.Commit();

                        return savedMerchant;
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        trans.Rollback();
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
            }
        }

        public bool CreateMerchantWithUserAndMid(List<Merchant> merchant, List<Account> account, List<Mid> mid)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    using (var trans = context.Database.BeginTransaction())
                    {
                        var saveMerchantList = context.Merchants.AddRange(merchant);
                        context.SaveChanges();

                        var id = saveMerchantList.Select(p => p.MerchantId).ToList();

                        for (int i = 0; i < account.Count; i++)
                        {
                            account[i].Password = Utility.Encrypt(account[i].Password);
                            account[i].DateCreated = DateTime.Now;
                            account[i].IsActive = true;
                            account[i].ParentId = Convert.ToInt32(id[i]);
                            account[i].ParentTypeId = 3; //Merchant
                            account[i].PasswordExpirationDate = DateTime.Now.AddDays(AppSettings.PasswordExpirationbyDays);
                            account[i].NeedsUpdate = false;
                        }

                        var saveAccountList = context.Accounts.AddRange(account);
                        context.SaveChanges();

                        for (int j = 0; j < mid.Count; j++)
                        {
                            mid[j].MerchantId = Convert.ToInt32(id[j]);
                        }

                        var saveMid = context.Mids.AddRange(mid);
                        context.SaveChanges();

                        trans.Commit();

                        return true;
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

                    //return false;
                }
            }
        }

        public Account CreateMerchantUser(Merchant merchant, Account account)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        account.Password = Utility.Encrypt(account.Password);
                        account.PIN = account.PIN;
                        account.DateCreated = DateTime.Now;
                        account.IsActive = true;
                        account.ParentId = merchant.MerchantId;
                        account.ParentTypeId = 3; // Merchant
                        account.PasswordExpirationDate = DateTime.Now.AddDays(AppSettings.PasswordExpirationbyDays);
                        account.NeedsUpdate = false;

                        var savedAccount = context.Accounts.Add(account);
                        context.SaveChanges();

                        trans.Commit();

                        return savedAccount;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public Merchant UpdateMerchant(Dictionary<string, object> param)
        {
            using (DataContext context = new DataContext())
            {
                List<SqlParameter> ps = new List<SqlParameter>();

                param.Add("@UpdateStatusMessage out", "");

                string sqlQuery = "sp_UpdateMerchantInfo ";
                foreach (var key in param.Keys)
                {
                    if (!key.Contains("out"))
                    {
                        sqlQuery += key + ", ";
                        var sqlParam = new SqlParameter(key.Replace("@", ""), param[key]);

                        if (key.EndsWith("Id"))
                        {
                            sqlParam.SqlValue = Convert.ToInt32(sqlParam.Value);
                            sqlParam.SqlDbType = System.Data.SqlDbType.Int;
                        }

                        ps.Add(sqlParam);
                    }
                    else
                    {
                        sqlQuery += key;
                        var newParam = new SqlParameter(key.Replace("@", "").Replace("out", ""), param[key]);
                        newParam.Direction = System.Data.ParameterDirection.Output;
                        ps.Add(newParam);
                    }
                }

                var result = context.Merchants.SqlQuery(sqlQuery, ps.ToArray()).SingleOrDefault();

                return result;
            }
        }

        public Merchant UpdateMerchantStatus(Merchant merchant)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(merchant).State = EntityState.Modified;
                context.SaveChanges();
                return merchant;
            }
        }

        public Merchant UpdateMerchantContact(Merchant merchant)
        {
            using (DataContext context = new DataContext())
            {
                //Update Merchant
                var um = context.Merchants.Attach(merchant);
                var entryM = context.Entry(merchant);
                entryM.Property(m => m.CurrencyId).IsModified = true;
                entryM.Property(m => m.NeedUpdateToCT).IsModified = true;
                //Update MFeature
                var uf = context.MerchantFeatures.Attach(merchant.MerchantFeatures);
                var entryF = context.Entry(merchant.MerchantFeatures);
                //Update ContactInformation
                var uc = context.ContactInformation.Attach(merchant.ContactInformation);

                var entryC = context.Entry(merchant.ContactInformation);
                entryM.Property(m => m.MerchantEmail).IsModified = true;
                entryF.Property(m => m.BillingCycleId).IsModified = true;
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

                return merchant;
            }
        }

        public Merchant UpdateMerchantEmail(Merchant merchant)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(merchant).State = EntityState.Modified;
                context.SaveChanges();
                return merchant;
            }
        }

        public MerchantFeatures UpdateMerchantFeatures(MerchantFeatures merchantF)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(merchantF).State = EntityState.Modified;
                context.SaveChanges();
                return merchantF;
            }
        }

        #region Assigned Device to Merchant

        public DeviceMerchantLink AssignMerchantDevice(DeviceMerchantLink mDevice)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        mDevice.AssignedDate = DateTime.Now;
                        var savedMerchantDevice = context.DeviceMerchantLink.Add(mDevice);
                        context.SaveChanges();

                        trans.Commit();

                        return savedMerchantDevice;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        #endregion Assigned Device to Merchant

        #region Get All Merchants

        public List<Merchant> GetAllMerchantsByReseller(int resellerId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Merchant> merchants = new List<Entities.Merchant>();
            using (DataContext context = new DataContext())
            {
                var q = context.Merchants.Include("MerchantBranches")
                    .Include("ContactInformation")
                    .Include("Reseller")
                    .Include("Reseller.Partner")
                    .Include("ContactInformation.Country")
                    .Include("MerchantFeatures")
                    .Where(u => u.IsDeleted == false && u.MerchantName.Contains(search) && u.ResellerId == resellerId);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                merchants = q.ToList();
            }

            return merchants;
        }

        public List<Merchant> GetAllMerchantsByReseller(int resellerId, string search)
        {
            List<Merchant> merchants = new List<Entities.Merchant>();
            using (DataContext context = new DataContext())
            {
                var q = context.Merchants.Include("MerchantBranches")
                    .Include("ContactInformation")
                    .Include("Reseller")
                    .Include("Reseller.Partner")
                    .Include("ContactInformation.Country")
                    .Include("MerchantFeatures")
                    .Where(u => u.IsDeleted == false && u.MerchantName.Contains(search) && u.ResellerId == resellerId);

                merchants = q.ToList();
            }

            return merchants;
        }

        public List<Merchant> GetAllMerchantsByPartner(int partnerId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Merchant> merchants = new List<Entities.Merchant>();
            using (DataContext context = new DataContext())
            {
                var q = context.Merchants.Include("MerchantBranches")
                    .Include("ContactInformation")
                    .Include("Reseller")
                    .Include("Reseller.Partner")
                    .Include("ContactInformation.Country")
                    .Include("MerchantFeatures")
                    .Where(u => u.IsDeleted == false && u.MerchantName.Contains(search) && u.Reseller.PartnerId == partnerId);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                merchants = q.ToList();
            }

            return merchants;
        }

        public List<Merchant> GetAllMerchantsByPartner(int partnerId, string search)
        {
            List<Merchant> merchants = new List<Entities.Merchant>();
            using (DataContext context = new DataContext())
            {
                var q = context.Merchants.Include("MerchantBranches")
                    .Include("ContactInformation")
                    .Include("Reseller")
                    .Include("Reseller.Partner")
                    .Include("ContactInformation.Country")
                    .Include("MerchantFeatures")
                    .Where(u => u.IsDeleted == false && u.MerchantName.Contains(search) && u.Reseller.PartnerId == partnerId);

                merchants = q.ToList();
            }

            return merchants;
        }

        public Account GetMerchantId(int merchantUserId, string search)
        {
            using (DataContext context = new DataContext())
            {
                return context.Accounts.Include("Role")
                    .Include("User")
                    .SingleOrDefault(u => u.UserId == merchantUserId);
            }
        }

        public Account GetMerchantId(int merchantUserId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Accounts.Include("Role")
                    .Include("User")
                    .SingleOrDefault(u => u.UserId == merchantUserId);
            }
        }

        public List<Merchant> GetAllMerchants()
        {
            using (DataContext context = new DataContext())
            {
                return context.Merchants.OrderByDescending(u => u.MerchantName).ToList();
            }
        }

        #endregion Get All Merchants

        #region Get Merchant Details

        public Merchant GetDetailsbyMerchantId(int merchantId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var merchant = context.Merchants
                        .Include("ContactInformation")
                        .Include("Partner")
                        .Include("Reseller")
                        .Include("MerchantFeatures")
                        .Include("MerchantFeatures.BillingCycle")
                        .Include("Currency")
                        .Include("ContactInformation.Country")
                        .SingleOrDefault(u => u.MerchantId == merchantId);

                    return merchant;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion Get Merchant Details

        public DeviceMerchantLink RemoveDeviceMerchantLink(DeviceMerchantLink device)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    DeviceMerchantLink result = context.DeviceMerchantLink.Where(i => i.MerchantId == device.MerchantId && i.DeviceId == device.DeviceId).SingleOrDefault();

                    context.DeviceMerchantLink.Remove(result);
                    context.SaveChanges();
                    return result;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<Merchant> GetMerchantById(int merchantId)
        {
            using (DataContext context = new DataContext())
            {
                var merchants = context.Merchants
                    .Include("ContactInformation")
                    .Include("Reseller")
                    .Include("Reseller.Partner")
                    .Include("ContactInformation.Country")
                    .Where(m => m.MerchantId == merchantId);

                return merchants.ToList();
            }
        }

        public List<Merchant> CheckMerchant(string merchantEmail)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    return context.Merchants.Where(e => e.MerchantEmail == merchantEmail && e.IsDeleted == false && e.IsActive == true).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<MerchantBranch> CheckMerchantBranch(int merchantId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    return context.MerchantBranches.Where(m => m.MerchantId == merchantId && m.IsDeleted == false && m.IsActive == true).OrderBy(m => m.MerchantBranchId).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        #region Update Merchant For CT API

        public Merchant UpdateMerchantNeedAddToCT(bool needAdd, int merchantId)
        {
            using (DataContext db = new DataContext())
            {
                try
                {
                    var data = db.Merchants.SingleOrDefault(m => m.MerchantId == merchantId);

                    data.NeedAddToCT = needAdd;

                    db.SaveChanges();
                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Merchant UpdateMerchantNeedUpdateToCT(bool needUpdate, int merchantId)
        {
            using (DataContext db = new DataContext())
            {
                try
                {
                    var data = db.Merchants.SingleOrDefault(m => m.MerchantId == merchantId);

                    data.NeedUpdateToCT = needUpdate;

                    db.SaveChanges();
                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion Update Merchant For CT API
    }
}
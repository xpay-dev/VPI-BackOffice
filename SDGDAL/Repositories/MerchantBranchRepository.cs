using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class MerchantBranchRepository
    {
        public MerchantBranch CreateBranchtWithUser(MerchantBranch branch, Account account)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        branch.DateCreated = DateTime.Now;
                        var savedBranch = context.MerchantBranches.Add(branch);
                        context.SaveChanges();

                        account.Password = Utility.Encrypt(account.Password);
                        account.DateCreated = DateTime.Now;
                        account.IsActive = true;
                        account.ParentId = savedBranch.MerchantBranchId;
                        account.ParentTypeId = 4;//Merchant Branch
                        account.PasswordExpirationDate = DateTime.Now.AddDays(AppSettings.PasswordExpirationbyDays);
                        account.NeedsUpdate = false;

                        var savedAccount = context.Accounts.Add(account);
                        context.SaveChanges();

                        trans.Commit();

                        return savedBranch;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public Account CreateBranchUser(MerchantBranch branch, Account account)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        account.Username = account.Username;
                        account.Password = Utility.Encrypt(account.Password);
                        account.PIN = "None";
                        account.DateCreated = DateTime.Now;
                        account.IsActive = true;
                        account.IsDeleted = account.IsDeleted;
                        account.ParentId = branch.MerchantBranchId;
                        account.ParentTypeId = 4; // Merchant Branch
                        account.RoleId = account.RoleId;
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

        public MerchantBranch UpdateMerchantBranch(Dictionary<string, object> param)
        {
            using (DataContext context = new DataContext())
            {
                List<SqlParameter> ps = new List<SqlParameter>();

                param.Add("@UpdateStatusMessage out", "");

                string sqlQuery = "sp_UpdateMerchantBranchInfo ";
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

                var result = context.MerchantBranches.SqlQuery(sqlQuery, ps.ToArray()).SingleOrDefault();

                return result;
            }
        }

        public MerchantBranch UpdateMerchantBranch(MerchantBranch branch)
        {
            using (DataContext context = new DataContext())
            {
                var b = context.ContactInformation.Attach(branch.ContactInformation);

                var entryBranch = context.Entry(branch.ContactInformation);

                entryBranch.Property(e => e.Address).IsModified = true;
                entryBranch.Property(e => e.City).IsModified = true;
                entryBranch.Property(e => e.StateProvince).IsModified = true;
                entryBranch.Property(e => e.ZipCode).IsModified = true;
                entryBranch.Property(e => e.CountryId).IsModified = true;
                entryBranch.Property(e => e.PrimaryContactNumber).IsModified = true;
                entryBranch.Property(e => e.MobileNumber).IsModified = true;
                entryBranch.Property(e => e.Fax).IsModified = true;
                entryBranch.Property(e => e.NeedsUpdate).IsModified = true;
                context.SaveChanges();

                return branch;
            }
        }

        public MerchantBranch UpdateMerchantBranchStatus(MerchantBranch merchantBranch)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(merchantBranch).State = EntityState.Modified;
                context.SaveChanges();
                return merchantBranch;
            }
        }

        #region Get All MerchantBranch

        public List<MerchantBranch> GetAllMerchantBranches()
        {
            using (DataContext context = new DataContext())
            {
                return context.MerchantBranches.OrderByDescending(u => u.MerchantBranchName).ToList();
            }
        }

        public List<MerchantBranch> GetAllBranchesByMerchant(int merchantId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<MerchantBranch> branches = new List<Entities.MerchantBranch>();
            using (DataContext context = new DataContext())
            {
                IQueryable<MerchantBranch> q = context.MerchantBranches.Include("MerchantPOSs")
                    .Include("ContactInformation")
                    .Include("Merchant")
                    .Include("MerchantPOSs")
                    .Include("Merchant.Reseller")
                    .Where(u => u.IsDeleted == false && u.MerchantBranchName.Contains(search) && u.MerchantId == merchantId);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                branches = q.ToList();
            }

            return branches;
        }

        public List<MerchantBranch> GetAllBranchesByMerchant(int merchantId, string search)
        {
            List<MerchantBranch> branches = new List<Entities.MerchantBranch>();
            using (DataContext context = new DataContext())
            {
                IQueryable<MerchantBranch> q = context.MerchantBranches.Include("MerchantPOSs")
                    .Include("ContactInformation")
                    .Include("Merchant")
                    .Include("MerchantPOSs")
                    .Include("Merchant.Reseller")
                    .Where(u => u.IsDeleted == false && u.MerchantBranchName.Contains(search) && u.MerchantId == merchantId);

                branches = q.ToList();
            }

            return branches;
        }

        public List<MerchantBranch> GetAllBranchesByPartner(int partnerId, string search,
              int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<MerchantBranch> branches = new List<Entities.MerchantBranch>();
            using (DataContext context = new DataContext())
            {
                var q = context.MerchantBranches.Include("MerchantPOSs")
                    .Include("ContactInformation")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("MerchantPOSs")
                    .Include("Merchant.Reseller")
                    .Where(u => u.IsDeleted == false && u.MerchantBranchName.Contains(search) && u.Merchant.Reseller.PartnerId == partnerId);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                branches = q.ToList();
            }

            return branches;
        }

        public List<MerchantBranch> GetAllBranchesByPartner(int partnerId, string search)
        {
            List<MerchantBranch> branches = new List<Entities.MerchantBranch>();
            using (DataContext context = new DataContext())
            {
                var q = context.MerchantBranches.Include("MerchantPOSs")
                    .Include("ContactInformation")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("MerchantPOSs")
                    .Include("Merchant.Reseller")
                    .Where(u => u.IsDeleted == false && u.MerchantBranchName.Contains(search) && u.Merchant.Reseller.PartnerId == partnerId);

                branches = q.ToList();
            }

            return branches;
        }

        public List<MerchantBranch> GetAllBranchesByReseller(int resellerId, string search,
              int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<MerchantBranch> branches = new List<Entities.MerchantBranch>();
            using (DataContext context = new DataContext())
            {
                var q = context.MerchantBranches.Include("MerchantPOSs")
                    .Include("ContactInformation")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("MerchantPOSs")
                    .Include("Merchant.Reseller")
                    .Where(u => u.IsDeleted == false && u.MerchantBranchName.Contains(search) && u.Merchant.Reseller.ResellerId == resellerId);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                branches = q.ToList();
            }

            return branches;
        }

        public List<MerchantBranch> GetAllBranchesByReseller(int resellerId, string search)
        {
            List<MerchantBranch> branches = new List<Entities.MerchantBranch>();
            using (DataContext context = new DataContext())
            {
                var q = context.MerchantBranches.Include("MerchantPOSs")
                    .Include("ContactInformation")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("MerchantPOSs")
                    .Include("Merchant.Reseller")
                    .Where(u => u.IsDeleted == false && u.MerchantBranchName.Contains(search) && u.Merchant.Reseller.ResellerId == resellerId);

                branches = q.ToList();
            }

            return branches;
        }

        #endregion Get All MerchantBranch

        #region Get MerchantBranch Details

        public MerchantBranch GetDetailsbyMerchantBranchId(int branchId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var merchantBranch = context.MerchantBranches
                        .Include("ContactInformation")
                        .Include("Merchant")
                        .SingleOrDefault(u => u.MerchantBranchId == branchId);

                    return merchantBranch;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion Get MerchantBranch Details
    }
}
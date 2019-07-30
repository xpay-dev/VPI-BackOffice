using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class ResellerRepository
    {
        public Reseller CreateResellerWithUser(Reseller reseller, Account account)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        reseller.DateCreated = DateTime.Now;
                        var savedReseller = context.Resellers.Add(reseller);
                        context.SaveChanges();

                        account.PIN = "None";
                        account.Password = Utility.Encrypt(account.Password);
                        account.DateCreated = DateTime.Now;
                        account.IsActive = true;
                        account.ParentId = savedReseller.ResellerId;
                        account.ParentTypeId = 2; // Reseller
                        account.PasswordExpirationDate = DateTime.Now.AddDays(AppSettings.PasswordExpirationbyDays);
                        account.NeedsUpdate = false;

                        var savedAccount = context.Accounts.Add(account);
                        context.SaveChanges();

                        trans.Commit();

                        return savedReseller;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public Account CreateResellerUser(Reseller reseller, Account account)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        //reseller.ParentResellerId =

                        account.PIN = "None";
                        account.Password = Utility.Encrypt(account.Password);
                        account.DateCreated = DateTime.Now;
                        account.IsActive = true;
                        account.ParentId = reseller.ResellerId;
                        account.ParentTypeId = 2; // Reseller
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

        public Reseller UpdateReseller(Dictionary<string, object> param)
        {
            using (DataContext context = new DataContext())
            {
                List<SqlParameter> ps = new List<SqlParameter>();

                param.Add("@UpdateStatusMessage out", "");

                string sqlQuery = "sp_UpdateResellerInfo ";
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

                var result = context.Resellers.SqlQuery(sqlQuery, ps.ToArray()).SingleOrDefault();

                return result;
            }
        }

        public Reseller UpdateReseller(Reseller reseller)
        {
            using (DataContext context = new DataContext())
            {
                var ur = context.Resellers.Attach(reseller);

                var entryR = context.Entry(reseller);

                //Update ContactInformation
                var uc = context.ContactInformation.Attach(reseller.ContactInformation);

                var entryC = context.Entry(reseller.ContactInformation);

                entryR.Property(r => r.ResellerEmail).IsModified = true;
                entryR.Property(r => r.ResellerName).IsModified = true;

                entryC.Property(e => e.Address).IsModified = true;
                entryC.Property(e => e.City).IsModified = true;
                entryC.Property(e => e.StateProvince).IsModified = true;
                entryC.Property(e => e.ZipCode).IsModified = true;
                entryC.Property(e => e.CountryId).IsModified = true;
                entryC.Property(e => e.PrimaryContactNumber).IsModified = true;
                entryC.Property(e => e.MobileNumber).IsModified = true;
                entryC.Property(e => e.Fax).IsModified = true;
                entryC.Property(e => e.NeedsUpdate).IsModified = true;
                context.SaveChanges();

                return reseller;
            }
        }

        #region Get All Resellers

        public List<Reseller> GetAllResellersByPartner(int partnerId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Reseller> resellers = new List<Entities.Reseller>();
            using (DataContext context = new DataContext())
            {
                var q = context.Resellers.Include("Merchants")
                    .Include("ContactInformation")
                    .Include("Partner")
                    .Include("ContactInformation.Country")
                    .Where(u => u.IsDeleted == false && u.ResellerName.Contains(search) && u.PartnerId == partnerId);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                resellers = q.ToList();
            }

            return resellers;
        }

        public List<Reseller> GetAllResellersByPartner(int partnerId, string search)
        {
            List<Reseller> resellers = new List<Entities.Reseller>();
            using (DataContext context = new DataContext())
            {
                var q = context.Resellers
                    .Include("Partner")
                    .Where(r => r.PartnerId == partnerId);

                resellers = q.ToList();
            }

            return resellers;
        }

        public List<Reseller> GetAllResellers()
        {
            using (DataContext context = new DataContext())
            {
                return context.Resellers.OrderByDescending(r => r.ResellerName).ToList();
            }
        }

        public List<Reseller> GetAllResellersByPartner(int partnerId, string search, bool includeMerchants = false)
        {
            using (DataContext context = new DataContext())
            {
                var q = context.Resellers.Include("Merchants")
                    .Include("ContactInformation")
                    .Include("Partner")
                    .Include("ContactInformation.Country");

                if (includeMerchants)
                    q.Include("Merchants");

                return q.Where(u => u.IsDeleted == false && u.ResellerName.Contains(search) && u.PartnerId == partnerId).ToList();
            }
        }

        #endregion Get All Resellers

        #region Get Reseller Details

        public Reseller GetDetailsbyResellerId(int resellerId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var reseller = context.Resellers
                        .Include("ContactInformation")
                        .Include("Partner")
                        .SingleOrDefault(u => u.ResellerId == resellerId);

                    return reseller;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion Get Reseller Details

        public List<Reseller> GetResellerById(int resellerId)
        {
            using (DataContext context = new DataContext())
            {
                var q = context.Resellers
                    .Include("ContactInformation")
                    .Include("ContactInformation.Country").Where(m => m.ResellerId == resellerId);

                return q.ToList();
            }
        }
    }
}
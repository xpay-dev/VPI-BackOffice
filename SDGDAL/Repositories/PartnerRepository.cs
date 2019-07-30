using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace SDGDAL.Repositories
{
    public class PartnerRepository
    {
        public Partner UpdatePartner(Dictionary<string, object> param)
        {
            using (DataContext context = new DataContext())
            {
                List<SqlParameter> ps = new List<SqlParameter>();

                param.Add("@UpdateStatusMessage out", "");

                string sqlQuery = "sp_UpdatePartnerInfo ";
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

                var result = context.Partners.SqlQuery(sqlQuery, ps.ToArray()).SingleOrDefault();

                return result;
            }
        }

        public Partner UpdatePartner(Partner partner)
        {
            using (DataContext context = new DataContext())
            {
                //Update Partner
                //  (if there's any, uncomment this then add the columns to be updated, see ContactInformation for sample)
                var up = context.Partners.Attach(partner);
                var entryp = context.Entry(partner);

                //Update ContactInformation
                var uc = context.ContactInformation.Attach(partner.ContactInformation);
                var entryC = context.Entry(partner.ContactInformation);

                entryp.Property(p => p.CompanyEmail).IsModified = true;
                entryp.Property(p => p.CompanyName).IsModified = true;

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

                return partner;
            }

            return null;
        }

        public Partner CreatePartnerWithUser(Partner partner, Account account)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        partner.DateCreated = DateTime.Now;
                        var savedPartner = context.Partners.Add(partner);
                        context.SaveChanges();

                        account.PIN = "None";
                        account.Password = Utility.Encrypt(account.Password);
                        account.DateCreated = DateTime.Now;
                        account.IsActive = true;
                        account.ParentId = savedPartner.PartnerId;
                        account.ParentTypeId = 1; // Partner
                        account.PasswordExpirationDate = DateTime.Now.AddDays(AppSettings.PasswordExpirationbyDays);
                        account.NeedsUpdate = false;

                        var savedAccount = context.Accounts.Add(account);
                        context.SaveChanges();

                        trans.Commit();

                        return savedPartner;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public Account CreatePartnerUser(Partner partner, Account account)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        account.PIN = "None";
                        account.Password = Utility.Encrypt(account.Password);
                        account.DateCreated = DateTime.Now;
                        account.IsActive = true;
                        account.ParentId = partner.ParentPartnerId.Value;
                        account.ParentTypeId = 1; // Partner
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

        public Account CreatePartnerUserForMarketing(Partner partner, Account account)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        account.PIN = "None";
                        account.Password = Utility.Encrypt(account.Password);
                        account.DateCreated = DateTime.Now;
                        account.IsActive = true;
                        account.ParentId = partner.ParentPartnerId.Value;
                        account.ParentTypeId = 6; //Marketing
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

        #region Get All Partners

        public List<Partner> GetAllPartnersByParent(int parentPartnerId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Partner> partners = new List<Entities.Partner>();
            using (DataContext context = new DataContext())
            {
                var q = context.Partners.Include("Resellers")
                    .Include("ContactInformation")
                    .Include("ParentPartner")
                    .Include("ContactInformation.Country")
                    .Where(u => u.IsDeleted == false && u.CompanyName.Contains(search)
                                && u.ParentPartnerId == parentPartnerId);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                partners = q.ToList();
            }

            return partners;
        }

        public List<Partner> GetAllPartnersByParent(int parentPartnerId, string search)
        {
            List<Partner> partners = new List<Entities.Partner>();
            using (DataContext context = new DataContext())
            {
                var q = context.Partners
                    .Include("ContactInformation")
                    .Include("ParentPartner")
                    .Include("ContactInformation.Country")
                    .Where(u => u.IsDeleted == false && u.CompanyName.Contains(search));

                if (parentPartnerId > 0)
                    q = q.Where(u => u.ParentPartnerId == parentPartnerId || u.PartnerId == parentPartnerId);

                partners = q.ToList();
            }

            return partners;
        }

        public List<Partner> GetAllPartnersByParentPartnerId(int parentPartnerId, string search)
        {
            List<Partner> partners = new List<Entities.Partner>();
            using (DataContext context = new DataContext())
            {
                var q = context.Partners
                    .Include("ContactInformation")
                    .Include("ParentPartner")
                    .Include("ContactInformation.Country")
                    .Where(u => u.ParentPartnerId == parentPartnerId);

                partners = q.ToList();
            }

            return partners;
        }

        #endregion Get All Partners

        #region Get Partner Details

        public Partner GetDetailsbyPartnerId(int partnerId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var partner = context.Partners
                        .Include("ContactInformation")
                        .Include("ParentPartner")
                        .SingleOrDefault(u => u.PartnerId == partnerId);

                    return partner;
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool IsSubPartner(int partnerId, int parentId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    return context.Partners.SingleOrDefault(p => p.PartnerId == partnerId && p.ParentPartnerId == parentId) != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        #endregion Get Partner Details

        public List<Partner> GetAllPartners(string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Partner> partners = new List<Entities.Partner>();
            using (DataContext context = new DataContext())
            {
                var q = context.Partners
                    .Include("ContactInformation")
                    .Include("ParentPartner")
                    .Include("ContactInformation.Country")
                    .Where(p => !p.IsDeleted && p.CompanyName.Contains(search) && p.PartnerId > 0);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                partners = q.ToList();
            }
            return partners;
        }

        public List<Partner> GetAllPartners()
        {
            List<Partner> partners = new List<Entities.Partner>();
            using (DataContext context = new DataContext())
            {
                try
                {
                    return context.Partners.OrderBy(p => p.CompanyName).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class RequestedMerchantRepository
    {
        public RequestedMerchant CreateRequestedMerchant(RequestedMerchant rMerchant)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        var savedMerchant = context.RequestedMerchants.Add(rMerchant);
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

        public List<RequestedMerchant> RequestedMerchantList(string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<RequestedMerchant> rMerchants = new List<Entities.RequestedMerchant>();

            using (DataContext context = new DataContext())
            {
                var q = context.RequestedMerchants.Where(u => u.IsDeleted == false && u.MerchantName.Contains(search));

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                rMerchants = q.ToList();
            }

            return rMerchants;
        }

        public List<RequestedMerchant> RequestedMerchantList(int parentId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<RequestedMerchant> rMerchants = new List<Entities.RequestedMerchant>();

            using (DataContext context = new DataContext())
            {
                var q = context.RequestedMerchants.Where(u => u.IsDeleted == false && u.ParentId == parentId && u.MerchantName.Contains(search));

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                rMerchants = q.ToList();
            }

            return rMerchants;
        }

        public RequestedMerchant GetMerchant(int id)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var rm = context.RequestedMerchants.SingleOrDefault(r => r.RequestedMerchantId == id);

                    return rm;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public RequestedMerchant UpdateRequestedMerchantStatus(RequestedMerchant merchant)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var um = context.RequestedMerchants.Attach(merchant);

                    var entryM = context.Entry(merchant);
                    entryM.Property(m => m.IsActive).IsModified = true;
                    //context.Entry(merchant).State = EntityState.Modified;
                    context.SaveChanges();
                    return merchant;
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

        public bool UpdateRequestedMerchant(RequestedMerchant merchant)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    context.Entry(merchant).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}
using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class EmailServerRepository
    {
        public EmailServer CreateEmailServer(EmailServer email)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        email.DateCreated = DateTime.Now;
                        var savedPartner = context.EmailServers.Add(email);
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

        public EmailServer UpdateEmailServer(EmailServer emailserver)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(emailserver).State = EntityState.Modified;
                context.SaveChanges();
                return emailserver;
            }
        }

        public List<EmailServer> GetAllEmailServerByPartner(int partnerId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<EmailServer> email = new List<Entities.EmailServer>();
            using (DataContext context = new DataContext())
            {
                var q = context.EmailServers.Where(es => es.PartnerId == partnerId && es.IsDeleted != true);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                email = q.ToList();
            }

            return email;
        }

        public EmailServer GetEmailServerById(int eId)
        {
            EmailServer switchess = new EmailServer();
            using (DataContext context = new DataContext())
            {
                var s = context.EmailServers.SingleOrDefault(p => p.EmailServerId == eId);

                return s;
            }
        }

        public List<EmailServer> GetEmailServerByPartnerId(int partnerId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    return context.EmailServers.Where(e => e.PartnerId == partnerId && e.IsDeleted == false && e.IsActive == true && e.IsPartnerDefaultEmail == true).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
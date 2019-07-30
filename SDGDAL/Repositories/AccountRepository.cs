using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class AccountRepository
    {
        public Account GetTypeIdByAccountId(int aId)
        {
            using (DataContext context = new DataContext())
            {
                var q = context.Accounts.Include("User")
                    .Include("Role")
                    .SingleOrDefault(u => u.UserId == aId);

                return q;
            }
        }

        public Account GetAccountInfoByMerchantId(int merchantId)
        {
            using (DataContext context = new DataContext())
            {
                var q = context.Accounts.SingleOrDefault(u => u.ParentId == merchantId);

                return q;            }
        }

        public List<Account> GetAuditTrailList(string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Account> app = new List<Account>();

            using (DataContext context = new DataContext())
            {
                var q = context.Accounts.Include("User")
                        .Include("Role")
                        .Where(u => u.LastLoggedIn != null && u.IPAddress != "::1");

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                return q.ToList();
            }
        }
    }
}
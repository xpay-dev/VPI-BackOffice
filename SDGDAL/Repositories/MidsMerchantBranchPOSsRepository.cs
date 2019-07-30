using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class MidsMerchantBranchPOSsRepository
    {
        public List<MidsMerchantBranchPOSs> GetAllMidsMerchantBranchPOSs(int midId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            using (DataContext context = new DataContext())
            {
                var midsmerchantbranchpos = context.MidsMerchantBranchPOSs
                    .Include("MerchantBranchPOS")
                    .Include("Mid.Switch")
                    .Include("Mid.CardType")
                    .Include("Mid.Currency")
                    .Where(m => m.MerchantBranchPOSId == midId && m.IsDeleted == false);

                totalRecords = midsmerchantbranchpos.Count();

                return midsmerchantbranchpos.ToList();
            }
        }

        public List<MidsMerchantBranchPOSs> GetAllMidsMerchantBranchPOSs()
        {
            using (DataContext context = new DataContext())
            {
                var midsmerchantbranchpos = context.MidsMerchantBranchPOSs
                    .Include("MerchantBranchPOS")
                    .Where(m => m.IsDeleted != true);

                return midsmerchantbranchpos.ToList();
            }
        }

        public List<MidsMerchantBranchPOSs> GetAllMidsMerchantBranchPOSs(int cId)
        {
            using (DataContext context = new DataContext())
            {
                var midsmerchantbranchpos = context.MidsMerchantBranchPOSs
                    .Include("MerchantBranchPOS")
                    .Include("Mid")
                    .Where(m => m.IsDeleted != true && cId != m.Mid.CardTypeId);

                return midsmerchantbranchpos.ToList();
            }
        }

        public MidsMerchantBranchPOSs GetMidsMerchantBranchesPossByPOSId(int pId)
        {
            using (DataContext context = new DataContext())
            {
                return context.MidsMerchantBranchPOSs.SingleOrDefault(m => m.MerchantBranchPOSId == pId);
            }
        }

        public List<MidsMerchantBranchPOSs> GetMidsMerchantBranchesPossByPOSIds(int posId)
        {
            using (DataContext context = new DataContext())
            {
                var list = context.MidsMerchantBranchPOSs
                    .Include("MerchantBranchPOS")
                    .Include("Mid.CardType")
                    .Where(m => m.MerchantBranchPOSId == posId);

                return list.ToList();
            }
        }

        public MidsMerchantBranchPOSs GetMidsMerchantBranchesPossByPOSIdandMid(int pId, int midId)
        {
            using (DataContext context = new DataContext())
            {
                return context.MidsMerchantBranchPOSs.SingleOrDefault(m => m.MerchantBranchPOSId == pId && m.MidId == midId);
            }
        }

        public List<MidsMerchantBranchPOSs> GetMidsMerchantBranchesPossByPOSId(int pId, string search)
        {
            using (DataContext context = new DataContext())
            {
                var md = context.MidsMerchantBranchPOSs.Where(m => m.MerchantBranchPOSId == pId);

                return md.ToList();
            }
        }

        public MidsMerchantBranchPOSs GetMidsMerchantBranchesPossByMidId(int mId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    return context.MidsMerchantBranchPOSs.SingleOrDefault(m => m.MidId == mId);
                }
                catch (InvalidOperationException ex)
                {
                    throw ex;
                }
            }
        }

        public MidsMerchantBranchPOSs UpdateIsDeleted(MidsMerchantBranchPOSs mids)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(mids).State = EntityState.Modified;
                context.SaveChanges();
                return mids;
            }
        }

        public bool IsPoSAssignedExist(int bId)
        {
            using (DataContext context = new DataContext())
            {
                var x = context.MidsMerchantBranchPOSs.SingleOrDefault(mmb => mmb.MerchantBranchPOSId.Equals(bId)) == null;

                //if (x)

                return x;
            }
        }
    }
}
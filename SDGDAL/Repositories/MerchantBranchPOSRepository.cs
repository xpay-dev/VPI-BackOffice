using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class MerchantBranchPOSRepository
    {
        public MerchantBranchPOS CreateMerchantBranchPOS(MerchantBranchPOS mPOS)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var savedpos = context.MerchantPOSs.Add(mPOS);
                    context.SaveChanges();

                    trans.Commit();
                    return savedpos;
                }
            }
        }

        public MerchantBranchPOS CreateMerchantBranchPOS(MobileApp mApp)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var savedMobileApp = context.MobileApps.Add(mApp);
                    context.SaveChanges();

                    trans.Commit();
                    return savedMobileApp.MerchantBranchPOS;
                }
            }
        }

        public bool CreateMerchantBranchPOSs(List<MobileApp> mApp)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        var savedMobileApp = context.MobileApps.AddRange(mApp);

                        context.SaveChanges();

                        trans.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        return false;
                    }
                }
            }
        }

        public List<MerchantBranchPOS> GetAllMerchantBranchPOSs()
        {
            using (DataContext context = new DataContext())
            {
                return context.MerchantPOSs.OrderByDescending(u => u.MerchantPOSName).ToList();
            }
        }

        public List<MerchantBranchPOS> GetAllMerchantThroughMerchantBranchPOS(int pId)
        {
            using (DataContext context = new DataContext())
            {
                return context.MerchantPOSs
                              .Include("MerchantBranch")
                              .Include("MerchantBranch.Merchant")
                              .Where(u => u.MerchantPOSId == pId).ToList();
            }
        }

        public List<MerchantBranchPOS> GetAllPOSsByBranch(int branchId, string search,
            int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<MerchantBranchPOS> merchantbranchPos = new List<MerchantBranchPOS>();

            using (DataContext context = new DataContext())
            {
                var poss = context.MerchantPOSs
                                  .Include("MobileApp")
                                  .Include("MerchantBranch")
                                  .Include("MerchantBranch.Merchant")
                                  .Include("MerchantBranch.Merchant.Reseller")
                                  .Where(p => p.MerchantBranchId == branchId
                                           && p.MerchantPOSName.Contains(search)
                                           && p.IsDeleted == false);

                totalRecords = poss.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    poss = poss.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    poss = poss.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                merchantbranchPos = poss.ToList();
            }

            return merchantbranchPos;
        }

        public List<MerchantBranchPOS> GetAllPOSsByBranch(int branchId, string search)
        {
            using (DataContext context = new DataContext())
            {
                var poss = context.MerchantPOSs
                                  .Include("MobileApp")
                                  .Include("MerchantBranch")
                                  .Include("MerchantBranch.Merchant")
                                  .Include("MerchantBranch.Merchant.Reseller")
                                  .Where(p => p.MerchantBranchId == branchId
                                           && p.MerchantPOSName.Contains(search)
                                           && p.IsDeleted == false);

                return poss.ToList();
            }
        }

        public List<MerchantBranchPOS> GetAllPOSsByMerchant(int merchantId, string search,
            int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            using (DataContext context = new DataContext())
            {
                var poss = context.MerchantPOSs
                                  .Include("MobileApp")
                                  .Include("MerchantBranch")
                                  .Include("MerchantBranch.Merchant")
                                  .Include("MerchantBranch.Merchant.Reseller")
                                  .Where(p => p.MerchantBranch.MerchantId == merchantId
                                           && p.MerchantPOSName.Contains(search)
                                           && p.IsDeleted == false);

                totalRecords = poss.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    poss = poss.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    poss = poss.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                return poss.ToList();
            }
        }

        public List<MerchantBranchPOS> GetAllPOSsByReseller(int resellerId, string search,
            int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            using (DataContext context = new DataContext())
            {
                var poss = context.MerchantPOSs
                                  .Include("MobileApp")
                                  .Include("MerchantBranch")
                                  .Include("MerchantBranch.Merchant")
                                  .Include("MerchantBranch.Merchant.Reseller")
                                  .Where(p => p.MerchantBranch.Merchant.ResellerId == resellerId
                                           && p.MerchantPOSName.Contains(search)
                                           && p.IsDeleted == false);

                totalRecords = poss.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    poss = poss.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    poss = poss.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                return poss.ToList();
            }
        }

        public List<MerchantBranchPOS> GetAllPOSsByPartner(int partnerId, string search,
            int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            using (DataContext context = new DataContext())
            {
                var poss = context.MerchantPOSs
                                  .Include("MobileApp")
                                  .Include("MerchantBranch")
                                  .Include("MerchantBranch.Merchant")
                                  .Include("MerchantBranch.Merchant.Reseller")
                                  .Where(p => p.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                           && p.MerchantPOSName.Contains(search)
                                           && p.IsDeleted == false);

                totalRecords = poss.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    poss = poss.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    poss = poss.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                return poss.ToList();
            }
        }

        public List<MerchantBranchPOS> GetAllPOSsByPartner(int partnerId, string search)
        {
            using (DataContext context = new DataContext())
            {
                var poss = context.MerchantPOSs
                                  .Include("MobileApp")
                                  .Include("MerchantBranch")
                                  .Include("MerchantBranch.Merchant")
                                  .Include("MerchantBranch.Merchant.Reseller")
                                  .Where(p => p.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                           && p.MerchantPOSName.Contains(search)
                                           && p.IsDeleted == false);

                return poss.ToList();
            }
        }

        public MerchantBranchPOS GetDetailsbyMerchantPOSId(int posid)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var merchantPos = context.MerchantPOSs
                                        .Include("MobileApp")
                                        .Include("MerchantBranch")
                                        .Include("MerchantBranch.Merchant")
                                        .SingleOrDefault(u => u.MerchantPOSId == posid);
                    return merchantPos;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public MerchantBranchPOS UpdateMerchantBranchPOS(MerchantBranchPOS p)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    MerchantBranchPOS pos = context.MerchantPOSs.Single(i => i.MerchantPOSId == p.MerchantPOSId);
                    pos.MerchantPOSName = p.MerchantPOSName;
                    context.SaveChanges();

                    return pos;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public MerchantBranchPOS UpdateMerchantBranchPOSStatus(MerchantBranchPOS p)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(p).State = EntityState.Modified;
                context.SaveChanges();
                return p;
            }
        }

        public List<MerchantBranchPOS> GetAllPOSsByMerchant(int merchantId)
        {
            using (DataContext context = new DataContext())
            {
                return context.MerchantPOSs
                                  .Include("MobileApp")
                                  .Include("MerchantBranch")
                                  .Include("MerchantBranch.Merchant")
                                  .Include("MerchantBranch.Merchant.Reseller")
                                  .Where(p => p.MerchantBranch.MerchantId == merchantId && p.IsDeleted == false).ToList();
            }
        }

        #region POS Device

        public DevicePOSLink AssignPosDevice(DevicePOSLink posDevice)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        posDevice.AssignedDate = DateTime.Now;
                        var savedPosDevice = context.DevicePOSLink.Add(posDevice);
                        context.SaveChanges();

                        trans.Commit();

                        return savedPosDevice;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public DevicePOSLink RemoveDevicePosLinkbyPosId(DevicePOSLink posDevice)
        {
            using (var context = new DataContext())
            {
                try
                {
                    var result = context.DevicePOSLink.Where(i => i.MerchantPOSId == posDevice.MerchantPOSId);

                    foreach (var posDeviceLink in result)
                    {
                        context.DevicePOSLink.Remove(posDeviceLink);
                    }

                    context.SaveChanges();

                    return posDevice;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        #endregion POS Device
    }
}
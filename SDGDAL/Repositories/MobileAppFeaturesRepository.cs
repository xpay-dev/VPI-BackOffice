using SDGDAL.Entities;
using System;
using System.Data.Entity;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class MobileAppFeaturesRepository
    {
        public MobileAppFeatures CreateMobileAppFeatures(MobileAppFeatures posAppFeatures, MobileApp mobileApp)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        var posFeatures = context.MobileAppFeatures.Add(posAppFeatures);
                        context.SaveChanges();

                        var mobileAppResult = context.MobileApps.SingleOrDefault(m => m.MerchantBranchPOSId == mobileApp.MerchantBranchPOSId);
                        mobileAppResult.MobileAppFeaturesId = posAppFeatures.MobileAppFeaturesId;
                        context.SaveChanges();

                        trans.Commit();

                        return posFeatures;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public MobileAppFeatures CreateMobileAppFeatures(MobileAppFeatures posAppFeatures)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        var posFeatures = context.MobileAppFeatures.Add(posAppFeatures);
                        context.SaveChanges();

                        trans.Commit();

                        return posFeatures;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public MobileAppFeatures GetMobileAppFeaturesById(int mposFeaturesId)
        {
            using (DataContext context = new DataContext())
            {
                var mobileFeatures = context.MobileAppFeatures
                    .Include("Currency")
                    .SingleOrDefault(m => m.MobileAppFeaturesId == mposFeaturesId);

                return mobileFeatures;
            }
        }

        public MobileAppFeatures UpdateMobileAppFeatures(MobileAppFeatures posFeatures)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(posFeatures).State = EntityState.Modified;
                context.SaveChanges();
                return posFeatures;
            }
        }
    }
}
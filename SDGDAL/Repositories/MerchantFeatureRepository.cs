using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class MerchantFeatureRepository
    {
        public Agreements CreateTermsOfService(Agreements agreements)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    using (var trans = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var savedAgreements = context.Agreements.Add(agreements);
                            context.SaveChanges();

                            trans.Commit();

                            return savedAgreements;
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return agreements;
        }

        public Agreements UpdateTermsOfService(Agreements agreements)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(agreements).State = EntityState.Modified;
                context.SaveChanges();
                return agreements;
            }
        }

        public MerchantFeatures UpdateAgreementsStatus(MerchantFeatures features)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(features).State = EntityState.Modified;
                context.SaveChanges();
                return features;
            }
        }

        public List<Agreements> GetAgreementsId()
        {
            using (DataContext context = new DataContext())
            {
                return context.Agreements.OrderByDescending(a => new { a.MerchantId, a.AgreementsId }).ToList();
            }
        }

        public Agreements GetDetailsByAgreementId(int merchId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var agreement = context.Agreements
                        .SingleOrDefault(u => u.MerchantId == merchId);

                    return agreement;
                }
                catch
                {
                    return null;
                }
            }
        }

        public MerchantFeatures GetDetailsByMerchantFeaturesId(int mechantId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var features = context.MerchantFeatures
                        .SingleOrDefault(u => u.MerchantFeaturesId == mechantId);

                    return features;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
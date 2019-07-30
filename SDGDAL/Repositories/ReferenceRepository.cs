using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class ReferenceRepository
    {
        public List<Country> GetAllCountries()
        {
            using (DataContext context = new DataContext())
            {
                var countries = context.Countries;

                return countries.ToList();
            }
        }

        public List<Provinces> GetAllProvincesByCountry(int countryId)
        {
            using (DataContext context = new DataContext())
            {
                var provinces = context.Provinces.Where(c => c.CountryId == countryId);

                return provinces.ToList();
            }
        }

        public List<Provinces> GetAllProvinces()
        {
            using (DataContext context = new DataContext())
            {
                var provinces = context.Provinces;

                return provinces.ToList();
            }
        }

        public List<CardType> GetAllCardTypes()
        {
            using (DataContext context = new DataContext())
            {
                var cardTypes = context.CardTypes;

                return cardTypes.ToList();
            }
        }

        public Country GetCountryNameByCode(string countryCode)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    return context.Countries.SingleOrDefault(d => d.CountryCode == countryCode);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<Switch> GetAllSwitches()
        {
            using (DataContext context = new DataContext())
            {
                var switches = context.Switches.Where(s => s.IsActive == true);

                return switches.ToList();
            }
        }

        public List<Currency> GetAllCurrencies()
        {
            using (DataContext context = new DataContext())
            {
                var currencies = context.Currencies;

                return currencies.ToList();
            }
        }

        public List<Currency> GetCurrencyDetailsByID(int currencyId)
        {
            using (DataContext context = new DataContext())
            {
                var currencies = context.Currencies.Where(c => c.CurrencyId == currencyId).ToList();

                return currencies;
            }
        }

        public List<BillingCycle> GetAllBillingCycles()
        {
            using (DataContext context = new DataContext())
            {
                var billing = context.BillingCycles;

                return billing.ToList();
            }
        }

        public List<DeviceFlowType> GetDeviceFlowType()
        {
            using (DataContext context = new DataContext())
            {
                var deviceflowtype = context.DeviceFlowTypes;

                return deviceflowtype.ToList();
            }
        }

        public List<DeviceType> GetDeviceType()
        {
            using (DataContext context = new DataContext())
            {
                var devicetype = context.DeviceTypes;

                return devicetype.ToList();
            }
        }

        #region Error Logging

        public ErrorLog LogError(ErrorLog err)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        var errSave = context.ErrorLogs.Add(err);
                        context.SaveChanges();

                        trans.Commit();

                        return errSave;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        #endregion Error Logging
    }
}
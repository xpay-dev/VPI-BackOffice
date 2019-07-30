using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class MerchantFeaturesModel
    {
        public int MerchantFeaturesId { get; set; }

        public int BillingCycleId { get; set; }
        public BillingCycle BillingCycle { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public string LanguageCode { get; set; }

        public bool TermsOfServiceEnabled { get; set; }
        public bool DisclaimerEnabled { get; set; }
        public bool UseDefaultAgreements { get; set; }
    }
}
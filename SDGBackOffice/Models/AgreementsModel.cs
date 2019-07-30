using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SDGBackOffice.Models
{
    public class AgreementsModel
    {
        public int AgreementsId { get; set; }

        public string TermsOfService { get; set; }
        public string Disclaimer { get; set; }

        public string LanguageCode { get; set; }

        public bool IsCustom { get; set; }

        public Nullable<int> MerchantId { get; set; }
        public Merchant Merchant { get; set; }
    }
}
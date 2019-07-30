using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SDGWebService.Classes
{
    public class CreditManualRequest
    {
        [DataMember]
        public string LanguageUser { get; set; }

        [DataMember]
        public decimal Tips { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string RefNumberApp { get; set; }

        [DataMember]
        public string RefNumberSale { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public string EncCardDetails { get; set; }

        [DataMember]
        public string NameOnCard { get; set; }

        [DataMember]
        public string BankName { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }

        [DataMember]
        public CustomerInfo CustomerInfo { get; set; }
    }
}
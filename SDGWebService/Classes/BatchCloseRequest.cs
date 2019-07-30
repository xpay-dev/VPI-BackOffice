using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SDGWebService.Classes
{
    public class BatchCloseRequest
    {
        [DataMember]
        public int MobileAppTransType { get; set; }

        [DataMember]
        public decimal TotalAmount { get; set; }

        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public string BatchNumber { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}
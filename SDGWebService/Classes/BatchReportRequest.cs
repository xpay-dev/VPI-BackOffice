using SDGWebService.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SDGWebService.Classes
{
    public class BatchReportsRequest
    {
        [DataMember]
        public int MobileAppTransType { get; set; }

        [DataMember]
        public int ReportsCriteria { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}
using SDGWebService.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class BatchResponse
    {
        [DataMember]
        public POSWSResponse POSWSResponse { get; set; }
        [DataMember]
        public string MerchantId { get; set; }
        [DataMember]
        public string TerminalId { get; set; }
        [DataMember]
        public string Host { get; set; }
        [DataMember]
        public string DateTime { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public string TotalAmount { get; set; }
        [DataMember]
        public string TotalCount { get; set; }
        [DataMember]
        public string BatchNumber { get; set; }
        [DataMember]
        public string XmlDetailedReport { get; set; }
        [DataMember]
        public Sales Sales { get; set; }
        [DataMember]
        public Void Void { get; set; }
        [DataMember]
        public Reversed Reversed { get; set; }
        [DataMember]
        public string CardType { get; internal set; }
    }
}
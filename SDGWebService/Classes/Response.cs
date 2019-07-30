using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SDGWebService.Classes
{
    public class Response
    {
        [DataMember]
        public string ApprovalCode { get; set; }
        [DataMember]
        public string ResponseCode { get; set; }
        [DataMember]
        public string Timestamp { get; set; }
        [DataMember]
        public string RetrievalReferenceNumber { get; set; }
        [DataMember]
        public string TransactionNumber { get; set; }
        [DataMember]
        public string CardNumber { get; set; }
        [DataMember]
        public decimal Total { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public string TransactionEntryType { get; set; }
        [DataMember]
        public string TransactionType { get; set; }
        [DataMember]
        public string CardType { get; set; }
        [DataMember]
        public string AuthNumber { get; set; }
        [DataMember]
        public string TransNumber { get; set; }
        [DataMember]
        public POSWSResponse POSWSResponse { get; set; }
    }
}
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class PurchaseResponse
    {
        [DataMember]
        public POSWSResponse POSWSResponse { get; set; }

        [DataMember]
        public string AuthNumber { get; set; }

        [DataMember]
        public string TransNumber { get; set; }

        [DataMember]
        public string SequenceNumber { get; set; }

        [DataMember]
        public string BatchNumber { get; set; }

        [DataMember]
        public string Timestamp { get; set; }

        [DataMember]
        public string TransactionNumber { get; set; }

        [DataMember]
        public string CardType { get; set; }

        [DataMember]
        public string TransactionType { get; set; }

        [DataMember]
        public string TransactionEntryType { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public decimal SubTotal { get; set; }

        [DataMember]
        public decimal Tax1Amount { get; set; }

        [DataMember]
        public string Tax1Name { get; set; }

        [DataMember]
        public decimal Tax1Rate { get; set; }

        [DataMember]
        public decimal Tax2Amount { get; set; }

        [DataMember]
        public string Tax2Name { get; set; }

        [DataMember]
        public decimal Tax2Rate { get; set; }

        [DataMember]
        public decimal Total { get; set; }

        [DataMember]
        public decimal Tips { get; set; }

        //Additional Debit Response
        [DataMember]
        public string TraceNumber { get; set; }

        [DataMember]
        public string RetrievalNumber { get; set; }

        [DataMember]
        public string TerminalId { get; set; }

        [DataMember]
        public string MerchantId { get; set; }

        [DataMember]
        public string Currency { get; set; }
    }
}
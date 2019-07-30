using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class Transaction 
    {
        [DataMember]
        public string BatchNumber { get; set; }

        [DataMember]
        public string AuthNumber { get; set; }

        [DataMember]
        public string SequenceNumber { get; set; }

        [DataMember]
        public string Timestamp { get; set; }

        [DataMember]
        public string TransactionNumber { get; set; }

        [DataMember]
        public string MerchantReferenceNumber { get; set; }

        [DataMember]
        public string ApprovalCode { get; set; }

        [DataMember]
        public string ResponseIsoCode { get; set; }

        [DataMember]
        public string TransactionEntryType { get; set; }

        [DataMember]
        public string CardType { get; set; }

        [DataMember]
        public string TransactionType { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public string NameOnCard { get; set; }

        [DataMember]
        public string SubTotal { get; set; }

        [DataMember]
        public string Tax1Amount { get; set; }

        [DataMember]
        public string Tax1Name { get; set; }

        [DataMember]
        public string Tax1Rate { get; set; }

        [DataMember]
        public string Tax2Amount { get; set; }

        [DataMember]
        public string Tax2Name { get; set; }

        [DataMember]
        public string Tax2Rate { get; set; }

        [DataMember]
        public string Total { get; set; }

        [DataMember]
        public string Tips { get; set; }

        [DataMember]
        public string TransactionSignature { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public string MobileAppTransType { get; set; }

        [DataMember]
        public string MerchantId { get; set; }

        [DataMember]
        public string TerminalId { get; set; }
    }
}
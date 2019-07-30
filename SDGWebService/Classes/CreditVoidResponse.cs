using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class CreditVoidRefundResponse
    {
        [DataMember]
        public string Date { get; set; }

        [DataMember]
        public string TransactionNumber { get; set; }

        [DataMember]
        public string AuthNumber { get; set; }

        [DataMember]
        public string Amount { get; set; }

        [DataMember]
        public string CardType { get; set; }

        [DataMember]
        public string TransactionType { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public string CardName { get; set; }

        [DataMember]
        public string Signature { get; set; }

        [DataMember]
        public string VoidReason { get; set; }

        [DataMember]
        public string VoidNote { get; set; }

        [DataMember]
        public POSWSResponse POSWSResponse { get; set; }

        [DataMember]
        public string Currency { get; set; }
    }
}
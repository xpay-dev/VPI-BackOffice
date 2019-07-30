using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class SmsResponse
    {
        [DataMember]
        public string MerchantName { get; set; }

        [DataMember]
        public string TransactionNumber { get; set; }

        [DataMember]
        public string Timestamp { get; set; }

        [DataMember]
        public string AuthNumber { get; set; }

        [DataMember]
        public string CardType { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public POSWSResponse POSWSResponse { get; set; }
    }
}
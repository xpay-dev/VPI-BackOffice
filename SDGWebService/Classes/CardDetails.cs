using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class CardDetails
    {
        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public string CSC { get; set; }

        [DataMember]
        public string NameOnCard { get; set; }

        [DataMember]
        public string ExpMonth { get; set; }

        [DataMember]
        public string ExpYear { get; set; }

        [DataMember]
        public string Track1 { get; set; }

        [DataMember]
        public string Track2 { get; set; }

        [DataMember]
        public string Track3 { get; set; }

        [DataMember]
        public string Ksn { get; set; }

        [DataMember]
        public string EpbKsn { get; set; }

        [DataMember]
        public string EmvICCData { get; set; }

        [DataMember]
        public string Epb { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string RefNumberApp { get; set; }

        [DataMember]
        public string RefNumberSale { get; set; }

        [DataMember]
        public string Currency { get; set; }
    }
}
using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class SmsRequest
    {
        [DataMember]
        public string TransNumber { get; set; }

        [DataMember]
        public int TransType { get; set; }

        [DataMember]
        public string CountryCode { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}
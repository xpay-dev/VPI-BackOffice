using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class TransactionSignatureRequest
    {
        [DataMember]
        public string TransNumber { get; set; }

        [DataMember]
        public string ImageLen { get; set; }

        [DataMember]
        public string Image { get; set; }

        [DataMember]
        public int MobileAppTransType { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}
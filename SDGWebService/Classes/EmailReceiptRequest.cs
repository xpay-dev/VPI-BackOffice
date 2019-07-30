using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class EmailReceiptRequest
    {
        [DataMember]
        public string TransNumber { get; set; }

        [DataMember]
        public EmailDetails EmailDetails { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}
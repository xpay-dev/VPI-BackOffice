using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class EmailDetails
    {
        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string ReceiptHeader { get; set; }

        [DataMember]
        public string ReceiptSec1 { get; set; }

        [DataMember]
        public string ReceiptSec2 { get; set; }

        [DataMember]
        public string ReceiptSec3 { get; set; }

        [DataMember]
        public string ReceiptFooter { get; set; }
    }
}
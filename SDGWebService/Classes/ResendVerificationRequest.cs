using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    public class ResendVerificationRequest
    {
        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}
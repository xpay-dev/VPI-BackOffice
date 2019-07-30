using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class RegistrationRequest
    {
        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string MerchantEmail { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string ConfirmPassword { get; set; }

        [DataMember]
        public string UserEmail { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}
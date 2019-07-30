using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    public class RegistrationResponse
    {
        [DataMember]
        public POSWSResponse POSWSResponse { get; set; }

        [DataMember]
        public string ActivationKey { get; set; }

    }
}
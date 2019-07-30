using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class LoginRequest
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Pin { get; set; }

        [DataMember]
        public string AppVersion { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}
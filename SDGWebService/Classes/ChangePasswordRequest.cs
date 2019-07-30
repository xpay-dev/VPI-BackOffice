using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    public class ChangePasswordRequest
    {
        [DataMember]
        public int AccountId { get; set; }

        [DataMember]
        public string OldPassword { get; set; }

        [DataMember]
        public string NewPassword { get; set; }

        [DataMember]
        public string ConfirmPassword { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}
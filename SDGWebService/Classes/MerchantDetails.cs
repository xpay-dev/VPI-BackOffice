using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class MerchantDetails
    {
        [DataMember]
        public string MerchantId { get; set; }

        [DataMember]
        public string MerchantName { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Zip { get; set; }

        [DataMember]
        public string PrimaryContactNumber { get; set; }

        [DataMember]
        public bool TOSEnabled { get; set; }

        [DataMember]
        public bool DisclaimerEnabled { get; set; }

        [DataMember]
        public string AppLanguage { get; set; }

        [DataMember]
        public string BranchName { get; set; }
    }
}
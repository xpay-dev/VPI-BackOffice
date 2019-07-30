using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class TransLookupRequest
    {
        [DataMember]
        public int SearchUsing { get; set; }

        [DataMember]
        public string SearchCriteria { get; set; }

        [DataMember]
        public int MobileAppTransType { get; set; }

        [DataMember]
        public int AccountId { get; set; }

        [DataMember]
        public int MobileAppId { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}
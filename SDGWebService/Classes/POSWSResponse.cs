using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class POSWSResponse
    {
        [DataMember]
        public string Status
        {
            get;
            set;
        }

        [DataMember]
        public string ErrNumber
        {
            get;
            set;
        }

        [DataMember]
        public string Message
        {
            get;
            set;
        }

        [DataMember]
        public bool UpdatePending
        {
            get;
            set;
        }

        [DataMember]
        public string SequenceNumber { get; set; }

        [DataMember]
        public string RToken
        {
            get;
            set;
        }

        [DataMember]
        public string AccountId
        {
            get;
            set;
        }

        [DataMember]
        public string MobileAppId
        {
            get;
            set;
        }
    }
}
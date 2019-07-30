using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class POSWSRequest
    {
        [DataMember]
        public string ActivationKey { get; set; }

        [DataMember]
        public string SystemMode { get; set; }

        [DataMember]
        public decimal GPSLat { get; set; }

        [DataMember]
        public decimal GPSLong { get; set; }

        [DataMember]
        public string RToken { get; set; }
    }
}
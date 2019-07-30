using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class POSInfo
    {
        [DataMember]
        public string Platform { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string Manufacturer { get; set; }

        [DataMember]
        public string Model { get; set; }

        [DataMember]
        public string OS { get; set; }

        [DataMember]
        public string IMEI { get; set; }
    }
}
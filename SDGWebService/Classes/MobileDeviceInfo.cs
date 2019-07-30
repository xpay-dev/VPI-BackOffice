using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class MobileDeviceInfo
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

        [DataMember]
        public string IP { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }

        [DataMember]
        public string PosType { get; set; }
    }
}
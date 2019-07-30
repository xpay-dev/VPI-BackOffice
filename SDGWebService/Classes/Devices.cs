using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class Devices
    {
        [DataMember]
        public string DeviceId { get; set; }

        [DataMember]
        public string SerialNumber { get; set; }

        [DataMember]
        public string MasterDeviceId { get; set; }

        [DataMember]
        public string DeviceFlowTypeId { get; set; }

        [DataMember]
        public string DeviceFlowType { get; set; }

        [DataMember]
        public string DeviceType { get; set; }
    }
}
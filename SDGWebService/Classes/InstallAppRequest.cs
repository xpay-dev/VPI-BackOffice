using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class InstallAppRequest
    {
        [DataMember]
        public string PackageName { get; set; }

        [DataMember]
        public int VersionCode { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}
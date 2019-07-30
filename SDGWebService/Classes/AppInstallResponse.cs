using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class AppInstallResponse
    {
        [DataMember]
        public string AppName { get; set; }

        [DataMember]
        public string PackageName { get; set; }

        [DataMember]
        public string VersionName { get; set; }

        [DataMember]
        public int VersionCode { get; set; }

        [DataMember]
        public string VersionBuild { get; set; }

        [DataMember]
        public POSWSResponse WSResponse { get; set; }
    }
}
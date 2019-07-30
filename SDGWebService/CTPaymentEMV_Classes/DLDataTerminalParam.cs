using System.Runtime.Serialization;

namespace SDGWebService.CTPaymentEMV_Classes
{
    [DataContract]
    public class DLDataTerminalParam
    {
        [DataMember]
        public string TC { get; set; }

        [DataMember]
        public string ATC { get; set; }

        [DataMember]
        public string TCOUC { get; set; }

        [DataMember]
        public string TT { get; set; }

        [DataMember]
        public string TCURC { get; set; }

        [DataMember]
        public string TCE { get; set; }

        [DataMember]
        public string TRCOUC { get; set; }

        [DataMember]
        public string TRCE { get; set; }

        [DataMember]
        public string TRCURC { get; set; }

        [DataMember]
        public string RFU { get; set; }
    }
}
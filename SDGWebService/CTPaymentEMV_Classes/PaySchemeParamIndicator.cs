using System.Runtime.Serialization;

namespace SDGWebService.CTPaymentEMV_Classes
{
    [DataContract]
    public class PaySchemeParamIndicator
    {
        [DataMember]
        public string VISA { get; set; }

        [DataMember]
        public string MasterCard { get; set; }

        [DataMember]
        public string AMEX { get; set; }

        [DataMember]
        public string Interac { get; set; }

        [DataMember]
        public string JCB { get; set; }

        [DataMember]
        public string Discover { get; set; }

        [DataMember]
        public string R1 { get; set; }

        [DataMember]
        public string R2 { get; set; }

        [DataMember]
        public string R3 { get; set; }

        [DataMember]
        public string R4 { get; set; }

        [DataMember]
        public string R5 { get; set; }

        [DataMember]
        public string R6 { get; set; }

        [DataMember]
        public string R7 { get; set; }

        [DataMember]
        public string R8 { get; set; }

        [DataMember]
        public string R9 { get; set; }

        [DataMember]
        public string R10 { get; set; }
    }
}
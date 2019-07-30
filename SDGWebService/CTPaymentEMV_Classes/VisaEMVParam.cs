using System.Runtime.Serialization;

namespace SDGWebService.CTPaymentEMV_Classes
{
    [DataContract]
    public class VisaEMVParam
    {
        [DataMember]
        public string AID { get; set; }

        [DataMember]
        public string ASI { get; set; }

        [DataMember]
        public string AAS { get; set; }

        [DataMember]
        public string AVN { get; set; }

        [DataMember]
        public string TACDEF { get; set; }

        [DataMember]
        public string TACDEN { get; set; }

        [DataMember]
        public string TACON { get; set; }

        [DataMember]
        public string MTPBRS { get; set; }

        [DataMember]
        public string TPRS { get; set; }

        [DataMember]
        public string TVBRS { get; set; }

        [DataMember]
        public string CTACDEF { get; set; }

        [DataMember]
        public string CTACDEN { get; set; }

        [DataMember]
        public string CTACON { get; set; }

        [DataMember]
        public string TRMD { get; set; }

        [DataMember]
        public string RFU { get; set; }
    }
}
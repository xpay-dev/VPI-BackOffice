using System.Runtime.Serialization;

namespace SDGWebService.CTPaymentEMV_Classes
{
    [DataContract]
    public class DLDataHostParam
    {
        [DataMember]
        public string TRT { get; set; }

        public string DAC { get; set; }

        public string PPN { get; set; }

        public string PIPADD { get; set; }

        public string SPN { get; set; }

        public string SIPADD { get; set; }

        public string MN { get; set; }

        public string MSADD { get; set; }

        public string MCPP { get; set; }

        public string TASV { get; set; }

        public string HDT { get; set; }

        public string ERD { get; set; }

        public string DCCVML { get; set; }

        public string VCCVML { get; set; }

        public string MCCCVML { get; set; }

        public string MTI { get; set; }

        public string F1 { get; set; }

        public string DEMVFL { get; set; }

        public string VEMVFL { get; set; }

        public string MCEMVFL { get; set; }

        public string AJEMVFL { get; set; }

        public string F2 { get; set; }

        public string IDPRS { get; set; }

        public string IDPCBS { get; set; }

        public string CCRS { get; set; }

        public string CCCBS { get; set; }

        public string RSL { get; set; }

        public string CBSL { get; set; }

        public string VDS { get; set; }

        public string AJCCVML { get; set; }

        public string ITCRRL { get; set; }

        public string PS { get; set; }

        public string DCFL { get; set; }

        public string VCFL { get; set; }

        public string MCCFL { get; set; }

        public string AJCFL { get; set; }

        public string DCTL { get; set; }

        public string VCTL { get; set; }

        public string MCCTL { get; set; }

        public string AJCTL { get; set; }

        public string TTI { get; set; }

        public string TOS { get; set; }

        public string F3 { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.Debit
{
    [DataContract]
    public class SwitchResponse
    {
        [DataMember]
        public string BalanceInqData { get; set; }

        [DataMember]
        public string AuthorizationID { get; set; }

        [DataMember]
        public string ReturnCode { get; set; }

        [DataMember]
        public string Reference { get; set; }

        [DataMember]
        public string SytemTraceAudit { get; set; }

        [DataMember]
        public string ErrNumber { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
}

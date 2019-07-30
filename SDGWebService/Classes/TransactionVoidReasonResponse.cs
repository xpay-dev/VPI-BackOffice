using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    public class TransactionVoidReasonResponse
    {
        [DataMember]
        public POSWSResponse POSWSResponse { get; set; }

        [DataMember]
        public List<TransactionVoidReason> VoidReason { get; set; }
    }
}
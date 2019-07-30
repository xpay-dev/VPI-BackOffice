using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class TransLookupResponse
    {
        [DataMember]
        public POSWSResponse POSWSResponse { get; set; }

        [DataMember]
        public List<Transaction> Transactions { get; set; }
    }
}
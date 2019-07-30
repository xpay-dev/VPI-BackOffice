
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGWebService.TLVFunctions
{
    public class ClassTLV 
    {
        public string Track2 { get; set; }
        public string CardNumber { get; set; }
        public string NameOnCard { get; set; }
        public string EmvIccData { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public List<TrackData> TrackData { get; set; }
        public List<SubField1Data> SubField1Data { get; set; }
        public List<SubField2Data> SubField2Data { get; set; }
    }
}

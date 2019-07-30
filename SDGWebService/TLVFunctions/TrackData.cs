using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGWebService.TLVFunctions
{
    public class TrackData
    {
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string ServiceCode { get; set; }
        public string DiscretionaryData { get; set; }
    }
}

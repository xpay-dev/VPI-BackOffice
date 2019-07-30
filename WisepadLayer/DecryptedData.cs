using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisepadLayer
{
    public class DecryptedData
    {
        public String cardholderName = "";
        public String track1 = "";
        public String track2 = "";
        public String track3 = "";
        public String tlv = "";

        public DecryptedData(String cardholderName, String track1, String track2, String track3, String tlv)
        {
            this.cardholderName = cardholderName;
            this.track1 = track1;
            this.track2 = track2;
            this.track3 = track3;
            this.tlv = tlv;
        }
    }
}

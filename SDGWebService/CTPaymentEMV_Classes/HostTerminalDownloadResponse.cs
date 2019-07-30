using CT_EMV_CLASSES;
using CT_EMV_CLASSES.DOWNLOAD;
using System.Runtime.Serialization;

namespace SDGWebService.CTPaymentEMV_Classes
{
    [DataContract]
    public class HostTerminalDownloadResponse
    {
        public HEADER_RESPONSE HR { get; set; }

        public PAY_SCHEME_PARAM_INDICATOR PSRI { get; set; }

        public DL_DATA_HOST_PARAMETER DDHP { get; set; }

        public DL_DATA_TERMINAL_PARAMETER DDTP { get; set; }

        public DL_DATA_EMV_PARAMETER DDEP { get; set; }

        public string ErrorNumber { get; set; }

        public string ErrorMessage { get; set; }
    }
}
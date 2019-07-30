using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.CTPaymentGateway
{
    public class ApiResponse
    {
        public string ErrorMessage { get; set; }
        public string ErrorNumber { get; set; }
        public string ResponseMessage { get; set; }
    }
}

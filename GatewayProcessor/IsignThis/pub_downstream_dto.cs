using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.IsignThis
{
    public class pub_downstream_dto
    {
        public string id { get; set; }
        public string uid { get; set; }
        public string context_uid { get; set; }
        public string secret { get; set; }
        public string mode { get; set; }
        public string expires_at { get; set; }

        public original_message original_message { get; set; }

        public List<transactions> transactions { get; set; }

        public string state { get; set; }
        public string compound_state { get; set; }

        public card_reference card_reference { get; set; }

        public identity identity { get; set; }

        public string redirect_url { get; set; }
    }

    public class original_message
    {
        public Guid original_message_ID { get; set; }
        public string merchant_id { get; set; }
        public string transaction_id { get; set; }
        public string reference { get; set; }
    }

    public class transactions
    {
        public Guid transactions_ID { get; set; }

        public Guid downstreamID { get; set; }

        public string bank_id { get; set; }
        public string error { get; set; }
        public string response_code { get; set; }
        public string success { get; set; }
        public string id { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string message_class { get; set; }
        public string datetime { get; set; }
        public string reference { get; set; }
    }

    public class dup_transactions
    {
        public string bank_id { get; set; }
        public string error { get; set; }
        public string response_code { get; set; }
        public string success { get; set; }
        public string id { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string message_class { get; set; }
        public string datetime { get; set; }
        public string reference { get; set; }
    }

    public class card_reference
    {
        public Guid card_reference_ID { get; set; }
        public string masked_pan { get; set; }
        public string card_token { get; set; }
    }

    public class identity
    {
        public Guid identity_ID { get; set; }
        public string uid { get; set; }
        public string url { get; set; }
    }
}

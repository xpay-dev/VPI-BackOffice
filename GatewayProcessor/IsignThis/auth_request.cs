using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.IsignThis
{
    public class auth_request
    {
        public Guid requestID { get; set; }
        public bool repeat { get; set; }
        public string acquirer_id { get; set; }
        public merchant merchant { get; set; }
        public cardholder cardholder { get; set; }
        public transactions transaction { get; set; }
        public client client { get; set; }
        public account account { get; set; }
        public string requested_workflow { get; set; }
    }

    public class merchant
    {
        public Guid requestID { get; set; }
        public string id { get; set; }
        public string terminal_id { get; set; }
        public string ppid { get; set; }
        public string name { get; set; }
        public string return_url { get; set; }
    }

    public class dup_merchant
    {
        public string id { get; set; }
        public string terminal_id { get; set; }
        public string ppid { get; set; }
        public string name { get; set; }
        public string return_url { get; set; }
    }

    public class cardholder
    {
        public Guid requestID { get; set; }
        public string pan { get; set; }
        public string expiration_date { get; set; }
        public string cvv { get; set; }
        public string name { get; set; }
    }

    public class client
    {
        public Guid requestID { get; set; }
        public string ip { get; set; }
        public string name { get; set; }
        public string dob { get; set; }
        public string citizen_country { get; set; }
        public string birth_country { get; set; }
        public string email { get; set; }
        public string address { get; set; }
    }

    public class account
    {
        public string identifier_type { get; set; }
        public string identifier { get; set; }
        public string secret { get; set; }
        public string full_name { get; set; }
        public string card_token { get; set; }
    }

    public class CCInfo
    {
        public string CardNo { get; set; }
        public string Cvv { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Amount { get; set; }
        public string NameOnCard { get; set; }
    }
}

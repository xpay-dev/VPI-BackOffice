using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class EmailServerModel
    {
        public int EmailServerId { get; set; }
        [StringLength(100), Required()]
        public string EmailServerName { get; set; }
        [StringLength(100), Required(), EmailAddress()]
        public string EmailAddress { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; }
        public bool DefaultCredential { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int MerchantId { get; set; }
        public bool DefaultEmailServer { get; set; }
    }
}
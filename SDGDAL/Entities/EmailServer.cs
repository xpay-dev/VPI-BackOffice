using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("EmailServers")]
    public class EmailServer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailServerId { get; set; }

        [StringLength(100), Required()]
        public string EmailServerName { get; set; }

        [StringLength(100), Required(), EmailAddress()]
        public string Email { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; }
        public bool DefaultCredential { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int PartnerId { get; set; }
        public bool IsPartnerDefaultEmail { get; set; }
    }
}
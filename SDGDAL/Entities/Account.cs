using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }

        [StringLength(50), Required(),]
        public string Username { get; set; }

        [StringLength(50), MinLength(8, ErrorMessage = "Password must contain at least 8 characters."), Required()]
        public string Password { get; set; }

        [StringLength(50), Required()]
        public string PIN { get; set; }

        public int ParentId { get; set; }
        public int ParentTypeId { get; set; }

        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int LogTries { get; set; }
        public string IPAddress { get; set; }
        public Nullable<DateTime> AccountAvailableDate { get; set; }
        public Nullable<DateTime> LastLoggedIn { get; set; }

        public DateTime PasswordExpirationDate { get; set; }
        public DateTime DateCreated { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public bool NeedsUpdate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        [StringLength(50), Required(),]
        public string Username { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$_@=|{}+!#<>])[a-zA-Z0-9$_@=|{}+!#<>]{8,50}",
            ErrorMessage = "Password must be have at least one (1) Upper case letter, one (1) lower case letter, a number and/or a special character.")]
        [StringLength(50), MinLength(8, ErrorMessage = "Password must contain at least 8 characters."), Required()]
        public string Password { get; set; }

        
        [Display(Name = "Confirm new password")]
        [Required(ErrorMessage = "Enter Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [StringLength(50)]
        public string PIN { get; set; }

        public int ParentId { get; set; }
        public int ParentTypeId { get; set; }

        public int RoleId { get; set; }

        [StringLength(100), Required(), EmailAddress]
        public string EmailAddress { get; set; }
        [StringLength(500)]
        public string PhotoUrl { get; set; }

        [StringLength(100), Required()]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [StringLength(100), Required()]
        public string LastName { get; set; }

        public bool AddressSameAsParent { get; set; }
        [StringLength(100)]
        public string Address { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        public string StateProvince { get; set; }

        public int CountryId { get; set; }
        [StringLength(10)]
        public string ZipCode { get; set; }

        [StringLength(30)]
        public string PrimaryContactNumber { get; set; } // Office?
        [StringLength(30)]
        public string Fax { get; set; }
        [StringLength(30)]
        public string MobileNumber { get; set; }

        public int ProvinceId { get; set; }
        public string ProvIsoCode { get; set; }
    }
}
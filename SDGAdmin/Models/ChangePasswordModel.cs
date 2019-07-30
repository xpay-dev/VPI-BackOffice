using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class ChangePasswordModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Enter Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter Current Password")]
        public string OldPassword { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$_@=|{}+!#<>])[a-zA-Z0-9$_@=|{}+!#<>]{8,50}",
            ErrorMessage = "New Password must be have at least one (1) Upper case letter, one (1) lower case letter, a number and/or a special character.")]
        [StringLength(50), MinLength(8, ErrorMessage = "New Password must contain at least 8 characters."), Required()]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm new password")]
        [Required(ErrorMessage = "Enter Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
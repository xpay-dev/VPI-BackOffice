using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [StringLength(100), Required(), EmailAddress]
        public string EmailAddress { get; set; }

        [StringLength(500)]
        public string PhotoUrl { get; set; }

        [StringLength(100), Required()]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [StringLength(100), Required()]
        public string LastName { get; set; }

        public int ContactInformationId { get; set; }

        [ForeignKey("ContactInformationId")]
        public virtual ContactInformation ContactInformation { get; set; }

        public decimal Price { get; set; }
    }
}
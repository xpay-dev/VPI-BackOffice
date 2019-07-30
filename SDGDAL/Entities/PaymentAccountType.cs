using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("PaymentAccountTypes")]
    public class PaymentAccountType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentAccountTypeId { get; set; }

        [StringLength(50), Required()]
        public string AccountType { get; set; }
    }
}
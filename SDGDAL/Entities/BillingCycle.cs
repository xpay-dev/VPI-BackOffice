using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("BillingCycles")]
    public class BillingCycle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillingCycleId { get; set; }

        [StringLength(50), Required()]
        public string CycleType { get; set; }
    }
}
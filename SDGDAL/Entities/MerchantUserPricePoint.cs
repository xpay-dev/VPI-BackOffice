using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MerchantUserPricePoints")]
    public class MerchantUserPricePoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PricePointId { get; set; }

        public int PricePointStart { get; set; }
        public int PricePointEnd { get; set; }
        public decimal Price { get; set; }

        public int MerchantId { get; set; }

        [ForeignKey("MerchantId")]
        public virtual Merchant Merchant { get; set; }
    }
}
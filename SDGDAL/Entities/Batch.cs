using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGDAL.Entities
{
    [Table("Batch")]
    public class Batch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchId { get; set; }

        public string BatchNumber { get; set; }

        public int PaymentTypeId { get; set; }
        [ForeignKey("PaymentTypeId")]
        public virtual PaymentType PaymentType { get; set; }

        public int MerchantId { get; set; }
        [ForeignKey("MerchantId")]
        public virtual Merchant Merchant { get; set; }

        public int MobileAppId { get; set; }
        [ForeignKey("MobileAppId")]
        public virtual MobileApp MobileApp { get; set; }

        public string Currency { get; set; }

        public decimal TotalAmount { get; set; }

        public int TotalCount { get; set; }

        public DateTime BatchDate { get; set; }
    }
}

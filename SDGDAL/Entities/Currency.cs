using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Currencies")]
    public class Currency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CurrencyId { get; set; }

        [StringLength(10), Required()]
        public string CurrencyCode { get; set; }

        [StringLength(100), Required()]
        public string CurrencyName { get; set; }

        public string IsoCode { get; set; }

        public bool IsEnabled { get; set; }
    }
}
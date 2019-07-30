using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("TaxTypes")]
    public class TaxType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaxTypeId { get; set; }

        [StringLength(50), Required()]
        public string TaxTypeName { get; set; }
    }
}
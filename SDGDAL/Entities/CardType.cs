using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("CardTypes")]
    public class CardType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CardTypeId { get; set; }

        [StringLength(100), Required()]
        public string TypeName { get; set; }

        public string IsoCode { get; set; }
    }
}
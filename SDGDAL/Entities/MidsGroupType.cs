using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MidsGroupTypes")]
    public class MidsGroupType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MidsGroupTypeId { get; set; }

        [StringLength(50), Required()]
        public string GroupType { get; set; }
    }
}
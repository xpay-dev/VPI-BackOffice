using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("SwitchParameters")]
    public class SwitchParameter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SwitchParameterId { get; set; }

        public int SwitchId { get; set; }
        public int ParameterId { get; set; }

        [StringLength(100), Required()]
        public string ParameterName { get; set; }

        public int ParameterTypeId { get; set; }

        [ForeignKey("ParameterTypeId")]
        public virtual SwitchParameterType ParameterType { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("DeviceFlowTypes")]
    public class DeviceFlowType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeviceFlowTypeId { get; set; }

        [StringLength(50), Required()]
        public string FlowType { get; set; }
    }
}
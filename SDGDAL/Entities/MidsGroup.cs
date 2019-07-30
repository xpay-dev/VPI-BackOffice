using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MidsGroups")]
    public class MidsGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MidsGroupId { get; set; }

        public string GroupName { get; set; }

        public int MidsGroupTypeId { get; set; }

        [ForeignKey("MidsGroupTypeId")]
        public virtual MidsGroupType MidsGroupType { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
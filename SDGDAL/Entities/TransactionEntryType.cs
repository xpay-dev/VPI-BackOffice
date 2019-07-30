using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("TransactionEntryTypes")]
    public class TransactionEntryType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionEntryTypeId { get; set; }

        [StringLength(100)]
        public string EntryType { get; set; }
    }
}
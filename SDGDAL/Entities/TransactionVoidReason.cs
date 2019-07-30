using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("TransactionVoidReason")]
    public class TransactionVoidReason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionVoidReasonId { get; set; }

        public string VoidReason { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("TransactionSignature")]
    public class TransactionSignature
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionSignatureId { get; set; }

        public string Path { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string FileData { get; set; }
    }
}
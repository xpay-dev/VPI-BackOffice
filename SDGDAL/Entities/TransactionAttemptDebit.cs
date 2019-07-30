using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("TransactionAttemptDebit")]
    public class TransactionAttemptDebit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionAttemptDebitId { get; set; }

        public int TransactionTypeId { get; set; }

        public int TransactionDebitId { get; set; }

        [ForeignKey("TransactionDebitId")]
        public virtual TransactionDebit TransactionDebit { get; set; }

        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public int DeviceId { get; set; }

        [ForeignKey("DeviceId")]
        public virtual Device Device { get; set; }

        public int MobileAppId { get; set; }

        [ForeignKey("MobileAppId")]
        public virtual MobileApp MobileApp { get; set; }

        public int TransactionChargesId { get; set; }

        [ForeignKey("TransactionChargesId")]
        public virtual TransactionCharges TransactionCharges { get; set; }

        public DateTime DateSent { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime DepositDate { get; set; }

        public decimal Amount { get; set; }

        [StringLength(50)]
        public string ReturnCode { get; set; }

        [StringLength(50)]
        public string AuthNumber { get; set; }

        [StringLength(50)]
        public string TraceNumber { get; set; }

        [StringLength(50)]
        public string RetrievalRefNumber { get; set; }

        [StringLength(50)]
        public string BatchNumber { get; set; }

        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [StringLength(50)]
        public string SeqNumber { get; set; }

        [StringLength(50)]
        public string ReferenceNumber { get; set; }

        [StringLength(50)]
        public string DisplayReceipt { get; set; }

        [StringLength(50)]
        public string DisplayTerminal { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public decimal GPSLat { get; set; }
        public decimal GPSLong { get; set; }

        public Nullable<int> TransactionSignatureId { get; set; }

        [ForeignKey("TransactionSignatureId")]
        public virtual TransactionSignature TransactionSignature { get; set; }
    }
}
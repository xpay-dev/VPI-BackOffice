using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("TransactionCash")]
    public class TransactionCash
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionCashId { get; set; }

        public int MerchantPOSId { get; set; }

        [ForeignKey("MerchantPOSId")]
        public virtual MerchantBranchPOS MerchantPOS { get; set; }

        public int TransactionEntryTypeId { get; set; }

        [ForeignKey("TransactionEntryTypeId")]
        public virtual TransactionEntryType TransactionEntryType { get; set; }

        public string Notes { get; set; }

        public string RefNumSales { get; set; }

        public string RefNumApp { get; set; }

        public decimal OriginalAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal FinalAmount { get; set; }

        public int CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }

        public int MidId { get; set; }

        [ForeignKey("MidId")]
        public virtual Mid Mid { get; set; }

        public DateTime DateCreated { get; set; }

        public ICollection<TransactionAttemptCash> TransactionAttemptCash { get; set; }
    }
}
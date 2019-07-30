using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("TransactionDebit")]
    public class TransactionDebit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionDebitId { get; set; }

        public int MerchantPOSId { get; set; }

        [ForeignKey("MerchantPOSId")]
        public virtual MerchantBranchPOS MerchantPOS { get; set; }

        public int TransactionEntryTypeId { get; set; }

        [ForeignKey("TransactionEntryTypeId")]
        public virtual TransactionEntryType TransactionEntryType { get; set; }

        public int AccountTypeId { get; set; }

        [ForeignKey("AccountTypeId")]
        public virtual AccountType AccountType { get; set; }

        public string CardNumber { get; set; }
        public string NameOnCard { get; set; }
        public string ExpYear { get; set; }
        public string ExpMonth { get; set; }
        public string EPB { get; set; }

        public string Notes { get; set; }
        public string RefNumSales { get; set; }
        public string RefNumApp { get; set; }

        public decimal OriginalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal FinalAmount { get; set; }

        public int CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }

        public int KeyId { get; set; }

        [ForeignKey("KeyId")]
        public virtual _Key Key { get; set; }

        public int MidId { get; set; }

        [ForeignKey("MidId")]
        public virtual Mid Mid { get; set; }

        public DateTime DateCreated { get; set; }

        public ICollection<TransactionAttemptDebit> TransactionAttemptDebit { get; set; }
    }
}
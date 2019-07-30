using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("TempTransactions")]
    public class TempTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        public int MerchantPOSId { get; set; }

        [ForeignKey("MerchantPOSId")]
        public virtual MerchantBranchPOS MerchantPOS { get; set; }

        public int TransactionEntryTypeId { get; set; }

        [ForeignKey("TransactionEntryTypeId")]
        public virtual TransactionEntryType TransactionEntryType { get; set; }

        public int CardTypeId { get; set; }

        [ForeignKey("CardTypeId")]
        public virtual CardType CardType { get; set; }

        public string CardNumber { get; set; }
        public string NameOnCard { get; set; }
        public string ExpYear { get; set; }
        public string ExpMonth { get; set; }
        public string CSC { get; set; }
        public string Track1 { get; set; }
        public string Track2 { get; set; }
        public string Track3 { get; set; }

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
    }
}
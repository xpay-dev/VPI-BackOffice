using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGBackOffice.Models
{
    public class TransactionModel
    {
        public string Amount { get; set; }
        public int AccountId { get; set; }
        public int MobileAppId { get; set; }
        public int MerchantPosId { get; set; }
        public int CardTypeId { get; set; }
        public int MidId { get; set; }
        public int TransactionChargesId { get; set; }
        public string Currency { get; set; }
        public string CurrencyId { get; set; }
        public string ErrNumber { get; set; }
        public string ErrMessage { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string GoodsTitle { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }

        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Cvv { get; set; }

        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public string SecureHash { get; set; }
        public string SignType { get; set; }

        public string MerchantName { get; set; }
        public string IssuingBank { get; set; }
        public string MerchantTradeId { get; set; }
        public string NotifyUrl { get; set; }
        public string paymentCode { get; set; }
        public string PayIp { get; set; }

        public string InvoiceNumber { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionId { get; set; }
        public string TransactionAttemptId { get; set; }
        public string Note { get; set; }
        public string ReasonId { get; set; }
    }
}
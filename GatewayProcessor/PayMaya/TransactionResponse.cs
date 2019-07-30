using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.PayMaya
{
    public class TransactionResponse
    {
        public string paymentTransactionReferenceNo { get; set; }
        public string status { get; set; }
        public string createdAt { get; set; }
        public Receipt receipt { get; set; }
        public PayerResult payer { get; set; }
        public AmountResult amount { get; set; }
    }

    public class Receipt
    {
        public string approvalCode { get; set; }
        public string batchNo { get; set; }
    }

    public class PayerResult
    {
        public FundingInstrumentResult fundingInstrument { get; set; }
    }

    public class FundingInstrumentResult
    {
        public CardResult card { get; set; }
    }

    public class CardResult
    {
        public string cardNumber { get; set; }
        public string expiryMonth { get; set; }
        public string expiryYear { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string csc { get; set; }
    }

    public class AmountResult
    {
        public TotalResult total { get; set; }
    }

    public class TotalResult
    {
        public string currency { get; set; }
        public string value { get; set; }
    }
}

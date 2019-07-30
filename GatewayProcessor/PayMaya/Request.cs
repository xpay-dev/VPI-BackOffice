using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.PayMaya
{
    public class Request
    {
        public Merchant merchant { get; set; }
        public Payer payer { get; set; }
        public Transaction transaction { get; set; }
    }

    public class Merchant
    {
        public string id { get; set; }
        public PaymentFacilitator paymentFacilitator { get; set; }
        public AcquiringTerminal acquiringTerminal { get; set; }
    }

    public class PaymentFacilitator
    {

    }

    public class AcquiringTerminal
    {
        public string id { get; set; }
        public InputCapability inputCapability { get; set; }
        public bool onMerchantPremise { get; set; }
        public bool terminalAttended { get; set; }
        public bool cardCaptureSupported { get; set; }
        public bool cardholderActivatedTerminal { get; set; }
    }

    public class InputCapability
    {
        public bool keyEntry { get; set; }
        public bool magstripeReader { get; set; }
        public bool emvChip { get; set; }
        public bool contactless { get; set; }
        public bool contactlessMagstripe { get; set; }
    }

    public class Payer
    {
        public FundingInstrument fundingInstrument { get; set; }
    }

    public class FundingInstrument
    {
        public Card card { get; set; }
    }

    public class Card
    {
        public string cardNumber { get; set; }
        public string expiryMonth { get; set; }
        public string expiryYear { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public CardPresentFields cardPresentFields { get; set; }
    }

    public class CardPresentFields
    {
        public string cardSeqNum { get; set; }
        public string emvIccData { get; set; }
        public string track1 { get; set; }
        public string track2 { get; set; }
    }

    public class Transaction
    {
        public Amount amount { get; set; }
        public string description { get; set; }
    }

    public class Amount
    {
        public Total total { get; set; }
    }

    public class Total
    {
        public string currency { get; set; }
        public string value { get; set; }
    }
}

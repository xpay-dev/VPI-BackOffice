using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.PayMaya {
    public class TransactionData {
        public TotalAmount totalAmount {
            get; set;
        }
        public Buyer buyer {
            get; set;
        }
        public List<Item> items {
            get; set;
        }
        public RedirectURL redirectUrl {
            get; set;
        }
        public Merchant merchant {
            get; set;
        }
        public string requestReferenceNumber {
            get; set;
        }
        public string status {
            get; set;
        }
        public string id {
            get; set;
        }
        public string transactionReferenceNumber {
            get; set;
        }
        public string receiptNumber {
            get; set;
        }
        public string createdAt {
            get; set;
        }
        public string updatedAt {
            get;set;
        }
        public string expiredAt {
            get; set;
        }
        public string paymentType {
            get; set;
        }
        public string paymentStatus {
            get; set;
        }
        public string voidStatus {
            get; set;
        }
    }
    public class TotalAmount {
        public string currency {
            get; set;
        }
        public string value {
            get; set;
        }
        public TotalAmountDetails details {
            get; set;
        }
    }
    public class TotalAmountDetails {
        public string discount {
            get; set;
        }
        public string serviceCharge {
            get; set;
        }
        public string shippingFee {
            get; set;
        }
        public string tax {
            get; set;
        }
        public string subtotal {
            get; set;
        }
    }
    public class Buyer {
        public string firstName {
            get; set;
        }
        public string middleName {
            get; set;
        }
        public string lastName {
            get; set;
        }
        public BuyerContact contact {
            get; set;
        }
        public BuyerAddress shippingAddress {
            get; set;
        }
        public BuyerAddress billingAddress {
            get; set;
        }
        public string ipAddress {
            get; set;
        }
    }
    public class BuyerContact {
        public string phone {
            get; set;
        }
        public string email {
            get; set;
        }
    }
    public class BuyerAddress {
        public string line1 {
            get; set;
        }
        public string line2 {
            get; set;
        }
        public string city {
            get; set;
        }
        public string state {
            get; set;
        }
        public string zipCode {
            get; set;
        }
        public string countryCode {
            get; set;
        }
    }
    public class Item {
        public string name {
            get; set;
        }
        public string code {
            get; set;
        }
        public string description {
            get; set;
        }
        public string quantity {
            get; set;
        }
        public ItemAmount amount {
            get; set;
        }
        public ItemAmount totalAmount {
            get; set;
        }
    }
    public class ItemAmount {
        public string value {
            get; set;
        }
        public ItemAmountDetails details {
            get; set;
        }
    }
    public class ItemAmountDetails {
        public string discount {
            get; set;
        }
        public string subtotal {
            get; set;
        }
    }
    public class RedirectURL {
        public string success {
            get; set;
        }
        public string failure {
            get; set;
        }
        public string cancel {
            get; set;
        }
    }
    public class Merchant {
        public string currency {
            get; set;
        }
        public string name {
            get; set;
        }
        public string email {
            get; set;
        }
        public string locale {
            get; set;
        }
        public string homepageUrl {
            get; set;
        }
        public bool isEmailToMerchantEnabled {
            get; set;
        }
        public bool isEmailToBuyerEnabled {
            get; set;
        }
        public bool isPaymentFacilitator {
            get; set;
        }
        public bool isPageCustomized {
            get; set;
        }
        public string[] supportedSchemes {
            get; set;
        }
    }
    public class InitializedCheckOut {
        public string checkoutId {
            get; set;
        }
        public string redirectUrl {
            get; set;
        }
        public InitialCheckOutErrorDetails error {
            get; set;
        }
    }
    public class InitialCheckOutErrorDetails {
        public string code {
            get; set;
        }
        public string message {
            get; set;
        }
        public string parameters {
            get; set;
        }
        public string description {
            get; set;
        }
    }
    public class PaymentData {
        public string checkoutId {
            get; set;
        }
        public string cardNumber {
            get; set;
        }
        public string cardType {
            get; set;
        }
        public PaymentDataCardExpiry cardExpiry {
            get; set;
        }
        public string cardCsc {
            get; set;
        }
        public PaymentDataCardHolder cardHolder {
            get; set;
        }
    }
    public class PaymentDataCardExpiry {
        public string month {
            get; set;
        }
        public string year {
            get; set;
        }
    }
    public class PaymentDataCardHolder {
        public string firstName {
            get; set;
        }
        public string lastName {
            get; set;
        }
        public string email {
            get; set;
        }
        public string phone {
            get; set;
        }
        public PaymenyDataCardHolderBillingAddress billingAddress {
            get; set;
        }
    }
    public class PaymenyDataCardHolderBillingAddress {
        public string line1 {
            get; set;
        }
        public string line2 {
            get; set;
        }
        public string city {
            get; set;
        }
        public string state {
            get; set;
        }
        public string countryCode {
            get; set;
        }
        public string zipCode {
            get; set;
        }
    }
}

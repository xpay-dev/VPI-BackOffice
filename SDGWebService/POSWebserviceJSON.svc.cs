using SDGWebService.Classes;
using SDGWebService.WebserviceFunctions;
using System;
using System.ServiceModel;

namespace SDGWebService {
     [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
     public class POSWebserviceJSON : IPOSWebserviceJSON {
          public POSWSResponse Test(string i) {
               return new POSWSResponse() {
                    ErrNumber = i.ToString(),
                    Message = "Working"
               };
          }

          private WebserviceFunction wsFunctions = new WebserviceFunction();
          private WebserviceFunctionCreditSwipe wsFunctionsCreditSwipe = new WebserviceFunctionCreditSwipe();
          private WebserviceFunctionsEMV wsFunctionsCreditEMV = new WebserviceFunctionsEMV();

          public POSWSResponse ActivateApp(MobileDeviceInfo posInfo) //NewActivateApp
          {
               return wsFunctions.ActivateApp(posInfo);
          }

          public POSFeaturesResponse UpdateApp(POSWSRequest request) {
               return wsFunctions.UpdateApp(request);
          }

          public POSWSResponse UpdateAppCompleted(POSWSRequest request) {
               return wsFunctions.UpdateAppCompleted(request);
          }

          public AppInstallResponse InstallApp(InstallAppRequest request) // NewInstallApp
          {
               return wsFunctions.InstallApp(request);
          }

          public POSWSResponse Login(LoginRequest request) {
               return wsFunctions.Login(request);
          }
          public POSWSResponse LoginByMobilePin(LoginRequest request) {
               return wsFunctions.LoginByMobilePin(request);
          }

          public POSFeaturesResponse IsAlive(POSWSRequest request, string appVersion) {
               return wsFunctions.IsAlive(request, appVersion);
          }

          public TransLookupResponse TransLookUp(TransLookupRequest request) {
               //throw new NotImplementedException();
               return wsFunctions.TransLookUp(request);
          }

          public POSWSResponse SendReceipt() {
               throw new NotImplementedException();
               //return wsFunctions.SendReceipt();
          }

          public POSWSResponse SendSMSReceipt() {
               throw new NotImplementedException();
               //return wsFunctions.SendSMSReceipt();
          }

          public POSWSResponse EmailReceipt(EmailReceiptRequest request) {
               return wsFunctions.EmailReceipt(request);
          }

          public POSWSResponse PrintReceipt() {
               throw new NotImplementedException();
          }

          public POSWSResponse CreateTicket(CreateTicketRequest request) {
               throw new NotImplementedException();
          }

          // TODO: Change device to deviceid. Make sure to pass correct parameters - Ralph
          public PurchaseResponse TransactionPurchaseCreditSwipe(TransactionRequest request) // TransPurchase_Credit_Swipe
          {
               return wsFunctionsCreditSwipe.TransactionPurchaseCreditSwipe(request);
          }

          public POSWSResponse CreditTransactionVoidRefundSwipe(TransactionRequest request) // NewCreditTransVoidRefund_Swipe
          {
               throw new NotImplementedException();
               //return wsFunctions.CreditTransactionVoidRefundSwipe(request);
          }

          // TODO: Change device to deviceid. Make sure to pass correct parameters - Ralph
          public PurchaseResponse TransactionPurchaseCreditEMV(TransactionRequest request) // TransPurchase_Credit_Swipe
          {
               return wsFunctionsCreditEMV.TransactionPurchaseCreditEMV(request);
          }

          public POSWSResponse ForgotMobilePin(POSWSRequest POSWSRequest, string UserEmail, int AccountId) {
               return wsFunctions.ForgotMobilePin(POSWSRequest, UserEmail, AccountId);
          }
          
          public TransactionVoidReasonResponse TransactionVoidReason(POSWSRequest request) {
               return wsFunctions.TransactionVoidReason(request);
          }

          public POSWSResponse TransSignatureCapture(TransactionSignatureRequest request) {
               return wsFunctions.TransSignatureCapture(request);
          }
          
          public CreditVoidRefundResponse CreditTransactionVoidRefund(TransactionVoidRefundRequest VoidRequest) {
               return wsFunctionsCreditSwipe.CreditTransactionVoidRefund(VoidRequest);
          }
     }
}
using CT_EMV_CLASSES.DOWNLOAD;
using SDGWebService.Classes;
using SDGWebService.WebserviceFunctions;
using System;
using System.IO;

namespace SDGWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "POSWebService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select POSWebService.svc or POSWebService.svc.cs at the Solution Explorer and start debugging.
    public class POSWebService : IPOSWebService
    {
        private WebserviceFunction wsFunctions = new WebserviceFunction();
        private WebserviceFunctionCreditSwipe wsFunctionsCreditSwipe = new WebserviceFunctionCreditSwipe();
        private WebserviceFunctionsOffline wsFunctionsOffline = new WebserviceFunctionsOffline();
        private WebserviceFunctionsEMV wsFunctionsCTEMV = new WebserviceFunctionsEMV();
        private WebserviceFunctionsDebit wsFunctionsDebit = new WebserviceFunctionsDebit();
        private WebserviceFunctionsCash wsFunctionsCash = new WebserviceFunctionsCash();
        private WebserviceFunctionsCreditManual wsFunctionsCreditManual = new WebserviceFunctionsCreditManual();

        public POSWSResponse ActivateApp(MobileDeviceInfo posInfo)
        {
            return wsFunctions.ActivateApp(posInfo);
        }

        public POSFeaturesResponse UpdateApp(POSWSRequest request)
        {
            return wsFunctions.UpdateApp(request);
        }

        public POSWSResponse UpdateAppCompleted(POSWSRequest request)
        {
            return wsFunctions.UpdateAppCompleted(request);
        }

        public AppInstallResponse InstallApp(InstallAppRequest request)
        {
            return wsFunctions.InstallApp(request);
        }

        public RegistrationResponse Registration(RegistrationRequest request)
        {
            return wsFunctions.Registration(request);
        }

        public POSWSResponse ChangePassword(ChangePasswordRequest request)
        {
            return wsFunctions.ChangePassword(request);
        }

        public POSWSResponse ResendVerificationCode(ResendVerificationRequest request)
        {
            return wsFunctions.ResendVerificationCode(request);
        }

        public POSWSResponse ForgotMobilePin(POSWSRequest POSWSRequest, string UserEmail, int AccountId)
        {
            return wsFunctions.ForgotMobilePin(POSWSRequest, UserEmail, AccountId);
        }

        public POSWSResponse Login(LoginRequest request)
        {
            return wsFunctions.LoginByMobilePin(request);
        }

        public POSFeaturesResponse IsAlive(POSWSRequest request, string appVersion)
        {
            return wsFunctions.IsAlive(request, appVersion);
        }

        public TransLookupResponse TransLookUp(TransLookupRequest request)
        {
            return wsFunctions.TransLookUp(request);
        }

        public POSWSResponse GiftReceipt(EmailReceiptRequest request)
        {
            return wsFunctions.GiftReceipt(request);
        }

        public POSWSResponse EmailReceipt(EmailReceiptRequest request)
        {
            return wsFunctions.EmailReceipt(request);
        }

        public POSWSResponse CreateTicket(CreateTicketRequest request)
        {
            return wsFunctions.CreateTicket(request);
        }

        public PurchaseResponse TransactionPurchaseCreditSwipe(TransactionRequest request)
        {
            return wsFunctionsCreditSwipe.TransactionPurchaseCreditSwipe(request);
        }

        public CreditVoidRefundResponse CreditTransactionVoidRefund(TransactionVoidRefundRequest VoidRequest)
        {
            return wsFunctionsCreditSwipe.CreditTransactionVoidRefund(VoidRequest);
        }

        public POSWSResponse TransSignatureCapture(TransactionSignatureRequest request)
        {
            return wsFunctions.TransSignatureCapture(request);
        }

        public TransactionVoidReasonResponse TransactionVoidReason(POSWSRequest request)
        {
            return wsFunctions.TransactionVoidReason(request);
        }

        public HOST_TERMINAL_DOWNLOAD EMVDownloadHostAndTerminal(POSWSRequest POSWSRequest, string terminalId)
        {
            return wsFunctionsCTEMV.EMVDownloadHostAndTerminal(POSWSRequest, terminalId);
        }

        public PurchaseResponse TransactionPurchaseCreditEMV(TransactionRequest request)
        {
            return wsFunctionsCTEMV.TransactionPurchaseCreditEMV(request);
        }

        public PurchaseResponse TestCTEmvCreditPurchase(string iccData, string amount)
        {
            return wsFunctionsOffline.TestCTEmvCreditPurchase(iccData, amount);
        }

        public PurchaseResponse TestCTSwipePurchase(string track1, string track2, string amount)
        {
            return wsFunctionsOffline.TestCTSwipePurchase(track1, track2, amount);
        }

        public PurchaseResponse TransactionPurchaseDebit(TransactionRequest request)
        {
            return wsFunctionsDebit.TransactionPurchaseDebit(request);
        }

        public DebitBalanceInquiryResponse DebitBalanceInquiry(TransactionRequest request)
        {
            return wsFunctionsDebit.DebitBalanceInquiry(request);
        }

        public PurchaseResponse TransactionPurchaseCash(CashTransactionRequest request)
        {
            return wsFunctionsCash.TransactionPurchaseCash(request);
        }

        public CashVoidRefundResponse CashTransactionVoidRefund(TransactionVoidRefundRequest request)
        {
            return wsFunctionsCash.CashTransactionVoidRefund(request);
        }

        public PurchaseResponse TransactionPurchaseCreditManual(CreditManualRequest request)
        {
            return wsFunctionsCreditManual.TransactionPurchaseCreditManual(request);
        }

        public BatchResponse BatchReport(BatchReportsRequest request)
        {
            return wsFunctionsDebit.BatchReport(request);
        }

        public POSWSResponse BatchClose(BatchCloseRequest request)
        {
            return wsFunctionsDebit.BatchClose(request);
        }

        public ReversalResponse ReversalTransactionDebit(ReversalRequest request)
        {
            return wsFunctionsDebit.ReversalTransactionDebit(request);
        }

        public ReversalResponse ReversalTransactionCredit(ReversalRequest request)
        {
            return wsFunctionsCreditSwipe.ReversalTransactionCredit(request);
        }

        public Response MerchantPushPayments(Request request)
        {
           return wsFunctions.MerchantPushPayments(request);
        }
    }
}
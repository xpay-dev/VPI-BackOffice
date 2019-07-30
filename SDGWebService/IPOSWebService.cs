using CT_EMV_CLASSES.DOWNLOAD;
using SDGWebService.Classes;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SDGWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPOSWebService" in both code and config file together.

    [ServiceContract(Namespace = "http://svc.sdgroup.com")]
    public interface IPOSWebService
    {   
        [OperationContract]
        POSWSResponse ActivateApp(MobileDeviceInfo posInfo);

        [OperationContract]
        POSFeaturesResponse UpdateApp(POSWSRequest request);

        [OperationContract]
        POSWSResponse UpdateAppCompleted(POSWSRequest request);

        [OperationContract]
        AppInstallResponse InstallApp(InstallAppRequest request);

        [OperationContract]
        RegistrationResponse Registration(RegistrationRequest request);

        [OperationContract]
        POSWSResponse ChangePassword(ChangePasswordRequest request);

        [OperationContract]
        POSWSResponse ResendVerificationCode(ResendVerificationRequest request);

        [OperationContract]
        POSWSResponse ForgotMobilePin(POSWSRequest POSWSRequest, string UserEmail, int AccountId);

        [OperationContract]
        POSWSResponse Login(LoginRequest request);

        [OperationContract]
        POSFeaturesResponse IsAlive(POSWSRequest request, string appVersion);

        [OperationContract]
        TransLookupResponse TransLookUp(TransLookupRequest request);

        [OperationContract]
        POSWSResponse EmailReceipt(EmailReceiptRequest request);

        [OperationContract]
        POSWSResponse GiftReceipt(EmailReceiptRequest request);

        [OperationContract]
        POSWSResponse CreateTicket(CreateTicketRequest request);

        [OperationContract]
        PurchaseResponse TransactionPurchaseCreditSwipe(TransactionRequest request);

        [OperationContract]
        CreditVoidRefundResponse CreditTransactionVoidRefund(TransactionVoidRefundRequest VoidRefundRequest);

        [OperationContract]
        POSWSResponse TransSignatureCapture(TransactionSignatureRequest request);

        [OperationContract]
        TransactionVoidReasonResponse TransactionVoidReason(POSWSRequest request);

        [OperationContract]
        HOST_TERMINAL_DOWNLOAD EMVDownloadHostAndTerminal(POSWSRequest POSWSRequest, string terminald);

        [OperationContract]
        PurchaseResponse TransactionPurchaseCreditEMV(TransactionRequest request);

        [OperationContract]
        PurchaseResponse TestCTEmvCreditPurchase(string iccData, string amount);

        [OperationContract]
        PurchaseResponse TestCTSwipePurchase(string track1, string track2, string amount);

        [OperationContract]
        PurchaseResponse TransactionPurchaseDebit(TransactionRequest request);

        [OperationContract]
        DebitBalanceInquiryResponse DebitBalanceInquiry(TransactionRequest request);

        [OperationContract]
        PurchaseResponse TransactionPurchaseCash(CashTransactionRequest request);

        [OperationContract]
        CashVoidRefundResponse CashTransactionVoidRefund(TransactionVoidRefundRequest request);

        [OperationContract]
        PurchaseResponse TransactionPurchaseCreditManual(CreditManualRequest request);
        
        [OperationContract]
        BatchResponse BatchReport(BatchReportsRequest request);

        [OperationContract]
        POSWSResponse BatchClose(BatchCloseRequest request);

        [OperationContract]
        ReversalResponse ReversalTransactionDebit(ReversalRequest request);

        [OperationContract]
        ReversalResponse ReversalTransactionCredit(ReversalRequest request);

        [OperationContract]
        Response MerchantPushPayments(Request request);
    }
}
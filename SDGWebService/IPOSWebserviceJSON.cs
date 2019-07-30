using SDGWebService.Classes;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SDGWebService
{
    [ServiceContract]
    public interface IPOSWebserviceJSON
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "Test", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSWSResponse Test(string i);

        /// <summary>
        ///     Activates the app using the Activation Code
        /// </summary>
        /// <param name="request"></param>
        /// <param name="posInfo"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "ActivateApp", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSWSResponse ActivateApp(MobileDeviceInfo posInfo);

        [OperationContract]
        [WebInvoke(UriTemplate = "UpdateApp", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSFeaturesResponse UpdateApp(POSWSRequest request);

        [OperationContract]
        [WebInvoke(UriTemplate = "UpdateAppCompleted", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSWSResponse UpdateAppCompleted(POSWSRequest request);

        /// <summary>
        ///     This is the NewInstallApp from the old web service
        /// </summary>
        /// <param name="request"></param>
        /// <param name="appStartLang"></param>
        /// <param name="appVersion"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "InstallApp", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        AppInstallResponse InstallApp(InstallAppRequest request);

        [OperationContract]
        [WebInvoke(UriTemplate = "Login", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSWSResponse Login(LoginRequest request);

        [OperationContract]
        [WebInvoke(UriTemplate = "IsAlive", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSFeaturesResponse IsAlive(POSWSRequest request, string appVersion);

        [OperationContract]
        [WebInvoke(UriTemplate = "TransLookUp", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        TransLookupResponse TransLookUp(TransLookupRequest request);

        [OperationContract]
        [WebInvoke(UriTemplate = "SendReceipt", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSWSResponse SendReceipt();

        [OperationContract]
        [WebInvoke(UriTemplate = "SendSMSReceipt", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSWSResponse SendSMSReceipt();

        [OperationContract]
        [WebInvoke(UriTemplate = "EmailReceipt", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSWSResponse EmailReceipt(EmailReceiptRequest request);

        [OperationContract]
        [WebInvoke(UriTemplate = "PrintReceipt", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSWSResponse PrintReceipt();

        [OperationContract]
        [WebInvoke(UriTemplate = "CreateTicket", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSWSResponse CreateTicket(CreateTicketRequest request);

        [OperationContract]
        [WebInvoke(UriTemplate = "TransactionPurchaseCreditSwipe", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        PurchaseResponse TransactionPurchaseCreditSwipe(TransactionRequest request); // TransPurchase_Credit_Swipe

        [OperationContract]
        [WebInvoke(UriTemplate = "CreditTransactionVoidRefundSwipe", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        POSWSResponse CreditTransactionVoidRefundSwipe(TransactionRequest request); // NewCreditTransVoidRefund_Swipe
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;
using CTPaymentUtilities;
using SDGUtil;
using CTPayment.Classes;

namespace CTPayment {
     public class ApiCTPayment
    {
        #region Process Merchant Bulk Boarding Module
        public async Task<MerchantBulkResponse> ProcessCreditCard(Dictionary<string, CreditCardRequest> request)
        {
            Dictionary<string, Request> sendRequest = new Dictionary<string, Request>();
            MerchantBulkResponse response = new MerchantBulkResponse();

            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 3, 00);
            httpClient.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["CTPaymentURI"].ToString());

            HttpResponseMessage httpResponse = httpClient.PostAsJsonAsync("hpa/api/merchant/creditcard", request).Result;

            response = await httpResponse.Content.ReadAsAsync<MerchantBulkResponse>();

            return response;
        }

        public async Task<MerchantBulkResponse> ProcessDebitCard(Dictionary<string, DebitCardRequest> request)
        {
            Dictionary<string, Request> sendRequest = new Dictionary<string, Request>();
            MerchantBulkResponse response = new MerchantBulkResponse();

            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 3, 00);
            httpClient.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["CTPaymentURI"].ToString());

            HttpResponseMessage httpResponse = httpClient.PostAsJsonAsync("hpa/api/merchant/debitcard", request).Result;

            response = await httpResponse.Content.ReadAsAsync<MerchantBulkResponse>();

            return response;
        }

        public async Task<List<string>> GetFileNames()
        {
            List<string> response = new List<string>();
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 3, 00);
            httpClient.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["CTPaymentURI"].ToString());

            HttpResponseMessage httpResponse = httpClient.GetAsync("hpa/api/getFileNames").Result;

            var stringContent = await httpResponse.Content.ReadAsAsync<List<string>>();

            return stringContent;
        }

        public async Task<MerchantBulkResponse> GetFileDetails(string filename)
        {
            MerchantBulkResponse response = new MerchantBulkResponse();
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 3, 00);
            httpClient.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["CTPaymentURI"].ToString());

            HttpResponseMessage httpResponse = httpClient.GetAsync("hpa/api/getFileDetails?filename=" + filename).Result;
           
            response = await httpResponse.Content.ReadAsAsync<MerchantBulkResponse>();

            string output = httpResponse.Content.ReadAsStringAsync().Result;

            return response;
        }

        public string ProcessMerchantBulk(MerchRequestandMiscTextRequest request)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = new TimeSpan(0, 5, 00);
                httpClient.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["CTPaymentURI"].ToString());

                HttpResponseMessage httpResponse = httpClient.PostAsJsonAsync("hpa/api/merchant/bulk/load/", request).Result;
                
                string content = httpResponse.Content.ReadAsAsync<string>().Result;

                return content;
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("CTPayment", ex.Message, "ProcessMerchantBulk", ex.StackTrace);

                return null;
            }
        }

        #endregion

        #region Process Add user for CTPayment
        public async Task<RecurOutput> ProcessCTPaymentAddUser(RecurAddUserInput input)
        {
            RecurOutput output = new RecurOutput();

            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 3, 00);
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["CTPaymentURL"].ToString());

                HttpResponseMessage httpResponse = client.PostAsJsonAsync("hpa/ctpayment/AddUser/", input).Result;

                output = await httpResponse.Content.ReadAsAsync<RecurOutput>();
            }
            catch(Exception ex)
            {
                ApplicationLog.LogError("CTPayment", ex.Message, "ProcessCTPaymentAddUser", ex.StackTrace);
            }

            return output;
        }

        public async Task<RecurOutput> ProcessCTPaymentRedirectUser(RecurRedirSessionInput input)
        {
            RecurOutput output = new RecurOutput();

            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 3, 00);
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["CTPaymentURL"].ToString());

                HttpResponseMessage httpResponse = client.PostAsJsonAsync("hpa/ctpayment/redirect/", input).Result;

                output = await httpResponse.Content.ReadAsAsync<RecurOutput>();
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("CTPayment", ex.Message, "ProcessCTPaymentRedirectUser", ex.StackTrace);
            }

            return output;
        }

        public async Task<RecurOutput> ProcessCTPaymentResponseRedirectUser(RecurRedirSessionResponseInput input)
        {
            RecurOutput output = new RecurOutput();

            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 3, 00);
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["CTPaymentURL"].ToString());

                HttpResponseMessage httpResponse = client.PostAsJsonAsync("hpa/ctpayment/response/redirect/", input).Result;

                output = await httpResponse.Content.ReadAsAsync<RecurOutput>();
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("CTPayment", ex.Message, "ProcessCTPaymentResponseRedirectUser", ex.StackTrace);
            }

            return output;
        }

        public async Task<AckOutput> AcceptCTAcknowledge(string id)
        {
            AckOutput response = new AckOutput();

            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 3, 00);
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["CTPaymentURL"].ToString());

                HttpResponseMessage httpResponse = client.PostAsJsonAsync("hpa/ctpayment/AddUser/Acknowledge/", id).Result;

                response = await httpResponse.Content.ReadAsAsync<AckOutput>();
            }
            catch (Exception ex)
            {
                ApplicationLog.LogError("CTPayment", ex.Message, "AcceptCTAcknowledge", ex.StackTrace);
            }

            return response;
        }

        public string stAPI(MerchRequestandMiscTextRequest request)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = new TimeSpan(0, 3, 00);
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["CTPaymentURI"].ToString());

                HttpResponseMessage httpResponse = httpClient.PostAsJsonAsync("hpa/api/merchant/bulk/load/", request).Result;

                return "";
            }
            catch(Exception ex)
            {
                ApplicationLog.LogError("CTPayment", ex.Message, "AcceptCTAcknowledge", ex.StackTrace);
                return null;
            }
        }
        #endregion
    }
}

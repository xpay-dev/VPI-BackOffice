using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Xml;

namespace CTPayment
{
    public class SendMerchantBulkRequest
    {
        public async Task<String> ProcessMerchantBulk(MerchRequestandMiscTextRequest request)
        {
            Dictionary<string, Request> sendRequest = new Dictionary<string, Request>();
            MerchantBulkResponse response = new MerchantBulkResponse();
            Request req = new Request();

            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 5, 00);
            httpClient.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["CTPaymentURI"].ToString());

            HttpResponseMessage httpResponse = httpClient.PostAsJsonAsync("hpa/api/merchant/bulk/load", request).Result;

            string content = httpResponse.Content.ReadAsAsync<string>().Result;

            return content;
        }

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

            HttpResponseMessage httpResponse = httpClient.PostAsJsonAsync("hpa/api/getFileNames", "").Result;

            var stringContent = await httpResponse.Content.ReadAsAsync<List<string>>();

            return stringContent;

        }

        public async Task<MerchantBulkResponse> GetFileDetails(string filename)
        {
            MerchantBulkResponse response = new MerchantBulkResponse();
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 3, 00);
            httpClient.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["CTPaymentURI"].ToString());

            HttpResponseMessage httpResponse = httpClient.PostAsJsonAsync("hpa/api/getFileDetails/", filename).Result;

            response = await httpResponse.Content.ReadAsAsync<MerchantBulkResponse>();

            return response;
        }

    }
}

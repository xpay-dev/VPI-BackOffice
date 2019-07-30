using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace GatewayProcessor.SMS_Gateway
{
    public class SendVerificationCode
    {
        public async Task<string> Post(Dictionary<string, string> postData, string URL)
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(postData);

                var response = await client.PostAsync(URL, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
        }
    }
}

using Accounts.Domain.Entities;
using Accounts.Domain.Rest;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.Rest
{
    public class SnsApi : ISnsApi
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public SnsApi(IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _clientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> SendEmailAsyn(string topicArn, string message)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
            _configuration["AWS_SNS_URL"]);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("AuthorizationToken", "7793B690-9EC7-4240-A152-1D3046693CB3");
            request.Content = new StringContent(
                JsonConvert.SerializeObject(new { topicArn, message }),
                Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<Response>(result);

                return obj?.Content?.ToString();
            }
            else
            {
                return "erro";
            }
        }
    }
}

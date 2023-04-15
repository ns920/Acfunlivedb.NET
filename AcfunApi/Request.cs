using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;

namespace AcfunApi
{
    public class Request
    {
        public LoginInformation loginInformation { get; set; }

        public Request()
        {
            loginInformation = GetAcfunToken.Get().Result;
        }

        public async Task<string> Post(string url, List<KeyValuePair<string, string>> formParams)
        {
            using HttpClient httpClient = new HttpClient();
            using (var request = new HttpRequestMessage())
            {
                Uri uri = new Uri($"{url}&userId={loginInformation.userId}&did={loginInformation._did}&{loginInformation.serviceTokenName}={loginInformation.serviceToken}");
                request.Method = new HttpMethod("POST");
                request.RequestUri = uri;
                var sign = new KeyValuePair<string, string>("__clientSign", GetSign.Sign(uri, formParams, loginInformation.securityKey));
                formParams.Add(sign);
                request.Content = new FormUrlEncodedContent(formParams);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpClient.DefaultRequestHeaders.Add("Cookie", $"_did={loginInformation._did};");
                httpClient.DefaultRequestHeaders.Add("Referer", "https://live.acfun.cn/");
                var response = await httpClient.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> GetNoSign(string url)
        {
            Uri uri = new Uri($"{url}&userId={loginInformation.userId}&did={loginInformation._did}&{loginInformation.serviceTokenName}={loginInformation.serviceToken}");
            using HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("cookie", $"_did={loginInformation._did};");
            var response = await httpClient.GetAsync(uri);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
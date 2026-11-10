using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SmartScanAPI.Helper
{
    public class OAuth2Validator(IHttpClientFactory factory, IOptions<OAuth2Settings> options)
    {
        private readonly HttpClient _httpClient = factory.CreateClient();
        private readonly OAuth2Settings _settings = options.Value;

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
            var request = new HttpRequestMessage(HttpMethod.Post, _settings.IntrospectionEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "token", token }
            });

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();
            var result = JObject.Parse(json);

            return result["active"]?.Value<bool>() ?? false;
        }
    }

    public class OAuth2Settings
    {
        public required string Authority { get; set; }
        public required string IntrospectionEndpoint { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string Scope { get; set; }
    }
}

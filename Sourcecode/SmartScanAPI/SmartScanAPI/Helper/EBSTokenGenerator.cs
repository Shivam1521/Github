using Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace SmartScanAPI.Helper
{
    public class EBSTokenGenerator(IOptions<EBSConfig> config)
    {
        private readonly EBSConfig _config = config.Value;
        public async Task<string?> GetToken()
        {
            try
            {
                var clientId = _config.ClientID;
                var clientSecret = _config.ClientSecret;
                var scope = _config.Scope;
                var tokenUri = _config.TokenEndpoint;

                if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                    throw new InvalidOperationException("Token configuration values missing.");

                string requestData = $"client_id={clientId}&client_secret={clientSecret}&scope={scope}&grant_type=client_credentials";

                string responseText = await PostStringAsync(tokenUri, requestData);

                if (responseText.Contains("access_token"))
                {
                    var res = JsonConvert.DeserializeObject<JObject>(responseText);
                    return res?["access_token"]?.ToString();
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> PostStringAsync(string uri, string requestData)
        {
            try
            {
                using var httpClient = new HttpClient();
                var content = new StringContent(requestData, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await httpClient.PostAsync(uri, content);

                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    response.EnsureSuccessStatusCode(); // will throw

                return responseText;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

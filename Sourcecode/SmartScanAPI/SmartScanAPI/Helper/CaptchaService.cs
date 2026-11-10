using Entities;
using Newtonsoft.Json;
namespace SmartScanAPI.Helper
{
    public class CaptchaService(HttpClient httpClient, IConfiguration configuration)
    {
        public async Task<bool> VerifyCaptchaAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var secret = configuration["GoogleReCaptcha:SecretKey"];

            var response = await httpClient.PostAsync(
                "https://www.google.com/recaptcha/api/siteverify",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                { "secret", secret },
                { "response", token }
                })
            );

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ReCaptchaResponse>(json);

            return result.Success;
        }
    }

}

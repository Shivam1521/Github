using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace SmartScanAPI.Helper
{
    public class OAuth2TokenService(IHttpClientFactory clientFactory, IOptions<OAuth2Settings> settings)
    {
        private readonly OAuth2Settings _settings = settings.Value;
        private string? _cachedToken;
        private DateTime _expiresAt;

        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _expiresAt)
                return _cachedToken;

            var client = clientFactory.CreateClient();

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _settings.IntrospectionEndpoint,
                ClientId = _settings.ClientId,
                ClientSecret = _settings.ClientSecret,
                Scope = _settings.Scope
            });

            if (tokenResponse.IsError)
                throw new Exception($"Failed to get OAuth2 token: {tokenResponse.Error}");

            _cachedToken = tokenResponse.AccessToken;
            _expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60);

            return _cachedToken!;
        }
    }
}

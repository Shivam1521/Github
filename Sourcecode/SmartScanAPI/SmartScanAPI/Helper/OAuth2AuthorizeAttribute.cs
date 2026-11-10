namespace SmartScanAPI.Helper
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class OAuth2AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var validator = context.HttpContext.RequestServices.GetRequiredService<OAuth2Validator>();
            var authHeader = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authHeader["Bearer ".Length..].Trim();

            var isValid = await validator.ValidateTokenAsync(token);
            if (!isValid)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}

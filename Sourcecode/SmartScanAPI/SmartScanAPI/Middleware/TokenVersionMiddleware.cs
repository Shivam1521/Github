using System.IdentityModel.Tokens.Jwt;
using IRepository;

namespace SmartScanAPI.Middleware
{
    public class TokenVersionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, IWrapperRepository userRepo)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = int.Parse(context.User.FindFirst(JwtRegisteredClaimNames.Sid)!.Value);
                var tokenVersion = int.Parse(context.User.FindFirst("tv")!.Value);

                var user = await userRepo.User.GetUserDetails(userId);

                if (user == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("User not found.");
                    return;
                }
                if (tokenVersion != user.TokenVersion)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Session expired. Please login again.");
                    return;
                }
            }

            await next(context);
        }
    }
}

using SlepoffStore.WebApi.Services;
using System.Net;
using System.Text;

namespace SlepoffStore.WebApi.Middleware
{
    public sealed class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IUserService userService)
        {
            string authHeader = httpContext.Request.Headers["Authorization"];
            if (authHeader != null)
            {
                string auth = authHeader.Split(new char[] { ' ' })[1];
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                var usernameAndPassword = encoding.GetString(Convert.FromBase64String(auth));
                string username = usernameAndPassword.Split(new char[] { ':' })[0];
                string password = usernameAndPassword.Split(new char[] { ':' })[1];
                if (userService.CheckCredentials(username, password))
                {
                    await _next(httpContext);
                }
                else
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
            }
            else
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
        }
    }

    public static class BasicAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthMiddleware>();
        }
    }
}

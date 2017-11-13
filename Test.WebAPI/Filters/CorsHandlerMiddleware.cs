using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MEC.WebApi.Filters
{
    public class CorsHandlerMiddleware
    {
        const string Origin = "Origin";
        const string AccessControlRequestMethod = "Access-Control-Request-Method";
        const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

        private readonly RequestDelegate _next;
        public CorsHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //_logger.LogInformation("Handling API key for: " + context.Request.Path);
            bool isCorsRequest = context.Request.Headers.Keys.Contains(Origin);
            bool isPreflightRequest = context.Request.Method == HttpMethod.Options.Method;
            if (isCorsRequest)
            {
                if (isPreflightRequest)
                {
                    await Task.Factory.StartNew<HttpResponseMessage>(() =>
                    {
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Headers.Add(AccessControlAllowOrigin, context.Request.Headers[Origin].First());

                        string accessControlRequestMethod = context.Request.Headers[AccessControlRequestMethod].FirstOrDefault();
                        if (accessControlRequestMethod != null)
                        {
                            response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
                        }

                        string requestedHeaders = string.Join(", ", context.Request.Headers[AccessControlRequestHeaders]);
                        if (!string.IsNullOrEmpty(requestedHeaders))
                        {
                            response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
                        }

                        return response;
                    });
                }
                else
                {
                    context.Response.OnStarting(state => {
                        var httpContext = (HttpContext)state;
                        httpContext.Response.Headers.Add(AccessControlAllowOrigin, context.Request.Headers[Origin].First());
                        return Task.FromResult(0);
                    }, context);

                    await _next.Invoke(context);
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}

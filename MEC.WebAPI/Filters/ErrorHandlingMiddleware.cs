using log4net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YD.Foundation.Util;

namespace MEC.WebApi.Filters
{
    public class ErrorHandlingMiddleware
    {
        private static readonly ILog logger = LogManager.GetLogger(WsConstants.LOG_REPOSITORY_NAME, typeof(ErrorHandlingMiddleware));

        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await next(context);
                if (context.Response.StatusCode == 404)
                {
                    throw new Exception("Not Found.");
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            logger.ErrorFormat(exception.ToString());
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected .OK;//

            //if (exception is MyNotFoundException) code = HttpStatusCode.NotFound;
            //else if (exception is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (exception is MyException) code = HttpStatusCode.BadRequest;

            var result = JsonConvert.SerializeObject(new { Status = code, Msg = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result, Encoding.Unicode);
        }
    }
}

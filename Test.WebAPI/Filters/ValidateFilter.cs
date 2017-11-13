using System;
using System.Collections.Generic;
using System.Linq;
using YD.Foundation.Util;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MEC.WebApi.Filters
{
    public class ValidateFilter : ActionFilterAttribute
    {
        private IConfigurationRoot configuration { get; set; }

        public ValidateFilter(IConfigurationRoot config)
            :base()
        {
            this.configuration = config;
        }

        /// <summary>
        /// Access-Control-Allow-Origin是HTML5中定义的一种服务器端返回Response header，用来解决资源（比如字体）的跨域权限问题。
        /// </summary>
        private const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        /// <summary>
        ///  originHeaderdefault的值可以使 URL 或 *，如果是 URL 则只会允许来自该 URL 的请求，* 则允许任何域的请求
        /// </summary>
        private const string originHeaderdefault = "*";

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            //需要分享出去的方法不用验证
            if (actionContext.Controller.GetType().GetTypeInfo().GetCustomAttributes<Shareable>().Count() == 0
                && ((ControllerActionDescriptor)(actionContext.ActionDescriptor)).MethodInfo.GetCustomAttributes<Shareable>().Count() == 0)
            {
                string key = configuration.GetSection("app").GetValue<string>("DesKey");
                string token = actionContext.HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                    throw new Exception("无效的请求");
                else
                    token = Util.DecryptString(token, key);
                if (token == null)
                    throw new Exception("token无效");
                string[] ps = token.Split('*');
                if (ps[0] != configuration.GetSection("app").GetValue<string>("TokenPrefix"))
                    throw new Exception("token无效");
            }
        }

        //public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        //{
        //    if (actionExecutedContext.Exception != null)
        //        throw actionExecutedContext.Exception;
        //    //允许api支持跨域调用
        //   // actionExecutedContext.Response.Headers.Add(AccessControlAllowOrigin, originHeaderdefault);
        //}
    }
}
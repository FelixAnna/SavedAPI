//using System.Web.Mvc;
using MEC.Model.Common;
using MEC.Model;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using YD.Foundation.Extensions;
using System;

namespace MEC.WebApi.Filters
{
    public class VaildLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userModel = filterContext.HttpContext.Session.Get<object>(SessionKey.UserInfo);
            if (userModel == null)
            {
                string token = string.Empty;
                //string token = filterContext.HttpContext.Request["token"];
                var tokenObj = filterContext.HttpContext.Request.Cookies["token"];//
                if (tokenObj != null) token = tokenObj; //.Value;

                //验证token格式
                //if (!string.IsNullOrEmpty(token))
                //{
                //    var GetUserInfo = new AdminUser().GetTokenUser(token);
                //    if (GetUserInfo != null)
                //    {
                //        filterContext.HttpContext.Session[SessionKey.UserInfo] = new AdminUser().GetSingleById(GetUserInfo.UserId);
                //    }
                //}
                userModel = filterContext.HttpContext.Session.Get<object>(SessionKey.UserInfo);

                if (userModel == null)
                    UnLoginOpreate(filterContext);
            }
            //else
            //{
            //    if (!new AdminPermission().IsHoutaiAuthorized(userModel as admin_user))
            //    {
            //        UnLoginOpreate(filterContext);
            //    }
            //}
        }

        private static void UnLoginOpreate(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var value = new JsonData { Status = -1, Msg = "抱歉,登录超时,请刷新页面重新登录！" };
                JsonResult js = new JsonResult(value);
                //{
                //    Data = new JsonData { Status = -1, Msg = "抱歉,登录超时,请刷新页面重新登录！" },
                //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                //};
                filterContext.Result = js;
            }
            else
            {
                //throw new Exception("Please Login.");
                //跳转到一个空白的页面，空白页面跳转到登录页面
                filterContext.Result = new RedirectResult("/Login/TransferToLogin");
                //回调转但还是会执行action
                //filterContext.HttpContext.Response.Write("<script>parent.location.href='" + "/Login/Index" + "'</script>");
                //filterContext.HttpContext.Response.Flush();
                //filterContext.HttpContext.Response.End();
            }
        }
    }
}

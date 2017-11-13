using MEC.Model.Common;
using MEC.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YD.Foundation.Extensions;

namespace MEC.WebApi.Filters
{
    public class Authorization : ActionFilterAttribute
    {
        private readonly string _actionType;
        /// <summary>
        /// 权限code
        /// </summary>
        /// <param name="type"></param>
        public Authorization(string type)
        {
            _actionType = type;
        }

        /// <summary>
        /// 授权访问 
        /// 有授权访问的地方不需要再验证是否登陆
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session.Get<string>(SessionKey.UserInfo) == null)
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
                    //跳转到一个空白的页面，空白页面跳转到登录页面
                    filterContext.Result = new RedirectResult("/Login/TransferToLogin");
                }
            }
            else
            {
                //if (!new AdminPermission().IsAuthorized(_actionType))
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        var value = new JsonData { Status = 0, Msg = "抱歉,你不具有当前操作的权限！" };
                        JsonResult js = new JsonResult(value);
                        filterContext.Result = js;
                    }
                    else
                    {
                        filterContext.Result = new ContentResult { Content = @"抱歉,你不具有当前操作的权限！" };
                        //filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Admin", action = "AuthError" }));
                    }
                }
            }
        }
    }
}

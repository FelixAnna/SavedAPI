using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MEC.Model;
using MEC.Model.Models;
using MEC.Logic;
using YD.Foundation.Config;
using YD.Data;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using YD.Foundation.Util;

namespace MEC.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Login")]
    public class LoginController : Controller
    {
        LoginLogic _loginLogic;

        public LoginController(LoginLogic service)
        {
            this._loginLogic = service;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        [Route("PostLogin")]
        [HttpPost]
        public ReturnModel<object> PostLogin(LoginDTO ent)
        {
            return _loginLogic.UserLogin(ent);
        }

        /// <summary>
        /// 身份有效期内获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("GetUser")]
        [HttpGet]
        public ReturnModel<object> GetUser(string token = "")
        {
            return _loginLogic.GetUser(token);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("LoginOut")]
        [HttpGet]
        public ReturnModel<bool> LoginOut(string token = "")
        {
            return _loginLogic.LoginOut(token);
        }

        //忘记密码


        ///// <summary>
        ///// 注册
        ///// </summary>
        ///// <param name="ent"></param>
        ///// <returns></returns>
        //[Route("Regis")]
        //[HttpPost]
        //public ReturnModel<object> Regis(LoginDTO ent)
        //{
        //    return _loginLogic.UserLogin(ent);
        //}

        //短信验证码
        [Route("SendVerCode")]
        [HttpGet]
        public ReturnModel<bool> SendVerCode(string phone)
        {
            return _loginLogic.SendVerCode(phone);
        }

        #region 图片验证码
        //图片验证码 - base64跨域
        [HttpGet]
        [Route("GetBs64ImageCode")]
        public ReturnModel<string> GetBs64ImageCode()
        {
            return _loginLogic.GetBs64ImgCode();
        }

        //图片验证码 - 跨域,未完成
        [HttpGet]
        [Route("GetImageCode")]
        public HttpContext GetImageCode()
        {
            //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    Content = new ByteArrayContent(_loginLogic.GetImageCode())
            //};
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            //result.Content.Headers.Add("Content-Type", "image/png");

            //string code = "";
            //MemoryStream ms = Util.CreateImageCode(out code);

            Microsoft.AspNetCore.Http.HttpContext context = HttpContext;
            //context.Response.Body = ms;
            //context.Response.Headers["Content-Type"] = "image/png";

            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext)state;
                httpContext.Response.Headers["Content-Type"] = "image/png";
                //httpContext.Response
                return Task.FromResult(0);
            }, context);

            return context;
        }

        /// <summary>
        /// 图形验证码 -无法跨域
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ValidateCode")]
        public IActionResult ValidateCode()
        {
            return File(_loginLogic.GetImageCode(), @"image/png");
        }

        #endregion


        //其他登录方式

    }
}
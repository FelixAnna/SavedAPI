using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MEC.Logic;

namespace MEC.WebApi.Controllers
{
    public class PayController : Controller
    {
        PayLogic _loginLogic;

        public PayController(PayLogic service)
        {
            this._loginLogic = service;
        }

        public void Pay()
        {
            //Response.Redirect("ZFB/PaySubmit?Id=" + Request.QueryString["Id"].ToString());
        }

    }
}
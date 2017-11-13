using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YD.Foundation.ZFBPay;

namespace MEC.WebApi.Controllers
{
    public class ZFBController : Controller
    {
        /// <summary>
        /// 支付页
        /// </summary>
        /// <returns></returns>
        public ActionResult PaySubmit()
        {
            string str = string.Empty;
            str = paySubmit.Pay("itaoyoutest2017626", "测试商品", "0.01", "测试商品");
            return Content(str, "text/html", Encoding.UTF8);
        }

        // GET: ZFB
        /// <summary>
        /// notify_url: 服务器后台通知,这个页面是支付宝服务器端自动调用这个页面的链接地址,这个页面根据支付宝反馈过来的信息修改网站的定单状态,更新完成后需要返回一个success给支付宝.,不能含有任何其它的字符包括html语言. 
        ///流程:买家付完款(trade_status= WAIT_SELLER_SEND_GOODS)--->支付宝通知 notify_url--->如果反馈给支付宝的是success(表示成功, 这个状态下不再反馈, 如果不是继续通知, 一般第一次发送和第二次发送的时间间隔是3分钟)
        /// </summary>
        /// <returns></returns>
        public ActionResult notify_url()
        {
            string str = string.Empty;
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表

                    //商户订单号

                    string out_trade_no = Request.Form["out_trade_no"];

                    //支付宝交易号

                    string trade_no = Request.Form["trade_no"];

                    //交易状态
                    string trade_status = Request.Form["trade_status"];
                    string total_fee = Request.Form["total_fee"];
                    if (trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS")
                    {

                        str = "success";

                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                    }
                    else
                    {
                        str = "fail";
                    }

                    //打印页面

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    str = "验证失败，fail";
                }
            }
            else
            {
                str = "无返回参数";
            }

            return Content(str);
        }

        public ActionResult return_url()
        {
            string str = string.Empty;
            SortedDictionary<string, string> sPara = GetRequestGet();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Query["notify_id"], Request.Query["sign"]);

                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表

                    //商户订单号

                    string out_trade_no = Request.Query["out_trade_no"];

                    //支付宝交易号

                    string trade_no = Request.Query["trade_no"];

                    //交易状态
                    string trade_status = Request.Query["trade_status"];
                    string total_fee = Request.Query["total_fee"];
                    str = Request.Query["trade_status"];
                    if (Request.Query["trade_status"] == "TRADE_FINISHED" || Request.Query["trade_status"] == "TRADE_SUCCESS")
                    {
                        str += "<br />../Order_details3.aspx?Id=" + out_trade_no;
                        //Response.Redirect("../Order_details3.aspx?Id=" + out_trade_no);
                    }
                    else
                    {
                        str += "<br />trade_status=" + Request.Query["trade_status"];
                        //Response.Write("trade_status=" + Request.Query["trade_status"]);
                    }

                    //打印页面
                    str += "验证成功<br />" + out_trade_no + total_fee + Request.Query["trade_status"];
                    //Response.Write("验证成功<br />" + out_trade_no + total_fee + Request.Query["trade_status"]);

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    str = "验证失败";
                }
            }
            else
            {
                str = "无返回参数";
            }

            return Content(str);
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            IFormCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.Keys.ToArray();
            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            IQueryCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Query;

            // Get names of all forms into a string array.
            String[] requestItem = coll.Keys.ToArray();

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Query[requestItem[i]]);
            }

            return sArray;
        }
    }
}
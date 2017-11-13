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
        /// ֧��ҳ
        /// </summary>
        /// <returns></returns>
        public ActionResult PaySubmit()
        {
            string str = string.Empty;
            str = paySubmit.Pay("itaoyoutest2017626", "������Ʒ", "0.01", "������Ʒ");
            return Content(str, "text/html", Encoding.UTF8);
        }

        // GET: ZFB
        /// <summary>
        /// notify_url: ��������̨֪ͨ,���ҳ����֧�������������Զ��������ҳ������ӵ�ַ,���ҳ�����֧����������������Ϣ�޸���վ�Ķ���״̬,������ɺ���Ҫ����һ��success��֧����.,���ܺ����κ��������ַ�����html����. 
        ///����:��Ҹ����(trade_status= WAIT_SELLER_SEND_GOODS)--->֧����֪ͨ notify_url--->���������֧��������success(��ʾ�ɹ�, ���״̬�²��ٷ���, ������Ǽ���֪ͨ, һ���һ�η��ͺ͵ڶ��η��͵�ʱ������3����)
        /// </summary>
        /// <returns></returns>
        public ActionResult notify_url()
        {
            string str = string.Empty;
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//�ж��Ƿ��д����ز���
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

                if (verifyResult)//��֤�ɹ�
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //������������̻���ҵ���߼��������


                    //�������������ҵ���߼�����д�������´�������ο�������
                    //��ȡ֧������֪ͨ���ز������ɲο������ĵ���ҳ����תͬ��֪ͨ�����б�

                    //�̻�������

                    string out_trade_no = Request.Form["out_trade_no"];

                    //֧�������׺�

                    string trade_no = Request.Form["trade_no"];

                    //����״̬
                    string trade_status = Request.Form["trade_status"];
                    string total_fee = Request.Form["total_fee"];
                    if (trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS")
                    {

                        str = "success";

                        //�жϸñʶ����Ƿ����̻���վ���Ѿ���������
                        //���û�������������ݶ����ţ�out_trade_no�����̻���վ�Ķ���ϵͳ�в鵽�ñʶ�������ϸ����ִ���̻���ҵ�����
                        //���������������ִ���̻���ҵ�����

                    }
                    else
                    {
                        str = "fail";
                    }

                    //��ӡҳ��

                    //�������������ҵ���߼�����д�������ϴ�������ο�������

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//��֤ʧ��
                {
                    str = "��֤ʧ�ܣ�fail";
                }
            }
            else
            {
                str = "�޷��ز���";
            }

            return Content(str);
        }

        public ActionResult return_url()
        {
            string str = string.Empty;
            SortedDictionary<string, string> sPara = GetRequestGet();

            if (sPara.Count > 0)//�ж��Ƿ��д����ز���
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Query["notify_id"], Request.Query["sign"]);

                if (verifyResult)//��֤�ɹ�
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //������������̻���ҵ���߼��������


                    //�������������ҵ���߼�����д�������´�������ο�������
                    //��ȡ֧������֪ͨ���ز������ɲο������ĵ���ҳ����תͬ��֪ͨ�����б�

                    //�̻�������

                    string out_trade_no = Request.Query["out_trade_no"];

                    //֧�������׺�

                    string trade_no = Request.Query["trade_no"];

                    //����״̬
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

                    //��ӡҳ��
                    str += "��֤�ɹ�<br />" + out_trade_no + total_fee + Request.Query["trade_status"];
                    //Response.Write("��֤�ɹ�<br />" + out_trade_no + total_fee + Request.Query["trade_status"]);

                    //�������������ҵ���߼�����д�������ϴ�������ο�������

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//��֤ʧ��
                {
                    str = "��֤ʧ��";
                }
            }
            else
            {
                str = "�޷��ز���";
            }

            return Content(str);
        }

        /// <summary>
        /// ��ȡ֧����POST����֪ͨ��Ϣ�����ԡ�������=����ֵ������ʽ�������
        /// </summary>
        /// <returns>request��������Ϣ��ɵ�����</returns>
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
        /// ��ȡ֧����GET����֪ͨ��Ϣ�����ԡ�������=����ֵ������ʽ�������
        /// </summary>
        /// <returns>request��������Ϣ��ɵ�����</returns>
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
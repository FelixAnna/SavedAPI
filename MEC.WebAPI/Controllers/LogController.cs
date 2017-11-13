
using MEC.Logic;
using MEC.Model;
using MEC.Model.Models;
using MEC.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using YD.Foundation.Config;

namespace MEC.WebApi.Controllers
{
    public class LogController : Controller
    {
        ILogLogicService logicService;

        public LogController(ILogLogicService service)
        {
            this.logicService = service;
        }

        public ReturnModel<IEnumerable<HishopLogs>> GetLogList(int pageIndex = 1, int pageSize = 20, string keyWord = "")
        {
            return logicService.GetLogList(keyWord, pageIndex, pageSize);
        }

        public string Exception()
        {
            throw new UnauthorizedAccessException("异常消息", new Exception("测试异常"));
        }

        [Shareable]
        public ReturnModel<IEnumerable<HishopLogs>> Public(int pageIndex = 1, int pageSize = 20, string keyWord = "")
        {
            return logicService.GetLogList(keyWord, pageIndex, pageSize);
        }

        [Shareable]
        public ReturnModel<IEnumerable<HishopLogs>> RabbitMQSend(int pageIndex = 1, int pageSize = 20, string keyWord = "")
        {
            var logs = logicService.EnqueueLogs(keyWord, pageIndex, pageSize);

            return logs;
        }

        [Shareable]
        public ReturnModel<IEnumerable<HishopLogs>> RabbitMQReceive(int pageIndex = 1, int pageSize = 20, string keyWord = "")
        {

            var logs = logicService.DequeueLogs(keyWord, pageIndex, pageSize);
            return logs;
        }

        [Shareable]
        public ReturnModel<HishopLogs> FindById(long logId)
        {
            var logs = logicService.FindById(logId);
            return logs;
        }
        
    }
}
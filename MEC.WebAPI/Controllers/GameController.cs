using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MEC.Logic.Services;
using MEC.Model;
using MEC.Model.DTO;

namespace MEC.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Games")]
    public class GameController : Controller
    {
        private readonly GameService service;

        public GameController(GameService theService)
        {
            this.service = theService;
        }

        #region 商品分类
        [Route("{gameId}")]
        [HttpGet]
        public ReturnModel<ProductTypeDTO> ProductType(int gameId)
        {
            return service.GetProductTypes(gameId, 0);
        }

        [Route("{gameId}/{parentId}")]
        [HttpGet]
        public ReturnModel<ProductTypeDTO> ProductType(int gameId, int parentId)
        {
            return service.GetProductTypes(gameId, parentId);
        }
        #endregion
    }
}
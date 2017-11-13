using MEC.Logic.Services;
using MEC.Model;
using MEC.Model.DTO;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MEC.WebApi.Controllers
{
    [Route("api/ProductTypes")]
    public class ProductTypeController : Controller
    {
        private readonly ProductTypeService service;

        public ProductTypeController(ProductTypeService theService)
        {
            this.service = theService;
        }

        //[OutputCache(Duration = 3600, VaryByParam = "searchTerm")]  //API输出缓存：低优先级以后可以再考虑
        [Route("{id}")]
        [HttpGet]
        public ReturnModel<ProductTypeDTO> ProductType(int id, [FromQuery]bool? inclGames = false )
        {
            return service.GetProductTypes(id, inclGames.Value);
        }
    }
}

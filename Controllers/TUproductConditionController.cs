using Microsoft.AspNetCore.Mvc;
using diveWebAPI.DTO;
using diveWebAPI.Models;
using System.Collections.Generic;
using System.Linq;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TUproductConditionController : ControllerBase
    {
        private readonly DiveShopperContext _context;
        public TUproductConditionController(DiveShopperContext context)
        {
            _context = context;
        }
        // GET: api/<TUproductConditionController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
        // 取得所有商品狀態類別
        [HttpGet]
        public ActionResult<IEnumerable<TUConditionDTO>> GetAllProductConditions()
        {
            var productConditions = _context.TUproductConditions
                .Select(pc => new TUConditionDTO
                {
                    productConditionId = pc.ProductConditionId,
                    condition = pc.Condition
                })
                .ToList();

            return Ok(productConditions);
        }

        // GET api/<TUproductConditionController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TUproductConditionController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TUproductConditionController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TUproductConditionController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

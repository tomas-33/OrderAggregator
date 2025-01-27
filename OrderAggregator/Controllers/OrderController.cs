using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAggregator.Logic.Interfaces;
using OrderAggregator.Model;
using OrderAggregator.Model.DB;

namespace OrderAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderLogic _orderLogic;

        public OrderController(IOrderLogic orderLogic)
        {
            _orderLogic = orderLogic;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ICollection<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Order>>> AddOrders([FromBody] ICollection<AddOrder> orders)
        {
            return Ok(await _orderLogic.AddOrdersAsync(orders));
        }
    }
}

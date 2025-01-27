using Microsoft.AspNetCore.Mvc;
using OrderAggregator.Logic.Interfaces;
using OrderAggregator.Model;
using OrderAggregator.Model.DB;

namespace OrderAggregator.Controllers
{
    /// <summary>
    /// Controller for orders.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderLogic _orderLogic;

        /// <summary>
        /// Consttructor
        /// </summary>
        public OrderController(IOrderLogic orderLogic)
        {
            _orderLogic = orderLogic;
        }

        /// <summary>
        /// Adds orders to DB and to aggregate queue. Invalid orders are skipped.
        /// </summary>
        /// <param name="orders">List of orders.</param>
        /// <returns>Created orders.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ICollection<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Order>>> AddOrders([FromBody] ICollection<AddOrder> orders)
        {
            return Ok(await _orderLogic.AddOrdersAsync(orders));
        }

        // Ostatní metody pro get, update, delete atd.
    }
}

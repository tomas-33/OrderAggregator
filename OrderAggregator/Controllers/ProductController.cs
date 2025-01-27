using Microsoft.AspNetCore.Mvc;
using OrderAggregator.Logic.Interfaces;
using OrderAggregator.Model;
using OrderAggregator.Model.DB;

namespace OrderAggregator.Controllers
{
    /// <summary>
    /// Controller for products.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductLogic _productLogic;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productLogic"></param>
        public ProductController(IProductLogic productLogic)
        {
            _productLogic = productLogic;
        }

        /// <summary>
        /// Adds products. Invalid products are not added.
        /// </summary>
        /// <param name="products">Products to add.</param>
        /// <returns>Added products.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ICollection<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Product>>> AddProducts([FromBody] ICollection<AddProduct> products)
        {
            return Ok(await _productLogic.AddProductsAsync(products));
        }
    }
}

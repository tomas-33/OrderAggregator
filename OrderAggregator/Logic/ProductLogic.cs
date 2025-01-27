using OrderAggregator.DB;
using OrderAggregator.Logic.Interfaces;
using OrderAggregator.Model.DB;
using OrderAggregator.Model;
using OrderAggregator.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace OrderAggregator.Logic
{
    /// <summary>
    /// Products logic.
    /// </summary>
    public class ProductLogic : IProductLogic
    {
        private readonly ILogger<ProductLogic> _logger;
        private readonly OrdersContext _context;
        private readonly IMemoryCache _cache;

        public ProductLogic(ILogger<ProductLogic> logger, OrdersContext ordersContext, IMemoryCache cache)
        {
            _logger = logger;
            _context = ordersContext;
            _cache = cache;
        }

        /// <summary>
        /// Adds products. Invalid products are not added.
        /// </summary>
        /// <param name="products">Products to add.</param>
        /// <returns>Added products.</returns>
        public async Task<ICollection<Product>> AddProductsAsync(ICollection<AddProduct> products)
        {
            var result = new List<Product>();

            if (products is null)
            {
                return result;
            }

            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.Name))
                {
                    _logger.LogWarning($"AddProductsAsync: Product name is missing");
                    continue;
                }

                if (await _context.Products.Where(x => x.Name == product.Name).AnyAsync())
                {
                    _logger.LogWarning($"AddProductsAsync: Product with name {product.Name} already exists.");
                    continue;
                }

                var newProduct = new Product()
                {
                    Name = product.Name
                };

                result.Add((await _context.AddAsync(newProduct)).Entity);
            }

            await _context.SaveChangesAsync();
            await ProductCache.LoadProductIdsAsync(_cache, _context);

            return result;
        }
    }
}

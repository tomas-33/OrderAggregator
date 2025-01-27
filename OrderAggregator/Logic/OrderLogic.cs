using Microsoft.Extensions.Caching.Memory;
using OrderAggregator.DB;
using OrderAggregator.Logic.Interfaces;
using OrderAggregator.Model;
using OrderAggregator.Model.DB;
using OrderAggregator.Utilities;

namespace OrderAggregator.Logic
{
    public class OrderLogic : IOrderLogic
    {
        private readonly ILogger<OrderLogic> _logger;
        private readonly OrdersContext _context;
        private readonly IOrderAggregatorForJob _orderSingleton;
        private readonly IMemoryCache _cache;

        public OrderLogic(ILogger<OrderLogic> logger, OrdersContext ordersContext, IOrderAggregatorForJob orderSingleton, IMemoryCache cache)
        {
            _logger = logger;
            _context = ordersContext;
            _orderSingleton = orderSingleton;
            _cache = cache;
        }

        /// <summary>
        /// Adds orders.
        /// </summary>
        /// <param name="orders">List of orders.</param>
        /// <returns>List of created orders.</returns>
        public async Task<ICollection<Order>> AddOrdersAsync(ICollection<AddOrder> orders)
        {
            var result = new List<Order>();
            if (orders is null)
            {
                return result;
            }

            foreach (var order in orders)
            {
                if (order.ProductId < 1)
                {
                    _logger.LogWarning($"AddOrdersAsync: ProductId is {order.ProductId}");
                    continue;
                }

                if (order.Quantity < 1)
                {
                    _logger.LogWarning($"AddOrdersAsync: Quantity is {order.Quantity}");
                    continue;
                }

                if (!(await ProductCache.GetProductIdsAsync(_cache, _context)).Contains(order.ProductId))
                {
                    _logger.LogWarning($"AddOrdersAsync: ProductId {order.ProductId} does not exist");
                    continue;
                }

                var newOrder = new Order()
                {
                    ProductId = order.ProductId,
                    Quantity = order.Quantity
                };

                result.Add((await _context.AddAsync(newOrder)).Entity);
            }

            await _context.SaveChangesAsync();

            _orderSingleton.AddOrders(result);

            return result;
        }
    }
}

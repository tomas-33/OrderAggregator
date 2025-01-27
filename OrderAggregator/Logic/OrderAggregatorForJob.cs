using Newtonsoft.Json;
using OrderAggregator.Logic.Interfaces;
using OrderAggregator.Model;
using OrderAggregator.Model.DB;
using System.Collections.Concurrent;

namespace OrderAggregator.Logic
{
    /// <summary>
    /// Aggregator for job.
    /// </summary>
    public class OrderAggregatorForJob : IOrderAggregatorForJob
    {
        private readonly ILogger<OrderAggregatorForJob> _logger;

        // Fronta by mohla být v persistentní paměti, v případě pádu aplikace nebo vytvořena synchronizační služba.
        private readonly ConcurrentDictionary<int, int> _orderAggregate;

        public OrderAggregatorForJob(ILogger<OrderAggregatorForJob> logger)
        {
            _logger = logger;
            _orderAggregate = new ConcurrentDictionary<int, int>();
        }

        /// <summary>
        /// Gets actual queue for testing purposes.
        /// </summary>
        public Dictionary<int, int> GetOrderAggregate()
        {
            return _orderAggregate.ToDictionary();
        }

        /// <summary>
        /// Adds orders to memory for process by job.
        /// </summary>
        public void AddOrders(ICollection<Order> orders)
        {
            foreach (var order in orders)
            {
                if (!order.ProductId.HasValue || !order.Quantity.HasValue)
                {
                    continue;
                }

                if (!_orderAggregate.ContainsKey(order.ProductId.Value))
                {
                    _orderAggregate.TryAdd(order.ProductId.Value, order.Quantity.Value);
                }
                else
                {
                    _orderAggregate[order.ProductId.Value] += order.Quantity.Value;
                }
            }

            _logger.LogInformation($"{orders.Count} order added for sending.");
        }

        /// <summary>
        /// Sends queued orders to log and clears memory.
        /// </summary>
        public void SendOrders()
        {
            var aggregates = new List<OrderAggregate>();
            foreach (var item in _orderAggregate)
            {
                if (item.Value > 0)
                {
                    aggregates.Add(new OrderAggregate() { ProductId = item.Key, Quantity = item.Value });
                    _orderAggregate[item.Key] = 0;
                }
            }

            if (aggregates.Count > 0)
            {
                _logger.LogInformation("{aggregates}", JsonConvert.SerializeObject(aggregates));
            }
        }
    }
}

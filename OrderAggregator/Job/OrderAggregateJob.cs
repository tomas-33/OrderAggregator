using OrderAggregator.Logic.Interfaces;
using Quartz;

namespace OrderAggregator.Job
{
    /// <summary>
    /// Job for sending aggregated orders.
    /// </summary>
    public class OrderAggregateJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IOrderAggregatorForJob _orderSingleton;

        public OrderAggregateJob(IOrderAggregatorForJob orderSingleton, ILogger<OrderAggregateJob> logger)
        {
            _orderSingleton = orderSingleton;
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Executing OrderAggregateJob");
            _orderSingleton.SendOrders();

            return Task.CompletedTask;
        }
    }
}

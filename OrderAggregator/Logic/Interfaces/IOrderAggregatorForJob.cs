using OrderAggregator.Model.DB;

namespace OrderAggregator.Logic.Interfaces
{
    public interface IOrderAggregatorForJob
    {
        /// <summary>
        /// Gets actual queue for testing purposes.
        /// </summary>
        Dictionary<int, int> GetOrderAggregate();

        /// <summary>
        /// Adds orders to memory for process by job.
        /// </summary>
        void AddOrders(ICollection<Order> orders);

        /// <summary>
        /// Sends queued orders to log and clears memory.
        /// </summary>
        void SendOrders();
    }
}

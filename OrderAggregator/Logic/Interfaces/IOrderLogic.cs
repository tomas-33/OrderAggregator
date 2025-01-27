using OrderAggregator.Model;
using OrderAggregator.Model.DB;

namespace OrderAggregator.Logic.Interfaces
{
    /// <summary>
    /// Orders interface
    /// </summary>
    public interface IOrderLogic
    {
        /// <summary>
        /// Adds orders to DB and to aggregate queue. Invalid orders are skipped.
        /// </summary>
        /// <param name="orders">List of orders.</param>
        /// <returns>List of created orders.</returns>
        Task<ICollection<Order>> AddOrdersAsync(ICollection<AddOrder> orders);
    }
}

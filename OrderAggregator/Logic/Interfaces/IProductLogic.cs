using OrderAggregator.Model.DB;
using OrderAggregator.Model;

namespace OrderAggregator.Logic.Interfaces
{
    /// <summary>
    /// Products interface.
    /// </summary>
    public interface IProductLogic
    {
        /// <summary>
        /// Adds products. Invalid products are not added.
        /// </summary>
        /// <param name="products">Products to add.</param>
        /// <returns>Added products.</returns>
        Task<ICollection<Product>> AddProductsAsync(ICollection<AddProduct> products);
    }
}

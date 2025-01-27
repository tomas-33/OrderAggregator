using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OrderAggregator.DB;

namespace OrderAggregator.Utilities
{
    public static class ProductCache
    {
        private static readonly string _productsCacheKey = "Products";

        /// <summary>
        /// Get productIds from cache. Loads from DB, if not present.
        /// </summary>
        public static async Task<HashSet<int>> GetProductIdsAsync(IMemoryCache cache, OrdersContext context)
        {
            if (cache.TryGetValue<HashSet<int>>(_productsCacheKey, out var ret))
            {
                ret ??= new HashSet<int>();
                return ret;
            }

            await LoadProductIdsAsync(cache, context);
            cache.TryGetValue<HashSet<int>>(_productsCacheKey, out var result);
            result ??= new HashSet<int>();

            return result;
        }

        /// <summary>
        /// Loads products to cache and clears original, if present.
        /// </summary>
        public static async Task LoadProductIdsAsync(IMemoryCache cache, OrdersContext context)
        {
            if (cache.TryGetValue<HashSet<int>>(_productsCacheKey, out var ret))
            {
                cache.Remove(_productsCacheKey);
            }

            var products = await context.Products.Select(x => x.Id).ToHashSetAsync();
            cache.Set<HashSet<int>>(_productsCacheKey, products, TimeSpan.FromHours(24));
        }
    }
}

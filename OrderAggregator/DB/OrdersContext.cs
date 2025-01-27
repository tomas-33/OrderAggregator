using Microsoft.EntityFrameworkCore;
using OrderAggregator.Model.DB;

namespace OrderAggregator.DB
{
    public class OrdersContext : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OrdersContext(DbContextOptions<OrdersContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne<Product>(d => d.Product)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ProductId);
            });
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}

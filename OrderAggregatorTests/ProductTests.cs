using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderAggregator.DB;
using OrderAggregator.Logic.Interfaces;
using OrderAggregator.Logic;
using OrderAggregator.Model.DB;
using OrderAggregator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace OrderAggregatorTests
{
    public class ProductTests
    {
        private readonly IProductLogic _productLogic;
        private readonly OrdersContext _context;

        public ProductTests()
        {
            var loggerFactory = new LoggerFactory();

            var optionsBuilder = new DbContextOptionsBuilder<OrdersContext>();
            optionsBuilder.UseInMemoryDatabase("OrderDb");
            _context = new OrdersContext(optionsBuilder.Options);

            _productLogic = new ProductLogic(loggerFactory.CreateLogger<ProductLogic>(), _context, new MemoryCache(new MemoryCacheOptions()));
        }

        [SetUp]
        public async Task SetUp()
        {
            var products = await _context.Products.ToListAsync();
            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _context.DisposeAsync();
        }

        [Test]
        public async Task AddProducts()
        {
            // Act
            var products = new List<AddProduct>()
            {
                new AddProduct()
                {
                    Name = "Product 1"
                },
                new AddProduct()
                {
                    Name = "Product 2"
                },
                new AddProduct()
                {
                    Name = "Product 3"
                }
            };

            var result = _productLogic.AddProductsAsync(products);

            // Assert
            Assert.That(result, Is.Not.Null, "result");
            var productsDb = await _context.Products.ToListAsync();
            Assert.That(productsDb, Is.Not.Null, "productsDb");
            Assert.That(productsDb.Count, Is.EqualTo(3), "Count of productsDb");
        }
    }
}

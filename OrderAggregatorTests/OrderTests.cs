using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderAggregator.Controllers;
using OrderAggregator.DB;
using OrderAggregator.Logic;
using OrderAggregator.Logic.Interfaces;
using OrderAggregator.Model;
using OrderAggregator.Model.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderAggregatorTests
{
    public class OrderTests
    {
        private readonly IOrderLogic _orderLogic;
        private readonly IOrderAggregatorForJob _orderAggregator;
        private readonly OrdersContext _context;

        public OrderTests()
        {
            var loggerFactory = new LoggerFactory();

            var optionsBuilder = new DbContextOptionsBuilder<OrdersContext>();
            optionsBuilder.UseInMemoryDatabase("OrderDb");
            _context = new OrdersContext(optionsBuilder.Options);

            _orderAggregator = new OrderAggregatorForJob(loggerFactory.CreateLogger<OrderAggregatorForJob>());
            _orderLogic = new OrderLogic(loggerFactory.CreateLogger<OrderLogic>(), _context, _orderAggregator, new MemoryCache(new MemoryCacheOptions()));
        }

        [SetUp]
        public async Task Setup()
        {
            var orders = await _context.Orders.ToListAsync();
            _context.RemoveRange(orders);
            await _context.SaveChangesAsync();
        }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await _context.AddAsync(new Product() { Name = "Product1" });
            await _context.AddAsync(new Product() { Name = "Product2" });
            await _context.AddAsync(new Product() { Name = "Product3" });
            await _context.SaveChangesAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _context.DisposeAsync();
        }

        [Test]
        public async Task AddOrders()
        {
            // Act
            var orders = new List<AddOrder>()
            {
                new AddOrder()
                {
                    ProductId = 1,
                    Quantity = 2
                },
                new AddOrder()
                {
                    ProductId = 2,
                    Quantity = 3
                },
                new AddOrder()
                {
                    ProductId = 1,
                    Quantity = 4
                },
            };

            var result = _orderLogic.AddOrdersAsync(orders);

            // Assert
            Assert.That(result, Is.Not.Null, "result");
            var ordersDb = await _context.Orders.OrderBy(x => x.Quantity).ToListAsync();
            Assert.That(ordersDb, Is.Not.Null, "ordersDb");
            Assert.That(ordersDb.Count, Is.EqualTo(3), "Count of ordersDb");

            Assert.That(ordersDb[0].Quantity, Is.EqualTo(2), "ordersDb[0].Quantity");
            Assert.That(ordersDb[1].Quantity, Is.EqualTo(3), "ordersDb[1].Quantity");
            Assert.That(ordersDb[2].Quantity, Is.EqualTo(4), "ordersDb[2].Quantity");
            Assert.That(ordersDb[0].ProductId, Is.EqualTo(1), "ordersDb[0].ProductId");
            Assert.That(ordersDb[1].ProductId, Is.EqualTo(2), "ordersDb[1].ProductId");
            Assert.That(ordersDb[2].ProductId, Is.EqualTo(1), "ordersDb[2].ProductId");

            var aggregates = _orderAggregator.GetOrderAggregate();
            Assert.That(aggregates, Is.Not.Null, "aggregates");
            Assert.That(aggregates.Count, Is.EqualTo(2), "Count of aggregates");
            Assert.That(aggregates[1], Is.EqualTo(6), "ProductId = 1");
            Assert.That(aggregates[2], Is.EqualTo(3), "ProductId = 2");
        }
    }
}

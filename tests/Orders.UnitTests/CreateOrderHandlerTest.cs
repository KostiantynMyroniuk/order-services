using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Orders.API.Features.CreateOrder;
using Orders.API.Infrastructure;
using Orders.API.Infrastructure.Services;
using Orders.API.Models;

namespace Orders.UnitTests
{
    public class CreateOrderHandlerTest : IDisposable
    {
        private readonly SqliteConnection _sqliteConnection;
        private readonly OrdersDbContext _context;
        private readonly Mock<ICatalogClientService> _catalogClientMock = new();
        private readonly Mock<ILogger<CreateOrderCommandHandler>> _loggerMock = new();

        public CreateOrderHandlerTest()
        {
            _sqliteConnection = new SqliteConnection("DataSource=:memory:");
            _sqliteConnection.Open();

            var options = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseSqlite(_sqliteConnection)
                .Options;

            _context = new OrdersDbContext(options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Dispose();
            _sqliteConnection.Dispose();
        }

        private CreateOrderCommandHandler CreateHandler() => new CreateOrderCommandHandler(_context, _catalogClientMock.Object, _loggerMock.Object);
        private static Address CreateAddress() => new("City1", "Street1", "Country1", "Zipcode1");

        [Fact]
        public async Task Handle_WhenOrderExistsForRequestId_ReturnsExistingOrderIdWithoutCallingCatalog()
        {
            var requestId = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var address = CreateAddress();

            var existingOrder = new Order(requestId, userId, address);
            _context.Orders.Add(existingOrder);
            await _context.SaveChangesAsync();

            var command = new CreateOrderCommand(requestId, userId, address, [new OrderItemRequest(Guid.NewGuid(), 1)]);
            var handler = CreateHandler();

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(existingOrder.Id, result.Value);

            _catalogClientMock.Verify(x => 
                x.GetProductsByIds(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()), 
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenOrderDoesNotAlreadyExists_ReturnsNewOrderIdWithItemsFromCatalog()
        {
            var requestId = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var address = CreateAddress();

            var product1Id = Guid.NewGuid();
            var product2Id = Guid.NewGuid();

            var catalogProducts = new List<CatalogItem>
            {
                new(product1Id, "TestProduct1", 111.1m),
                new(product2Id, "TestProduct2", 222.2m)
            };

            _catalogClientMock
                    .Setup(x => x.GetProductsByIds(
                        It.Is<IEnumerable<Guid>>(ids => ids.OrderBy(id => id).SequenceEqual(new[] { product1Id, product2Id }.OrderBy(id => id))),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(catalogProducts);

            var orderItems = new List<OrderItemRequest>
            {
                new(product1Id, 1),
                new(product2Id, 2)
            };

            var command = new CreateOrderCommand(requestId, userId, address, orderItems);
            var handler = CreateHandler();

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);

            var dbOrder = await _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.RequestId == requestId);

            Assert.NotNull(dbOrder);
            Assert.Equal(result.Value, dbOrder.Id);
            Assert.Equal(userId, dbOrder.UserId);
            Assert.Equal(address, dbOrder.Address);
            Assert.Equal(OrderStatus.Submitted, dbOrder.Status);
            Assert.Equal(2, dbOrder.OrderItems.Count);

            _catalogClientMock.Verify(x => 
                x.GetProductsByIds(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}

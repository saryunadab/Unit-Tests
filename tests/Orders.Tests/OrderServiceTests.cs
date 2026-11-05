using Moq;
using Orders.Abstractions;
using Orders.Models;
using Orders.Services;

namespace Orders.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repoMock = new();
    private readonly Mock<IMessagePublisher> _pubMock = new();
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(_repoMock.Object, _pubMock.Object, new DiscountCalculator());
    }

    private static NewOrder CreateValidCommand() =>
        new(new List<OrderItem> { new("Книга", 300m, 2) });

    [Fact]
    public async Task CreateAsync_ValidOrder_SavesOrder()
    {
        var cmd = CreateValidCommand();
        await _sut.CreateAsync(cmd);
        _repoMock.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidOrder_PublishesEventWithSameId()
    {
        var cmd = CreateValidCommand();
        var id = await _sut.CreateAsync(cmd);

        _pubMock.Verify(p => p.PublishAsync(It.Is<OrderCreated>(e => e.OrderId == id)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SaveFails_DoesNotPublish()
    {
        var cmd = CreateValidCommand();
        _repoMock.Setup(r => r.SaveAsync(It.IsAny<Order>())).ThrowsAsync(new Exception("DB error"));

        await Assert.ThrowsAsync<Exception>(() => _sut.CreateAsync(cmd));
        _pubMock.Verify(p => p.PublishAsync(It.IsAny<OrderCreated>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_NullCommand_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CreateAsync(null!));
    }

    [Fact]
    public async Task CreateAsync_EmptyItems_ThrowsAndDoesNotSave()
    {
        var cmd = new NewOrder(new List<OrderItem>());

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(cmd));
        _repoMock.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_PublishFails_ExceptionPropagates()
    {
        var cmd = CreateValidCommand();
        _pubMock.Setup(p => p.PublishAsync(It.IsAny<OrderCreated>())).ThrowsAsync(new InvalidOperationException());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(cmd));
    }

    [Fact]
    public async Task GetPayableAsync_TotalBelowThreshold_NoDiscount()
    {
        var order = new Order(new List<OrderItem> { new("Товар", 500m, 1) });
        _repoMock.Setup(r => r.FindAsync(order.Id)).ReturnsAsync(order);

        var result = await _sut.GetPayableAsync(order.Id);
        Assert.Equal(500m, result);
    }

    [Fact]
    public async Task GetPayableAsync_TotalAboveThreshold_AppliesDiscount()
    {
        var order = new Order(new List<OrderItem> { new("Товар", 1500m, 1) });
        _repoMock.Setup(r => r.FindAsync(order.Id)).ReturnsAsync(order);

        var result = await _sut.GetPayableAsync(order.Id);
        Assert.Equal(1350m, result);
    }

    [Fact]
    public async Task GetPayableAsync_OrderNotFound_Throws()
    {
        _repoMock.Setup(r => r.FindAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetPayableAsync(Guid.NewGuid()));
    }
}
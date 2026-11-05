using Moq;
using Orders.Abstractions;
using Orders.Handlers;
using Orders.Models;

namespace Orders.Tests;

public class OrderCreatedHandlerTests
{
    private readonly Mock<INotificationSender> _senderMock = new();
    private readonly OrderCreatedHandler _sut;

    public OrderCreatedHandlerTests()
    {
        _sut = new OrderCreatedHandler(_senderMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidMessage_SendsNotification()
    {
        var id = Guid.NewGuid();
        var message = new OrderCreated(id);

        await _sut.HandleAsync(message);

        _senderMock.Verify(s => s.SendAsync(It.Is<string>(text => text.Contains(id.ToString()))), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_EmptyOrderId_Throws()
    {
        var message = new OrderCreated(Guid.Empty);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.HandleAsync(message));

        Assert.Contains("OrderId", ex.Message);
        _senderMock.Verify(s => s.SendAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_NullMessage_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.HandleAsync(null!));
    }
}
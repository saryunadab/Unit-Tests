using Orders.Models;
using Orders.Repositories;

namespace Orders.Tests;

public class InMemoryOrderRepositoryTests
{
    private readonly InMemoryOrderRepository _sut = new();

    [Fact]
    public async Task SaveAsync_ThenFindAsync_ReturnsSameOrder()
    {
        var order = new Order(new List<OrderItem> { new("Товар", 100m, 1) });

        await _sut.SaveAsync(order);
        var found = await _sut.FindAsync(order.Id);

        Assert.NotNull(found);
        Assert.Equal(order.Id, found.Id);
    }

    [Fact]
    public async Task FindAsync_UnknownId_ReturnsNull()
    {
        var result = await _sut.FindAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task SaveAsync_Null_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SaveAsync(null!));
    }
}
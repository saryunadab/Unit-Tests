using Orders.Models;

namespace Orders.Tests;

public class OrderTests
{
    // Готовый пример теста. Остальные тесты вы пишете по аналогии.
    [Fact]
    public void Total_TwoItems_ReturnsSum()
    {
        // Arrange
        var order = new Order();
        order.Add("Книга", 300m, 2);
        order.Add("Ручка", 50m, 1);

        // Act
        var total = order.Total();

        // Assert
        Assert.Equal(650m, total);
    }

    [Fact]
    public void Total_EmptyOrder_ReturnsZero()
    {
        var order = new Order();
        Assert.Equal(0m, order.Total());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Add_InvalidName_Throws(string? name)
    {
        var order = new Order();
        Assert.Throws<ArgumentException>(() => order.Add(name!, 100m, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Add_NonPositivePrice_Throws(decimal price)
    {
        var order = new Order();
        Assert.Throws<ArgumentOutOfRangeException>(() => order.Add("Книга", price, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void Add_InvalidQuantity_Throws(int quantity)
    {
        var order = new Order();
        Assert.Throws<ArgumentOutOfRangeException>(() => order.Add("Книга", 100m, quantity));
    }

    [Fact]
    public void Add_ValidItem_AddsToItems()
    {
        var order = new Order();
        order.Add("Книга", 300m, 2);

        Assert.Single(order.Items);
        Assert.Equal("Книга", order.Items[0].Name);
        Assert.Equal(300m, order.Items[0].Price);
        Assert.Equal(2, order.Items[0].Quantity);
    }

    [Fact]
    public void Constructor_WithItems_SameTotalAsAdd()
    {
        var items = new List<OrderItem> { new("Книга", 300m, 2), new("Ручка", 50m, 1) };
        var order = new Order(items);
        Assert.Equal(650m, order.Total());
    }
}

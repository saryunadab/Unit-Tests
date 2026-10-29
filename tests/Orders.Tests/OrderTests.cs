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
}

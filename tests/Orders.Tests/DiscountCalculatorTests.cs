using Orders.Services;

namespace Orders.Tests;

public class DiscountCalculatorTests
{
    private readonly DiscountCalculator _sut = new();

    [Theory]
    [InlineData(0, 0)]
    [InlineData(999.99, 0)]
    [InlineData(1000, 100)]
    [InlineData(2000, 200)]
    public void Calculate_ReturnsExpected(decimal total, decimal expectedDiscount)
    {
        Assert.Equal(expectedDiscount, _sut.Calculate(total));
    }

    [Fact]
    public void Calculate_NegativeTotal_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Calculate(-1));
    }
}
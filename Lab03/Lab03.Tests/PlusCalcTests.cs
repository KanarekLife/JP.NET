using Autofac;
using Lab03.Abstract;
using Lab03.Calculator;
using Xunit;

namespace Lab03.Tests;

public class PlusCalcTests : BaseTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_AddsNumbers_When_GivenNumericStrings_Should_ReturnSum(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var calc = container.ResolveNamed<ICalculator>(DI.PlusCalc);
        var result = calc.Eval("5", "3");

        // Assert
        Assert.Equal("8", result);
    }
}
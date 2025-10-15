using Autofac;
using Lab03.Abstract;
using Lab03.Calculator;
using Xunit;

namespace Lab03.Tests;

public class CatCalcTests : BaseTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_ConcatenatesWithSpace_When_GivenTwoStrings_Should_ReturnConcatenatedString(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var calc = container.ResolveNamed<ICalculator>(DI.CatCalc);
        var result = calc.Eval("Hello", "World");

        // Assert
        Assert.Equal("Hello World", result);
    }
}
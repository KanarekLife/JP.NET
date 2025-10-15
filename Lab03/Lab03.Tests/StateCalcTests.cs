using Autofac;
using Lab03.Abstract;
using Lab03.Calculator;
using Xunit;

namespace Lab03.Tests;

public class StateCalcTests : BaseTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_IsSingleton_When_ResolvedTwice_Should_ReturnSameInstance(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var calc1 = container.ResolveNamed<ICalculator>(DI.StateCalc);
        var calc2 = container.ResolveNamed<ICalculator>(DI.StateCalc);

        // Assert
        Assert.Same(calc1, calc2);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_SharesState_When_CalledMultipleTimes_Should_IncrementState(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var calc1 = container.ResolveNamed<ICalculator>(DI.StateCalc);
        var result1 = calc1.Eval("First", "Call");

        var calc2 = container.ResolveNamed<ICalculator>(DI.StateCalc);
        var result2 = calc2.Eval("Second", "Call");

        // Assert
        Assert.Equal("First Call 0", result1);
        Assert.Equal("Second Call 1", result2);
        Assert.Same(calc1, calc2);
    }
}
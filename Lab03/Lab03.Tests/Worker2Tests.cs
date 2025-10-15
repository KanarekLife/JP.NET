using Autofac;
using Xunit;
using Worker2 = Lab03.Worker.Worker2;

namespace Lab03.Tests;

public class Worker2Tests : BaseTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_AddsNumericStrings_When_Worker2UsesPlusCalc_Should_ReturnSum(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var worker = container.Resolve<Worker2>();
        var result = worker.Work("5", "3");

        // Assert
        Assert.Equal("8", result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_AppendsState_When_Worker2UsesStateCalc_Should_ReturnStatefulString(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var worker = container.ResolveNamed<Worker2>(DI.StateWorker);
        var result = worker.Work("X", "Y");

        // Assert
        Assert.Equal("X Y 0", result);
    }
}
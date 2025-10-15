using Autofac;
using Xunit;
using Worker3 = Lab03.Worker.Worker3;

namespace Lab03.Tests;

public class Worker3Tests : BaseTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_ConcatenatesStrings_When_Worker3UsesCatCalc_Should_ReturnConcatenatedString(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var worker = container.Resolve<Worker3>();
        var result = worker.Work("Test", "123");

        // Assert
        Assert.Equal("Test 123", result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_AppendsState_When_Worker3UsesStateCalc_Should_ReturnStatefulString(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var worker = container.ResolveNamed<Worker3>(DI.StateWorker);
        var result = worker.Work("M", "N");

        // Assert
        Assert.Equal("M N 0", result);
    }
}
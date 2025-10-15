using Autofac;
using Xunit;
using Worker1 = Lab03.Worker.Worker;

namespace Lab03.Tests;

public class WorkerTests : BaseTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_ConcatenatesStrings_When_WorkerUsesCatCalc_Should_ReturnConcatenatedString(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var worker = container.Resolve<Worker1>();
        var result = worker.Work("Hello", "World");

        // Assert
        Assert.Equal("Hello World", result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_AppendsState_When_WorkerUsesStateCalc_Should_ReturnStatefulString(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var worker = container.ResolveNamed<Worker1>(DI.StateWorker);
        var result = worker.Work("A", "B");

        // Assert
        Assert.Equal("A B 0", result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_CreatesSeparateInstances_When_ResolvedMultipleTimes_Should_NotBeSame(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var worker1 = container.Resolve<Worker1>();
        var worker2 = container.Resolve<Worker1>();

        // Assert
        Assert.NotSame(worker1, worker2);
    }
}
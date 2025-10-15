using Autofac;
using Lab03.Abstract;
using Lab03.Calculator;
using Lab03.Transaction;
using Microsoft.Extensions.Configuration;
using Xunit;
using Worker1 = Lab03.Worker.Worker;
using Worker2 = Lab03.Worker.Worker2;
using Worker3 = Lab03.Worker.Worker3;

namespace Lab03.Tests;

public class IntegrationTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_ResolvesAllComponents_When_ConfigurationApplied_Should_Succeed(bool useDeclarative)
    {
        // Arrange
        var builder = new ContainerBuilder();

        if (useDeclarative)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .Build();

            DI.ConfigureDeclarative(builder, config);
        }
        else
        {
            DI.ConfigureAll(builder);
        }

        using var container = builder.Build();

        // Act & Assert
        Assert.NotNull(container.Resolve<Worker1>());
        Assert.NotNull(container.Resolve<Worker2>());
        Assert.NotNull(container.Resolve<Worker3>());
        Assert.NotNull(container.ResolveNamed<ICalculator>(DI.CatCalc));
        Assert.NotNull(container.ResolveNamed<ICalculator>(DI.PlusCalc));
        Assert.NotNull(container.ResolveNamed<ICalculator>(DI.StateCalc));
    }

    [Fact]
    public void It_ResolvesTransactionComponents_When_UsingImperativeConfiguration_Should_Succeed()
    {
        // Arrange
        var builder = new ContainerBuilder();
        DI.ConfigureAll(builder);
        using var container = builder.Build();

        // Act & Assert
        Assert.NotNull(container.Resolve<TransactionProcessor>());

        using var scope = container.BeginLifetimeScope(TransactionProcessor.TransactionTag);
        Assert.NotNull(scope.Resolve<StepOneService>());
        Assert.NotNull(scope.Resolve<StepTwoService>());
        Assert.NotNull(scope.Resolve<ITransactionContext>());
    }
}
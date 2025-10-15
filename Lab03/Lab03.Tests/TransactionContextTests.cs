using System.Reflection;
using Autofac;
using Autofac.Core;
using Lab03.Abstract;
using Lab03.Transaction;
using Xunit;

namespace Lab03.Tests;

public class TransactionContextTests : BaseTests
{

    [Fact]
    public void It_SharedContext_When_ResolvedWithinTransactionScope_Should_BeSameForBothServices()
    {
        // Arrange
        using var container = CreateContainer(false);
        using var transactionScope = container.BeginLifetimeScope(
            TransactionProcessor.TransactionTag
        );

        // Act
        var step1 = transactionScope.Resolve<StepOneService>();
        var step2 = transactionScope.Resolve<StepTwoService>();

        var context1 = GetPrivateField<ITransactionContext>(step1, "_context");
        var context2 = GetPrivateField<ITransactionContext>(step2, "_context");

        // Assert
        Assert.NotNull(context1);
        Assert.NotNull(context2);
        Assert.Same(context1, context2);
        Assert.Equal(context1.TransactionId, context2.TransactionId);
    }

    [Fact]
    public void It_SeparateScopes_When_ResolvedInDifferentTransactionScopes_Should_BeDifferentInstances()
    {
        // Arrange
        using var container = CreateContainer(false);

        ITransactionContext context1;
        ITransactionContext context2;

        // Act
        using (var scope1 = container.BeginLifetimeScope(TransactionProcessor.TransactionTag))
        {
            var step1 = scope1.Resolve<StepOneService>();
            context1 = GetPrivateField<ITransactionContext>(step1, "_context");
        }

        using (var scope2 = container.BeginLifetimeScope(TransactionProcessor.TransactionTag))
        {
            var step2 = scope2.Resolve<StepOneService>();
            context2 = GetPrivateField<ITransactionContext>(step2, "_context");
        }

        // Assert
        Assert.NotNull(context1);
        Assert.NotNull(context2);
        Assert.NotSame(context1, context2);
        Assert.NotEqual(context1.TransactionId, context2.TransactionId);
    }

    [Fact]
    public void It_ThrowsDependencyResolution_When_ResolvedOutsideTransactionScope_Should_Fail()
    {
        // Arrange
        using var container = CreateContainer(false);
        using var regularScope = container.BeginLifetimeScope();

        // Act & Assert
        Assert.Throws<DependencyResolutionException>(() =>
        {
            var step = regularScope.Resolve<StepOneService>();
        });
    }

    [Fact]
    public void It_ProcessesTransaction_When_ProcessorResolved_Should_UseSharedContext()
    {
        // Arrange
        using var container = CreateContainer();
        var processor = container.Resolve<TransactionProcessor>();

        // Act & Assert
        processor.ProcessTransaction();
    }

    [Fact]
    public void It_GeneratesUniqueIds_When_MultipleTransactionsExecuted_Should_HaveDistinctIds()
    {
        // Arrange
        using var container = CreateContainer();
        var ids = new List<Guid>();

        // Act
        for (var i = 0; i < 5; i++)
        {
            using var scope = container.BeginLifetimeScope(TransactionProcessor.TransactionTag);
            var step = scope.Resolve<StepOneService>();
            var context = GetPrivateField<ITransactionContext>(step, "_context");
            ids.Add(context.TransactionId);
        }

        // Assert
        Assert.Equal(5, ids.Distinct().Count());
    }

    [Fact]
    public void It_CreatesSeparateTransactions_When_ExecutedMultipleTimes_Should_WriteStartMessages()
    {
        // Arrange
        using var container = CreateContainer();
        var processor = container.Resolve<TransactionProcessor>();
        var originalOut = Console.Out;
        var outputs = new List<string>();

        // Act
        try
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);

            processor.ProcessTransaction();
            processor.ProcessTransaction();

            var output = sw.ToString();
            outputs.AddRange(output.Split('\n', StringSplitOptions.RemoveEmptyEntries));
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        // Assert
        var startCount = outputs.Count(line => line.Contains("Starting new transaction"));
        Assert.Equal(2, startCount);
    }

    private static T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType()
            .GetField(
                fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance
            );

        if (field == null)
            throw new ArgumentException($"Field {fieldName} not found");

        return (T)field.GetValue(obj)!;
    }
}
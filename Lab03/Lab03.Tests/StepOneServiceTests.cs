using System.Reflection;
using Autofac;
using Lab03.Abstract;
using Lab03.Transaction;
using Xunit;

namespace Lab03.Tests;

public class StepOneServiceTests : BaseTests
{
    [Theory]
    [InlineData(false)]
    public void It_Resolves_When_InTransactionScope_Should_ReturnStepOneService(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);
        using var transactionScope = container.BeginLifetimeScope(
            TransactionProcessor.TransactionTag
        );

        // Act
        var step1 = transactionScope.Resolve<StepOneService>();

        // Assert
        Assert.NotNull(step1);
    }

    [Theory]
    [InlineData(false)]
    public void It_HasTransactionContext_When_ResolvedInTransactionScope_Should_HaveValidTransactionId(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);
        using var transactionScope = container.BeginLifetimeScope(
            TransactionProcessor.TransactionTag
        );

        // Act
        var step1 = transactionScope.Resolve<StepOneService>();
        var context = GetPrivateField<ITransactionContext>(step1, "_context");

        // Assert
        Assert.NotNull(context);
        Assert.NotEqual(Guid.Empty, context.TransactionId);
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
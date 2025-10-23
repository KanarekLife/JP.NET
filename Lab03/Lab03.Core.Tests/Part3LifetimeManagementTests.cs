using Autofac;
using Lab03.Core;
using Lab03.Core.Abstract;

namespace Lab03.Core.Tests;

/// <summary>
/// Unit tests for Part 3: Advanced Lifetime Management
/// Tests InstancePerLifetimeScope and InstancePerMatchingLifetimeScope
/// </summary>
[TestFixture]
public class Part3LifetimeManagementTests
{
    /// <summary>
    /// Gets container based on configuration type
    /// </summary>
    private IContainer GetContainer(bool useImperative)
    {
        return useImperative
            ? ImperativeAutofacConfig.ConfigureContainer()
            : DeclarativeAutofacConfig.ConfigureContainer();
    }

    #region IUnitOfWork - InstancePerLifetimeScope Tests

    [Test]
    [TestCase(true, TestName = "UnitOfWork_SameInstanceWithinScope_Imperative")]
    [TestCase(false, TestName = "UnitOfWork_SameInstanceWithinScope_Declarative")]
    public void UnitOfWork_InstancePerLifetimeScope_ReturnsSameInstanceWithinScope(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act
        using var scope = container.BeginLifetimeScope();
        var uow1 = scope.Resolve<IUnitOfWork>();
        var uow2 = scope.Resolve<IUnitOfWork>();

        // Assert
        Assert.That(ReferenceEquals(uow1, uow2), Is.True,
            "Within the same lifetime scope, IUnitOfWork should return the same instance");
        Assert.That(uow1.Id, Is.EqualTo(uow2.Id),
            "Same instance should have the same ID");
        Assert.That(uow1.GetHashCode(), Is.EqualTo(uow2.GetHashCode()),
            "Same instance should have the same hash code");
    }

    [Test]
    [TestCase(true, TestName = "UnitOfWork_DifferentInstancesAcrossScopes_Imperative")]
    [TestCase(false, TestName = "UnitOfWork_DifferentInstancesAcrossScopes_Declarative")]
    public void UnitOfWork_InstancePerLifetimeScope_ReturnsDifferentInstancesInDifferentScopes(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        IUnitOfWork? uow1 = null;
        IUnitOfWork? uow2 = null;

        // Act
        using (var scope1 = container.BeginLifetimeScope())
        {
            uow1 = scope1.Resolve<IUnitOfWork>();
        }

        using (var scope2 = container.BeginLifetimeScope())
        {
            uow2 = scope2.Resolve<IUnitOfWork>();
        }

        // Assert
        Assert.That(uow1, Is.Not.Null);
        Assert.That(uow2, Is.Not.Null);
        Assert.That(ReferenceEquals(uow1, uow2), Is.False,
            "Different lifetime scopes should get different IUnitOfWork instances");
        Assert.That(uow1.Id, Is.Not.EqualTo(uow2.Id),
            "Different instances should have different IDs");
    }

    [Test]
    [TestCase(true, TestName = "UnitOfWork_DifferentFromRootScope_Imperative")]
    [TestCase(false, TestName = "UnitOfWork_DifferentFromRootScope_Declarative")]
    public void UnitOfWork_InstancePerLifetimeScope_ChildScopeGetsDifferentInstanceThanRoot(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act
        var rootUow = container.Resolve<IUnitOfWork>();
        
        using var childScope = container.BeginLifetimeScope();
        var childUow = childScope.Resolve<IUnitOfWork>();

        // Assert
        Assert.That(ReferenceEquals(rootUow, childUow), Is.False,
            "Child scope should get different instance than root scope");
        Assert.That(rootUow.Id, Is.Not.EqualTo(childUow.Id),
            "Different scopes should have different IDs");
    }

    [Test]
    [TestCase(true, TestName = "UnitOfWork_NestedScopes_EachGetsOwnInstance_Imperative")]
    [TestCase(false, TestName = "UnitOfWork_NestedScopes_EachGetsOwnInstance_Declarative")]
    public void UnitOfWork_InstancePerLifetimeScope_NestedScopesGetOwnInstances(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var ids = new List<Guid>();

        // Act
        using (var scope1 = container.BeginLifetimeScope())
        {
            var uow1 = scope1.Resolve<IUnitOfWork>();
            ids.Add(uow1.Id);

            using (var scope2 = scope1.BeginLifetimeScope())
            {
                var uow2 = scope2.Resolve<IUnitOfWork>();
                ids.Add(uow2.Id);

                using (var scope3 = scope2.BeginLifetimeScope())
                {
                    var uow3 = scope3.Resolve<IUnitOfWork>();
                    ids.Add(uow3.Id);
                }
            }
        }

        // Assert
        Assert.That(ids.Count, Is.EqualTo(3));
        Assert.That(ids.Distinct().Count(), Is.EqualTo(3),
            "Each nested scope should have its own unique UnitOfWork instance");
    }

    [Test]
    [TestCase(true, TestName = "UnitOfWork_MultipleResolvesInScope_GeneratesSingleId_Imperative")]
    [TestCase(false, TestName = "UnitOfWork_MultipleResolvesInScope_GeneratesSingleId_Declarative")]
    public void UnitOfWork_InstancePerLifetimeScope_SingleIdPerScope(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act
        using var scope = container.BeginLifetimeScope();
        var ids = new List<Guid>();
        
        for (int i = 0; i < 5; i++)
        {
            var uow = scope.Resolve<IUnitOfWork>();
            ids.Add(uow.Id);
        }

        // Assert
        Assert.That(ids.Distinct().Count(), Is.EqualTo(1),
            "All resolves within the same scope should return the same ID");
    }

    #endregion

    #region ITransactionContext - InstancePerMatchingLifetimeScope Tests

    [Test]
    [TestCase(true, TestName = "TransactionContext_SameInstanceInTransactionScope_Imperative")]
    [TestCase(false, TestName = "TransactionContext_SameInstanceInTransactionScope_Declarative")]
    public void TransactionContext_InstancePerMatchingLifetimeScope_ReturnsSameInstanceInTransactionScope(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act
        using var txScope = container.BeginLifetimeScope("transaction");
        var ctx1 = txScope.Resolve<ITransactionContext>();
        var ctx2 = txScope.Resolve<ITransactionContext>();

        // Assert
        Assert.That(ReferenceEquals(ctx1, ctx2), Is.True,
            "Within the same 'transaction' scope, ITransactionContext should return the same instance");
        Assert.That(ctx1.TransactionId, Is.EqualTo(ctx2.TransactionId),
            "Same instance should have the same TransactionId");
    }

    [Test]
    [TestCase(true, TestName = "TransactionContext_DifferentInstancesAcrossTransactionScopes_Imperative")]
    [TestCase(false, TestName = "TransactionContext_DifferentInstancesAcrossTransactionScopes_Declarative")]
    public void TransactionContext_InstancePerMatchingLifetimeScope_ReturnsDifferentInstancesInDifferentScopes(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        ITransactionContext? ctx1 = null;
        ITransactionContext? ctx2 = null;

        // Act
        using (var txScope1 = container.BeginLifetimeScope("transaction"))
        {
            ctx1 = txScope1.Resolve<ITransactionContext>();
        }

        using (var txScope2 = container.BeginLifetimeScope("transaction"))
        {
            ctx2 = txScope2.Resolve<ITransactionContext>();
        }

        // Assert
        Assert.That(ctx1, Is.Not.Null);
        Assert.That(ctx2, Is.Not.Null);
        Assert.That(ReferenceEquals(ctx1, ctx2), Is.False,
            "Different 'transaction' scopes should get different ITransactionContext instances");
        Assert.That(ctx1.TransactionId, Is.Not.EqualTo(ctx2.TransactionId),
            "Different transaction scopes should have different TransactionIds");
    }

    [Test]
    [TestCase(true, TestName = "TransactionContext_SharedBetweenServices_Imperative")]
    [TestCase(false, TestName = "TransactionContext_SharedBetweenServices_Declarative")]
    public void TransactionContext_InstancePerMatchingLifetimeScope_SharedBetweenStepServices(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act
        using var txScope = container.BeginLifetimeScope("transaction");
        var stepOne = txScope.Resolve<StepOneService>();
        var stepTwo = txScope.Resolve<StepTwoService>();
        var context = txScope.Resolve<ITransactionContext>();

        // Execute steps to ensure they use the context
        stepOne.Execute("TestData");
        stepTwo.Execute("TestData");

        var logs = context.GetLogs();

        // Assert
        Assert.That(logs.Count, Is.GreaterThan(0),
            "Context should have collected logs from both services");
        Assert.That(logs.Any(l => l.Contains("StepOneService")), Is.True,
            "Logs should contain entries from StepOneService");
        Assert.That(logs.Any(l => l.Contains("StepTwoService")), Is.True,
            "Logs should contain entries from StepTwoService");
        Assert.That(logs.Any(l => l.Contains(context.TransactionId.ToString())), Is.True,
            "Log entries should reference the TransactionId");
    }

    [Test]
    [TestCase(true, TestName = "TransactionContext_IsolatedLogsPerScope_Imperative")]
    [TestCase(false, TestName = "TransactionContext_IsolatedLogsPerScope_Declarative")]
    public void TransactionContext_InstancePerMatchingLifetimeScope_LogsAreIsolatedPerTransactionScope(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        int logs1Count, logs2Count;

        // Act
        using (var txScope1 = container.BeginLifetimeScope("transaction"))
        {
            var stepOne = txScope1.Resolve<StepOneService>();
            var context1 = txScope1.Resolve<ITransactionContext>();
            
            stepOne.Execute("Transaction1Data");
            logs1Count = context1.GetLogs().Count;
        }

        using (var txScope2 = container.BeginLifetimeScope("transaction"))
        {
            var stepTwo = txScope2.Resolve<StepTwoService>();
            var context2 = txScope2.Resolve<ITransactionContext>();
            
            stepTwo.Execute("Transaction2Data");
            logs2Count = context2.GetLogs().Count;
        }

        // Assert
        Assert.That(logs1Count, Is.GreaterThan(0),
            "First transaction should have logged entries");
        Assert.That(logs2Count, Is.GreaterThan(0),
            "Second transaction should have logged entries");
        Assert.That(logs2Count, Is.LessThanOrEqualTo(logs1Count),
            "Second transaction should have its own isolated log collection, not accumulated logs");
    }

    [Test]
    [TestCase(true, TestName = "TransactionContext_ServicesShareSameContext_Imperative")]
    [TestCase(false, TestName = "TransactionContext_ServicesShareSameContext_Declarative")]
    public void TransactionContext_StepOneAndStepTwoServices_ShareSameContextInstance(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act
        using var txScope = container.BeginLifetimeScope("transaction");
        
        var stepOne = txScope.Resolve<StepOneService>();
        var stepTwo = txScope.Resolve<StepTwoService>();
        var directContext = txScope.Resolve<ITransactionContext>();

        // Execute steps
        stepOne.Execute("SharedTest");
        var logsAfterStepOne = directContext.GetLogs().Count;
        
        stepTwo.Execute("SharedTest");
        var logsAfterStepTwo = directContext.GetLogs().Count;

        // Assert
        Assert.That(logsAfterStepTwo, Is.GreaterThan(logsAfterStepOne),
            "Both services should be logging to the same context instance");
        
        var allLogs = directContext.GetLogs();
        Assert.That(allLogs.Any(l => l.Contains("StepOneService")), Is.True);
        Assert.That(allLogs.Any(l => l.Contains("StepTwoService")), Is.True);
    }

    [Test]
    [TestCase(true, TestName = "TransactionContext_NonMatchingScopeThrows_Imperative")]
    [TestCase(false, TestName = "TransactionContext_NonMatchingScopeThrows_Declarative")]
    public void TransactionContext_InstancePerMatchingLifetimeScope_ThrowsWhenResolvedFromNonMatchingScope(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act & Assert
        using var regularScope = container.BeginLifetimeScope(); // No "transaction" tag
        
        Assert.Throws<Autofac.Core.DependencyResolutionException>(() => 
            regularScope.Resolve<ITransactionContext>(),
            "ITransactionContext should only be resolvable from 'transaction' tagged scope");
    }

    #endregion

    #region TransactionProcessor Integration Tests

    [Test]
    [TestCase(true, TestName = "TransactionProcessor_CreatesIsolatedTransactions_Imperative")]
    [TestCase(false, TestName = "TransactionProcessor_CreatesIsolatedTransactions_Declarative")]
    public void TransactionProcessor_ProcessesTransactionsWithIsolatedContexts(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var processor = container.Resolve<TransactionProcessor>();
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        processor.ProcessTransaction("Data1");
        var output1 = output.ToString();
        output.GetStringBuilder().Clear();

        processor.ProcessTransaction("Data2");
        var output2 = output.ToString();

        // Assert
        Assert.That(output1, Does.Contain("Transaction"));
        Assert.That(output2, Does.Contain("Transaction"));
        
        // Each should have its own transaction ID
        Assert.That(output1, Does.Contain("Starting Transaction"));
        Assert.That(output2, Does.Contain("Starting Transaction"));
    }

    [Test]
    [TestCase(true, TestName = "TransactionProcessor_CanBeResolvedAndExecuted_Imperative")]
    [TestCase(false, TestName = "TransactionProcessor_CanBeResolvedAndExecuted_Declarative")]
    public void TransactionProcessor_CanBeResolvedAndExecutesSuccessfully(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var processor = container.Resolve<TransactionProcessor>();
            var output = new StringWriter();
            Console.SetOut(output);
            processor.ProcessTransaction("TestData");
        });
    }

    #endregion

    #region Combined Lifetime Tests

    [Test]
    [TestCase(true, TestName = "AllLifetimeScopes_WorkCorrectly_Imperative")]
    [TestCase(false, TestName = "AllLifetimeScopes_WorkCorrectly_Declarative")]
    public void AllLifetimeScopes_ConfiguredCorrectly(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act & Assert - Singleton (StateCalc)
        var calc1 = container.ResolveNamed<ICalculator>("state");
        var calc2 = container.ResolveNamed<ICalculator>("state");
        Assert.That(ReferenceEquals(calc1, calc2), Is.True, "StateCalc should be singleton");

        // Act & Assert - InstancePerLifetimeScope (UnitOfWork)
        using (var scope = container.BeginLifetimeScope())
        {
            var uow1 = scope.Resolve<IUnitOfWork>();
            var uow2 = scope.Resolve<IUnitOfWork>();
            Assert.That(ReferenceEquals(uow1, uow2), Is.True, "UnitOfWork should be scoped");
        }

        // Act & Assert - InstancePerMatchingLifetimeScope (TransactionContext)
        using (var txScope = container.BeginLifetimeScope("transaction"))
        {
            var ctx1 = txScope.Resolve<ITransactionContext>();
            var ctx2 = txScope.Resolve<ITransactionContext>();
            Assert.That(ReferenceEquals(ctx1, ctx2), Is.True, "TransactionContext should be scoped to transaction");
        }

        // Act & Assert - Transient (Services)
        // Services must be resolved within a transaction scope because they depend on ITransactionContext
        using (var txScope2 = container.BeginLifetimeScope("transaction"))
        {
            var svc1 = txScope2.Resolve<StepOneService>();
            var svc2 = txScope2.Resolve<StepOneService>();
            Assert.That(ReferenceEquals(svc1, svc2), Is.False, "Services should be transient");
        }
    }

    #endregion
}

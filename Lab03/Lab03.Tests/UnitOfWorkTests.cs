using Autofac;
using Lab03.Abstract;
using Lab03.UnitOfWork;
using Xunit;

namespace Lab03.Tests;

public class UnitOfWorkTests : BaseTests
{
    [Theory]
    [InlineData(false)]
    public void It_ReturnsSameInstance_When_InSameScope_Should_BeSingletonWithinScope(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);
        using var scope = container.BeginLifetimeScope();

        // Act
        var uow1 = scope.Resolve<IUnitOfWork>();
        var uow2 = scope.Resolve<IUnitOfWork>();

        // Assert
        Assert.Same(uow1, uow2);
        Assert.Equal(uow1.Id, uow2.Id);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_ReturnsDifferentInstances_When_InDifferentScopes_Should_NotBeSame(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        IUnitOfWork uow1;
        IUnitOfWork uow2;

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
        Assert.NotSame(uow1, uow2);
        Assert.NotEqual(uow1.Id, uow2.Id);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_ReturnsDifferentInstances_When_NestedScopes_Should_NotShareInstances(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);
        using var outerScope = container.BeginLifetimeScope();

        // Act
        var outerUow = outerScope.Resolve<IUnitOfWork>();

        IUnitOfWork innerUow;
        using (var innerScope = outerScope.BeginLifetimeScope())
        {
            innerUow = innerScope.Resolve<IUnitOfWork>();
        }

        // Assert
        Assert.NotSame(outerUow, innerUow);
        Assert.NotEqual(outerUow.Id, innerUow.Id);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_ProvidesUniqueInstances_When_ThreeLevelNesting_Should_HaveDistinctInstances(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);

        // Act
        var ids = new List<Guid>();

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
        Assert.Equal(3, ids.Distinct().Count());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void It_ProducesDifferentInstances_When_ParallelScopes_Should_NotBeSame(bool useDeclarative)
    {
        // Arrange
        using var container = CreateContainer(useDeclarative);
        using var parentScope = container.BeginLifetimeScope();

        // Act
        IUnitOfWork uow1;
        IUnitOfWork uow2;

        using (var scope1 = parentScope.BeginLifetimeScope())
        {
            uow1 = scope1.Resolve<IUnitOfWork>();
        }

        using (var scope2 = parentScope.BeginLifetimeScope())
        {
            uow2 = scope2.Resolve<IUnitOfWork>();
        }

        // Assert
        Assert.NotSame(uow1, uow2);
        Assert.NotEqual(uow1.Id, uow2.Id);
    }

    [Fact]
    public void It_ResolvesFromRootContainer_When_Requested_Should_Succeed()
    {
        // Arrange
        var builder = new ContainerBuilder();
        DI.ConfigureAll(builder);
        using var container = builder.Build();

        // Act
        var uow = container.Resolve<IUnitOfWork>();

        // Assert
        Assert.NotNull(uow);
        Assert.NotEqual(Guid.Empty, uow.Id);
    }
}
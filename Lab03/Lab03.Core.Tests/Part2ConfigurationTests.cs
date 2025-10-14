using Autofac;
using Lab03.Core;
using Lab03.Core.Abstract;

namespace Lab03.Core.Tests;

/// <summary>
/// Unit tests for Part 2: Basic Autofac Configuration
/// Tests both imperative and declarative configurations
/// </summary>
[TestFixture]
public class Part2ConfigurationTests
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

    #region Default Worker Tests (CatCalc)

    [Test]
    [TestCase(true, TestName = "DefaultWorker_UsesCatCalc_Imperative")]
    [TestCase(false, TestName = "DefaultWorker_UsesCatCalc_Declarative")]
    public void DefaultWorker_UsesCatCalc_ConcatenatesStrings(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var worker = container.Resolve<Worker>();
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker.Work("Hello", "World");

        // Assert
        var result = output.ToString().Trim();
        Assert.That(result, Is.EqualTo("HelloWorld"));
    }

    [Test]
    [TestCase(true, TestName = "DefaultWorker_CatCalc_MultipleInputs_Imperative")]
    [TestCase(false, TestName = "DefaultWorker_CatCalc_MultipleInputs_Declarative")]
    public void DefaultWorker_CatCalc_HandlesMultipleInputs(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var worker = container.Resolve<Worker>();
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker.Work("Foo", "Bar");

        // Assert
        var result = output.ToString().Trim();
        Assert.That(result, Is.EqualTo("FooBar"));
    }

    #endregion

    #region Default Worker2 Tests (PlusCalc)

    [Test]
    [TestCase(true, TestName = "DefaultWorker2_UsesPlusCalc_Imperative")]
    [TestCase(false, TestName = "DefaultWorker2_UsesPlusCalc_Declarative")]
    public void DefaultWorker2_UsesPlusCalc_AddsNumbers(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var worker2 = container.Resolve<Worker2>();
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker2.Work("100", "200");

        // Assert
        var result = output.ToString().Trim();
        Assert.That(result, Is.EqualTo("300"));
    }

    [Test]
    [TestCase(true, TestName = "DefaultWorker2_PlusCalc_DifferentNumbers_Imperative")]
    [TestCase(false, TestName = "DefaultWorker2_PlusCalc_DifferentNumbers_Declarative")]
    public void DefaultWorker2_PlusCalc_AddsDifferentNumbers(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var worker2 = container.Resolve<Worker2>();
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker2.Work("123", "111");

        // Assert
        var result = output.ToString().Trim();
        Assert.That(result, Is.EqualTo("234"));
    }

    #endregion

    #region Default Worker3 Tests (CatCalc)

    [Test]
    [TestCase(true, TestName = "DefaultWorker3_UsesCatCalc_Imperative")]
    [TestCase(false, TestName = "DefaultWorker3_UsesCatCalc_Declarative")]
    public void DefaultWorker3_UsesCatCalc_ConcatenatesStrings(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var worker3 = container.Resolve<Worker3>();
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker3.Work("Test", "123");

        // Assert
        var result = output.ToString().Trim();
        Assert.That(result, Is.EqualTo("Test123"));
    }

    [Test]
    [TestCase(true, TestName = "DefaultWorker3_CatCalc_EmptyStrings_Imperative")]
    [TestCase(false, TestName = "DefaultWorker3_CatCalc_EmptyStrings_Declarative")]
    public void DefaultWorker3_CatCalc_HandlesEmptyStrings(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var worker3 = container.Resolve<Worker3>();
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker3.Work("", "Value");

        // Assert
        var result = output.ToString().Trim();
        Assert.That(result, Is.EqualTo("Value"));
    }

    #endregion

    #region Named Worker "state" Tests (StateCalc)

    [Test]
    [TestCase(true, TestName = "NamedWorker_State_UsesStateCalc_Imperative")]
    [TestCase(false, TestName = "NamedWorker_State_UsesStateCalc_Declarative")]
    public void NamedWorker_State_UsesStateCalc_WithCounter(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var workerState = container.ResolveNamed<Worker>("state");
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        workerState.Work("A", "B");
        var firstResult = output.ToString().Trim();
        output.GetStringBuilder().Clear();

        workerState.Work("C", "D");
        var secondResult = output.ToString().Trim();

        // Assert
        Assert.That(firstResult, Is.EqualTo("AB1"));
        Assert.That(secondResult, Is.EqualTo("CD2"));
    }

    [Test]
    [TestCase(true, TestName = "NamedWorker_State_CounterIncrementsCorrectly_Imperative")]
    [TestCase(false, TestName = "NamedWorker_State_CounterIncrementsCorrectly_Declarative")]
    public void NamedWorker_State_CounterIncrementsCorrectly(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var workerState = container.ResolveNamed<Worker>("state");
        var output = new StringWriter();
        Console.SetOut(output);

        // Act & Assert
        workerState.Work("Test", "1");
        Assert.That(output.ToString().Trim(), Does.EndWith("1"));
        output.GetStringBuilder().Clear();

        workerState.Work("Test", "2");
        Assert.That(output.ToString().Trim(), Does.EndWith("2"));
        output.GetStringBuilder().Clear();

        workerState.Work("Test", "3");
        Assert.That(output.ToString().Trim(), Does.EndWith("3"));
    }

    #endregion

    #region Named Worker2 "state" Tests (StateCalc)

    [Test]
    [TestCase(true, TestName = "NamedWorker2_State_UsesStateCalc_Imperative")]
    [TestCase(false, TestName = "NamedWorker2_State_UsesStateCalc_Declarative")]
    public void NamedWorker2_State_UsesStateCalc_WithCounter(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var worker2State = container.ResolveNamed<Worker2>("state");
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker2State.Work("X", "Y");
        var firstResult = output.ToString().Trim();
        output.GetStringBuilder().Clear();

        worker2State.Work("Z", "W");
        var secondResult = output.ToString().Trim();

        // Assert - Counter continues from previous tests due to singleton
        Assert.That(firstResult, Does.Match(@"XY\d+"));
        Assert.That(secondResult, Does.Match(@"ZW\d+"));
    }

    #endregion

    #region Named Worker3 "state" Tests (StateCalc)

    [Test]
    [TestCase(true, TestName = "NamedWorker3_State_UsesStateCalc_Imperative")]
    [TestCase(false, TestName = "NamedWorker3_State_UsesStateCalc_Declarative")]
    public void NamedWorker3_State_UsesStateCalc_WithCounter(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var worker3State = container.ResolveNamed<Worker3>("state");
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker3State.Work("P", "Q");
        var firstResult = output.ToString().Trim();
        output.GetStringBuilder().Clear();

        worker3State.Work("R", "S");
        var secondResult = output.ToString().Trim();

        // Assert - Counter continues from previous tests due to singleton
        Assert.That(firstResult, Does.Match(@"PQ\d+"));
        Assert.That(secondResult, Does.Match(@"RS\d+"));
    }

    #endregion

    #region StateCalc Singleton Tests

    [Test]
    [TestCase(true, TestName = "StateCalc_IsSingleton_SameInstanceFromContainer_Imperative")]
    [TestCase(false, TestName = "StateCalc_IsSingleton_SameInstanceFromContainer_Declarative")]
    public void StateCalc_IsSingleton_ReturnsSameInstance(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act
        var calc1 = container.ResolveNamed<ICalculator>("state");
        var calc2 = container.ResolveNamed<ICalculator>("state");

        // Assert
        Assert.That(ReferenceEquals(calc1, calc2), Is.True, 
            "StateCalc should be registered as singleton and return the same instance");
        Assert.That(calc1.GetHashCode(), Is.EqualTo(calc2.GetHashCode()),
            "Same instance should have the same hash code");
    }

    [Test]
    [TestCase(true, TestName = "StateCalc_IsSingleton_SharedStateAcrossResolves_Imperative")]
    [TestCase(false, TestName = "StateCalc_IsSingleton_SharedStateAcrossResolves_Declarative")]
    public void StateCalc_IsSingleton_SharesStateBetweenResolves(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var calc1 = container.ResolveNamed<ICalculator>("state");
        var calc2 = container.ResolveNamed<ICalculator>("state");

        // Act
        var result1 = calc1.Eval("First", "Call");
        var result2 = calc2.Eval("Second", "Call");

        // Assert
        Assert.That(result1, Does.Match(@"FirstCall\d+"));
        Assert.That(result2, Does.Match(@"SecondCall\d+"));
        
        // Extract numbers and verify second is greater
        var num1 = int.Parse(new string(result1.Where(char.IsDigit).ToArray()));
        var num2 = int.Parse(new string(result2.Where(char.IsDigit).ToArray()));
        Assert.That(num2, Is.GreaterThan(num1), 
            "Counter should increment, proving shared state");
    }

    [Test]
    [TestCase(true, TestName = "StateCalc_IsSingleton_MultipleWorkersShareState_Imperative")]
    [TestCase(false, TestName = "StateCalc_IsSingleton_MultipleWorkersShareState_Declarative")]
    public void StateCalc_IsSingleton_MultipleWorkersShareSameCalculator(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);
        var worker1 = container.ResolveNamed<Worker>("state");
        var worker2 = container.ResolveNamed<Worker2>("state");
        var worker3 = container.ResolveNamed<Worker3>("state");
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker1.Work("W1", "");
        var result1 = output.ToString().Trim();
        output.GetStringBuilder().Clear();

        worker2.Work("W2", "");
        var result2 = output.ToString().Trim();
        output.GetStringBuilder().Clear();

        worker3.Work("W3", "");
        var result3 = output.ToString().Trim();

        // Assert - All should have incrementing counters
        Assert.That(result1, Does.Match(@"W1\d+"));
        Assert.That(result2, Does.Match(@"W2\d+"));
        Assert.That(result3, Does.Match(@"W3\d+"));
        
        var num1 = int.Parse(new string(result1.Where(char.IsDigit).ToArray()));
        var num2 = int.Parse(new string(result2.Where(char.IsDigit).ToArray()));
        var num3 = int.Parse(new string(result3.Where(char.IsDigit).ToArray()));
        
        Assert.That(num2, Is.GreaterThan(num1));
        Assert.That(num3, Is.GreaterThan(num2));
    }

    #endregion

    #region Integration Tests

    [Test]
    [TestCase(true, TestName = "AllWorkers_ResolveSuccessfully_Imperative")]
    [TestCase(false, TestName = "AllWorkers_ResolveSuccessfully_Declarative")]
    public void AllWorkers_CanBeResolvedFromContainer(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act & Assert
        Assert.DoesNotThrow(() => container.Resolve<Worker>());
        Assert.DoesNotThrow(() => container.Resolve<Worker2>());
        Assert.DoesNotThrow(() => container.Resolve<Worker3>());
        Assert.DoesNotThrow(() => container.ResolveNamed<Worker>("state"));
        Assert.DoesNotThrow(() => container.ResolveNamed<Worker2>("state"));
        Assert.DoesNotThrow(() => container.ResolveNamed<Worker3>("state"));
    }

    [Test]
    [TestCase(true, TestName = "AllCalculators_ResolveSuccessfully_Imperative")]
    [TestCase(false, TestName = "AllCalculators_ResolveSuccessfully_Declarative")]
    public void AllCalculators_CanBeResolvedFromContainer(bool useImperative)
    {
        // Arrange
        var container = GetContainer(useImperative);

        // Act & Assert
        Assert.DoesNotThrow(() => container.ResolveNamed<ICalculator>("cat"));
        Assert.DoesNotThrow(() => container.ResolveNamed<ICalculator>("plus"));
        Assert.DoesNotThrow(() => container.ResolveNamed<ICalculator>("state"));
    }

    #endregion
}

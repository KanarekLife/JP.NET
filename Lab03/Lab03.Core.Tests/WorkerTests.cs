using Lab03.Core;

namespace Lab03.Core.Tests;

/// <summary>
/// Unit tests for Worker, Worker2, and Worker3 classes
/// </summary>
[TestFixture]
public class WorkerTests
{
    #region Worker (Constructor Injection) Tests

    [Test]
    public void Worker_WithCatCalc_ConcatenatesCorrectly()
    {
        // Arrange
        var calc = new CatCalc();
        var worker = new Worker(calc);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker.Work("Hello", "World");

        // Assert
        var result = output.ToString().Trim();
        Assert.That(result, Is.EqualTo("HelloWorld"));
    }

    [Test]
    public void Worker_WithPlusCalc_AddsCorrectly()
    {
        // Arrange
        var calc = new PlusCalc();
        var worker = new Worker(calc);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker.Work("100", "200");

        // Assert
        var result = output.ToString().Trim();
        Assert.That(result, Is.EqualTo("300"));
    }

    [Test]
    public void Worker_WithStateCalc_IncrementsCounter()
    {
        // Arrange
        var calc = new StateCalc(1);
        var worker = new Worker(calc);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker.Work("A", "B");
        var result1 = output.ToString().Trim();
        output.GetStringBuilder().Clear();

        worker.Work("C", "D");
        var result2 = output.ToString().Trim();

        // Assert
        Assert.That(result1, Is.EqualTo("AB1"));
        Assert.That(result2, Is.EqualTo("CD2"));
    }

    [Test]
    public void Worker_ThrowsArgumentNullException_WhenCalculatorIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Worker(null!));
    }

    [Test]
    public void Worker_WorkMethod_WritesToConsole()
    {
        // Arrange
        var calc = new CatCalc();
        var worker = new Worker(calc);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker.Work("Test", "Output");

        // Assert
        Assert.That(output.ToString(), Is.Not.Empty);
        Assert.That(output.ToString().Trim(), Is.EqualTo("TestOutput"));
    }

    #endregion

    #region Worker2 (Setter Injection) Tests

    [Test]
    public void Worker2_SetCalculator_WorksCorrectly()
    {
        // Arrange
        var calc = new CatCalc();
        var worker = new Worker2();
        worker.SetCalculator(calc);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker.Work("Hello", "World");

        // Assert
        var result = output.ToString().Trim();
        Assert.That(result, Is.EqualTo("HelloWorld"));
    }

    [Test]
    public void Worker2_ThrowsArgumentNullException_WhenSetCalculatorGetsNull()
    {
        // Arrange
        var worker = new Worker2();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => worker.SetCalculator(null!));
    }

    [Test]
    public void Worker2_ThrowsInvalidOperationException_WhenCalculatorNotSet()
    {
        // Arrange
        var worker = new Worker2();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => worker.Work("A", "B"));
    }

    [Test]
    public void Worker2_CanChangeCalculator()
    {
        // Arrange
        var catCalc = new CatCalc();
        var plusCalc = new PlusCalc();
        var worker = new Worker2();
        var output = new StringWriter();
        Console.SetOut(output);

        // Act - First calculator
        worker.SetCalculator(catCalc);
        worker.Work("Hello", "World");
        var result1 = output.ToString().Trim();
        output.GetStringBuilder().Clear();

        // Act - Second calculator
        worker.SetCalculator(plusCalc);
        worker.Work("10", "20");
        var result2 = output.ToString().Trim();

        // Assert
        Assert.That(result1, Is.EqualTo("HelloWorld"));
        Assert.That(result2, Is.EqualTo("30"));
    }

    [Test]
    public void Worker2_WithPlusCalc_AddsCorrectly()
    {
        // Arrange
        var calc = new PlusCalc();
        var worker = new Worker2();
        worker.SetCalculator(calc);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker.Work("123", "111");

        // Assert
        var result = output.ToString().Trim();
        Assert.That(result, Is.EqualTo("234"));
    }

    #endregion

    #region Worker3 (Property Injection) Tests

    [Test]
    public void Worker3_PropertyInjection_WorksCorrectly()
    {
        // Arrange
        var calc = new CatCalc();
        var worker = new Worker3 { Calculator = calc };
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker.Work("Test", "123");

        // Assert
        var result = output.ToString().Trim();
        Assert.That(result, Is.EqualTo("Test123"));
    }

    [Test]
    public void Worker3_ThrowsInvalidOperationException_WhenCalculatorNotSet()
    {
        // Arrange
        var worker = new Worker3();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => worker.Work("A", "B"));
    }

    [Test]
    public void Worker3_CanChangeCalculatorViaProperty()
    {
        // Arrange
        var catCalc = new CatCalc();
        var plusCalc = new PlusCalc();
        var worker = new Worker3();
        var output = new StringWriter();
        Console.SetOut(output);

        // Act - First calculator
        worker.Calculator = catCalc;
        worker.Work("Foo", "Bar");
        var result1 = output.ToString().Trim();
        output.GetStringBuilder().Clear();

        // Act - Second calculator
        worker.Calculator = plusCalc;
        worker.Work("50", "75");
        var result2 = output.ToString().Trim();

        // Assert
        Assert.That(result1, Is.EqualTo("FooBar"));
        Assert.That(result2, Is.EqualTo("125"));
    }

    [Test]
    public void Worker3_WithStateCalc_MaintainsState()
    {
        // Arrange
        var calc = new StateCalc(10);
        var worker = new Worker3 { Calculator = calc };
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        worker.Work("X", "Y");
        var result1 = output.ToString().Trim();
        output.GetStringBuilder().Clear();

        worker.Work("Z", "W");
        var result2 = output.ToString().Trim();

        // Assert
        Assert.That(result1, Is.EqualTo("XY10"));
        Assert.That(result2, Is.EqualTo("ZW11"));
    }

    [Test]
    public void Worker3_CalculatorPropertyIsNullableInitially()
    {
        // Arrange & Act
        var worker = new Worker3();

        // Assert
        Assert.That(worker.Calculator, Is.Null);
    }

    #endregion

    #region Cross-Worker Comparison Tests

    [Test]
    public void AllWorkers_WithSameCalculator_ProduceSameOutput()
    {
        // Arrange
        var calc = new CatCalc();
        var worker1 = new Worker(calc);
        var worker2 = new Worker2();
        worker2.SetCalculator(calc);
        var worker3 = new Worker3 { Calculator = calc };
        
        var output1 = new StringWriter();
        var output2 = new StringWriter();
        var output3 = new StringWriter();

        // Act
        Console.SetOut(output1);
        worker1.Work("Same", "Output");
        var result1 = output1.ToString().Trim();

        Console.SetOut(output2);
        worker2.Work("Same", "Output");
        var result2 = output2.ToString().Trim();

        Console.SetOut(output3);
        worker3.Work("Same", "Output");
        var result3 = output3.ToString().Trim();

        // Assert
        Assert.That(result1, Is.EqualTo("SameOutput"));
        Assert.That(result2, Is.EqualTo("SameOutput"));
        Assert.That(result3, Is.EqualTo("SameOutput"));
        Assert.That(result1, Is.EqualTo(result2));
        Assert.That(result2, Is.EqualTo(result3));
    }

    [Test]
    public void AllWorkers_CanUseAnyCalculator()
    {
        // Arrange
        var catCalc = new CatCalc();
        var plusCalc = new PlusCalc();
        var stateCalc = new StateCalc(1);

        // Act & Assert - Worker
        Assert.DoesNotThrow(() =>
        {
            new Worker(catCalc).Work("a", "b");
            new Worker(plusCalc).Work("1", "2");
            new Worker(stateCalc).Work("x", "y");
        });

        // Act & Assert - Worker2
        Assert.DoesNotThrow(() =>
        {
            var w = new Worker2();
            w.SetCalculator(catCalc);
            w.Work("a", "b");
            w.SetCalculator(plusCalc);
            w.Work("1", "2");
        });

        // Act & Assert - Worker3
        Assert.DoesNotThrow(() =>
        {
            new Worker3 { Calculator = catCalc }.Work("a", "b");
            new Worker3 { Calculator = plusCalc }.Work("1", "2");
            new Worker3 { Calculator = stateCalc }.Work("x", "y");
        });
    }

    #endregion
}

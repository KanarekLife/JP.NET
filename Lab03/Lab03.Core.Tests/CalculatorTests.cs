using Lab03.Core;
using Lab03.Core.Abstract;

namespace Lab03.Core.Tests;

/// <summary>
/// Unit tests for individual calculator implementations
/// </summary>
[TestFixture]
public class CalculatorTests
{
    #region CatCalc Tests

    [Test]
    public void CatCalc_ConcatenatesTwoStrings()
    {
        // Arrange
        var calc = new CatCalc();

        // Act
        var result = calc.Eval("Hello", "World");

        // Assert
        Assert.That(result, Is.EqualTo("HelloWorld"));
    }

    [Test]
    public void CatCalc_HandlesEmptyStrings()
    {
        // Arrange
        var calc = new CatCalc();

        // Act
        var result1 = calc.Eval("", "World");
        var result2 = calc.Eval("Hello", "");
        var result3 = calc.Eval("", "");

        // Assert
        Assert.That(result1, Is.EqualTo("World"));
        Assert.That(result2, Is.EqualTo("Hello"));
        Assert.That(result3, Is.EqualTo(""));
    }

    [Test]
    public void CatCalc_HandlesSpecialCharacters()
    {
        // Arrange
        var calc = new CatCalc();

        // Act
        var result = calc.Eval("Test@#$", "123!@#");

        // Assert
        Assert.That(result, Is.EqualTo("Test@#$123!@#"));
    }

    #endregion

    #region PlusCalc Tests

    [Test]
    public void PlusCalc_AddsPositiveNumbers()
    {
        // Arrange
        var calc = new PlusCalc();

        // Act
        var result = calc.Eval("123", "111");

        // Assert
        Assert.That(result, Is.EqualTo("234"));
    }

    [Test]
    public void PlusCalc_AddsNegativeNumbers()
    {
        // Arrange
        var calc = new PlusCalc();

        // Act
        var result = calc.Eval("-10", "5");

        // Assert
        Assert.That(result, Is.EqualTo("-5"));
    }

    [Test]
    public void PlusCalc_AddsZero()
    {
        // Arrange
        var calc = new PlusCalc();

        // Act
        var result1 = calc.Eval("0", "0");
        var result2 = calc.Eval("100", "0");
        var result3 = calc.Eval("0", "200");

        // Assert
        Assert.That(result1, Is.EqualTo("0"));
        Assert.That(result2, Is.EqualTo("100"));
        Assert.That(result3, Is.EqualTo("200"));
    }

    [Test]
    public void PlusCalc_AddsLargeNumbers()
    {
        // Arrange
        var calc = new PlusCalc();

        // Act
        var result = calc.Eval("1000000", "2000000");

        // Assert
        Assert.That(result, Is.EqualTo("3000000"));
    }

    [Test]
    public void PlusCalc_ThrowsOnInvalidInput()
    {
        // Arrange
        var calc = new PlusCalc();

        // Act & Assert
        Assert.Throws<FormatException>(() => calc.Eval("abc", "123"));
        Assert.Throws<FormatException>(() => calc.Eval("123", "xyz"));
    }

    #endregion

    #region StateCalc Tests

    [Test]
    public void StateCalc_StartsWithInitialCounter()
    {
        // Arrange
        var calc = new StateCalc(1);

        // Act
        var result = calc.Eval("Test", "Data");

        // Assert
        Assert.That(result, Is.EqualTo("TestData1"));
    }

    [Test]
    public void StateCalc_IncrementsCounterOnEachCall()
    {
        // Arrange
        var calc = new StateCalc(1);

        // Act
        var result1 = calc.Eval("A", "B");
        var result2 = calc.Eval("C", "D");
        var result3 = calc.Eval("E", "F");

        // Assert
        Assert.That(result1, Is.EqualTo("AB1"));
        Assert.That(result2, Is.EqualTo("CD2"));
        Assert.That(result3, Is.EqualTo("EF3"));
    }

    [Test]
    public void StateCalc_CanStartWithDifferentInitialCounter()
    {
        // Arrange
        var calc = new StateCalc(100);

        // Act
        var result1 = calc.Eval("X", "Y");
        var result2 = calc.Eval("Z", "W");

        // Assert
        Assert.That(result1, Is.EqualTo("XY100"));
        Assert.That(result2, Is.EqualTo("ZW101"));
    }

    [Test]
    public void StateCalc_MaintainsStateAcrossMultipleCalls()
    {
        // Arrange
        var calc = new StateCalc(1);
        var results = new List<string>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            results.Add(calc.Eval("Num", i.ToString()));
        }

        // Assert
        Assert.That(results[0], Is.EqualTo("Num01"));
        Assert.That(results[9], Is.EqualTo("Num910"));
    }

    [Test]
    public void StateCalc_HandlesEmptyStringsWithCounter()
    {
        // Arrange
        var calc = new StateCalc(1);

        // Act
        var result1 = calc.Eval("", "");
        var result2 = calc.Eval("", "Test");

        // Assert
        Assert.That(result1, Is.EqualTo("1"));
        Assert.That(result2, Is.EqualTo("Test2"));
    }

    #endregion

    #region Calculator Interface Tests

    [Test]
    public void AllCalculators_ImplementICalculator()
    {
        // Arrange & Assert
        Assert.That(new CatCalc(), Is.InstanceOf<ICalculator>());
        Assert.That(new PlusCalc(), Is.InstanceOf<ICalculator>());
        Assert.That(new StateCalc(1), Is.InstanceOf<ICalculator>());
    }

    [Test]
    public void AllCalculators_EvalMethodReturnsString()
    {
        // Arrange
        ICalculator catCalc = new CatCalc();
        ICalculator plusCalc = new PlusCalc();
        ICalculator stateCalc = new StateCalc(1);

        // Act
        var result1 = catCalc.Eval("a", "b");
        var result2 = plusCalc.Eval("1", "2");
        var result3 = stateCalc.Eval("x", "y");

        // Assert
        Assert.That(result1, Is.TypeOf<string>());
        Assert.That(result2, Is.TypeOf<string>());
        Assert.That(result3, Is.TypeOf<string>());
    }

    #endregion
}

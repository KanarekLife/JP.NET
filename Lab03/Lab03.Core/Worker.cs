using Lab03.Core.Abstract;

namespace Lab03.Core;

public class Worker
{
    private readonly ICalculator _calculator;
    
    public Worker(ICalculator calculator)
    {
        _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
    }

    public void Work(string a, string b)
    {
        Console.WriteLine(_calculator.Eval(a, b));
    }
}

using Lab03.Core.Abstract;

namespace Lab03.Core;

public class Worker2
{
    private ICalculator? _calculator;
    
    public void SetCalculator(ICalculator calc)
    {
        _calculator = calc ?? throw new ArgumentNullException(nameof(calc));
    }

    public void Work(string a, string b)
    {
        Console.WriteLine(_calculator?.Eval(a, b));
    }
}

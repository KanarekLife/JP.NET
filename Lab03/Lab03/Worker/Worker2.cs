using Lab03.Abstract;
using Lab03.Calculator;

namespace Lab03.Worker;

public class Worker2 : IWorker
{
    private ICalculator? _calculator;

    public string Work(string a, string b)
    {
        return _calculator?.Eval(a, b) ?? throw new InvalidOperationException("Calculator not set");
    }

    public void SetCalculator(ICalculator calculator)
    {
        _calculator = calculator;
    }
}
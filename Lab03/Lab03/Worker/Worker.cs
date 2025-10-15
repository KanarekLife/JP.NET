using Lab03.Abstract;
using Lab03.Calculator;

namespace Lab03.Worker;

public class Worker : IWorker
{
    private readonly ICalculator _calculator;

    public Worker(ICalculator calculator)
    {
        _calculator = calculator;
    }

    public string Work(string a, string b)
    {
        return _calculator.Eval(a, b);
    }
}
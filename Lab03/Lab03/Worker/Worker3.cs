using Lab03.Abstract;
using Lab03.Calculator;

namespace Lab03.Worker;

public class Worker3 : IWorker
{
    public ICalculator? Calculator { get; set; }

    public string Work(string a, string b)
    {
        return Calculator?.Eval(a, b) ?? throw new InvalidOperationException("Calculator not set");
    }
}
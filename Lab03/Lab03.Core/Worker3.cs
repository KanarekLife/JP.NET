using Lab03.Core.Abstract;

namespace Lab03.Core;

public class Worker3
{
    public ICalculator? Calculator { get; set; }
    
    public void Work(string a, string b)
    {
        Console.WriteLine(Calculator?.Eval(a, b));
    }
}

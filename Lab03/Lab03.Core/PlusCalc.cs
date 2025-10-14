using Lab03.Core.Abstract;

namespace Lab03.Core;

public class PlusCalc : ICalculator
{
    public string Eval(string a, string b)
        => $"{int.Parse(a) + int.Parse(b)}";
}

using Lab03.Core.Abstract;

namespace Lab03.Core;

public class CatCalc : ICalculator
{
    public string Eval(string a, string b)
    {
        return a + b;
    }
}

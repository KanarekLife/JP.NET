using Lab03.Abstract;

namespace Lab03.Calculator;

public class CatCalc : ICalculator
{
    public string Eval(string a, string b)
    {
        return $"{a} {b}";
    }
}
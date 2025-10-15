using Lab03.Abstract;

namespace Lab03.Calculator;

public class PlusCalc : ICalculator
{
    public string Eval(string a, string b)
    {
        return (int.Parse(a) + int.Parse(b)).ToString();
    }
}
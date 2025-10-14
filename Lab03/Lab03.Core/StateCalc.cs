using Lab03.Core.Abstract;

namespace Lab03.Core;

public class StateCalc : ICalculator
{
    private int _counter;
    
    public StateCalc(int initialCounter)
    {
        _counter = initialCounter;
    }
    
    public string Eval(string a, string b) => a + b + _counter++;
}

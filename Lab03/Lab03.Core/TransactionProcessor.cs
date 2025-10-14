using Autofac;
using Lab03.Core.Abstract;

namespace Lab03.Core;

public class TransactionProcessor
{
    private readonly ILifetimeScope _scope;
    
    public TransactionProcessor(ILifetimeScope scope) => _scope = scope;
    
    public void ProcessTransaction()
    {
        using var transactionScope = _scope.BeginLifetimeScope("transaction");
        Console.WriteLine(" -> Rozpoczęto nową transakcję...");
        var step1 = transactionScope.Resolve<StepOneService>();
        var step2 = transactionScope.Resolve<StepTwoService>();
        step1.Execute();
        step2.Execute();
        Console.WriteLine(" -> Transakcja zakończona.");
    }
}
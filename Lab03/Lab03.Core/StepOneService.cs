using Lab03.Core.Abstract;

namespace Lab03.Core;

public class StepOneService
{
    private readonly ITransactionContext _context;
    public StepOneService(ITransactionContext context) => _context = context;
    public void Execute() => Console.WriteLine($"Krok 1: Przetwarzanie w ramach transakcji {_context.TransactionId}");
}
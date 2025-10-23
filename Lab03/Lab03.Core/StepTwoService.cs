using Lab03.Core.Abstract;

namespace Lab03.Core;

public class StepTwoService
{
    private readonly ITransactionContext _context;
    public StepTwoService(ITransactionContext context) => _context = context;
    public void Execute() => Console.WriteLine($"Krok 2: Zapisywanie w ramach transakcji {_context.TransactionId}");
}

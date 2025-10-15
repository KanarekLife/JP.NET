using Lab03.Abstract;

namespace Lab03.Transaction;

public class StepOneService
{
    private readonly ITransactionContext _context;

    public StepOneService(ITransactionContext context)
    {
        _context = context;
    }

    public void Execute()
    {
        Console.WriteLine($"Step 1 in scope of transaction [{_context.TransactionId}]");
    }
}
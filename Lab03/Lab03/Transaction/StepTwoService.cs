using Lab03.Abstract;

namespace Lab03.Transaction;

public class StepTwoService
{
    private readonly ITransactionContext _context;

    public StepTwoService(ITransactionContext context)
    {
        _context = context;
    }

    public void Execute()
    {
        Console.WriteLine($"Step 2 in scope of transaction [{_context.TransactionId}]");
    }
}
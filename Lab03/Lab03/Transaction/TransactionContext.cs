using Lab03.Abstract;

namespace Lab03.Transaction;

public class TransactionContext : ITransactionContext
{
    public TransactionContext()
    {
        Console.WriteLine($"Created new transaction [{TransactionId}]");
    }

    public Guid TransactionId { get; } = Guid.NewGuid();
}
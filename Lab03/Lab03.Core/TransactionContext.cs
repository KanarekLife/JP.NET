using Lab03.Core.Abstract;

namespace Lab03.Core;

public class TransactionContext : ITransactionContext
{
    public Guid TransactionId { get; } = Guid.NewGuid();
    
    public TransactionContext()
    {
        Console.WriteLine($"UTWORZONO NOWY KONTEKST TRANSAKCJI: {TransactionId}");
    }
}
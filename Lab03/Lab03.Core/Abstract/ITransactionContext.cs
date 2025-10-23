namespace Lab03.Core.Abstract;

public interface ITransactionContext
{
    Guid TransactionId { get; }
    void Log(string message);
    List<string> GetLogs();
}

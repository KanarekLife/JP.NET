using Lab03.Abstract;

namespace Lab03.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public Guid Id { get; } = Guid.NewGuid();
}
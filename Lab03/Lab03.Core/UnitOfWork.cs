using Lab03.Core.Abstract;

namespace Lab03.Core;

public class UnitOfWork : IUnitOfWork
{
    public Guid Id { get; } = Guid.NewGuid();
}

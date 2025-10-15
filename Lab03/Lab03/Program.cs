using Autofac;
using Lab03;
using Lab03.Abstract;
using Lab03.Transaction;
using Lab03.UnitOfWork;
using Lab03.Worker;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var builder = new ContainerBuilder();

if (args.Length > 0 && args[0].Equals("config", StringComparison.CurrentCultureIgnoreCase))
{
    Console.WriteLine("config");
    DI.ConfigureDeclarative(builder, config);
}
else
{
    Console.WriteLine("code");
    DI.ConfigureAll(builder);
}

var container = builder.Build();

List<IWorker> workers = [];
for (var i = 0; i < 3; i++)
{
    workers.Add(container.Resolve<Worker>());
    workers.Add(container.Resolve<Worker2>());
    workers.Add(container.Resolve<Worker3>());

    workers.Add(container.ResolveNamed<Worker>(DI.StateWorker));
    workers.Add(container.ResolveNamed<Worker2>(DI.StateWorker));
    workers.Add(container.ResolveNamed<Worker3>(DI.StateWorker));
}

foreach (var worker in workers) Console.WriteLine(worker.Work("123", "456"));

builder.RegisterType<UnitOfWork>().Named<IUnitOfWork>("scoped").InstancePerLifetimeScope();
using (var scope = container.BeginLifetimeScope())
{
    var scopedUnitOfWork1 = scope.Resolve<IUnitOfWork>();
    var scopedUnitOfWork2 = scope.Resolve<IUnitOfWork>();
    Console.WriteLine(
        $"{scopedUnitOfWork1.Id} == {scopedUnitOfWork2.Id} ? {scopedUnitOfWork1.Id == scopedUnitOfWork2.Id}"
    );
}

var transactionProcessor = container.Resolve<TransactionProcessor>();
const int transactionCount = 3;
for (var i = 0; i < transactionCount; i++) transactionProcessor.ProcessTransaction();
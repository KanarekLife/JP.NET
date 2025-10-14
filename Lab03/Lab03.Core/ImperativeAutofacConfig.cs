using Autofac;
using Lab03.Core.Abstract;

namespace Lab03.Core;

/// <summary>
/// Imperative Autofac configuration
/// </summary>
public class ImperativeAutofacConfig
{
    /// <summary>
    /// Configures the Autofac container imperatively
    /// </summary>
    /// <returns>Configured container</returns>
    public static IContainer ConfigureContainer()
    {
        var builder = new ContainerBuilder();

        // ===== Part 2: Basic Autofac Configuration =====
        
        // Register StateCalc as singleton with named registration
        builder.RegisterType<StateCalc>()
            .Named<ICalculator>("state")
            .WithParameter("initialCounter", 1)
            .SingleInstance();

        // Register CatCalc with named registration
        builder.RegisterType<CatCalc>()
            .Named<ICalculator>("cat");

        // Register PlusCalc with named registration
        builder.RegisterType<PlusCalc>()
            .Named<ICalculator>("plus");

        // Default Worker uses CatCalc (constructor injection)
        builder.Register(c => new Worker(c.ResolveNamed<ICalculator>("cat")))
            .AsSelf();

        // Named Worker "state" uses StateCalc (constructor injection)
        builder.Register(c => new Worker(c.ResolveNamed<ICalculator>("state")))
            .Named<Worker>("state");

        // Default Worker2 uses PlusCalc (method injection via SetCalculator)
        builder.Register(c =>
            {
                var worker = new Worker2();
                worker.SetCalculator(c.ResolveNamed<ICalculator>("plus"));
                return worker;
            })
            .AsSelf();

        // Named Worker2 "state" uses StateCalc (method injection via SetCalculator)
        builder.Register(c =>
            {
                var worker = new Worker2();
                worker.SetCalculator(c.ResolveNamed<ICalculator>("state"));
                return worker;
            })
            .Named<Worker2>("state");

        // Default Worker3 uses CatCalc (property injection)
        builder.Register(c => new Worker3 { Calculator = c.ResolveNamed<ICalculator>("cat") })
            .AsSelf();

        // Named Worker3 "state" uses StateCalc (property injection)
        builder.Register(c => new Worker3 { Calculator = c.ResolveNamed<ICalculator>("state") })
            .Named<Worker3>("state");

        // ===== Part 3: Advanced Lifetime Management =====
        
        // Register IUnitOfWork with InstancePerLifetimeScope
        builder.RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();

        // Register ITransactionContext with InstancePerMatchingLifetimeScope("transaction")
        builder.RegisterType<TransactionContext>()
            .As<ITransactionContext>()
            .InstancePerMatchingLifetimeScope("transaction");

        // Register transaction services
        builder.RegisterType<StepOneService>().AsSelf();
        builder.RegisterType<StepTwoService>().AsSelf();
        builder.RegisterType<TransactionProcessor>().AsSelf();

        return builder.Build();
    }
}

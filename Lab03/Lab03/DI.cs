using Autofac;
using Autofac.Configuration;
using Lab03.Abstract;
using Lab03.Calculator;
using Lab03.Transaction;
using Lab03.UnitOfWork;
using Lab03.Worker;
using Microsoft.Extensions.Configuration;

namespace Lab03;

public static class DI
{
    public const string CatCalc = "catCalc";
    public const string PlusCalc = "plusCalc";
    public const string StateCalc = "stateCalc";
    public const string StateWorker = "stateWorker";

    public static void ConfigureAll(ContainerBuilder builder)
    {
        builder.RegisterType<CatCalc>().Named<ICalculator>(CatCalc);
        builder.RegisterType<PlusCalc>().Named<ICalculator>(PlusCalc);
        builder
            .RegisterType<StateCalc>()
            .Named<ICalculator>(StateCalc)
            .WithParameter("state", "0")
            .SingleInstance();
        
        builder.RegisterType<UnitOfWork.UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
        
        builder
            .RegisterType<TransactionContext>()
            .As<ITransactionContext>()
            .InstancePerMatchingLifetimeScope(TransactionProcessor.TransactionTag);
        builder.RegisterType<StepOneService>().AsSelf();
        builder.RegisterType<StepTwoService>().AsSelf();
        builder.RegisterType<TransactionProcessor>().AsSelf();
        
        SetupWorkers(builder);
    }

    public static void ConfigureDeclarative(ContainerBuilder builder, IConfiguration config)
    {
        builder.RegisterModule(new ConfigurationModule(config.GetSection("autofac")));
        
        SetupWorkers(builder);
    }
    
    private static void SetupWorkers(ContainerBuilder builder)
    {
        builder
            .RegisterType<Worker.Worker>()
            .WithParameter(
                (pi, _) => pi.ParameterType == typeof(ICalculator),
                (_, ctx) => ctx.ResolveNamed<ICalculator>(CatCalc)
            );
        builder
            .RegisterType<Worker.Worker>()
            .Named<Worker.Worker>(StateWorker)
            .WithParameter(
                (pi, _) => pi.ParameterType == typeof(ICalculator),
                (_, ctx) => ctx.ResolveNamed<ICalculator>(StateCalc)
            );
        
        builder
            .RegisterType<Worker2>()
            .OnActivated(e =>
            {
                var calc = e.Context.ResolveNamed<ICalculator>(PlusCalc);
                e.Instance.SetCalculator(calc);
            });
        builder
            .RegisterType<Worker2>()
            .Named<Worker2>(StateWorker)
            .OnActivated(e =>
                e.Instance.SetCalculator(e.Context.ResolveNamed<ICalculator>(StateCalc))
            );
        
        builder
            .RegisterType<Worker3>()
            .OnActivated(e => e.Instance.Calculator = e.Context.ResolveNamed<ICalculator>(CatCalc));
        builder
            .RegisterType<Worker3>()
            .Named<Worker3>(StateWorker)
            .OnActivated(e =>
                e.Instance.Calculator = e.Context.ResolveNamed<ICalculator>(StateCalc)
            );
    }
}
using Autofac;
using Autofac.Configuration;
using Lab03.Core.Abstract;
using Microsoft.Extensions.Configuration;

namespace Lab03.Core;

/// <summary>
/// Declarative Autofac configuration using appsettings.json
/// </summary>
public class DeclarativeAutofacConfig
{
    /// <summary>
    /// Configures the Autofac container declaratively from appsettings.json
    /// </summary>
    /// <returns>Configured container</returns>
    public static IContainer ConfigureContainer()
    {
        var builder = new ContainerBuilder();

        // Build configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        // Register the ConfigurationModule
        var module = new ConfigurationModule(config);
        builder.RegisterModule(module);

        return builder.Build();
    }
}

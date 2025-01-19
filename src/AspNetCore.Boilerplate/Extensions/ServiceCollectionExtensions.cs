using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.Domain.Default;
using AspNetCore.Boilerplate.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.Boilerplate.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBoilerplateServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, DefaultCurrentUser>();
        services.AddScoped<ICurrentTenant, DefaultCurrentTenant>();

        return services;
    }

    public static IServiceCollection AddModule<TModule>(this IServiceCollection services)
        where TModule : IModule, new()
    {
        var module = new TModule();
        module.ConfigureServices(services);

        if (module is IAutoRegister autoRegister)
            autoRegister.AddDependencies(services);

        return services;
    }

    public static IConfiguration GetConfiguration(this IServiceCollection services)
    {
        return services.GetConfigurationOrNull()
            ?? throw new Exception(
                "Could not find an implementation of "
                    + typeof(IConfiguration).AssemblyQualifiedName
                    + " in the service collection."
            );
    }

    private static IConfiguration? GetConfigurationOrNull(this IServiceCollection services)
    {
        var hostBuilderContext = services.GetSingletonInstanceOrNull<HostBuilderContext>();
        if (hostBuilderContext?.Configuration != null)
            return hostBuilderContext.Configuration as IConfigurationRoot;

        return services.GetSingletonInstanceOrNull<IConfiguration>();
    }

    private static T? GetSingletonInstanceOrNull<T>(this IServiceCollection services)
    {
        return (T?)
            services
                .FirstOrDefault(d => d.ServiceType == typeof(T))
                ?.NormalizedImplementationInstance();
    }
}

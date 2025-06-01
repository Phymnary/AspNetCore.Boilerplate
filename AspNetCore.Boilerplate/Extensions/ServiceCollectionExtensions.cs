using AspNetCore.Boilerplate.Api;
using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.Boilerplate.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModule<TModule>(this IServiceCollection services)
        where TModule : IModule, new()
    {
        var module = new TModule();
        module.ConfigureServices(services);

        if (module is IAutoRegister autoRegister)
            autoRegister.AddDependencies(services);

        return services;
    }

    public static IServiceCollection ConfigureSection<TConfig>(
        this IServiceCollection services,
        Action<TConfig>? setupWithConfig = null
    )
        where TConfig : class, IConfigSection
    {
        var configurations = services.GetConfiguration();
        var configureSection = configurations.GetSection(TConfig.Section);
        services.Configure<TConfig>(configureSection);
        setupWithConfig?.Invoke(configureSection.Get<TConfig>()!);

        return services;
    }

    /// <summary>
    /// Default Implementation for <see cref="ICurrentUser"/>, <see cref="ICurrentTenant"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddBoilerplateServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
        services.AddScoped<ICurrentTenant, HttpContextCurrentTenant>();

        return services;
    }

    public static IServiceCollection AddEfCoreServices<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        if (DomainFeatureFlags.IsAuditingEnable)
            services.AddScoped<
                IEfSaveChangesInterceptor,
                AuditSaveChangesInterceptor<TDbContext>
            >();

        if (DomainFeatureFlags.IsMultiTenantEnable)
            services.AddScoped<
                IEfSaveChangesInterceptor,
                MultiTenantSaveChangesInterceptor<TDbContext>
            >();

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

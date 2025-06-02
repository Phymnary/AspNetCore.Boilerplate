using System.Security.Claims;
using AspNetCore.Boilerplate.Api;
using AspNetCore.Boilerplate.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Boilerplate.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseBoilerplateServices(this IApplicationBuilder app)
    {
        app.Use(
            (context, next) =>
            {
                if (
                    context.RequestServices.GetService<ICancellationTokenProvider>()
                    is HttpContextCancellationTokenProvider provider
                )
                    provider.Set(context.RequestAborted);

                if (
                    context.User.FindFirstValue("sub") is { } subId
                    && context.RequestServices.GetRequiredService<ICurrentUser>()
                        is HttpContextCurrentUser currentUser
                )
                    currentUser.Id = new Guid(subId);

                if (
                    DomainFeatureFlags.IsMultiTenantEnable
                    && context.User.FindFirstValue("tenant") is { } tenantId
                    && context.RequestServices.GetRequiredService<ICurrentTenant>()
                        is HttpContextCurrentTenant currentTenant
                )
                    currentTenant.Id = new Guid(tenantId);

                return next(context);
            }
        );

        app.UseExceptionHandler();

        return app;
    }
}

using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.EntityFrameworkCore;
using AspNetCore.Boilerplate.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Boilerplate;

[Auto]
public partial class AspNetCoreBoilerplateTestModule : IModule
{
    public AspNetCoreBoilerplateTestModule()
    {
        DomainFeatureFlags.IsAuditingEnable = true;
        DomainFeatureFlags.IsMultiTenantEnable = true;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        try
        {
            services.GetConfiguration();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        services.AddBoilerplateServices().AddEfCoreServices<BookStoreDbContext>();
    }
}

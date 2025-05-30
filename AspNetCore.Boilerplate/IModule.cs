using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Boilerplate;

public interface IModule
{
    void ConfigureServices(IServiceCollection services);
}

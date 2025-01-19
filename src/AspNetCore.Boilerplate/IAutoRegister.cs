using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Boilerplate;

public interface IAutoRegister
{
    void AddDependencies(IServiceCollection services);
}

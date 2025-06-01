using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.EntityFrameworkCore.Interceptors;

namespace AspNetCore.Boilerplate.EntityFrameworkCore;

public class RepositoryDependencies(IEnumerable<IEfSaveChangesInterceptor> saveChangesInterceptors)
{
    public IEnumerable<IEfSaveChangesInterceptor> SaveChangesInterceptors =>
        saveChangesInterceptors;
}

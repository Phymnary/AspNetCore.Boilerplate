using AspNetCore.Boilerplate.EntityFrameworkCore.Interceptors;

namespace AspNetCore.Boilerplate.EntityFrameworkCore;

public class EfRepositoryAddons(IEnumerable<IEfSaveChangesInterceptor> saveChangesInterceptors)
{
    public IEnumerable<IEfSaveChangesInterceptor> SaveChangesInterceptors =>
        saveChangesInterceptors;
}

using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.EntityFrameworkCore.Interceptors;

namespace AspNetCore.Boilerplate.EntityFrameworkCore;

public class EfRepositoryAddons(
    ICancellationTokenProvider cancellationTokenProvider,
    IEnumerable<IEfSaveChangesInterceptor> saveChangesInterceptors
)
{
    public IEnumerable<IEfSaveChangesInterceptor> SaveChangesInterceptors =>
        saveChangesInterceptors;

    public ICancellationTokenProvider CancellationTokenProvider => cancellationTokenProvider;
}

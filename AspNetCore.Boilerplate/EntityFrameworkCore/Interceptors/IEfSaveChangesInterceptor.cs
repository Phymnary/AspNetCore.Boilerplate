namespace AspNetCore.Boilerplate.EntityFrameworkCore.Interceptors;

public interface IEfSaveChangesInterceptor
{
    /// <summary>
    /// Run when EfRepository update database context and before DbContext.SaveChangesAsync call
    /// </summary>
    /// <param name="cancellationToken">Stopping token.</param>
    ValueTask RunAsync(CancellationToken cancellationToken);
}

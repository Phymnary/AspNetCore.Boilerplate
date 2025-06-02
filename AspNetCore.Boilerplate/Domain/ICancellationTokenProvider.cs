namespace AspNetCore.Boilerplate.Domain;

public interface ICancellationTokenProvider
{
    CancellationToken Get();
}

using AspNetCore.Boilerplate.Domain;

namespace AspNetCore.Boilerplate.Api;

internal class HttpContextCancellationTokenProvider : ICancellationTokenProvider
{
    private CancellationToken _token;

    public void Set(CancellationToken token)
    {
        _token = token;
    }

    public CancellationToken Get()
    {
        return _token;
    }
}

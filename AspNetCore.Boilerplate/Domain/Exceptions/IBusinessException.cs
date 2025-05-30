using System.Net;

namespace AspNetCore.Boilerplate.Domain;

public interface IBusinessException
{
    HttpStatusCode StatusCode { get; }

    string? ErrorCode => null;
}

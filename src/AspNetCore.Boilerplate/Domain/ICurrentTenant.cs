namespace AspNetCore.Boilerplate.Domain;

public interface ICurrentTenant
{
    Guid? Id { get; }
}

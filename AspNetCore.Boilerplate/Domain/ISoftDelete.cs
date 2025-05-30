namespace AspNetCore.Boilerplate.Domain;

public interface ISoftDelete
{
    Guid? DeletedById { get; set; }

    DateTime? DeletedAt { get; set; }
}

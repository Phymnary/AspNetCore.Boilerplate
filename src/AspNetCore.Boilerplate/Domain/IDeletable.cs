namespace AspNetCore.Boilerplate.Domain;

public interface IDeletable
{
    Guid? DeletedById { get; set; }
    DateTime? DeletedAt { get; set; }
}
